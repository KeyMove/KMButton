namespace KMButton
{
    partial class ButtonUI
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ButtonUI));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.CMUM = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.设置按键ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存按键ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.加载按键ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NameInput = new System.Windows.Forms.TextBox();
            this.DebugInfo = new System.Windows.Forms.TextBox();
            this.CMUM.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.CMUM;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "KMButton";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.opo);
            // 
            // CMUM
            // 
            this.CMUM.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CMUM.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置按键ToolStripMenuItem,
            this.保存按键ToolStripMenuItem,
            this.加载按键ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.CMUM.Name = "CMUM";
            this.CMUM.Size = new System.Drawing.Size(169, 132);
            // 
            // 设置按键ToolStripMenuItem
            // 
            this.设置按键ToolStripMenuItem.Name = "设置按键ToolStripMenuItem";
            this.设置按键ToolStripMenuItem.Size = new System.Drawing.Size(168, 32);
            this.设置按键ToolStripMenuItem.Text = "设置按键";
            this.设置按键ToolStripMenuItem.Click += new System.EventHandler(this.设置按键ToolStripMenuItem_Click);
            // 
            // 保存按键ToolStripMenuItem
            // 
            this.保存按键ToolStripMenuItem.Name = "保存按键ToolStripMenuItem";
            this.保存按键ToolStripMenuItem.Size = new System.Drawing.Size(168, 32);
            this.保存按键ToolStripMenuItem.Text = "保存按键";
            this.保存按键ToolStripMenuItem.Click += new System.EventHandler(this.保存按键ToolStripMenuItem_Click);
            // 
            // 加载按键ToolStripMenuItem
            // 
            this.加载按键ToolStripMenuItem.Name = "加载按键ToolStripMenuItem";
            this.加载按键ToolStripMenuItem.Size = new System.Drawing.Size(168, 32);
            this.加载按键ToolStripMenuItem.Text = "加载按键";
            this.加载按键ToolStripMenuItem.Click += new System.EventHandler(this.加载按键ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(168, 32);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // NameInput
            // 
            this.NameInput.BackColor = System.Drawing.Color.Gray;
            this.NameInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NameInput.Font = new System.Drawing.Font("宋体", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.NameInput.Location = new System.Drawing.Point(12, 12);
            this.NameInput.Name = "NameInput";
            this.NameInput.Size = new System.Drawing.Size(80, 34);
            this.NameInput.TabIndex = 0;
            this.NameInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NameInput.Visible = false;
            // 
            // DebugInfo
            // 
            this.DebugInfo.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.DebugInfo.Location = new System.Drawing.Point(447, 12);
            this.DebugInfo.Multiline = true;
            this.DebugInfo.Name = "DebugInfo";
            this.DebugInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DebugInfo.Size = new System.Drawing.Size(124, 434);
            this.DebugInfo.TabIndex = 1;
            this.DebugInfo.Click += new System.EventHandler(this.ClickLook);
            // 
            // ButtonUI
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Fuchsia;
            this.ClientSize = new System.Drawing.Size(583, 458);
            this.ControlBox = false;
            this.Controls.Add(this.DebugInfo);
            this.Controls.Add(this.NameInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ButtonUI";
            this.Opacity = 0.7D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.Load += new System.EventHandler(this.ButtonUI_Load);
            this.CMUM.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip CMUM;
        private System.Windows.Forms.ToolStripMenuItem 设置按键ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.TextBox NameInput;
        private System.Windows.Forms.ToolStripMenuItem 保存按键ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 加载按键ToolStripMenuItem;
        private System.Windows.Forms.TextBox DebugInfo;
    }
}

