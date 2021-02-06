using Synkrono.FocusWindow.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FocusWindow.Tests
{
    public class Tests
    {
        [Fact]
        public void FocusMainWindow()
        {
            var windowfinder = new WindowFinder();
            (IntPtr mainWindow, IntPtr childWindow) = windowfinder.FindWindowWithText("test", null); // first top level window it can find with title that contains "test"
            var focuser = new WindowFocuser();
            focuser.SetFocus(mainWindow, childWindow, true);
        }

        [Fact]
        public void FocusMainWindowForProcess()
        {
            var windowfinder = new WindowFinder();
            (IntPtr mainWindow, IntPtr childWindow) = windowfinder.FindWindowWithText(null, "Notepad"); // first notepad window it can find
            var focuser = new WindowFocuser();
            focuser.SetFocus(mainWindow, childWindow, true);
        }

        [Fact]
        public void FocusChildWindowForProcess()
        {
            var windowfinder = new WindowFinder();
            (IntPtr mainWindow, IntPtr childWindow) = windowfinder.FindWindowWithText("test", "Notepad"); // notepad window with title that contains "test"
            var focuser = new WindowFocuser();
            focuser.SetFocus(mainWindow, childWindow, true);
        }

        [Fact]
        public void FocusWindow()
        {
            var focuser = new WindowFocuser();
            focuser.SetFocus(new IntPtr(0x00021A28), IntPtr.Zero, false); // find specific window with given window handle
        }
    }
}
