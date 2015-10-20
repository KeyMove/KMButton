using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KMButton.Control;
using NLua;

namespace KMButton
{
    public partial class ButtonUI : Form
    {
        

        Graphics gp;
        //Bitmap DrawMap;
        Lua LuaVM = new Lua();

        bool DebugMode = false;
        bool DebugInfoEnable = true;
        int w, h;

        bool EditStats=false;

        //------屏幕键盘-----------
        public static bool ScreenKeyboard = false;
        int DefOpacity = 70;
        Opactiy opc = new Opactiy();
        static IntPtr SKHandl = IntPtr.Zero;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        //-------------------------//





        //CDmSoft DM = new CDmSoft();

        public enum TOUCH
        {
            MOVE = 0x0001,
            DOWN = 0x0002,
            UP = 0x0004,
            WM_TOUCH = 0x0240,
            WM_MOUSEACTIVATE = 0x0021,
            GWL_EXSTYLE = -20,
            WS_EX_NOACTIVATE = 0x08000000,
            LWA_ALPHA = 0x2,
            LWA_COLORKEY=0x00000001,
            WS_EX_LAYERED = 0x80000,
            WS_EX_TRANSPARENT = 0x20,
        }

        [DllImport("user32.dll", CharSet = CharSet.Ansi, EntryPoint = "SetWindowLongA", ExactSpelling = true, SetLastError = true)]
        private static extern long SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("User32.Dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("User32.Dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetLayeredWindowAttributes(IntPtr Handle, int crKey, byte bAlpha, int dwFlags);
        [DllImport("User32.Dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr FindWindow(String sClassName, String sAppName);
        [DllImport("User32.Dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);


        bool MouseStats = false;
        void MouseDebug()
        {    
            base.MouseDown += (new MouseEventHandler((object sender,MouseEventArgs e)=> {
                if (EditStats)
                {
                    EditUI.MouseDown(new Point(e.X, e.Y));
                    return;
                }
                if (!DebugMode) return;
                if (MouseStats) return;
                TOUCHINPUT ti = new TOUCHINPUT();
                ti.x = e.X * 100;
                ti.y = e.Y * 100;
                ti.dwID = 0;
                Touch.OnTouchDown(ti);
                MouseStats = true;
            }));
            base.MouseUp += (new MouseEventHandler((object sender, MouseEventArgs e) => {
                if (EditStats)
                {
                    EditUI.MouseUp();
                    return;
                }
                if (!DebugMode) return;
                if (!MouseStats) return;
                TOUCHINPUT ti = new TOUCHINPUT();
                ti.x = e.X * 100;
                ti.y = e.Y * 100;
                ti.dwID = 0;
                Touch.OnTouchUp(ti);
                MouseStats = false;
            }));
            
            base.MouseMove += (new MouseEventHandler((object sender, MouseEventArgs e) => {
                if (EditStats)
                {
                    EditUI.MouseMove(new Point(e.X,e.Y));
                    return;
                }
                if (!DebugMode) return;
                if (!MouseStats) return;
                TOUCHINPUT ti = new TOUCHINPUT();
                ti.x = e.X * 100;
                ti.y = e.Y * 100;
                ti.dwID = 0;
                Touch.OnTouchMove(ti);
            }));
        }

        void SetWindowActivate(bool act)
        {
            if(act)
                SetWindowLong(this.Handle, (int)TOUCH.GWL_EXSTYLE, (GetWindowLong(this.Handle, (int)TOUCH.GWL_EXSTYLE) | (int)TOUCH.WS_EX_NOACTIVATE));
            else
                SetWindowLong(this.Handle, (int)TOUCH.GWL_EXSTYLE, (GetWindowLong(this.Handle, (int)TOUCH.GWL_EXSTYLE) &(~(int)TOUCH.WS_EX_NOACTIVATE)));

        }


        const int WS_EX_LAYERED = 0x80000;
        const int LWA_ALPHA = 0x2;
        const int GWL_EXSTYLE = -20;


        void SetScreenKeyboardOpacity(int opacity)
        {
            if (opacity < 0 || opacity > 100) return;
            IntPtr Handle = FindWindow("IPTip_Main_Window", null);
            if (Handle != IntPtr.Zero)
            {
                SetWindowLong(Handle, GWL_EXSTYLE, (GetWindowLong(Handle, GWL_EXSTYLE) | WS_EX_LAYERED));
                SetLayeredWindowAttributes(Handle, 0, (byte)(255 * (opacity) / 100), LWA_ALPHA);
            }
        }

        public static void ShowScreenKeyboard()
        {
            try {
                System.Diagnostics.Process.Start(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe");
            }
            catch
            {

            }
        }

        public static void CloseScreenKeyboard()
        {
            SKHandl = FindWindow("IPTip_Main_Window", null);
            if (SKHandl != IntPtr.Zero)
            {
                SendMessage(SKHandl, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
        }


        void InitWindow()
        {
            Touch.Init(this.Handle,gp);
            EditUI.Init(gp,this.NameInput, new EditUI.ShowOK(() =>
            {
                DelayUpdate.Start();
                this.TopMost = true;
                this.KeyDown -= ButtonUI_KeyDown;
                this.NameInput.KeyDown -= ButtonUI_KeyDown;
                SetWindowActivate(true);
            }));
            //opc.Show();
            if (System.IO.File.Exists(@"C:\Program Files\Common Files\Microsoft Shared\ink\TabTip.exe"))
            {
                ScreenKeyboard = true;
                opc.Deactivate += new EventHandler((object sender, EventArgs e) => {
                    opc.Hide();
                    opc.TopMost = false;
                });
                opc.Op.Scroll += new EventHandler((object sender, EventArgs e) => {
                    SetScreenKeyboardOpacity(((TrackBar)sender).Value);
                });
            }

            if (ScreenKeyboard)
                SetScreenKeyboardOpacity(DefOpacity);
        }

        byte GetVkCode(string s)
        {
            string str=s.ToUpper();
            switch (str)
            {
                case "TAB":return 9;
                case "BACK":return 8;
                case "RETURN":return 0x0d;
                case "SHIFT":return 0x10;
                case "CTRL":return 0x11;
                case "ALT":return 0x12;
                case "END":return 0x23;
                case "HOME":return 0x24;
                case "LEFT":return 0x25;
                case "UP": return 0x26;
                case "RIGHT":return 0x27;
                case "DOWN":return 0x28;
                case "INSERT":return 0x2d;
                case "DELETE":return 0x2E;
                default:return (byte)str.ToCharArray()[0];
            }
        }


        public int RegStick(LuaTable arg)
        {
            int arglen = arg.Keys.Count;
            try
            {
                if (arglen >= 4)
                {
                    int X = (int)(double)arg["X"];
                    int Y = (int)(double)arg["Y"];
                    int R = (int)(double)arg["R"];
                    LuaTable keytable = (LuaTable)arg["KEY"];
                    if (keytable.Values.Count < 4) return -1;
                    byte[] key = new byte[4];
                    int i = 0;
                    foreach (object k in keytable.Values)
                    {
                        if (k is string)
                            key[i++] = GetVkCode((string)k);
                        else if (k is double)
                            key[i++] = (byte)(int)(double)k;
                        else
                            return -1;
                    }
                    return Touch.AddMob(new Stick(X, Y, R, key));
                }
                else
                    return -1;
            }
            catch
            {
                return -1;
            }
        }

        string LoadStr(string s)
        {
            char[] clist = s.ToCharArray();
            byte[] blist = new byte[clist.Length];
            for (int i = 0; i < clist.Length; i++)
            {
                blist[i] = (byte)clist[i];
            }
            return System.Text.Encoding.Default.GetString(blist);
        }

        public int RegButton(LuaTable arg)
        {
            try
            {
                int X = (int)(double)arg["X"];
                int Y = (int)(double)arg["Y"];
                int W = (int)(double)arg["W"];
                int H = (int)(double)arg["H"];
                string Name = LoadStr((string)arg["NAME"]);

                object Key = arg["KEY"];
                int vk = 0;
                if (Key is string)
                    vk = GetVkCode((string)Key);
                else if (Key is double)
                    vk = (int)(double)Key;
                else return -1;
                int val = 0;
                if (arg.Values.Count > 6)
                {
                    val = (int)(double)arg["MODE"];
                }
                return Touch.AddMob(new Control.Button(X, Y, W, H, Name, (short)vk, val));
            }
            catch
            {
                return -1;
            }
        }


        void LuaInit()
        {
            LuaVM.LoadCLRPackage();
            LuaVM.RegisterFunction("RegisterStick", this, this.GetType().GetMethod("RegStick"));
            LuaVM.RegisterFunction("RegisterButton", this,this.GetType().GetMethod("RegButton"));
        }

        public ButtonUI()
        {
            InitializeComponent();
            //MessageBox.Show(((Keys)58).ToString());
            this.TopMost = true;
            this.Left = 0;
            this.Top = 0;
            this.Width = w = Screen.PrimaryScreen.Bounds.Width;
            this.Height = h = Screen.PrimaryScreen.Bounds.Height;
            this.Opacity = 0.5;            
            this.FormBorderStyle = FormBorderStyle.None;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.Paint += TouchButton_Paint;
            SetWindowActivate(true);
            this.DebugInfo.LostFocus += DebugInfo_LostFocus;
        }

        private void DebugInfo_LostFocus(object sender, EventArgs e)
        {
            this.DebugInfo.Clear();
            DebugInfoEnable = true;
        }

        Timer DelayUpdate = new Timer();
        private void ButtonUI_Load(object sender, EventArgs e)
        {
            Timer UpdateDraw = new Timer();
            UpdateDraw.Interval = 100;
            UpdateDraw.Tick += (new EventHandler((object s, EventArgs arg) => {
                Touch.UpdateButton();
            }));
            UpdateDraw.Start();

            

            //this.Left = 0;
            //this.Top = 0;
            

            gp = this.CreateGraphics();
            gp.Clear(Color.Black);
            gp.FillEllipse(Brushes.Blue, this.ClientRectangle);
            InitWindow();
            MouseDebug();
            LuaInit();
            string ConfigPath = Directory.GetCurrentDirectory() + "/Config.lua";
            if (File.Exists(ConfigPath))
            {
                LuaVM.DoFile(ConfigPath);
            }
            else
            {
                Touch.AddMob(new Stick(40, h - 240, 100, new byte[] { (byte)'W', (byte)'S', (byte)'A', (byte)'D' }));
                Touch.AddMob(new Control.Button(w - 240, h - 240, 100, 100, "J", (short)'J'));
                Touch.AddMob(new Control.Button(w - 240, h - 135, 100, 100, "K", (short)'K'));
                Touch.AddMob(new Control.Button(w - 135, h - 240, 100, 100, "I", (short)'I'));
                Touch.AddMob(new Control.Button(w - 135, h - 135, 100, 100, "L", (short)'L'));
                Touch.Save(Directory.GetCurrentDirectory() + "/Config.lua");
            }


            
            DelayUpdate.Interval = 100;
            DelayUpdate.Tick += (new EventHandler((object s, EventArgs arg) =>
            {
                EditStats = false;
                gp.Clear(Color.Fuchsia);
                Touch.OnDraw();
                //EditUI.Show();
                //EditStats = true;
                ((Timer)s).Stop();
            }));
            DelayUpdate.Start();
        }

        void TouchButton_Paint(object sender, PaintEventArgs e)
        {
            Touch.OnDraw();
        }


        protected override void WndProc(ref Message m)
        {
            if(DebugInfoEnable)
                DebugInfo.Text += string.Format("{0:X}\r\n",m.Msg);
            if (m.Msg != (int)TOUCH.WM_TOUCH)
            {
                switch (m.Msg)
                {
                    case (int)TOUCH.WM_MOUSEACTIVATE:
                        m.Result = (IntPtr)4;
                        break;
                    case 716:
                        m.Result = (IntPtr)16777217;
                        break;
                    case 0x204:
                    case 0x205:
                        m.Result = (IntPtr)4;
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
                return;
            }

            if (EditStats)
            {
                base.WndProc(ref m);
                return;
            }
            Touch.TouchMessage(m);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            if (ScreenKeyboard)
                SetScreenKeyboardOpacity(100);
            System.Environment.Exit(0);
        }

        private void 设置按键ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetWindowActivate(false);
            this.KeyDown += ButtonUI_KeyDown;
            this.NameInput.KeyDown += ButtonUI_KeyDown;
            this.TopMost = false;
            EditUI.Show();
            EditStats = true;
        }

        private void ButtonUI_KeyDown(object sender, KeyEventArgs e)
        {
            EditUI.KeyPress(e);
        }
        SaveFileDialog sf = new SaveFileDialog();
        private void 保存按键ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sf.Filter = "lua脚本文件|*.lua";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                Touch.Save(sf.FileName);
            }
        }
        OpenFileDialog of = new OpenFileDialog();

        private void opo(object sender, MouseEventArgs e)
        {
            if (ScreenKeyboard&&e.Button==MouseButtons.Left)
            {
                opc.Show();
                opc.Pos= new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y);
                opc.TopMost = true;
                opc.Activate();
                //opc.Op.Focus();
                //opc.TopMost=false;
            }
        }

        private void ClickLook(object sender, EventArgs e)
        {
            DebugInfoEnable = false;
        }



        private void 加载按键ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            of.Filter = "lua脚本文件|*.lua";
            if (of.ShowDialog() == DialogResult.OK)
            {
                Touch.MobList.Clear();
                LuaVM.DoFile(of.FileName);
                gp.Clear(Color.Fuchsia);
                Touch.OnDraw();
            }
        }


    }
}
