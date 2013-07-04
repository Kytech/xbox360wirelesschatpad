using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Xbox360WirelessChatpad
{
    static class MainApplication
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window_Main());
        }
    }
}
