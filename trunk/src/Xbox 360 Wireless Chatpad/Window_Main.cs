using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace Xbox360WirelessChatpad
{
    // Necessary to update the form items in a thread-safe manner
    delegate void controllerDisconnectCallback();
    delegate void controllerConnectCallback();
    delegate void mouseModeLabelCallback(string labelText);
    delegate void logCallback(string logMessage);

    public partial class Window_Main : Form
    {
        // The Xbox Wireless Receiver connected to the computer via USB
        private Receiver xboxReceiver;

        // The Xbox Wireless Controller comprised of a Gamepad (Joystick and Buttons)
        // and a Chatpad (Attached Keyboard)
        private Controller xboxController;

        public Window_Main()
        {
            InitializeComponent();
        }

        private void Window_Main_Load(object sender, EventArgs e)
        {
            // Load the keyboardType configuration variable into the radio selections
            switch (Properties.Settings.Default.keyboardType)
            {
                case "QWERTY":
                    qwertyButton.Checked = true;
                    break;
                case "QWERTZ":
                    qwertzButton.Checked = true;
                    break;
                case "AZERTY":
                    azertyButton.Checked = true;
                    break;
                default:
                    // Use QWERTY if the configuration file has junk data
                    qwertyButton.Checked = true;
                    break;
            }

            // Load the ffxivFlag configuration variable into the checkbox
            if (Properties.Settings.Default.ffxivFlag)
                ffxivFlag.Checked = true;
            else
                ffxivFlag.Checked = false;

            // Instantiates the Controller and loads the keyboard and trigger configuration
            xboxController = new Controller(this);
            xboxController.configureChatpad(Properties.Settings.Default.keyboardType);
            xboxController.configureGamepad(Properties.Settings.Default.triggerType);

            // Instantiates the Receiver
            xboxReceiver = new Receiver(this);

            // Setup classes
            xboxReceiver.registerClasses(xboxController);

            // Connect to the Wireless Receiver
            xboxReceiver.connectReceiver();
        }

        private void Window_Main_Resize(object sender, EventArgs e)
        {
            // Hides the window and minimizes to the System Tray upon Resize
            if (WindowState == FormWindowState.Minimized)
                Hide();
        }

        private void Window_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Cleanup the Wireless Receiver
                xboxReceiver.killReceiver();

                // Save the configuraiton file variables
                Properties.Settings.Default.ffxivFlag = ffxivFlag.Checked;
                Properties.Settings.Default.Save();
            }
            catch
            {
                // If we have an exception, force the process to close
                System.Environment.Exit(0);
            }
        }

        private void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            // Moves the window to the Taskbar when clicking the System Tray icon
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            // Close the Window
            this.Close();
        }

        private void appLogTextbox_TextChanged(object sender, EventArgs e)
        {
            // Automatically scrolls to the end of the textbox whenever the text changes
            appLogTextbox.SelectionStart = appLogTextbox.Text.Length;
            appLogTextbox.ScrollToCaret();
            appLogTextbox.Refresh();
        }

        private void keyboardType_Selected(object sender, EventArgs e)
        {
            // Set the keyboardType setting to the selected radio button
            if (((RadioButton)sender).Checked)
                Properties.Settings.Default.keyboardType = ((RadioButton)sender).Text;
        }

        private void chatpadTextBox_Enter(object sender, EventArgs e)
        {
            // Removes out pre-populated message in the chatpadTextBox
            chatpadTextBox.TextAlign = HorizontalAlignment.Left;
            chatpadTextBox.Text = "";

            // Removes the event handler so we don't continually remove the text
            chatpadTextBox.Enter -= chatpadTextBox_Enter;
        }

        private void leftDeadzone_ValueChanged(object sender, EventArgs e)
        {
            // Set the left deadzone to slider percentage of 32767, the maximum
            // value of an analog stick
            xboxController.deadzoneL = (int)Math.Round(leftDeadzone.Value * 327.67);

            // Set the left deadzone label to the new value
            leftDeadzonePercentLabel.Text = leftDeadzone.Value.ToString() + "%";

            // Set the left deadzone configuration property to the new value
            Properties.Settings.Default.leftDeadzone = leftDeadzone.Value;
        }

        private void rightDeadzone_ValueChanged(object sender, EventArgs e)
        {
            // Set the right deadzone to slider percentage of 32767, the maximum
            // value of an analog stick
            xboxController.deadzoneR = (int)Math.Round(rightDeadzone.Value * 327.67);

            // Set the right deadzone label to the new value
            rightDeadzonePercentLabel.Text = rightDeadzone.Value.ToString() + "%";

            // Set the right deadzone configuration property to the new value
            Properties.Settings.Default.rightDeadzone = rightDeadzone.Value;
        }

        public void controllerConnected()
        {
            // Disable the Keyboard Type visibility
            keyboardTypeGroupBox.Enabled = false;

            // Disable the FFXIV check box
            ffxivFlag.Enabled = false;

            // Enable the chatpad test box
            chatpadTextBox.Enabled = true;

            // Enable the mouse mode visibility
            mouseModeLabel.Enabled = true;

            // Set the Controller deadzones to configuration values
            // and enabled the deadzone visibility.
            deadzoneGroupBox.Enabled = true;
            leftDeadzone.Value = Properties.Settings.Default.leftDeadzone;
            rightDeadzone.Value = Properties.Settings.Default.rightDeadzone;
            xboxController.deadzoneL = (int)Math.Round(leftDeadzone.Value * 327.67);
            xboxController.deadzoneR = (int)Math.Round(rightDeadzone.Value * 327.67);
        }

        public void controllerDisconnected()
        {
            // Reset the keyabord type box visibility
            keyboardTypeGroupBox.Enabled = true;

            // Reset the FFXIV check box visibility
            ffxivFlag.Enabled = true;

            // Reset the chatpad test box
            chatpadTextBox.Enabled = false;
            chatpadTextBox.TextAlign = HorizontalAlignment.Center;
            chatpadTextBox.Text = "-Test Chatpad Here-";
            chatpadTextBox.Enter += chatpadTextBox_Enter;

            // Reset the mouse mode visibility
            mouseModeLabel.Enabled = false;

            // Reset the deadzone group visibility
            deadzoneGroupBox.Enabled = false;
        }

        public void mouseModeLabelUpdate(string labelText)
        {
            mouseModeLabel.Text = labelText;
        }

        public void logUpdate(string logMessage)
        {
            appLogTextbox.Text += logMessage;
        }
    }
}
