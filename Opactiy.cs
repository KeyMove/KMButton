using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KMButton
{
    public partial class Opactiy : Form
    {
        int v=70;
        public int Value
        {
            get { return v; }
            set { this.Op.Value = v = value; }
        }

        public Opactiy()
        {
            InitializeComponent();
            this.Op.Value = 70;
            this.Width = 65;
        }

        private void Op_Scroll(object sender, EventArgs e)
        {
            v = this.Op.Value;
        }

        private void Show(object sender, EventArgs e)
        {
            //this.TopMost = true;
            this.Width = 65;
        }

        public Point Pos
        {
            get { return this.Location; }
            set
            {
                this.Left = value.X - this.Width;
                this.Top = value.Y - this.Height;
            }
        }
    }
}
