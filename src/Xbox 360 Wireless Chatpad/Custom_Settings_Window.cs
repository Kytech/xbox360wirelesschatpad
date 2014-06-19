using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Xbox360WirelessChatpad
{
    public partial class Custom_Settings_Window : Form
    {
        //stores the value of the controller number being configured
        private int controllerNumber;

        //stores the path for the controller profile
        public string profilePath;

        public Custom_Settings_Window(int ctrlNum)
        {
            InitializeComponent();
            this.controllerNumber = ctrlNum;
        }

        private void Custom_Settings_Window_Load(object sender, EventArgs e)
        {
        }

        private void profileSelector_FileOk(object sender, CancelEventArgs e)
        {
            filePath.Text = profileSelector.FileName;
            profileSelector.FileName = this.profilePath;
            profileSelector.Dispose();
        }

        private void browse_Click(object sender, EventArgs e)
        {
            string cd = Environment.CurrentDirectory;
            if (Directory.Exists(cd + @"\Profiles") == false)
            {
                Directory.CreateDirectory(cd + @"Profiles");
            }
            profileSelector.InitialDirectory = cd + @"\Profiles";
            profileSelector.ShowDialog();
        }
    }
}
