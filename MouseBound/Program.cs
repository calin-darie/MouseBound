using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using PInvoke;

namespace MouseBound
{ 
    class Program
    {
        private static User32.SafeHookHandle _hookId;

        public static void Main()
        {
            ClipCursorToCurrentScreen();
            _hookId = SetHook(HookCallback);
            ClipCursorOnWindowsEvent(User32.WindowsEventHookType.EVENT_SYSTEM_MOVESIZESTART);
            ClipCursorOnWindowsEvent(User32.WindowsEventHookType.EVENT_SYSTEM_MOVESIZEEND);
            ClipCursorOnWindowsEvent(User32.WindowsEventHookType.EVENT_SYSTEM_FOREGROUND);

            Application.Run();
            //todo UnhookWindowsHookEx(hookId);
        }

        private static void ClipCursorOnWindowsEvent(User32.WindowsEventHookType winEvent)
        {
            User32.SetWinEventHook(
                        winEvent,
                        User32.WindowsEventHookType.EVENT_SYSTEM_MOVESIZEEND,
                        IntPtr.Zero,
                        Marshal.GetFunctionPointerForDelegate((WinEventDelegate)WinEventProc),
                        0,
                        0,
                        User32.WindowsEventHookFlags.WINEVENT_OUTOFCONTEXT
                        );
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
                return User32.SetWindowsHookEx(
                    User32.WindowsHookType.WH_KEYBOARD_LL,
                    proc,
                    Kernel32.GetModuleHandle(curModule.ModuleName).DangerousGetHandle(),
                    0);
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
            if (nCode < 0)
            {
                Console.WriteLine("!! " + nCode);
            }
            else
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if ((Keys)vkCode == Keys.LControlKey || (Keys)vkCode == Keys.RControlKey)
                {
                    if (wParam == (IntPtr)User32.WindowMessage.WM_KEYDOWN)
                    {
                        Cursor.Clip = Rectangle.Empty;
                    }
                    else if (wParam == (IntPtr)User32.WindowMessage.WM_KEYUP)
                    {
                        ClipCursorToCurrentScreen();
                    }
                }
            }

            return User32.CallNextHookEx(_hookId.DangerousGetHandle(), nCode, wParam, lParam);
        }

        private static void ClipCursorToCurrentScreen()
        {
            var screen = Screen.FromPoint(Cursor.Position);
            Cursor.Clip = screen.Bounds;
        } 
    }


}