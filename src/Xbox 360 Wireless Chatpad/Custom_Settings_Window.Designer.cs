namespace Xbox360WirelessChatpad
{
    partial class Custom_Settings_Window
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
            this.profileSelector = new System.Windows.Forms.OpenFileDialog();
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.newprofile = new System.Windows.Forms.Button();
            this.filePath = new System.Windows.Forms.TextBox();
            this.browse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.editButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // profileSelector
            // 
            this.profileSelector.DefaultExt = "ini";
            this.profileSelector.Filter = "INI files (*.ini)|*.ini";
            this.profileSelector.Title = "Select Controller Profile";
            this.profileSelector.FileOk += new System.ComponentModel.CancelEventHandler(this.profileSelector_FileOk);
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(14, 55);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 1;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(370, 55);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 0;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            // 
            // newprofile
            // 
            this.newprofile.Location = new System.Drawing.Point(271, 55);
            this.newprofile.Name = "newprofile";
            this.newprofile.Size = new System.Drawing.Size(93, 23);
            this.newprofile.TabIndex = 2;
            this.newprofile.Text = "New Profile...";
            this.newprofile.UseVisualStyleBackColor = true;
            // 
            // filePath
            // 
            this.filePath.Location = new System.Drawing.Point(14, 29);
            this.filePath.Name = "filePath";
            this.filePath.Size = new System.Drawing.Size(350, 20);
            this.filePath.TabIndex = 3;
            // 
            // browse
            // 
            this.browse.Location = new System.Drawing.Point(370, 27);
            this.browse.Name = "browse";
            this.browse.Size = new System.Drawing.Size(75, 23);
            this.browse.TabIndex = 4;
            this.browse.Text = "Browse...";
            this.browse.UseVisualStyleBackColor = true;
            this.browse.Click += new System.EventHandler(this.browse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Select Controller File";
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(190, 55);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 6;
            this.editButton.Text = "Edit...";
            this.editButton.UseVisualStyleBackColor = true;
            // 
            // Custom_Settings_Window
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(457, 90);
            this.ControlBox = false;
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browse);
            this.Controls.Add(this.filePath);
            this.Controls.Add(this.newprofile);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Custom_Settings_Window";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Custom Controller Settings";
            this.Load += new System.EventHandler(this.Custom_Settings_Window_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog profileSelector;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button newprofile;
        private System.Windows.Forms.TextBox filePath;
        private System.Windows.Forms.Button browse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button editButton;
    }
}