using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
namespace TouchButton
{
    public class Button : ContBase
    {
        public const int Button_IsUse = 2;
        public const int Button_IsRect = 4;
        public Rectangle rect;
        public String name;
        public short key;
        public int mode;
        public bool update;
        public Point p;
        public SolidBrush s1 = new SolidBrush(Color.FromArgb(255, 208, 208, 208));
        public SolidBrush s2 = new SolidBrush(Color.FromArgb(255, 100, 100, 100));
        public Font deffont = new Font("宋体", 16f);
        public Brush fontbrush = new SolidBrush(Color.Black);
        public Button(int x, int y, int sx, int sy, string str, short key)
        {
            this.rect = new Rectangle(x, y, sx, sy);
            this.name = str;
            this.key = key;
            this.mode = 0;
            this.p=new Point();
            p.X = sx / 2 + x;
            p.Y = sy / 2 + y-8;
            GraphicsPath path = new GraphicsPath();
            path.AddString(str, deffont.FontFamily, (int)deffont.Style, deffont.Size, new PointF(0, 0), null);
            RectangleF r = path.GetBounds();
            p.X -= (int)r.Width - (int)r.Width/10;

        }
        public Button(int x, int y, int sx, int sy, string str, short key,int mode)
        {
            this.rect = new Rectangle(x, y, sx, sy);
            this.name = str;
            this.key = key;
            this.mode = mode;
            this.p = new Point();
            p.X = sx / 2 + x;
            p.Y = sy / 2 + y - 8;
            GraphicsPath path = new GraphicsPath();
            path.AddString(str, deffont.FontFamily, (int)deffont.Style, deffont.Size,new PointF(0,0), null);
            RectangleF r=path.GetBounds();
            p.X -= (int)r.Width-4;
        }
        public override bool check(Point p)
        {
            if (rect.Contains(p))
            {
                mode |= Button_IsUse;
                keycont.KeyEvent(this.key, 0);
                return true;
            }
            return false;
        }
        public override void checkup()
        {
            mode &= ~Button_IsUse;
            keycont.KeyEvent(this.key, 1);
        }
        public override void draw(Graphics g)
        {
            if ((mode & Button_IsRect) != 0)
            {
                if ((mode & Button_IsUse) != 0)
                    g.FillRectangle(s2, rect);
                else
                    g.FillRectangle(s1, rect);
            }
            else
            {
                if ((mode & Button_IsUse) != 0)
                    g.FillEllipse(s2, rect);
                else
                    g.FillEllipse(s1, rect);
            }
            g.DrawString(name, deffont, fontbrush, p);
        }
        public void save(string file,int id)
        {
            string name = "Button" + id;
            ini.Writue(file, name, "x", "" + this.rect.X);
            ini.Writue(file, name, "y", "" + this.rect.Y);
            ini.Writue(file, name, "ex", "" + this.rect.Width);
            ini.Writue(file, name, "ey", "" + this.rect.Height);
            ini.Writue(file, name, "name", "" + this.name);
            ini.Writue(file, name, "key", "" + this.key);
        }
        public static Button load(string file, int id)
        {
            int x, y, ex, ey, k;
            string n;
            string name = "Button" + id;
            string val;
            val = ini.ReadValue(file, name, "x");
            if (val == null)
                return null;
            x = int.Parse(val);
            val = ini.ReadValue(file, name, "y");
            if (val == null)
                return null;
            y = int.Parse(val);
            val = ini.ReadValue(file, name, "ex");
            if (val == null)
                return null;
            ex = int.Parse(val);
            val = ini.ReadValue(file, name, "ey");
            if (val == null)
                return null;
            ey = int.Parse(val);
            val = ini.ReadValue(file, name, "name");
            if (val == null)
                return null;
            n = val;
            val = ini.ReadValue(file, name, "key");
            if (val == null)
                return null;
            k = int.Parse(val);
            return new Button(x,y,ex,ey,n,(short)k);
        }
    }
}
