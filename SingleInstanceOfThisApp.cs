using System;
using System.Windows.Forms;
using System.Threading;

namespace MouseBound
{
    static class SingleInstanceOfThisApp
    {
        static Mutex _mutex;
        public static void Install()
        {
            const string appGuid = "7037a256-9908-4b7c-8fcb-ce91fc59d831";
            _mutex = new Mutex(true, "Global\\" + appGuid);

            if (!_mutex.WaitOne(0, false))
            {
                MessageBox.Show(nameof(MouseBound) + " is already running. Check the system tray, or Task Manager.");
                throw new Exception("instance already running");
            }
        }

        public static void Uninstall()
        {
            _mutex.Dispose();
        }
    }
}