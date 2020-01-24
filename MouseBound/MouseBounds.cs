using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using PInvoke;
using System.Collections.Generic;

namespace MouseBound
{
    public class MouseBounds
    {
        public static void Install()
        {
            ClipCursorToCurrentScreen();
            _keyboardHook = SetHook(HookCallback);
            ClipCursorOnWindowsEvent(User32.WindowsEventHookType.EVENT_SYSTEM_MOVESIZESTART);
            ClipCursorOnWindowsEvent(User32.WindowsEventHookType.EVENT_SYSTEM_MOVESIZEEND);
            ClipCursorOnWindowsEvent(User32.WindowsEventHookType.EVENT_SYSTEM_FOREGROUND);
        }

        public static void Uninstall()
        {
            _hooks.ForEach(h => h.Dispose());
            UnclipCursor();
        }

        private static void ClipCursorOnWindowsEvent(User32.WindowsEventHookType winEvent)
        {
            var hook = User32.SetWinEventHook(
                        winEvent,
                        User32.WindowsEventHookType.EVENT_SYSTEM_MOVESIZEEND,
                        IntPtr.Zero,
                        Marshal.GetFunctionPointerForDelegate((WinEventDelegate)WinEventProc),
                        0,
                        0,
                        User32.WindowsEventHookFlags.WINEVENT_OUTOFCONTEXT
                        );
            _hooks.Add(hook);
        }

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            ClipCursorToCurrentScreen();
        }

        private static User32.SafeHookHandle SetHook(User32.WindowsHookDelegate proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                var hook = User32.SetWindowsHookEx(
                    User32.WindowsHookType.WH_KEYBOARD_LL,
                    proc,
                    Kernel32.GetModuleHandle(curModule.ModuleName).DangerousGetHandle(),
                    0);
                _hooks.Add(hook);
                return hook;
            }
        }
        private static User32.SafeHookHandle SetMouseHook(User32.WindowsHookDelegate proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return User32.SetWindowsHookEx(
                    User32.WindowsHookType.WH_MOUSE_LL,
                    proc,
                    Kernel32.GetModuleHandle(curModule.ModuleName).DangerousGetHandle(),
                    0);
            }
        }

        private static int HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            if ((Keys)vkCode == Keys.LControlKey || (Keys)vkCode == Keys.RControlKey)
            {
                if (wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN)
                {
                    UnclipCursor();
                }
                else if (wParam == (IntPtr)User32.WindowMessage.WM_KEYUP)
                {
                    ClipCursorToCurrentScreen();
                }
            }

            return User32.CallNextHookEx(_keyboardHook.DangerousGetHandle(), nCode, wParam, lParam);
        }

        private static void ClipCursorToCurrentScreen()
        {
            var screen = Screen.FromPoint(Cursor.Position);
            Cursor.Clip = screen.Bounds;
        }

        private static void UnclipCursor()
        {
            Cursor.Clip = Rectangle.Empty;
        }
        
        private static User32.SafeHookHandle _keyboardHook;
        private static List<IDisposable> _hooks = new List<IDisposable>();
    }
}