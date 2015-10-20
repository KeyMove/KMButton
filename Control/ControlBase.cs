using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMButton.Control
{
    public class ControlBase
    {
        public int id = -1;
        public Graphics g;
        public bool redraw = false;
        public Rectangle rect=new Rectangle();
        public virtual void draw() { }
        public virtual bool check(Point p) { return false; }
        public virtual void checkup() { }
        public virtual bool checkRect(Point p) { return rect.Contains(p); }
        public virtual void move(Point p) { }
        public virtual void setpos(Point p) { }
        public virtual void setsize(Size p) { }
        public virtual ControlBase clone() { ControlBase b=new ControlBase(); b.g=g;b.redraw=redraw;b.id=-1;return b; }
        public virtual string getInfo() { return ""; }

    }
}
