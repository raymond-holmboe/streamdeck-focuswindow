using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Synkrono.FocusWindow.Util
{
    public class WindowFinder
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, IntPtr lParam);

        // Delegate to filter which windows to include 
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary> Get the text for the window pointed to by hWnd </summary>
        public string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary> Find first window that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true if match is found</param>
        private IntPtr FindWindow(Func<IntPtr, bool> filter)
        {
            var window = IntPtr.Zero;
            EnumWindowsProc del = (wnd, par) =>
            {
                if (filter(wnd))
                {
                    window = wnd; // only add the windows that pass the filter
                    return false;
                }
                return true; // return true here to iterate to next window
            };
            EnumWindows(del, IntPtr.Zero);
            return window;
        }

        /// <summary> Find first child window handle for the given main window handle matching the given filter </summary>
        /// <param name="hWnd">Main window handle </param>
        /// <param name="filter"> A delegate that returns true if match is found</param>
        public IntPtr FindChildWindow(IntPtr hWnd, Func<IntPtr, bool> filter)
        {
            var window = IntPtr.Zero;
            EnumWindowsProc del = (wnd, par) =>
            {
                if (filter(wnd))
                {
                    window = wnd; // only add the windows that pass the filter
                    return false;
                }
                return true; // return true here to iterate to next window
            };
            EnumChildWindows(hWnd, del, IntPtr.Zero);
            return window;
        }

        /// <summary> Find the first window from processname given </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        private IntPtr FindWindowFromProcess(string processname)
        {
            var processFinder = new ProcessFinder();
            var validprocesses = processFinder.GetProcessesWithMainWindow(processname);
            var process = validprocesses.FirstOrDefault(f => f.ProcessName.IndexOf(processname, StringComparison.OrdinalIgnoreCase) >= 0);
            if (process == null)
                throw new Exception("Can not find process with name " + processname);
            return process.MainWindowHandle;
        }

        /// <summary> Find first window that contains the given title text. If titleText is missing, then the main handle is returned
        /// <param name="titleText"> The text that the window title must contain. </param>
        /// <param name="processname"/>Name of the process, without the .exe suffix </param>
        public (IntPtr mainWindow, IntPtr childWindow) FindWindowWithText(string titleText, string processname)
        {
            IntPtr main = FindWindowFromProcess(processname);
            if (string.IsNullOrWhiteSpace(titleText))
                return (main, IntPtr.Zero);

            IntPtr mainWindow = FindWindowFromProcess(processname);
            Func<IntPtr, bool> filter = wnd =>
            {
                string windowtext = GetWindowText(wnd);
                bool windowfound = windowtext.IndexOf(titleText, StringComparison.OrdinalIgnoreCase) >= 0;
                return windowfound;
            };
            IntPtr childWindow = FindChildWindow(mainWindow, filter);
            return (mainWindow, childWindow);
        }

        public List<ChildWindow> GetChildWindows(string processname)
        {
            IntPtr mainWindow = FindWindowFromProcess(processname);

            var childWindows = new List<ChildWindow>();
            EnumWindowsProc del = (wnd, par) =>
            {
                string title = GetWindowText(wnd);
                if (string.IsNullOrWhiteSpace(title))
                    return true; // true to iterate to next window
                if (!childWindows.Any(c => c.Title.IndexOf(title, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    var childWindow = new ChildWindow { Title = title, WindowsHandle = wnd };
                    childWindows.Add(childWindow);
                }
                return true; // true to iterate to next window
            };
            EnumChildWindows(mainWindow, del, IntPtr.Zero);
            return childWindows;
        }
    }
}
