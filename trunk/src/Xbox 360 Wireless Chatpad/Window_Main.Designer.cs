namespace Xbox360WirelessChatpad
{
    partial class Window_Main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window_Main));
            this.appLogTextbox = new System.Windows.Forms.TextBox();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayIconMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectButton = new System.Windows.Forms.Button();
            this.chatpadTextBox = new System.Windows.Forms.TextBox();
            this.deadzoneGroupBox = new System.Windows.Forms.GroupBox();
            this.rightDeadzonePercentLabel = new System.Windows.Forms.Label();
            this.leftDeadzonePercentLabel = new System.Windows.Forms.Label();
            this.rightDeadzone = new System.Windows.Forms.TrackBar();
            this.rightDeadzoneLabel = new System.Windows.Forms.Label();
            this.leftDeadzone = new System.Windows.Forms.TrackBar();
            this.leftDeadzoneLabel = new System.Windows.Forms.Label();
            this.keyboardTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.azertyButton = new System.Windows.Forms.RadioButton();
            this.qwertzButton = new System.Windows.Forms.RadioButton();
            this.qwertyButton = new System.Windows.Forms.RadioButton();
            this.ffxivFlag = new System.Windows.Forms.CheckBox();
            this.mouseModeFlag = new System.Windows.Forms.CheckBox();
            this.trayIconMenu.SuspendLayout();
            this.deadzoneGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightDeadzone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftDeadzone)).BeginInit();
            this.keyboardTypeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // appLogTextbox
            // 
            this.appLogTextbox.Location = new System.Drawing.Point(176, 15);
            this.appLogTextbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.appLogTextbox.Multiline = true;
            this.appLogTextbox.Name = "appLogTextbox";
            this.appLogTextbox.ReadOnly = true;
            this.appLogTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.appLogTextbox.Size = new System.Drawing.Size(412, 166);
            this.appLogTextbox.TabIndex = 0;
            this.appLogTextbox.TextChanged += new System.EventHandler(this.appLogTextbox_TextChanged);
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayIconMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Xbox 360 Wireless Chatpad";
            this.trayIcon.Visible = true;
            this.trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);
            // 
            // trayIconMenu
            // 
            this.trayIconMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitMenuItem});
            this.trayIconMenu.Name = "trayIconMenu";
            this.trayIconMenu.Size = new System.Drawing.Size(103, 28);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(102, 24);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(16, 188);
            this.connectButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(152, 24);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect Controller";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // chatpadTextBox
            // 
            this.chatpadTextBox.Enabled = false;
            this.chatpadTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chatpadTextBox.Location = new System.Drawing.Point(176, 188);
            this.chatpadTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chatpadTextBox.Name = "chatpadTextBox";
            this.chatpadTextBox.Size = new System.Drawing.Size(413, 23);
            this.chatpadTextBox.TabIndex = 3;
            this.chatpadTextBox.Text = "-Test Chatpad Here-";
            this.chatpadTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.chatpadTextBox.Enter += new System.EventHandler(this.chatpadTextBox_Enter);
            // 
            // deadzoneGroupBox
            // 
            this.deadzoneGroupBox.Controls.Add(this.rightDeadzonePercentLabel);
            this.deadzoneGroupBox.Controls.Add(this.leftDeadzonePercentLabel);
            this.deadzoneGroupBox.Controls.Add(this.rightDeadzone);
            this.deadzoneGroupBox.Controls.Add(this.rightDeadzoneLabel);
            this.deadzoneGroupBox.Controls.Add(this.leftDeadzone);
            this.deadzoneGroupBox.Controls.Add(this.leftDeadzoneLabel);
            this.deadzoneGroupBox.Enabled = false;
            this.deadzoneGroupBox.Location = new System.Drawing.Point(597, 15);
            this.deadzoneGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.deadzoneGroupBox.Name = "deadzoneGroupBox";
            this.deadzoneGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.deadzoneGroupBox.Size = new System.Drawing.Size(196, 198);
            this.deadzoneGroupBox.TabIndex = 4;
            this.deadzoneGroupBox.TabStop = false;
            this.deadzoneGroupBox.Text = "Analog Deadzones";
            // 
            // rightDeadzonePercentLabel
            // 
            this.rightDeadzonePercentLabel.Location = new System.Drawing.Point(148, 111);
            this.rightDeadzonePercentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rightDeadzonePercentLabel.Name = "rightDeadzonePercentLabel";
            this.rightDeadzonePercentLabel.Size = new System.Drawing.Size(40, 16);
            this.rightDeadzonePercentLabel.TabIndex = 5;
            this.rightDeadzonePercentLabel.Text = "0%";
            this.rightDeadzonePercentLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // leftDeadzonePercentLabel
            // 
            this.leftDeadzonePercentLabel.Location = new System.Drawing.Point(148, 26);
            this.leftDeadzonePercentLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.leftDeadzonePercentLabel.Name = "leftDeadzonePercentLabel";
            this.leftDeadzonePercentLabel.Size = new System.Drawing.Size(40, 16);
            this.leftDeadzonePercentLabel.TabIndex = 4;
            this.leftDeadzonePercentLabel.Text = "0%";
            this.leftDeadzonePercentLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // rightDeadzone
            // 
            this.rightDeadzone.Location = new System.Drawing.Point(9, 130);
            this.rightDeadzone.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rightDeadzone.Maximum = 30;
            this.rightDeadzone.Name = "rightDeadzone";
            this.rightDeadzone.Size = new System.Drawing.Size(179, 56);
            this.rightDeadzone.TabIndex = 3;
            this.rightDeadzone.ValueChanged += new System.EventHandler(this.rightDeadzone_ValueChanged);
            // 
            // rightDeadzoneLabel
            // 
            this.rightDeadzoneLabel.AutoSize = true;
            this.rightDeadzoneLabel.Location = new System.Drawing.Point(8, 111);
            this.rightDeadzoneLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rightDeadzoneLabel.Name = "rightDeadzoneLabel";
            this.rightDeadzoneLabel.Size = new System.Drawing.Size(123, 17);
            this.rightDeadzoneLabel.TabIndex = 2;
            this.rightDeadzoneLabel.Text = "Right Analog Stick";
            // 
            // leftDeadzone
            // 
            this.leftDeadzone.Location = new System.Drawing.Point(9, 46);
            this.leftDeadzone.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.leftDeadzone.Maximum = 30;
            this.leftDeadzone.Name = "leftDeadzone";
            this.leftDeadzone.Size = new System.Drawing.Size(179, 56);
            this.leftDeadzone.TabIndex = 1;
            this.leftDeadzone.ValueChanged += new System.EventHandler(this.leftDeadzone_ValueChanged);
            // 
            // leftDeadzoneLabel
            // 
            this.leftDeadzoneLabel.AutoSize = true;
            this.leftDeadzoneLabel.Location = new System.Drawing.Point(8, 26);
            this.leftDeadzoneLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.leftDeadzoneLabel.Name = "leftDeadzoneLabel";
            this.leftDeadzoneLabel.Size = new System.Drawing.Size(114, 17);
            this.leftDeadzoneLabel.TabIndex = 0;
            this.leftDeadzoneLabel.Text = "Left Analog Stick";
            // 
            // keyboardTypeGroupBox
            // 
            this.keyboardTypeGroupBox.Controls.Add(this.azertyButton);
            this.keyboardTypeGroupBox.Controls.Add(this.qwertzButton);
            this.keyboardTypeGroupBox.Controls.Add(this.qwertyButton);
            this.keyboardTypeGroupBox.Location = new System.Drawing.Point(16, 15);
            this.keyboardTypeGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.keyboardTypeGroupBox.Name = "keyboardTypeGroupBox";
            this.keyboardTypeGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.keyboardTypeGroupBox.Size = new System.Drawing.Size(152, 112);
            this.keyboardTypeGroupBox.TabIndex = 5;
            this.keyboardTypeGroupBox.TabStop = false;
            this.keyboardTypeGroupBox.Text = "Keyboard Type";
            // 
            // azertyButton
            // 
            this.azertyButton.AutoSize = true;
            this.azertyButton.Location = new System.Drawing.Point(8, 80);
            this.azertyButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.azertyButton.Name = "azertyButton";
            this.azertyButton.Size = new System.Drawing.Size(104, 21);
            this.azertyButton.TabIndex = 2;
            this.azertyButton.TabStop = true;
            this.azertyButton.Text = "A Z E R T Y";
            this.azertyButton.UseVisualStyleBackColor = true;
            this.azertyButton.CheckedChanged += new System.EventHandler(this.keyboardType_Selected);
            // 
            // qwertzButton
            // 
            this.qwertzButton.AutoSize = true;
            this.qwertzButton.Location = new System.Drawing.Point(8, 50);
            this.qwertzButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.qwertzButton.Name = "qwertzButton";
            this.qwertzButton.Size = new System.Drawing.Size(110, 21);
            this.qwertzButton.TabIndex = 1;
            this.qwertzButton.TabStop = true;
            this.qwertzButton.Text = "Q W E R T Z";
            this.qwertzButton.UseVisualStyleBackColor = true;
            this.qwertzButton.CheckedChanged += new System.EventHandler(this.keyboardType_Selected);
            // 
            // qwertyButton
            // 
            this.qwertyButton.AutoSize = true;
            this.qwertyButton.Location = new System.Drawing.Point(8, 21);
            this.qwertyButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.qwertyButton.Name = "qwertyButton";
            this.qwertyButton.Size = new System.Drawing.Size(110, 21);
            this.qwertyButton.TabIndex = 0;
            this.qwertyButton.TabStop = true;
            this.qwertyButton.Text = "Q W E R T Y";
            this.qwertyButton.UseVisualStyleBackColor = true;
            this.qwertyButton.CheckedChanged += new System.EventHandler(this.keyboardType_Selected);
            // 
            // ffxivFlag
            // 
            this.ffxivFlag.AutoSize = true;
            this.ffxivFlag.Location = new System.Drawing.Point(24, 159);
            this.ffxivFlag.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ffxivFlag.Name = "ffxivFlag";
            this.ffxivFlag.Size = new System.Drawing.Size(139, 21);
            this.ffxivFlag.TabIndex = 6;
            this.ffxivFlag.Text = "Final Fantasy XIV";
            this.ffxivFlag.UseVisualStyleBackColor = true;
            // 
            // mouseModeFlag
            // 
            this.mouseModeFlag.AutoSize = true;
            this.mouseModeFlag.Location = new System.Drawing.Point(24, 130);
            this.mouseModeFlag.Margin = new System.Windows.Forms.Padding(4);
            this.mouseModeFlag.Name = "mouseModeFlag";
            this.mouseModeFlag.Size = new System.Drawing.Size(111, 21);
            this.mouseModeFlag.TabIndex = 7;
            this.mouseModeFlag.Text = "Mouse Mode";
            this.mouseModeFlag.UseVisualStyleBackColor = true;
            // 
            // Window_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 226);
            this.Controls.Add(this.mouseModeFlag);
            this.Controls.Add(this.ffxivFlag);
            this.Controls.Add(this.keyboardTypeGroupBox);
            this.Controls.Add(this.deadzoneGroupBox);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.appLogTextbox);
            this.Controls.Add(this.chatpadTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Window_Main";
            this.Text = "Xbox 360 Wireless Chatpad";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Window_Main_FormClosing);
            this.Load += new System.EventHandler(this.Window_Main_Load);
            this.Resize += new System.EventHandler(this.Window_Main_Resize);
            this.trayIconMenu.ResumeLayout(false);
            this.deadzoneGroupBox.ResumeLayout(false);
            this.deadzoneGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightDeadzone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftDeadzone)).EndInit();
            this.keyboardTypeGroupBox.ResumeLayout(false);
            this.keyboardTypeGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox appLogTextbox;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip trayIconMenu;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox chatpadTextBox;
        private System.Windows.Forms.GroupBox deadzoneGroupBox;
        private System.Windows.Forms.TrackBar rightDeadzone;
        private System.Windows.Forms.Label rightDeadzoneLabel;
        private System.Windows.Forms.TrackBar leftDeadzone;
        private System.Windows.Forms.Label leftDeadzoneLabel;
        private System.Windows.Forms.Label rightDeadzonePercentLabel;
        private System.Windows.Forms.Label leftDeadzonePercentLabel;
        private System.Windows.Forms.GroupBox keyboardTypeGroupBox;
        private System.Windows.Forms.RadioButton azertyButton;
        private System.Windows.Forms.RadioButton qwertzButton;
        private System.Windows.Forms.RadioButton qwertyButton;
        private System.Windows.Forms.CheckBox ffxivFlag;
        private System.Windows.Forms.CheckBox mouseModeFlag;
    }
}

