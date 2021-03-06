﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMButton
{
    public static class HookManager
    {
        private struct Point
        {
            public int X;

            public int Y;
        }

        private struct MouseLLHookStruct
        {
            public HookManager.Point Point;

            public int MouseData;

            public int Flags;

            public int Time;

            public int ExtraInfo;
        }

        private struct KeyboardHookStruct
        {
            public int VirtualKeyCode;

            public int ScanCode;

            public int Flags;

            public int Time;

            public int ExtraInfo;
        }

        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        private const int WH_MOUSE_LL = 14;

        private const int WH_KEYBOARD_LL = 13;

        private const int WH_MOUSE = 7;

        private const int WH_KEYBOARD = 2;

        private const int WM_MOUSEMOVE = 512;

        private const int WM_LBUTTONDOWN = 513;

        private const int WM_RBUTTONDOWN = 516;

        private const int WM_MBUTTONDOWN = 519;

        private const int WM_LBUTTONUP = 514;

        private const int WM_RBUTTONUP = 517;

        private const int WM_MBUTTONUP = 520;

        private const int WM_LBUTTONDBLCLK = 515;

        private const int WM_RBUTTONDBLCLK = 518;

        private const int WM_MBUTTONDBLCLK = 521;

        private const int WM_MOUSEWHEEL = 522;

        private const int WM_KEYDOWN = 256;

        private const int WM_KEYUP = 257;

        private const int WM_SYSKEYDOWN = 260;

        private const int WM_SYSKEYUP = 261;

        private const byte VK_SHIFT = 16;

        private const byte VK_CAPITAL = 20;

        private const byte VK_NUMLOCK = 144;

        private static HookManager.HookProc s_MouseDelegate;

        private static int s_MouseHookHandle;

        private static int m_OldX;

        private static int m_OldY;

        private static HookManager.HookProc s_KeyboardDelegate;

        private static int s_KeyboardHookHandle;

        private static MouseButtons s_PrevClickedButton;

        private static Timer s_DoubleClickTimer;

        private static event MouseEventHandler s_MouseMove;

        public static event MouseEventHandler MouseMove
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalMouseEvents();
                HookManager.s_MouseMove += value;
            }
            remove
            {
                HookManager.s_MouseMove -= value;
                HookManager.TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event EventHandler<MouseEventExtArgs> s_MouseMoveExt;

        public static event EventHandler<MouseEventExtArgs> MouseMoveExt
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalMouseEvents();
                HookManager.s_MouseMoveExt += value;
            }
            remove
            {
                HookManager.s_MouseMoveExt -= value;
                HookManager.TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseClick;

        public static event MouseEventHandler MouseClick
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalMouseEvents();
                HookManager.s_MouseClick += value;
            }
            remove
            {
                HookManager.s_MouseClick -= value;
                HookManager.TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event EventHandler<MouseEventExtArgs> s_MouseClickExt;

        public static event EventHandler<MouseEventExtArgs> MouseClickExt
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalMouseEvents();
                HookManager.s_MouseClickExt += value;
            }
            remove
            {
                HookManager.s_MouseClickExt -= value;
                HookManager.TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseDown;

        public static event MouseEventHandler MouseDown
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalMouseEvents();
                HookManager.s_MouseDown += value;
            }
            remove
            {
                HookManager.s_MouseDown -= value;
                HookManager.TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseUp;

        public static event MouseEventHandler MouseUp
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalMouseEvents();
                HookManager.s_MouseUp += value;
            }
            remove
            {
                HookManager.s_MouseUp -= value;
                HookManager.TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseWheel;

        public static event MouseEventHandler MouseWheel
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalMouseEvents();
                HookManager.s_MouseWheel += value;
            }
            remove
            {
                HookManager.s_MouseWheel -= value;
                HookManager.TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event MouseEventHandler s_MouseDoubleClick;

        public static event MouseEventHandler MouseDoubleClick
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalMouseEvents();
                if (HookManager.s_MouseDoubleClick == null)
                {
                    HookManager.s_DoubleClickTimer = new Timer
                    {
                        Interval = HookManager.GetDoubleClickTime(),
                        Enabled = false
                    };
                    HookManager.s_DoubleClickTimer.Tick += new EventHandler(HookManager.DoubleClickTimeElapsed);
                    HookManager.MouseUp += new MouseEventHandler(HookManager.OnMouseUp);
                }
                HookManager.s_MouseDoubleClick += value;
            }
            remove
            {
                if (HookManager.s_MouseDoubleClick != null)
                {
                    HookManager.s_MouseDoubleClick -= value;
                    if (HookManager.s_MouseDoubleClick == null)
                    {
                        HookManager.MouseUp -= new MouseEventHandler(HookManager.OnMouseUp);
                        HookManager.s_DoubleClickTimer.Tick -= new EventHandler(HookManager.DoubleClickTimeElapsed);
                        HookManager.s_DoubleClickTimer = null;
                    }
                }
                HookManager.TryUnsubscribeFromGlobalMouseEvents();
            }
        }

        private static event KeyPressEventHandler s_KeyPress;

        public static event KeyPressEventHandler KeyPress
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalKeyboardEvents();
                HookManager.s_KeyPress += value;
            }
            remove
            {
                HookManager.s_KeyPress -= value;
                HookManager.TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static event KeyEventHandler s_KeyUp;

        public static event KeyEventHandler KeyUp
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalKeyboardEvents();
                HookManager.s_KeyUp += value;
            }
            remove
            {
                HookManager.s_KeyUp -= value;
                HookManager.TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        private static event KeyEventHandler s_KeyDown;

        public static event KeyEventHandler KeyDown
        {
            add
            {
                HookManager.EnsureSubscribedToGlobalKeyboardEvents();
                HookManager.s_KeyDown += value;
            }
            remove
            {
                HookManager.s_KeyDown -= value;
                HookManager.TryUnsubscribeFromGlobalKeyboardEvents();
            }
        }

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetWindowsHookEx(int idHook, HookManager.HookProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32")]
        public static extern int GetDoubleClickTime();

        [DllImport("user32")]
        private static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        private static extern short GetKeyState(int vKey);

        private static int MouseHookProc(int nCode, int wParam, IntPtr lParam)
        {
            int result;
            if (nCode >= 0)
            {
                HookManager.MouseLLHookStruct mouseLLHookStruct = (HookManager.MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(HookManager.MouseLLHookStruct));
                MouseButtons buttons = MouseButtons.None;
                short num = 0;
                int num2 = 0;
                bool flag = false;
                bool flag2 = false;
                switch (wParam)
                {
                    case 513:
                        flag = true;
                        buttons = MouseButtons.Left;
                        num2 = 1;
                        break;
                    case 514:
                        flag2 = true;
                        buttons = MouseButtons.Left;
                        num2 = 1;
                        break;
                    case 515:
                        buttons = MouseButtons.Left;
                        num2 = 2;
                        break;
                    case 516:
                        flag = true;
                        buttons = MouseButtons.Right;
                        num2 = 1;
                        break;
                    case 517:
                        flag2 = true;
                        buttons = MouseButtons.Right;
                        num2 = 1;
                        break;
                    case 518:
                        buttons = MouseButtons.Right;
                        num2 = 2;
                        break;
                    case 522:
                        num = (short)(mouseLLHookStruct.MouseData >> 16 & 65535);
                        break;
                }
                MouseEventExtArgs mouseEventExtArgs = new MouseEventExtArgs(buttons, num2, mouseLLHookStruct.Point.X, mouseLLHookStruct.Point.Y, (int)num);
                if (HookManager.s_MouseUp != null && flag2)
                {
                    HookManager.s_MouseUp(null, mouseEventExtArgs);
                }
                if (HookManager.s_MouseDown != null && flag)
                {
                    HookManager.s_MouseDown(null, mouseEventExtArgs);
                }
                if (HookManager.s_MouseClick != null && num2 > 0)
                {
                    HookManager.s_MouseClick(null, mouseEventExtArgs);
                }
                if (HookManager.s_MouseClickExt != null && num2 > 0)
                {
                    HookManager.s_MouseClickExt(null, mouseEventExtArgs);
                }
                if (HookManager.s_MouseDoubleClick != null && num2 == 2)
                {
                    HookManager.s_MouseDoubleClick(null, mouseEventExtArgs);
                }
                if (HookManager.s_MouseWheel != null && num != 0)
                {
                    HookManager.s_MouseWheel(null, mouseEventExtArgs);
                }
                if ((HookManager.s_MouseMove != null || HookManager.s_MouseMoveExt != null) && (HookManager.m_OldX != mouseLLHookStruct.Point.X || HookManager.m_OldY != mouseLLHookStruct.Point.Y))
                {
                    HookManager.m_OldX = mouseLLHookStruct.Point.X;
                    HookManager.m_OldY = mouseLLHookStruct.Point.Y;
                    if (HookManager.s_MouseMove != null)
                    {
                        HookManager.s_MouseMove(null, mouseEventExtArgs);
                    }
                    if (HookManager.s_MouseMoveExt != null)
                    {
                        HookManager.s_MouseMoveExt(null, mouseEventExtArgs);
                    }
                }
                if (mouseEventExtArgs.Handled)
                {
                    result = -1;
                    return result;
                }
            }
            result = HookManager.CallNextHookEx(HookManager.s_MouseHookHandle, nCode, wParam, lParam);
            return result;
        }

        private static void EnsureSubscribedToGlobalMouseEvents()
        {
            if (HookManager.s_MouseHookHandle == 0)
            {
                HookManager.s_MouseDelegate = new HookManager.HookProc(HookManager.MouseHookProc);
                HookManager.s_MouseHookHandle = HookManager.SetWindowsHookEx(14, HookManager.s_MouseDelegate, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (HookManager.s_MouseHookHandle == 0)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }

        private static void TryUnsubscribeFromGlobalMouseEvents()
        {
            if (HookManager.s_MouseClick == null && HookManager.s_MouseDown == null && HookManager.s_MouseMove == null && HookManager.s_MouseUp == null && HookManager.s_MouseClickExt == null && HookManager.s_MouseMoveExt == null && HookManager.s_MouseWheel == null)
            {
                HookManager.ForceUnsunscribeFromGlobalMouseEvents();
            }
        }

        private static void ForceUnsunscribeFromGlobalMouseEvents()
        {
            if (HookManager.s_MouseHookHandle != 0)
            {
                int num = HookManager.UnhookWindowsHookEx(HookManager.s_MouseHookHandle);
                HookManager.s_MouseHookHandle = 0;
                HookManager.s_MouseDelegate = null;
                if (num == 0)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }

        private static int KeyboardHookProc(int nCode, int wParam, IntPtr lParam)
        {
            bool flag = false;
            if (nCode >= 0)
            {
                HookManager.KeyboardHookStruct keyboardHookStruct = (HookManager.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(HookManager.KeyboardHookStruct));
                if (HookManager.s_KeyDown != null && (wParam == 256 || wParam == 260))
                {
                    Keys virtualKeyCode = (Keys)keyboardHookStruct.VirtualKeyCode;
                    KeyEventArgs keyEventArgs = new KeyEventArgs(virtualKeyCode);
                    HookManager.s_KeyDown(null, keyEventArgs);
                    flag = keyEventArgs.Handled;
                }
                if (HookManager.s_KeyPress != null && wParam == 256)
                {
                    bool flag2 = (HookManager.GetKeyState(16) & 128) == 128;
                    bool keyState = HookManager.GetKeyState(20) != 0;
                    byte[] array = new byte[256];
                    HookManager.GetKeyboardState(array);
                    byte[] array2 = new byte[2];
                    if (HookManager.ToAscii(keyboardHookStruct.VirtualKeyCode, keyboardHookStruct.ScanCode, array, array2, keyboardHookStruct.Flags) == 1)
                    {
                        char c = (char)array2[0];
                        if ((keyState ^ flag2) && char.IsLetter(c))
                        {
                            c = char.ToUpper(c);
                        }
                        KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs(c);
                        HookManager.s_KeyPress(null, keyPressEventArgs);
                        flag = (flag || keyPressEventArgs.Handled);
                    }
                }
                if (HookManager.s_KeyUp != null && (wParam == 257 || wParam == 261))
                {
                    Keys virtualKeyCode = (Keys)keyboardHookStruct.VirtualKeyCode;
                    KeyEventArgs keyEventArgs = new KeyEventArgs(virtualKeyCode);
                    HookManager.s_KeyUp(null, keyEventArgs);
                    flag = (flag || keyEventArgs.Handled);
                }
            }
            int result;
            if (flag)
            {
                result = -1;
            }
            else
            {
                result = HookManager.CallNextHookEx(HookManager.s_KeyboardHookHandle, nCode, wParam, lParam);
            }
            return result;
        }

        private static void EnsureSubscribedToGlobalKeyboardEvents()
        {
            if (HookManager.s_KeyboardHookHandle == 0)
            {
                HookManager.s_KeyboardDelegate = new HookManager.HookProc(HookManager.KeyboardHookProc);
                HookManager.s_KeyboardHookHandle = HookManager.SetWindowsHookEx(13, HookManager.s_KeyboardDelegate, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (HookManager.s_KeyboardHookHandle == 0)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }

        private static void TryUnsubscribeFromGlobalKeyboardEvents()
        {
            if (HookManager.s_KeyDown == null && HookManager.s_KeyUp == null && HookManager.s_KeyPress == null)
            {
                HookManager.ForceUnsunscribeFromGlobalKeyboardEvents();
            }
        }

        private static void ForceUnsunscribeFromGlobalKeyboardEvents()
        {
            if (HookManager.s_KeyboardHookHandle != 0)
            {
                int num = HookManager.UnhookWindowsHookEx(HookManager.s_KeyboardHookHandle);
                HookManager.s_KeyboardHookHandle = 0;
                HookManager.s_KeyboardDelegate = null;
                if (num == 0)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error);
                }
            }
        }

        private static void DoubleClickTimeElapsed(object sender, EventArgs e)
        {
            HookManager.s_DoubleClickTimer.Enabled = false;
            HookManager.s_PrevClickedButton = MouseButtons.None;
        }

        private static void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Clicks >= 1)
            {
                if (e.Button.Equals(HookManager.s_PrevClickedButton))
                {
                    if (HookManager.s_MouseDoubleClick != null)
                    {
                        HookManager.s_MouseDoubleClick(null, e);
                    }
                    HookManager.s_DoubleClickTimer.Enabled = false;
                    HookManager.s_PrevClickedButton = MouseButtons.None;
                }
                else
                {
                    HookManager.s_DoubleClickTimer.Enabled = true;
                    HookManager.s_PrevClickedButton = e.Button;
                }
            }
        }
    }


    public class MouseEventExtArgs : MouseEventArgs
    {
        private bool m_Handled;

        public bool Handled
        {
            get
            {
                return this.m_Handled;
            }
            set
            {
                this.m_Handled = value;
            }
        }

        public MouseEventExtArgs(MouseButtons buttons, int clicks, int x, int y, int delta) : base(buttons, clicks, x, y, delta)
        {
        }

        internal MouseEventExtArgs(MouseEventArgs e) : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
        }
    }


    public class GlobalEventProvider : Component
    {
        private event MouseEventHandler m_MouseMove;

        public event MouseEventHandler MouseMove
        {
            add
            {
                if (this.m_MouseMove == null)
                {
                    HookManager.MouseMove += new MouseEventHandler(this.HookManager_MouseMove);
                }
                this.m_MouseMove += value;
            }
            remove
            {
                this.m_MouseMove -= value;
                if (this.m_MouseMove == null)
                {
                    HookManager.MouseMove -= new MouseEventHandler(this.HookManager_MouseMove);
                }
            }
        }

        private event MouseEventHandler m_MouseClick;

        public event MouseEventHandler MouseClick
        {
            add
            {
                if (this.m_MouseClick == null)
                {
                    HookManager.MouseClick += new MouseEventHandler(this.HookManager_MouseClick);
                }
                this.m_MouseClick += value;
            }
            remove
            {
                this.m_MouseClick -= value;
                if (this.m_MouseClick == null)
                {
                    HookManager.MouseClick -= new MouseEventHandler(this.HookManager_MouseClick);
                }
            }
        }

        private event MouseEventHandler m_MouseDown;

        public event MouseEventHandler MouseDown
        {
            add
            {
                if (this.m_MouseDown == null)
                {
                    HookManager.MouseDown += new MouseEventHandler(this.HookManager_MouseDown);
                }
                this.m_MouseDown += value;
            }
            remove
            {
                this.m_MouseDown -= value;
                if (this.m_MouseDown == null)
                {
                    HookManager.MouseDown -= new MouseEventHandler(this.HookManager_MouseDown);
                }
            }
        }

        private event MouseEventHandler m_MouseUp;

        public event MouseEventHandler MouseUp
        {
            add
            {
                if (this.m_MouseUp == null)
                {
                    HookManager.MouseUp += new MouseEventHandler(this.HookManager_MouseUp);
                }
                this.m_MouseUp += value;
            }
            remove
            {
                this.m_MouseUp -= value;
                if (this.m_MouseUp == null)
                {
                    HookManager.MouseUp -= new MouseEventHandler(this.HookManager_MouseUp);
                }
            }
        }

        private event MouseEventHandler m_MouseDoubleClick;

        public event MouseEventHandler MouseDoubleClick
        {
            add
            {
                if (this.m_MouseDoubleClick == null)
                {
                    HookManager.MouseDoubleClick += new MouseEventHandler(this.HookManager_MouseDoubleClick);
                }
                this.m_MouseDoubleClick += value;
            }
            remove
            {
                this.m_MouseDoubleClick -= value;
                if (this.m_MouseDoubleClick == null)
                {
                    HookManager.MouseDoubleClick -= new MouseEventHandler(this.HookManager_MouseDoubleClick);
                }
            }
        }

        private event EventHandler<MouseEventExtArgs> m_MouseMoveExt;

        public event EventHandler<MouseEventExtArgs> MouseMoveExt
        {
            add
            {
                if (this.m_MouseMoveExt == null)
                {
                    HookManager.MouseMoveExt += new EventHandler<MouseEventExtArgs>(this.HookManager_MouseMoveExt);
                }
                this.m_MouseMoveExt += value;
            }
            remove
            {
                this.m_MouseMoveExt -= value;
                if (this.m_MouseMoveExt == null)
                {
                    HookManager.MouseMoveExt -= new EventHandler<MouseEventExtArgs>(this.HookManager_MouseMoveExt);
                }
            }
        }

        private event EventHandler<MouseEventExtArgs> m_MouseClickExt;

        public event EventHandler<MouseEventExtArgs> MouseClickExt
        {
            add
            {
                if (this.m_MouseClickExt == null)
                {
                    HookManager.MouseClickExt += new EventHandler<MouseEventExtArgs>(this.HookManager_MouseClickExt);
                }
                this.m_MouseClickExt += value;
            }
            remove
            {
                this.m_MouseClickExt -= value;
                if (this.m_MouseClickExt == null)
                {
                    HookManager.MouseClickExt -= new EventHandler<MouseEventExtArgs>(this.HookManager_MouseClickExt);
                }
            }
        }

        private event KeyPressEventHandler m_KeyPress;

        public event KeyPressEventHandler KeyPress
        {
            add
            {
                if (this.m_KeyPress == null)
                {
                    HookManager.KeyPress += new KeyPressEventHandler(this.HookManager_KeyPress);
                }
                this.m_KeyPress += value;
            }
            remove
            {
                this.m_KeyPress -= value;
                if (this.m_KeyPress == null)
                {
                    HookManager.KeyPress -= new KeyPressEventHandler(this.HookManager_KeyPress);
                }
            }
        }

        private event KeyEventHandler m_KeyUp;

        public event KeyEventHandler KeyUp
        {
            add
            {
                if (this.m_KeyUp == null)
                {
                    HookManager.KeyUp += new KeyEventHandler(this.HookManager_KeyUp);
                }
                this.m_KeyUp += value;
            }
            remove
            {
                this.m_KeyUp -= value;
                if (this.m_KeyUp == null)
                {
                    HookManager.KeyUp -= new KeyEventHandler(this.HookManager_KeyUp);
                }
            }
        }

        private event KeyEventHandler m_KeyDown;

        public event KeyEventHandler KeyDown
        {
            add
            {
                if (this.m_KeyDown == null)
                {
                    HookManager.KeyDown += new KeyEventHandler(this.HookManager_KeyDown);
                }
                this.m_KeyDown += value;
            }
            remove
            {
                this.m_KeyDown -= value;
                if (this.m_KeyDown == null)
                {
                    HookManager.KeyDown -= new KeyEventHandler(this.HookManager_KeyDown);
                }
            }
        }

        protected override bool CanRaiseEvents
        {
            get
            {
                return true;
            }
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.m_MouseMove != null)
            {
                this.m_MouseMove(this, e);
            }
        }

        private void HookManager_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.m_MouseClick != null)
            {
                this.m_MouseClick(this, e);
            }
        }

        private void HookManager_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.m_MouseDown != null)
            {
                this.m_MouseDown(this, e);
            }
        }

        private void HookManager_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.m_MouseUp != null)
            {
                this.m_MouseUp(this, e);
            }
        }

        private void HookManager_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.m_MouseDoubleClick != null)
            {
                this.m_MouseDoubleClick(this, e);
            }
        }

        private void HookManager_MouseMoveExt(object sender, MouseEventExtArgs e)
        {
            if (this.m_MouseMoveExt != null)
            {
                this.m_MouseMoveExt(this, e);
            }
        }

        private void HookManager_MouseClickExt(object sender, MouseEventExtArgs e)
        {
            if (this.m_MouseClickExt != null)
            {
                this.m_MouseClickExt(this, e);
            }
        }

        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.m_KeyPress != null)
            {
                this.m_KeyPress(this, e);
            }
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.m_KeyUp != null)
            {
                this.m_KeyUp(this, e);
            }
        }

        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            this.m_KeyDown(this, e);
        }
    }

}
