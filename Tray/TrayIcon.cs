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
                ContextMenuStrip = new ContextMenuStrip(),
                Visible = true
            };
            _notifyIcon.ContextMenuStrip.Items.AddRange(contextMenuActions
                .Select(contextMenuAction => new ToolStripMenuItem( contextMenuAction.Key, null, (_, __) => contextMenuAction.Value(), contextMenuAction.Key))
                .ToArray()
                );
        }

        public static void Uninstall()
        {
            _notifyIcon.Visible = false;
        }
    }
}