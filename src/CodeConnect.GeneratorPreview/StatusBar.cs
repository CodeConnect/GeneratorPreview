using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeConnect.GeneratorPreview
{
    internal class StatusBar
    {
        static IVsStatusbar _statusBar;

        internal static void Initialize()
        {
            _statusBar = ServiceProvider.GlobalProvider.GetService(typeof(SVsStatusbar)) as IVsStatusbar;
        }

        internal static void ShowStatus(string message)
        {
            if (_statusBar == null)
            {
                return;
            }
            int frozen;
            _statusBar.IsFrozen(out frozen);
            if (frozen == 0)
            {
                _statusBar.SetText(message);
            }
        }
    }
}
