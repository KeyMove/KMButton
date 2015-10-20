using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMButton.Control;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace KMButton
{
    public static class EditUI
    {
        public static SolidBrush MColor = new SolidBrush(Color.FromArgb(255, 228, 228, 228));
        public static SolidBrush SColor = new SolidBrush(Color.FromArgb(255, 170, 170, 170));
        public static Brush BColor = Brushes.Black;
        static Bitmap DrawMap;
        static Graphics gp;
        static Graphics Sgp;
        static Rectangle rect;
        static Point startpoint;
        static List<Control.Button> ContList = new List<Control.Button>();
        static List<ControlBase> NewList = new List<ControlBase>();

        static List<Control.Button> ButtonInfo = new List<Control.Button>();
        static List<Control.Button> StickInfo = new List<Control.Button>();

        static ControlBase SelectItem = null;
        static ControlBase SaveItem = null; 
        
        static int xoffset = 0;
        static int yoffset = 0;

        static bool SelectBlock = false;
        static bool isNewList = false;
        static bool isEditKey = false;
        static bool isEditName = false;
        static bool ButtonXSize = true;
        static bool ButtonYSize = true;
        static int SelectButtonIndex = -1;
        public delegate void ShowOK();

        static ShowOK CallShowOK = null;

        public static Font DefFont = new Font("宋体", 16f);

        static byte[] DefKeyList = new byte[]{(byte)'W',(byte)'S',(byte)'A',(byte)'D' };

        static int StickXpos = -1,StickYpos = -1;

        static TextBox NameInput;

        static Timer DrawDelay = new Timer();

        static void DrawRoundedRectangle(Graphics g,Pen p, Rectangle rect, int cornerRadius)
        {
            g.DrawArc(p, rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            g.DrawLine(p, rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius , rect.Y);
            g.DrawArc(p, rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            g.DrawLine(p, rect.Right, rect.Y + cornerRadius, rect.Right, rect.Y + rect.Height - cornerRadius);
            g.DrawArc(p, rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            g.DrawLine(p, rect.Right - cornerRadius, rect.Bottom, rect.X + cornerRadius, rect.Bottom);
            g.DrawArc(p, rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            g.DrawLine(p, rect.X, rect.Bottom - cornerRadius, rect.X, rect.Y + cornerRadius);
        }

        public static void Init(Graphics g,TextBox t,ShowOK CallBack)
        {
            NameInput = t;
            CallShowOK = CallBack;
            DrawMap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            gp = Graphics.FromImage(DrawMap);
            Sgp = g;
            rect = new Rectangle(Screen.PrimaryScreen.Bounds.Width/2-300, 0, 600, 400);
            startpoint = new Point(rect.X, rect.Y);

            DrawDelay.Interval = 10;
            DrawDelay.Tick += (new EventHandler((object sender, EventArgs e) => {
                DrawUI();
                ((Timer)sender).Stop();
            }));

            NameInput.Visible = false;
            NameInput.Location = new Point(startpoint.X + 130, 329);
            //NameInput.Parent.Focus();
            //NameInput.KeyDown += new KeyEventHandler((object sender, KeyEventArgs e) => {
            //    if (isEditName)
            //    {
            //        if (e.KeyCode == Keys.Enter)
            //        {
            //            ((Control.Button)SaveItem).SetName(NameInput.Text);
            //            NameInput.Parent.Focus();
            //            NameInput.Visible = false;
            //            isEditName = false;
            //            DrawDelay.Start();
            //        }
            //    }
            //    else
            //    {
            //        ((TextBox)sender).Visible = false;
            //    }
            //});
            //--------按钮属性
            ButtonInfo.Add(new Control.Button(startpoint.X + 10, 310, 100, 80, "", 4, new Control.Button.ButtonCallBack((Control.Button b) => {
                isEditKey = true;
                SelectButtonIndex = 0;
                if (ButtonUI.ScreenKeyboard)
                    ButtonUI.ShowScreenKeyboard();
            })));

            ButtonInfo.Add(new Control.Button(startpoint.X + 120, 310, 100, 80, "", 4, new Control.Button.ButtonCallBack((Control.Button b) => {
                isEditName = true;
                NameInput.Visible = true;
                NameInput.Focus();
                if (ButtonUI.ScreenKeyboard)
                    ButtonUI.ShowScreenKeyboard();
            })));

            ButtonInfo.Add(new Control.Button(startpoint.X + 380, 310, 100, 80, "X方向缩放", 4, new Control.Button.ButtonCallBack((Control.Button b) => {
                if ((((Control.Button)SaveItem).mode & Control.Button.Button_IsRect) == 0)
                {
                    if (ButtonXSize)
                    {
                        ButtonXSize = ButtonYSize = false;
                        ButtonInfo[0].mode &= ~Control.Button.Button_IsUse;
                        ButtonInfo[1].mode &= ~Control.Button.Button_IsUse;
                    }
                    else
                    {
                        ButtonXSize = ButtonYSize = true;
                        ButtonInfo[0].mode |= Control.Button.Button_IsUse;
                        ButtonInfo[1].mode |= Control.Button.Button_IsUse;
                    }
                }
                else
                {
                    if (ButtonXSize)
                    {
                        ButtonXSize = false;
                        b.mode &= ~Control.Button.Button_IsUse;
                    }
                    else
                    {
                        ButtonXSize = true;
                        b.mode |= Control.Button.Button_IsUse;
                    }
                }
                DrawUI();
            })));

            ButtonInfo.Add(new Control.Button(startpoint.X + 490, 310, 100, 80, "Y方向缩放", 4, new Control.Button.ButtonCallBack((Control.Button b) =>
            {
                if ((((Control.Button)SaveItem).mode & Control.Button.Button_IsRect)==0)
                {
                    if (ButtonYSize)
                    {
                        ButtonXSize = ButtonYSize = false;
                        ButtonInfo[0].mode &= ~Control.Button.Button_IsUse;
                        ButtonInfo[1].mode &= ~Control.Button.Button_IsUse;
                    }
                    else
                    {
                        ButtonXSize = ButtonYSize = true;
                        ButtonInfo[0].mode |= Control.Button.Button_IsUse;
                        ButtonInfo[1].mode |= Control.Button.Button_IsUse;
                    }
                }
                else
                {
                    if (ButtonYSize)
                    {
                        ButtonYSize = false;
                        b.mode &= ~Control.Button.Button_IsUse;
                    }
                    else
                    {
                        ButtonYSize = true;
                        b.mode |= Control.Button.Button_IsUse;
                    }
                }
                DrawUI();
            })));
            //---------

            //------摇杆属性
            StickInfo.Add(new Control.Button(startpoint.X + 10, 310, 100, 80, "",  4, new Control.Button.ButtonCallBack((Control.Button b) =>
            {
                SelectButtonIndex = 0;
                isEditKey = true;
                if (ButtonUI.ScreenKeyboard)
                    ButtonUI.ShowScreenKeyboard();
            })));
            StickInfo.Add(new Control.Button(startpoint.X + 160, 310, 100, 80, "", 4, new Control.Button.ButtonCallBack((Control.Button b) =>
            {
                SelectButtonIndex = 1;
                isEditKey = true;
                if (ButtonUI.ScreenKeyboard)
                    ButtonUI.ShowScreenKeyboard();
            })));
            StickInfo.Add(new Control.Button(startpoint.X + 310, 310, 100, 80, "", 4, new Control.Button.ButtonCallBack((Control.Button b) =>
            {
                SelectButtonIndex = 2;
                isEditKey = true;
                if (ButtonUI.ScreenKeyboard)
                    ButtonUI.ShowScreenKeyboard();
            })));
            StickInfo.Add(new Control.Button(startpoint.X + 490, 310, 100, 80, "", 4, new Control.Button.ButtonCallBack((Control.Button b) =>
            {
                SelectButtonIndex = 3;
                isEditKey = true;
                if (ButtonUI.ScreenKeyboard)
                    ButtonUI.ShowScreenKeyboard();
            })));
            //-------

            NewList.Add(new Control.Button(startpoint.X + 100 - 50, 200 - 50, 100, 100, "圆形按钮", 0, null, new Control.Button.ButtonCallBack((Control.Button b) => {

                Point p = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - 50, 160);
                ControlBase item = new Control.Button(p.X,p.Y,100,100, "圆形按钮",(short)'A',0);
                item.g = gp;
                Touch.MobList.Add(item);
                SaveItem = item;
                DrawUI();
            })));

            NewList.Add(new Control.Button(startpoint.X + 300 - 50, 200 - 40, 100, 80, "方形按钮", 4, null, new Control.Button.ButtonCallBack((Control.Button b) => {
                Point p = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - 50, 160);
                ControlBase item = new Control.Button(p.X, p.Y, 100, 80, "方形按钮", (short)'A', 4);
                item.g = gp;
                Touch.MobList.Add(item);
                SaveItem = item;
                DrawUI();
            })));

            NewList.Add(new Control.Stick(startpoint.X + 500 - 50, 200 - 50, 50, DefKeyList, 0, 110, 0.3F, 0.5F, new Stick.StickCallBack((Stick s) =>
            {
                Point p = new Point(Screen.PrimaryScreen.Bounds.Width / 2 - 50, 160);
                ControlBase item = new Stick(p.X, p.Y, 50, DefKeyList);
                item.g = gp;
                Touch.MobList.Add(item);
                SaveItem = item;
                DrawUI();
            })));




            ContList.Add(new Control.Button(startpoint.X + 10, 0, 80, 50, "添加", 4, new Control.Button.ButtonCallBack((Control.Button b) => 
            {
                isNewList = true;
                DrawNewList();
            })));

            ContList.Add(new Control.Button(startpoint.X + 110, 0, 80, 50, "删除", 4, new Control.Button.ButtonCallBack((Control.Button b) => 
            {
                if (SaveItem != null)
                {
                    Touch.MobList.Remove(SaveItem);
                    SaveItem = null;
                    DrawUI();
                    GC.Collect();
                }
            })));

            ContList.Add(new Control.Button(startpoint.X + 210, 0, 80, 50, "复制", 4, new Control.Button.ButtonCallBack((Control.Button b) => 
            {
                if (SaveItem != null)
                {
                    Point p = new Point(Screen.PrimaryScreen.Bounds.Width / 2-SaveItem.rect.Width/2,200-SaveItem.rect.Height/2);
                    ControlBase item = SaveItem.clone();
                    item.setpos(p);
                    Touch.MobList.Add(item);
                    DrawUI();
                }
            })));

            ContList.Add(new Control.Button(startpoint.X + 310, 0, 80, 50, "放大", 4, new Control.Button.ButtonCallBack((Control.Button b) => 
            {
                if (SaveItem != null)
                {
                    if (SaveItem is Control.Button)
                    {
                        SaveItem.setsize(new Size((ButtonXSize) ? SaveItem.rect.Width + 10 : SaveItem.rect.Width, (ButtonYSize) ? SaveItem.rect.Height + 10 : SaveItem.rect.Height));
                    }
                    else
                    {
                        SaveItem.setsize(new Size(SaveItem.rect.Width + 10, SaveItem.rect.Height + 10));
                    }
                    SaveItem.setpos(SaveItem.rect.Location);
                    DrawUI();
                }
            })));

            ContList.Add(new Control.Button(startpoint.X + 410, 0, 80, 50, "缩小", 4, new Control.Button.ButtonCallBack((Control.Button b) => 
            {

                if (SaveItem != null)
                {
                    if (SaveItem.rect.Width > 10 && SaveItem.rect.Height > 10)
                    {
                        if(SaveItem is Control.Button)
                            SaveItem.setsize(new Size((ButtonXSize) ? SaveItem.rect.Width - 10 : SaveItem.rect.Width, (ButtonYSize) ? SaveItem.rect.Height - 10 : SaveItem.rect.Height));
                        else
                            SaveItem.setsize(new Size(SaveItem.rect.Width - 10, SaveItem.rect.Height - 10));
                        SaveItem.setpos(SaveItem.rect.Location);
                        DrawUI();
                    }
                }
            })));
            ContList.Add(new Control.Button(startpoint.X + 510, 0, 80, 50, "关闭", 4,null, new Control.Button.ButtonCallBack((Control.Button b) => { Hide(); if (CallShowOK != null)CallShowOK(); })));

            foreach (Control.Button b in ContList)
                b.g = gp;
            foreach (Control.ControlBase b in NewList)
                b.g = gp;
            foreach (Control.ControlBase b in ButtonInfo)
                b.g = gp;
            foreach (Control.ControlBase b in StickInfo)
                b.g = gp;
        }

        static void DrawButtonInfo()
        {
            if (SaveItem == null) return;
            if(SaveItem is Control.Button)
            {
                Control.Button b = (Control.Button)SaveItem;

                if ((b.key >= (int)'0' && b.key <= (int)'9'))
                {
                    ButtonInfo[0].SetName("" + (char)b.key); ;
                }
                else
                {
                    ButtonInfo[0].SetName(((Keys)b.key).ToString());
                }
                //if (isEditKey)
                //{
                //    ButtonInfo[0].mode |= Control.Button.Button_IsUse;
                //}
                //else
                //{
                //    ButtonInfo[0].mode &= ~Control.Button.Button_IsUse;
                //}
                ButtonInfo[0].draw();

                

                if (isEditName)
                {
                    ButtonInfo[1].mode |= Control.Button.Button_IsUse;
                }
                else
                {
                    ButtonInfo[1].mode &= ~Control.Button.Button_IsUse;
                }
                ButtonInfo[1].SetName(b.name);
                NameInput.Text = b.name;
                NameInput.SelectAll();
                ButtonInfo[1].draw();
                if ((b.mode & Control.Button.Button_IsRect)!=0)
                {
                    if(ButtonXSize)
                        ButtonInfo[2].mode |= Control.Button.Button_IsUse;
                    else
                        ButtonInfo[2].mode &= ~Control.Button.Button_IsUse;
                    ButtonInfo[2].draw();
                    if(ButtonYSize)
                        ButtonInfo[3].mode |= Control.Button.Button_IsUse;
                    else
                        ButtonInfo[3].mode &=~Control.Button.Button_IsUse;
                    ButtonInfo[3].draw();
                }
            }
            else if(SaveItem is Stick)
            {
                Stick s = (Stick)SaveItem;
                Control.Button Sk;
                for(int i = 0; i < 4; i++)
                {
                    Sk = StickInfo[i];
                    if ((s.keylist[i] >= (int)'0' && s.keylist[i] <= (int)'9'))
                    {
                        Sk.SetName(("" + (char)s.keylist[i]));
                    }
                    else
                    {
                        Sk.SetName(((Keys)s.keylist[i]).ToString());
                    }
                    Sk.draw();
                }
            }
        }

        static SizeF NameSize(string s)
        {
            Bitmap b = new Bitmap(100, 100);
            Graphics graphics = Graphics.FromImage(b);
            SizeF sizeF = graphics.MeasureString(s, DefFont);
            graphics.Dispose();
            b.Dispose();
            return sizeF;
        }
        static Point DrawRectString(Rectangle r,string s)
        {
            SizeF size = NameSize(s);
            Point p = new Point(r.X + (int)(float)(r.Width / 2 - size.Width / 2), r.Y + (int)(float)(r.Height / 2 - size.Height / 2));
            gp.DrawString(s, DefFont, Brushes.Black, p.X, p.Y);
            return p;
        }

        static void DrawNewList()
        {
            foreach(Control.ControlBase b in NewList)
            {
                b.draw();
                if (b is Stick)
                {

                    if (StickXpos != -1 && StickYpos != -1)
                    {
                        gp.DrawString("摇杆", DefFont, Brushes.Black, StickXpos, StickYpos);
                    }
                    else
                    {
                        Point p = DrawRectString(b.rect, "摇杆");
                        StickXpos = p.X;
                        StickYpos = p.Y;
                    }
                }
            }
        }

        static void DrawButton()
        {
            foreach (Control.Button b in ContList)
            {
                b.draw();
            }
        }



        static void DrawUI()
        {
            gp.Clear(Color.Black);
            gp.FillRectangle(Brushes.Gray, rect);
            DrawButton();
            Touch.OnDraw();
            if (SaveItem != null)
            {
                Rectangle rc = new Rectangle(SaveItem.rect.X-5,SaveItem.rect.Y-5,SaveItem.rect.Width+10,SaveItem.rect.Height+10);
                DrawRoundedRectangle(gp, Pens.Red, rc,15);
                DrawButtonInfo();
            }
            Sgp.DrawImage(DrawMap, 0, 0);
        }



        public static void Show()
        {
            foreach (ControlBase b in Touch.MobList)
            {
                b.g = gp;
            }
            DrawUI();
        }

        public static void Hide()
        {
            gp.Clear(Color.Fuchsia);
            Touch.OnDraw();
            foreach (ControlBase b in Touch.MobList)
            {
                b.g = Sgp;
            }
            NameInput.Parent.Focus();
            NameInput.Visible = false;
        }

        public static void MouseDown(Point p)
        {
            if (isEditName)
            {
                NameInput.Visible = false;
                isEditName = false;
                DrawDelay.Start();
            }
            if (isEditKey)
            {
                ButtonInfo[0].mode &= ~Control.Button.Button_IsUse;
                isEditKey = false;
            }
            //isEditName = isEditKey = false;
            foreach (Control.Button b in ContList)
            {
                if (b.check(p))
                {
                    SelectItem = b;
                    SelectBlock = false;
                    b.draw();
                    Sgp.DrawImage(DrawMap, 0, 0);
                    return;
                }
            }

            if (isNewList)
            {
                foreach (Control.ControlBase b in NewList)
                {
                    if (b.check(p))
                    {
                        b.checkup();
                        isNewList = false;
                        return;
                    }
                }
            }

            foreach (ControlBase s in Touch.MobList)
            {
                if (s.checkRect(p))
                {
                    ButtonXSize = true;
                    ButtonYSize = true;
                    xoffset = p.X - s.rect.X;
                    yoffset = p.Y - s.rect.Y;
                    SaveItem = SelectItem = s;
                    SelectBlock = true;
                    DrawUI();
                    return;
                }
            }
            if (SaveItem != null)
            {
                if (SaveItem is Stick)
                {
                    foreach (Control.ControlBase b in StickInfo)
                    {
                        if (b.check(p))
                        {
                            DrawUI();
                            return;
                        }
                    }
                }
                else if (SaveItem is Control.Button)
                {
                    foreach (Control.ControlBase b in ButtonInfo)
                    {
                        if (b.check(p))
                        {
                            DrawUI();
                            return;
                        }
                    }

                }
            }
            SaveItem = SelectItem = null;
            DrawUI();
        }

        public static void MouseMove(Point p)
        {
            if (SelectItem != null && SelectBlock) 
            {
                p.X-=xoffset;
                p.Y-=yoffset;
                SelectItem.setpos(p);
                DrawUI();
            }
        }

        public static void MouseUp()
        {
            if (SelectItem != null)
            {
                if (!SelectBlock)
                {
                    SelectItem.checkup();
                    SelectItem.draw();
                    Sgp.DrawImage(DrawMap, 0, 0);
                }
                SelectItem = null;
            }
        }

        public static void KeyPress(KeyEventArgs key)
        {
            if (isEditKey)
            {
                int keyval = (int)key.KeyCode;
                string s;
                if ((keyval >= (int)'0' && keyval <= (int)'9'))
                {
                    s = "" + (char)keyval;
                }
                else
                {
                    s = ((Keys)keyval).ToString();
                }
                if (SaveItem is Control.Button)
                {
                    ((Control.Button)SaveItem).key = (short)keyval;
                    Control.Button b = ButtonInfo[0];
                    b.SetName(s);
                    b.checkup();
                    if (ButtonUI.ScreenKeyboard)
                        ButtonUI.CloseScreenKeyboard();
                    //b.mode &= ~Control.Button.Button_IsUse;
                    DrawUI();
                }
                else if (SaveItem is Control.Stick)
                {
                    ((Control.Stick)SaveItem).keylist[SelectButtonIndex] = (byte)keyval;
                    Control.Button b = StickInfo[SelectButtonIndex];
                    b.SetName(s);
                    //b.mode &= ~Control.Button.Button_IsUse;
                    b.checkup();
                    if (ButtonUI.ScreenKeyboard)
                        ButtonUI.CloseScreenKeyboard();
                    DrawUI();
                }
                isEditKey = false;
            }
            else if (isEditName)
            {
                if (key.KeyCode == Keys.Enter)
                {
                    ((Control.Button)SaveItem).SetName(NameInput.Text);
                    NameInput.Visible = false;
                    isEditName = false;
                    if (ButtonUI.ScreenKeyboard)
                        ButtonUI.CloseScreenKeyboard();
                    DrawDelay.Start();
                }
            }
        }

    }
}
