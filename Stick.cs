using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TouchButton
{
    public class Stick : ContBase
    {
        public const int Stick_K1 = 0x10;
        public const int Stick_K2 = 0x20;
        public const int Stick_K3 = 0x40;
        public const int Stick_K4 = 0x80;
        public const int Stick_KM = 0x01;
        public Point point;
        public Point dp;
        public int r;
        public int irv;
        public int srv;
        public int inv;
        public bool update;
        public byte[] keylist;
        public int ang;
        public float ir;//检测圈
        public float sr;//中心大小
        public int mode = 0;
        public SolidBrush msb = new SolidBrush(Color.FromArgb(255, 208, 208, 208));
        public SolidBrush ssb = new SolidBrush(Color.FromArgb(255, 190, 190, 190));
        public Rectangle mrect;
        public Rectangle srect;
        public int[] hang = new int[4];
        public int[] lang = new int[4];
        int senx=Screen.PrimaryScreen.Bounds.Width;
        int seny=Screen.PrimaryScreen.Bounds.Height;

        public Stick(int x, int y, int r, int val)
        {
            this.mrect = new Rectangle(x, y, r * 2, r * 2);
            this.point = new Point(x + r, y + r);
            this.r = r;
            this.mode = Stick_KM;
            this.inv = val;
            this.keylist = null;
            SetAng(110);
            sr = 0.25F;
            srv = (int)(r * sr) * 2;
            ir = 0.3F;
            irv = (int)(srv * ir);
            this.srect = new Rectangle(x + r - srv, y + r - srv, srv * 2, srv * 2);
        }

        public Stick(int x, int y, int r, byte[] list)
        {
            this.mrect = new Rectangle(x, y, r * 2, r * 2);
            this.point = new Point(x + r, y + r);
            this.r = r;
            this.keylist = list;
            SetAng(110);
            sr = 0.3F;
            srv = (int)(r*sr)*2;
            ir = 0.5F;
            irv = (int)(srv * ir);
            this.srect = new Rectangle(x + r-srv, y + r-srv, srv * 2, srv * 2);
        }

        public Stick(int x, int y, int r, byte[] list,int mode, int ang, float ir, float sr)
        {
            this.mrect = new Rectangle(x, y, r * 2, r * 2);
            this.point = new Point(x+r, y+r);
            this.r = r;
            this.mode = mode;
            this.keylist = list;
            SetAng(ang);
            this.ir = ir;
            this.sr = sr;
            srv = (int)(r * sr) * 2;
            irv = (int)(srv * ir);
            this.srect = new Rectangle(x + r - srv, y + r - srv, srv * 2, srv * 2);
        }

        public void SetAng(int ang)
        {
            ang /= 2;
            if (ang < 55)
                ang = 55;
            if (ang >= 89)
                ang = 89;
            this.ang = ang;
            this.lang[0] = 270 - ang;
            this.hang[0] = 270 + ang;
            this.lang[1] = 90 - ang;
            this.hang[1] = 90 + ang;
            this.lang[2] = 180 - ang;
            this.hang[2] = 180 + ang;
            this.lang[3] = 360 - ang;
            this.hang[3] = ang;
        }

        public static float InvSqrt(float x)
        {
            //return OpenGLTK.MathInverseSqrtFast.InverseSqrtFast(x);
            unsafe
            {
                float xhalf = 0.5f * x;
                int i = *(int*)&x;	      // Read bits as integer.
                i = 0x5f375a86 - (i >> 1);      // Make an initial guess for Newton-Raphson approximation
                x = *(float*)&i;		// Convert bits back to float
                x = x * (1.5f - xhalf * x * x); // Perform left single Newton-Raphson step.
                return x;
            }
        }

        public static int GetDistance(Point p1, Point p2)
        {
            int x = p1.X - p2.X;
            int y = p1.Y - p2.Y;
            int dt = (int)(1 / InvSqrt((float)(x * x + y * y)));
            return dt;
        }


        public static int GetDirection(Point p1, Point p2)
        {
            int x = p1.X - p2.X;
            int y = p1.Y - p2.Y;
            int ag = (int)(180 / (3.14159 / Math.Acos((float)x / (1 / InvSqrt((float)(x * x + y * y))))));
            if (y < 0)
                ag = -ag + 360;
            else if ((y == 0) && (x < 0))
                ag = 180;
            return ag;
        }

        public static Point GetDistancePoint(Point pt, Point dp,int slen, int len)
        {
            Point dpa = new Point();
            float dt = (float)len / (float)slen;
            if (dt < 1)
            {
                dpa.X = (int)((pt.X - dp.X) * dt);
                dpa.Y = (int)((pt.Y - dp.Y) * dt);
                dpa.X += dp.X;
                dpa.Y += dp.Y;
            }
            else
            {
                dpa.X = pt.X;
                dpa.Y = pt.Y;
            }
            return dpa;
        }

        public override bool check(Point p)
        {
            if (GetDistance(this.point, p) < this.r)
            {
                if ((this.mode & Stick_KM) != 0)
                {
                    //keycont.MouseEvent(65535 * senx / 2 / senx, 65535 * seny / 2 / seny, keycont.MOUSEEVENTF_MOVE | keycont.MOUSEEVENTF_ABSOLUTE, 1);
                    //keycont.MouseEvent(0, 0, keycont.MOUSEEVENTF_LEFTUP, 0);
                }
                move(p);
                return true;
            }
            return false;
        }
        public void move(Point p)
        {
            int dl = GetDistance(this.point, p);
            this.dp = GetDistancePoint(p, this.point, dl, this.r - this.srv);
            this.srect.X = dp.X - this.srv;
            this.srect.Y = dp.Y - this.srv;
            int ag = GetDirection(p, this.point);
            
            if (dl < irv) return;
            if ((this.mode & Stick_KM) != 0)
            {
                int x = 0;
                int y = 0;
                if (ag > lang[0] && ag < hang[0])
                {
                    y = -this.inv;
                }
                if (ag > lang[1] && ag < hang[1])
                {
                    y = this.inv;
                }
                if (ag > lang[2] && ag < hang[2])
                {
                    x = -this.inv;
                }
                if (ag > lang[3] || ag < hang[3])
                {
                    x = this.inv;
                }
                keycont.MouseEvent(x, y, SendInputData.MOUSEEVENTF_MOVE, 0);
            }
            else
            {
                if (ag > lang[0] && ag < hang[0])
                {
                    if ((mode & Stick_K1) == 0)
                    {
                        mode |= Stick_K1;
                        keycont.KeyEvent(this.keylist[0], 0);
                    }
                }
                else
                {
                    if ((mode & Stick_K1) != 0)
                    {
                        mode &= ~Stick_K1;
                        keycont.KeyEvent(this.keylist[0], 1);
                    }
                }

                if (ag > lang[1] && ag < hang[1])
                {
                    if ((mode & Stick_K2) == 0)
                    {
                        mode |= Stick_K2;
                        keycont.KeyEvent(this.keylist[1], 0);
                    }
                }
                else
                {
                    if ((mode & Stick_K2) != 0)
                    {
                        mode &= ~Stick_K2;
                        keycont.KeyEvent(this.keylist[1], 1);
                    }
                }

                if (ag > lang[2] && ag < hang[2])
                {
                    if ((mode & Stick_K3) == 0)
                    {
                        mode |= Stick_K3;
                        keycont.KeyEvent(this.keylist[2], 0);
                    }
                }
                else
                {
                    if ((mode & Stick_K3) != 0)
                    {
                        mode &= ~Stick_K3;
                        keycont.KeyEvent(this.keylist[2], 1);
                    }
                }

                if (ag > lang[3] || ag < hang[3])
                {
                    if ((mode & Stick_K4) == 0)
                    {
                        mode |= Stick_K4;
                        keycont.KeyEvent(this.keylist[3], 0);
                    }
                }
                else
                {
                    if ((mode & Stick_K4) != 0)
                    {
                        mode &= ~Stick_K4;
                        keycont.KeyEvent(this.keylist[3], 1);
                    }
                }
            }
        }

        public override void checkup()
        {
            if ((mode & Stick_K1) != 0)
            {
                mode &= ~Stick_K1;
                keycont.KeyEvent(this.keylist[0], 1);
            }
            if ((mode & Stick_K2) != 0)
            {
                mode &= ~Stick_K2;
                keycont.KeyEvent(this.keylist[1], 1);
            }
            if ((mode & Stick_K3) != 0)
            {
                mode &= ~Stick_K3;
                keycont.KeyEvent(this.keylist[2], 1);
            }
            if ((mode & Stick_K4) != 0)
            {
                mode &= ~Stick_K4;
                keycont.KeyEvent(this.keylist[3], 1);
            }
            this.srect.X = this.point.X - this.srv;
            this.srect.Y = this.point.Y - this.srv;
        }

        public override void draw(Graphics g)
        {
            g.FillEllipse(msb, mrect);
            g.FillEllipse(ssb, srect);
        }
        public void save(string file, int id)
        {
            string name = "Stick" + id;
            ini.Writue(file, name, "x", "" + this.mrect.X);
            ini.Writue(file, name, "y", "" + this.mrect.Y);
            ini.Writue(file, name, "r", "" + this.r);
            ini.Writue(file, name, "k0", "" + (int)(keylist[0]));
            ini.Writue(file, name, "k1", "" + (int)(keylist[1]));
            ini.Writue(file, name, "k2", "" + (int)(keylist[2]));
            ini.Writue(file, name, "k3", "" + (int)(keylist[3]));
        }
        public static Stick load(string file, int id)
        {
            int x, y, r;
            byte[] b = new byte[4];
            string name = "Stick" + id;
            string val;
            val = ini.ReadValue(file, name, "x");
            if (val == null)
                return null;
            x = int.Parse(val);
            val = ini.ReadValue(file, name, "y");
            if (val == null)
                return null;
            y = int.Parse(val);
            val = ini.ReadValue(file, name, "r");
            if (val == null)
                return null;
            r = int.Parse(val);
            for (int i = 0; i < 4; i++)
            {
                val = ini.ReadValue(file, name, "k"+i);
                if (val == null)
                    return null;
                b[i] = byte.Parse(val);
            }
            return new Stick(x, y, r, b);
        }
    }
}
