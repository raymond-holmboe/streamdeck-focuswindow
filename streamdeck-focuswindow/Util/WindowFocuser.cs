using BarRaider.SdTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Synkrono.FocusWindow.Util
{
    public class WindowFocuser
    {
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        private const int SW_RESTORE = 9;

        private const int ALT = 0xA4;
        private const int EXTENDEDKEY = 0x1;
        private const int KEYUP = 0x2;

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();


        public void SetFocus(IntPtr mainWindow, IntPtr childWindow, bool showWindow)
        {
            IntPtr focusWindow = childWindow != IntPtr.Zero ? childWindow : mainWindow;
            if (focusWindow == GetForegroundWindow())
                return;
            // sometimes the window may be minimized and the setforeground function cannot bring it to focus
            // there are various values of nCmdShow 3, 5 ,9. What 9 does is: 
            // Activates and displays the window. If the window is minimized or maximized, *the system restores it to its original size and position. An application *should specify this flag when restoring a minimized window */
            if (showWindow && IsIconic(mainWindow))
            {
                ShowWindow(mainWindow, SW_RESTORE);
                Logger.Instance.LogMessage(TracingLevel.INFO, $"Restored main window: {mainWindow}");
            }

            // simulate a keyboard input, in this case an innocent ALT key up, to pretend we are the current foreground window
            // because only foreground windows are allowed to set another foreground window
            keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

            // focuses on the window
            SetForegroundWindow(focusWindow);
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Gave focus to main handle: {mainWindow}, window handle: {focusWindow}");
        }
    }
}
