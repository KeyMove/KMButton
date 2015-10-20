using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMButton.Control
{

    public struct TOUCHINPUT
    {
        public int x;
        public int y;
        public IntPtr hSource;
        public int dwID;
        public int dwFlags;
        public int dwMask;
        public int dwTime;
        public IntPtr dwExtraInfo;
        public int cxContact;
        public int cyContact;
    };

    public enum TOUCH
    {
        MOVE = 0x0001,
        DOWN = 0x0002,
        UP = 0x0004,
        WM_TOUCH = 0x0240,
        WM_MOUSEACTIVATE = 0x0021,
        GWL_EXSTYLE = -20,
        WS_EX_NOACTIVATE = 0x08000000,
    }



    public static class Touch
    {
        public static Graphics gc;
        public static bool KeyMode = false;
        public static List<ControlBase> MobList = new List<ControlBase>();
        static int UpdateDrawTick=0;
        public static void Init(IntPtr Handle,Graphics e)
        {
            gc = e;
            KeyMode=Input.Init();
            RegisterTouchWindow(Handle, 0);
        }

        public static Point GetPoint(TOUCHINPUT t)
        {
            Point p = new Point();
            p.X = t.x / 100;
            p.Y = t.y / 100;
            return p;
        }

        public static int AddMob(ControlBase b){
            b.g = gc;
            MobList.Add(b);
            return MobList.IndexOf(b);
        }

        public static void OnTouchDown(TOUCHINPUT t)
        {
            int id = t.dwID;
            Point p = GetPoint(t);
            foreach (ControlBase b in MobList)
            {
                if(b.id == -1)
                {
                    if (b.check(p))
                    {
                        b.id = id;
                        b.draw();
                        return;
                    }
                }
            }
        }

        public static void OnTouchUp(TOUCHINPUT t)
        {
            int id = t.dwID;
            Point p = GetPoint(t);

            foreach (ControlBase b in MobList)
            {
                if (b.id == id)
                {
                    b.checkup();
                    b.draw();
                    b.id = -1;
                }
            }
        }

        public static void OnTouchMove(TOUCHINPUT t)
        {
            int id = t.dwID;
            Point p = GetPoint(t);

            foreach (ControlBase b in MobList)
            {
                if (b.id == id)
                {
                    b.move(p);
                }
            }
        }

        public static void UpdateButton()
        {
            if (++UpdateDrawTick >= 20)
            {
                UpdateDrawTick = 0;
                OnDraw();
            }
            else
                foreach (ControlBase s in MobList)
                {
                    if (s.redraw)
                    {
                        s.redraw = false;
                        s.draw();
                    }
                }
        }

        public static void OnDraw()
        {
            foreach (ControlBase b in MobList)
            {
                b.draw();
            }
        }

        const int maxbuff = 20;
        static TOUCHINPUT[] inputlist = new TOUCHINPUT[maxbuff];

        [DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern bool RegisterTouchWindow(IntPtr HWND, int ulFlags);
        [DllImport("User32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern bool GetTouchInputInfo(IntPtr HTOUCHINPUT, uint cInputs, ref TOUCHINPUT PTOUCHINPUT, int cbSize);

        public static void Save(string path)
        {
            if (MobList.Count == 0) return;
            System.IO.File.Create(path).Close();
            System.IO.StreamWriter f = new System.IO.StreamWriter(path,true,Encoding.Default);
            foreach(ControlBase b in MobList)
            {
                f.Write(b.getInfo());
            }
            f.Flush();
            f.Close();
        }

        public static bool TouchMessage(Message m)
        {
            if (m.Msg != (int)TOUCH.WM_TOUCH)
            {
                return true;
            }
            int count = m.WParam.ToInt32();
            if (count > maxbuff)
                count = maxbuff;
            if (GetTouchInputInfo(m.LParam, (uint)count, ref inputlist[0], 40))
            {
                foreach (TOUCHINPUT t in inputlist)
                {
                    int flag = t.dwFlags & 7;
                    switch (flag)
                    {
                        case (int)TOUCH.MOVE:
                            OnTouchMove(t);
                            break;
                        case (int)TOUCH.DOWN:
                            OnTouchDown(t);
                            break;
                        case (int)TOUCH.UP:
                            OnTouchUp(t);
                            break;
                    }
                }
            }
            return false;
        }
    
    }
}
