using BarRaider.SdTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synkrono.FocusWindow
{
    class Program
    {
        static void Main(string[] args)
        {
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            SDWrapper.Run(args);
        }
    }
}
