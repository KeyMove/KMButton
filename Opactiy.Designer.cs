namespace KMButton
{
    partial class Opactiy
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Op = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Op)).BeginInit();
            this.SuspendLayout();
            // 
            // Op
            // 
            this.Op.Location = new System.Drawing.Point(12, 2);
            this.Op.Maximum = 100;
            this.Op.Name = "Op";
            this.Op.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.Op.Size = new System.Drawing.Size(45, 159);
            this.Op.TabIndex = 0;
            this.Op.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.Op.Scroll += new System.EventHandler(this.Op_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(7, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "透明度";
            // 
            // Opactiy
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(65, 187);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Op);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Opactiy";
            this.ShowInTaskbar = false;
            this.Text = "Opactiy";
            this.Shown += new System.EventHandler(this.Show);
            ((System.ComponentModel.ISupportInitialize)(this.Op)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TrackBar Op;
        private System.Windows.Forms.Label label1;
    }
}