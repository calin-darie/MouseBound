using System;
using System.Windows.Forms;
using MouseBound.Tray;
using System.Collections.Generic;

namespace MouseBound
{
    class Program
    {   
        public static void Main()
        {
            MouseBounds.Install();
            TrayIcon.Install(new Dictionary<string, Action> { { "Quit", Quit } });
            Application.Run();
        }

        private static void Quit()
        {
            TrayIcon.Uninstall();
            MouseBounds.Uninstall();
            Application.Exit();
        }
    }
}