using System.Windows.Forms;
using MouseBound.Tray;
using System.Collections.Generic;
using System;

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
                Console.WriteLine(e);
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