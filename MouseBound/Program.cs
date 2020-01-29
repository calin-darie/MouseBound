using System.Windows.Forms;
using MouseBound.Tray;
using System.Collections.Generic;
using System;
using System.IO;

namespace MouseBound
{
    class Program
    {   
        public static void Main()
        {
            try
            {
                SingleInstanceOfThisApp.Install();
                MouseBounds.Install();
                TrayIcon.Install(new Dictionary<string, Action> { { "Quit", Application.Exit } });
                Application.Run();
            }
            catch (Exception e)
            {
                File.AppendAllText("log.txt", DateTime.UtcNow + " " + e);
            }
            finally
            {
                SingleInstanceOfThisApp.Uninstall();
                TrayIcon.Uninstall();
                MouseBounds.Uninstall();
            }
        }
    }
}