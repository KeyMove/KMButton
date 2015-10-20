using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
namespace KMButton.Control
{
    public class Button : ControlBase
    {
        public const int Button_IsUse = 2;
        public const int Button_IsRect = 4;
        //public Rectangle rect;
        public String name;
        public short key;
        public int mode;
        public bool update;
        public Point NamePos;
        public SizeF NameRect;
        public SolidBrush s1 = new SolidBrush(Color.FromArgb(255, 208, 208, 208));
        public SolidBrush s2 = new SolidBrush(Color.FromArgb(255, 100, 100, 100));
        public Font deffont = new Font("宋体", 16f);
        public Brush fontbrush = new SolidBrush(Color.Black);
        public delegate void ButtonCallBack(Button b);
        public ButtonCallBack ButtonDown = null;
        public ButtonCallBack ButtonUp = null;
        public Button(int x, int y, int sx, int sy, string str, short key)
        {
            this.rect = new Rectangle(x, y, sx, sy);
            this.name = str;
            this.key = key;
            this.mode = 0;
            NameRect = NameSize(name);
            this.NamePos = new Point();
            NamePos.X = x + (int)(float)(sx / 2 - NameRect.Width / 2);
            NamePos.Y = y + (int)(float)(sy / 2 - NameRect.Height / 2);
        }
        public Button(int x, int y, int sx, int sy, string str, short key,int mode)
        {
            this.rect = new Rectangle(x, y, sx, sy);
            this.name = str;
            this.key = key;
            this.mode = mode;
            NameRect = NameSize(name);
            this.NamePos = new Point();
            NamePos.X = x + (int)(float)(sx / 2 - NameRect.Width / 2);
            NamePos.Y = y + (int)(float)(sy / 2 - NameRect.Height / 2);
        }

        public Button(int x, int y, int sx, int sy, string str, int mode, ButtonCallBack Down=null, ButtonCallBack Up = null)
        {
            this.rect = new Rectangle(x, y, sx, sy);
            this.name = str;
            this.key = -1;
            this.mode = mode;
            NameRect = NameSize(name);
            this.NamePos = new Point();
            NamePos.X = x + (int)(float)(sx / 2 - NameRect.Width / 2);
            NamePos.Y = y + (int)(float)(sy / 2 - NameRect.Height / 2);
            ButtonDown = Down;
            ButtonUp = Up;
        }

        SizeF NameSize(string s)
        {
            Bitmap b = new Bitmap(100,100);
            Graphics graphics = Graphics.FromImage(b);
            SizeF sizeF = graphics.MeasureString(s, deffont);
            graphics.Dispose();
            b.Dispose();
            return sizeF;
        }

        public override bool check(Point p)
        {
            if (rect.Contains(p))
            {
                mode |= Button_IsUse;
                if (this.key != -1)
                    Input.KeyInput(this.key, Input.KeyFlag.Down);
                else if (ButtonDown != null)
                    ButtonDown(this);
                return true;
            }
            return false;
        }
        public override void checkup()
        {
            mode &= ~Button_IsUse;
            if (this.key != -1)
                Input.KeyInput(this.key, Input.KeyFlag.Up);
            else if (ButtonUp != null)
                ButtonUp(this);
        }
        public override void draw()
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
            g.DrawString(name, deffont, fontbrush, NamePos);
        }

        public override void setpos(Point p)
        {
            this.NamePos.X -= rect.X;
            this.NamePos.Y -= rect.Y;
            rect.X = p.X;
            rect.Y = p.Y;
            this.NamePos.X += rect.X;
            this.NamePos.Y += rect.Y;
        }

        public override void setsize(Size p)
        {
            rect.Width = p.Width;
            rect.Height = p.Height;
            NamePos.X = rect.X + (int)((float)rect.Width / 2 - NameRect.Width / 2);
            NamePos.Y = rect.Y + (int)((float)rect.Height / 2 - NameRect.Height / 2);
        }

        public void SetName(string s)
        {
            this.name = s;
            NameRect = NameSize(s);
            NamePos.X = rect.X + (int)((float)rect.Width / 2 - NameRect.Width / 2);
            NamePos.Y = rect.Y + (int)((float)rect.Height / 2 - NameRect.Height / 2);
        }

        public override ControlBase clone()
        {
            Button b = new Button(this.rect.X, this.rect.Y, this.rect.Width, this.rect.Height, this.name, this.key,this.mode);
            b.g = g; 
            b.redraw = redraw; 
            b.id = -1;
            return b;
        }

        public override string getInfo()
        {
            return "RegisterButton({X=" + rect.X + ",Y=" + rect.Y + ",W=" + rect.Width + ",H=" + rect.Height + ",NAME=\"" + name + "\",KEY=" + key + ",MODE=" + mode+"})\r\n";
        }
    }
}
