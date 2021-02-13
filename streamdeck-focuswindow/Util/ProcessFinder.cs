using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synkrono.FocusWindow.Util
{
    public class ProcessFinder
    {
        public List<string> GetProcessNamesWithMainWindow()
        {
            var processes = GetProcessesWithMainWindow();
            var processnames = processes
                .Select(p => p.ProcessName)
                .Distinct()
                .ToList();
            return processnames;
        }

        public List<Process> GetProcessesWithMainWindow()
        {
            var allprocesses = Process.GetProcesses();
            var processes = allprocesses
                .Where(p => (long)p.MainWindowHandle != 0)
                .ToList();
            return processes;
        }

        public List<Process> GetProcessesWithMainWindow(string processname)
        {
            var allprocesses = Process.GetProcessesByName(processname);
            var processes = allprocesses
                .Where(p => (long)p.MainWindowHandle != 0)
                .ToList();
            return processes;
        }
    }
}
