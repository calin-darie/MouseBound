using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MouseBound.Tray
{
    public class TrayIcon
    {
        private static NotifyIcon _notifyIcon;
        public static void Install(Dictionary<string, Action> contextMenuActions)
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = Resources.Icon,
                ContextMenu = new ContextMenu(
                    contextMenuActions
                        .Select(contextMenuAction => new MenuItem( contextMenuAction.Key, (_, __) => contextMenuAction.Value()))
                        .ToArray()),
                Visible = true
            };
        }

        public static void Uninstall()
        {
            _notifyIcon.Visible = false;
        }
    }
}
