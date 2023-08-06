using MouseBound.Tray;

namespace MouseBound
{
    static class Program
    {
        [STAThread]
        static void Main() 
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