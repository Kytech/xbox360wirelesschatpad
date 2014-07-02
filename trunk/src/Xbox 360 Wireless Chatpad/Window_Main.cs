using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace Xbox360WirelessChatpad
{
    // Necessary to update the form items in a thread-safe manner
    delegate void controllerDisconnectCallback(int ctrl);
    delegate void controllerConnectCallback(int ctrl);
    delegate void mouseModeLabelCallback(int ctrl, bool modeStatus);
    delegate void logCallback(string message);

    public partial class Window_Main : Form
    {
        // The Xbox Wireless Receiver connected to the computer via USB
        private Receiver xboxReceiver;

        // The Xbox Wireless Controllers. Each controller is comprised of a
        // Gamepad (Joystick and Buttons) and a Chatpad (Attached Keyboard)
        private Controller[] xboxControllers = new Controller[4];

        public Window_Main()
        {
            // Instantiate the Controllers
            xboxControllers[0] = new Controller(this);
            xboxControllers[1] = new Controller(this);
            xboxControllers[2] = new Controller(this);
            xboxControllers[3] = new Controller(this);

            // Initialize the Form Components
            InitializeComponent();
        }

        private void Window_Main_Load(object sender, EventArgs e)
        {
            // Load the Configuration Settings
            Properties.Settings.Default.Reload();

            // Load Controller 1 Configuration
            // Keyboard Type
            xboxControllers[0].configureChatpad(Properties.Settings.Default.ctrl1KeyboardType);
            switch (Properties.Settings.Default.ctrl1KeyboardType)
            {
                case "Q W E RT Y":
                    ctrl1QwertyButton.Checked = true;
                    break;
                case "Q W E R T Z":
                    ctrl1QwertzButton.Checked = true;
                    break;
                case "A Z E R T Y":
                    ctrl1AzertyButton.Checked = true;
                    break;
                default:
                    // Use QWERTY if the configuration file has junk data
                    ctrl1QwertyButton.Checked = true;
                    break;
            }

            // Trigger Type
            if (!Properties.Settings.Default.ctrl1CustomConfig || Properties.Settings.Default.ctrl1Profile == "")
            {
                xboxControllers[0].configureGamepad(Properties.Settings.Default.ctrl1TriggerAsButton);
                if (Properties.Settings.Default.ctrl1TriggerAsButton)
                    ctrl1TriggerTypeBox.Checked = true;
                else
                    ctrl1TriggerTypeBox.Checked = false;
            }

            // Mouse Mode
            xboxControllers[0].mouseModeFlag = Properties.Settings.Default.ctrl1MouseMode;
            if (Properties.Settings.Default.ctrl1MouseMode)
                ctrl1MouseModeBox.Checked = true;
            else
                ctrl1MouseModeBox.Checked = false;

            // Deadzones
            ctrl1LeftDeadzone.Value = Properties.Settings.Default.ctrl1DeadzoneL;
            ctrl1RightDeadzone.Value = Properties.Settings.Default.ctrl1DeadzoneR;

            // Custom Mappings
            if (Properties.Settings.Default.ctrl1CustomConfig && Properties.Settings.Default.ctrl1Profile != "")
            {
                if (File.Exists(Properties.Settings.Default.ctrl1Profile))
                {
                    xboxControllers[0].configureGamepad(Properties.Settings.Default.ctrl1Profile);
                    ctrl1CustomBox.Checked = true; 
                }
                else
                {
                    xboxControllers[0].configureGamepad(Properties.Settings.Default.ctrl1TriggerAsButton);
                    Properties.Settings.Default.ctrl1Profile = "";
                    ctrl1CustomBox.Checked = false;
                }
            }
            else
                ctrl1CustomBox.Checked = false;


            // Load Controller 2 Configuration
            // Keyboard Type
            xboxControllers[1].configureChatpad(Properties.Settings.Default.ctrl2KeyboardType);
            switch (Properties.Settings.Default.ctrl2KeyboardType)
            {
                case "Q W E RT Y":
                    ctrl2QwertyButton.Checked = true;
                    break;
                case "Q W E R T Z":
                    ctrl2QwertzButton.Checked = true;
                    break;
                case "A Z E R T Y":
                    ctrl2AzertyButton.Checked = true;
                    break;
                default:
                    // Use QWERTY if the configuration file has junk data
                    ctrl2QwertyButton.Checked = true;
                    break;
            }

            // Trigger Type
            if (!Properties.Settings.Default.ctrl2CustomConfig || Properties.Settings.Default.ctrl2Profile == "")
            {
                xboxControllers[1].configureGamepad(Properties.Settings.Default.ctrl2TriggerAsButton);
                if (Properties.Settings.Default.ctrl2TriggerAsButton)
                    ctrl2TriggerTypeBox.Checked = true;
                else
                    ctrl2TriggerTypeBox.Checked = false;
            }

            // Mouse Mode
            xboxControllers[1].mouseModeFlag = Properties.Settings.Default.ctrl2MouseMode;
            if (Properties.Settings.Default.ctrl2MouseMode)
                ctrl2MouseModeBox.Checked = true;
            else
                ctrl2MouseModeBox.Checked = false;

            // Deadzones
            ctrl2LeftDeadzone.Value = Properties.Settings.Default.ctrl2DeadzoneL;
            ctrl2RightDeadzone.Value = Properties.Settings.Default.ctrl2DeadzoneR;

            // Custom Mappings
            if (Properties.Settings.Default.ctrl2CustomConfig && Properties.Settings.Default.ctrl2Profile != "")
            {
                if (File.Exists(Properties.Settings.Default.ctrl2Profile))
                {
                    xboxControllers[1].configureGamepad(Properties.Settings.Default.ctrl2Profile);
                    ctrl2CustomBox.Checked = true;
                }
                else
                {
                    xboxControllers[1].configureGamepad(Properties.Settings.Default.ctrl2TriggerAsButton);
                    Properties.Settings.Default.ctrl2Profile = "";
                    ctrl2CustomBox.Checked = false;
                }
            }
            else
                ctrl2CustomBox.Checked = false;

            // Load Controller 3 Configuration
            // Keyboard Type
            xboxControllers[2].configureChatpad(Properties.Settings.Default.ctrl3KeyboardType);
            switch (Properties.Settings.Default.ctrl3KeyboardType)
            {
                case "Q W E RT Y":
                    ctrl3QwertyButton.Checked = true;
                    break;
                case "Q W E R T Z":
                    ctrl3QwertzButton.Checked = true;
                    break;
                case "A Z E R T Y":
                    ctrl3AzertyButton.Checked = true;
                    break;
                default:
                    // Use QWERTY if the configuration file has junk data
                    ctrl3QwertyButton.Checked = true;
                    break;
            }

            // Trigger Type
            if (!Properties.Settings.Default.ctrl3CustomConfig || Properties.Settings.Default.ctrl3Profile == "")
            {
                xboxControllers[2].configureGamepad(Properties.Settings.Default.ctrl3TriggerAsButton);
                if (Properties.Settings.Default.ctrl3TriggerAsButton)
                    ctrl3TriggerTypeBox.Checked = true;
                else
                    ctrl3TriggerTypeBox.Checked = false;
            }

            // Mouse Mode
            xboxControllers[2].mouseModeFlag = Properties.Settings.Default.ctrl3MouseMode;
            if (Properties.Settings.Default.ctrl3MouseMode)
                ctrl3MouseModeBox.Checked = true;
            else
                ctrl3MouseModeBox.Checked = false;

            // Deadzones
            ctrl3LeftDeadzone.Value = Properties.Settings.Default.ctrl3DeadzoneL;
            ctrl3RightDeadzone.Value = Properties.Settings.Default.ctrl3DeadzoneR;

            // Custom Mappings
            if (Properties.Settings.Default.ctrl3CustomConfig && Properties.Settings.Default.ctrl3Profile != "")
            {
                if (File.Exists(Properties.Settings.Default.ctrl3Profile))
                {
                    xboxControllers[2].configureGamepad(Properties.Settings.Default.ctrl3Profile);
                    ctrl3CustomBox.Checked = true;
                }
                else
                {
                    xboxControllers[2].configureGamepad(Properties.Settings.Default.ctrl3TriggerAsButton);
                    Properties.Settings.Default.ctrl3Profile = "";
                    ctrl3CustomBox.Checked = false;
                }
            }
            else
                ctrl3CustomBox.Checked = false;

            // Load Controller 4 Configuration
            // Keyboard Type
            xboxControllers[3].configureChatpad(Properties.Settings.Default.ctrl4KeyboardType);
            switch (Properties.Settings.Default.ctrl4KeyboardType)
            {
                case "Q W E RT Y":
                    ctrl4QwertyButton.Checked = true;
                    break;
                case "Q W E R T Z":
                    ctrl4QwertzButton.Checked = true;
                    break;
                case "A Z E R T Y":
                    ctrl4AzertyButton.Checked = true;
                    break;
                default:
                    // Use QWERTY if the configuration file has junk data
                    ctrl4QwertyButton.Checked = true;
                    break;
            }

            // Trigger Type
            if (!Properties.Settings.Default.ctrl4CustomConfig || Properties.Settings.Default.ctrl4Profile == "")
            {
                xboxControllers[3].configureGamepad(Properties.Settings.Default.ctrl4TriggerAsButton);
                if (Properties.Settings.Default.ctrl4TriggerAsButton)
                    ctrl4TriggerTypeBox.Checked = true;
                else
                    ctrl4TriggerTypeBox.Checked = false;
            }

            // Mouse Mode
            xboxControllers[3].mouseModeFlag = Properties.Settings.Default.ctrl4MouseMode;
            if (Properties.Settings.Default.ctrl4MouseMode)
                ctrl4MouseModeBox.Checked = true;
            else
                ctrl4MouseModeBox.Checked = false;

            // Deadzones
            ctrl4LeftDeadzone.Value = Properties.Settings.Default.ctrl4DeadzoneL;
            ctrl4RightDeadzone.Value = Properties.Settings.Default.ctrl4DeadzoneR;

            // Custom Mappings
            if (Properties.Settings.Default.ctrl4CustomConfig && Properties.Settings.Default.ctrl4Profile != "")
            {
                if (File.Exists(Properties.Settings.Default.ctrl4Profile))
                {
                    xboxControllers[3].configureGamepad(Properties.Settings.Default.ctrl4Profile);
                    ctrl4CustomBox.Checked = true;
                }
                else
                {
                    xboxControllers[3].configureGamepad(Properties.Settings.Default.ctrl4TriggerAsButton);
                    Properties.Settings.Default.ctrl4Profile = "";
                    ctrl4CustomBox.Checked = false;
                }
            }
            else
                ctrl4CustomBox.Checked = false;

            // Register each Controller to a vJoy Joystick
            xboxControllers[0].registerJoystick(1);
            xboxControllers[1].registerJoystick(2);
            xboxControllers[2].registerJoystick(3);
            xboxControllers[3].registerJoystick(4);

            // Instantiate and Connect to the Receiver
            xboxReceiver = new Receiver(xboxControllers, this);
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

        private void chatpadTextBox_Enter(object sender, EventArgs e)
        {
            // Removes out pre-populated message in the chatpadTextBox
            chatpadTextBox.TextAlign = HorizontalAlignment.Left;
            chatpadTextBox.Text = "";

            // Removes the event handler so we don't continually remove the text
            chatpadTextBox.Enter -= chatpadTextBox_Enter;
        }

        private void triggerType_CheckChanged(object sender, EventArgs e)
        {
            // Set corresponding controller trigger type based on the check box
            string checkBoxName = ((CheckBox)sender).Name;

            if (checkBoxName.Contains("1"))
            {
                if (((CheckBox)sender).Checked)
                    Properties.Settings.Default.ctrl1TriggerAsButton = true;
               else
                    Properties.Settings.Default.ctrl1TriggerAsButton = false;

                xboxControllers[0].configureGamepad(Properties.Settings.Default.ctrl1TriggerAsButton);
            }
            else if (checkBoxName.Contains("2"))
            {
                if (((CheckBox)sender).Checked)
                    Properties.Settings.Default.ctrl2TriggerAsButton = true;
                else
                    Properties.Settings.Default.ctrl2TriggerAsButton = false;

                xboxControllers[1].configureGamepad(Properties.Settings.Default.ctrl2TriggerAsButton);
            }
            else if (checkBoxName.Contains("3"))
            {
                if (((CheckBox)sender).Checked)
                    Properties.Settings.Default.ctrl3TriggerAsButton = true;
                else
                    Properties.Settings.Default.ctrl3TriggerAsButton = false;

                xboxControllers[2].configureGamepad(Properties.Settings.Default.ctrl3TriggerAsButton);
            }
            else
            {
                if (((CheckBox)sender).Checked)
                    Properties.Settings.Default.ctrl4TriggerAsButton = true;
                else
                    Properties.Settings.Default.ctrl4TriggerAsButton = false;

                xboxControllers[3].configureGamepad(Properties.Settings.Default.ctrl4TriggerAsButton);
            }
        }

        private void keyboardType_Selected(object sender, EventArgs e)
        {
            // Set the corresponding controller keyboard type based on the radio buttons
            string radioButtonName = ((RadioButton)sender).Name;

            if (radioButtonName.Contains("1"))
            {
                if (((RadioButton)sender).Checked)
                {
                    Properties.Settings.Default.ctrl1KeyboardType = ((RadioButton)sender).Text;
                    xboxControllers[0].configureChatpad(((RadioButton)sender).Text);
                }
            }
            else if (radioButtonName.Contains("2"))
            {   
                if (((RadioButton)sender).Checked)
                {
                    Properties.Settings.Default.ctrl2KeyboardType = ((RadioButton)sender).Text;
                    xboxControllers[1].configureChatpad(((RadioButton)sender).Text);
                }
            }
            else if (radioButtonName.Contains("3"))
            {
                if (((RadioButton)sender).Checked)
                {
                    Properties.Settings.Default.ctrl3KeyboardType = ((RadioButton)sender).Text;
                    xboxControllers[2].configureChatpad(((RadioButton)sender).Text);
                }
            }
            else
            {
                if (((RadioButton)sender).Checked)
                {
                    Properties.Settings.Default.ctrl4KeyboardType = ((RadioButton)sender).Text;
                    xboxControllers[3].configureChatpad(((RadioButton)sender).Text);
                }
            }
        }

        private void deadzoneL_ValueChanged(object sender, EventArgs e)
        {
            // Set the corresponding controllers right deadzone based on the slider
            string trackBarName = ((TrackBar)sender).Name;

            if (trackBarName.Contains("1"))
            {
                // Controller 1
                Properties.Settings.Default.ctrl1DeadzoneL = ctrl1LeftDeadzone.Value;
                xboxControllers[0].deadzoneL = (int)Math.Round(ctrl1LeftDeadzone.Value * 327.67);
                ctrl1LeftDeadzonePercentLabel.Text = ctrl1LeftDeadzone.Value.ToString() + "%";
            }
            else if (trackBarName.Contains("2"))
            {
                // Controller 2
                Properties.Settings.Default.ctrl2DeadzoneL = ctrl2LeftDeadzone.Value;
                xboxControllers[1].deadzoneL = (int)Math.Round(ctrl2LeftDeadzone.Value * 327.67);
                ctrl2LeftDeadzonePercentLabel.Text = ctrl2LeftDeadzone.Value.ToString() + "%";
            }
            else if (trackBarName.Contains("3"))
            {
                // Controller 3
                Properties.Settings.Default.ctrl3DeadzoneL = ctrl3LeftDeadzone.Value;
                xboxControllers[2].deadzoneL = (int)Math.Round(ctrl3LeftDeadzone.Value * 327.67);
                ctrl3LeftDeadzonePercentLabel.Text = ctrl3LeftDeadzone.Value.ToString() + "%";
            }
            else
            {
                // Controller 4
                Properties.Settings.Default.ctrl4DeadzoneL = ctrl4LeftDeadzone.Value;
                xboxControllers[3].deadzoneL = (int)Math.Round(ctrl4LeftDeadzone.Value * 327.67);
                ctrl4LeftDeadzonePercentLabel.Text = ctrl4LeftDeadzone.Value.ToString() + "%";
            }
        }

        private void deadzoneR_ValueChanged(object sender, EventArgs e)
        {
            // Set the corresponding controllers right deadzone based on the slider
            string trackBarName = ((TrackBar)sender).Name;

            if (trackBarName.Contains("1"))
            {
                // Controller 1
                Properties.Settings.Default.ctrl1DeadzoneR = ctrl1RightDeadzone.Value;
                xboxControllers[0].deadzoneR = (int)Math.Round(ctrl1RightDeadzone.Value * 327.67);
                ctrl1RightDeadzonePercentLabel.Text = ctrl1RightDeadzone.Value.ToString() + "%";
            }
            else if (trackBarName.Contains("2"))
            {
                // Controller 2
                Properties.Settings.Default.ctrl2DeadzoneR = ctrl2RightDeadzone.Value;
                xboxControllers[1].deadzoneR = (int)Math.Round(ctrl2RightDeadzone.Value * 327.67);
                ctrl2RightDeadzonePercentLabel.Text = ctrl2RightDeadzone.Value.ToString() + "%";
            }
            else if (trackBarName.Contains("3"))
            {
                // Controller 3
                Properties.Settings.Default.ctrl3DeadzoneR = ctrl3RightDeadzone.Value;
                xboxControllers[2].deadzoneR = (int)Math.Round(ctrl3RightDeadzone.Value * 327.67);
                ctrl3RightDeadzonePercentLabel.Text = ctrl3RightDeadzone.Value.ToString() + "%";
            }
            else
            {
                // Controller 4
                Properties.Settings.Default.ctrl4DeadzoneR = ctrl4RightDeadzone.Value;
                xboxControllers[3].deadzoneR = (int)Math.Round(ctrl4RightDeadzone.Value * 327.67);
                ctrl4RightDeadzonePercentLabel.Text = ctrl4RightDeadzone.Value.ToString() + "%";
            }            
        }

        public void controllerConnected(int ctrl)
        {
            switch (ctrl)
            {
                case 1:
                    ctrl1Group.Enabled = true;
                    break;
                case 2:
                    ctrl2Group.Enabled = true;
                    break;
                case 3:
                    ctrl3Group.Enabled = true;
                    break;
                case 4:
                    ctrl4Group.Enabled = true;
                    break;
                default:
                    break;
            }
        }

        public void controllerDisconnected(int ctrl)
        {
            switch (ctrl)
            {
                case 1:
                    ctrl1Group.Enabled = false;
                    break;
                case 2:
                    ctrl2Group.Enabled = false;
                    break;
                case 3:
                    ctrl3Group.Enabled = false;
                    break;
                case 4:
                    ctrl4Group.Enabled = false;
                    break;
                default:
                    break;
            }
        }

        public void mouseModeUpdate(int ctrl, bool modeStatus)
        {
            switch (ctrl)
            {
                case 1:
                    ctrl1MouseModeBox.Checked = modeStatus;
                    Properties.Settings.Default.ctrl1MouseMode = modeStatus;
                    break;
                case 2:
                    ctrl2MouseModeBox.Checked = modeStatus;
                    Properties.Settings.Default.ctrl1MouseMode = modeStatus;
                    break;
                case 3:
                    ctrl3MouseModeBox.Checked = modeStatus;
                    Properties.Settings.Default.ctrl1MouseMode = modeStatus;
                    break;
                case 4:
                    ctrl4MouseModeBox.Checked = modeStatus;
                    Properties.Settings.Default.ctrl1MouseMode = modeStatus;
                    break;
                default:
                    break;
            }            
        }

        public void logMessage(string message)
        {
            string currentTime;
            currentTime = DateTime.Now.ToString("G");

            // Pre-pend the Text Box text with timestamp and new message
            appLogTextbox.Text = "[" + currentTime + "] - " + message + "\r\n" + appLogTextbox.Text;

            // Scroll to Top of Textbox
            appLogTextbox.Select(0, 0);
            appLogTextbox.ScrollToCaret();
        }

        private void customBox_CheckChanged(object sender, EventArgs e)
        {
            // Enable corresponding controller custom mappings option based on the check box name
            string checkBoxName = ((CheckBox)sender).Name;

            if (checkBoxName.Contains("1"))
            {
                if (((CheckBox)sender).Checked)
                {
                    Properties.Settings.Default.ctrl1CustomConfig = true;
                    ctrl1ConfigButton.Enabled = true;
                    ctrl1TriggerTypeBox.Enabled = false;
                }
                else
                {
                    Properties.Settings.Default.ctrl1CustomConfig = false;
                    ctrl1ConfigButton.Enabled = false;
                    ctrl1TriggerTypeBox.Enabled = true;
                }
            }
            else if (checkBoxName.Contains("2"))
            {
                if (((CheckBox)sender).Checked)
                {
                    Properties.Settings.Default.ctrl2CustomConfig = true;
                    ctrl2ConfigButton.Enabled = true;
                    ctrl2TriggerTypeBox.Enabled = false;
                }
                else
                {
                    Properties.Settings.Default.ctrl2CustomConfig = false;
                    ctrl2ConfigButton.Enabled = false;
                    ctrl2TriggerTypeBox.Enabled = true;
                }
            }
            else if (checkBoxName.Contains("3"))
            {
                if (((CheckBox)sender).Checked)
                {
                    Properties.Settings.Default.ctrl3CustomConfig = true;
                    ctrl3ConfigButton.Enabled = true;
                    ctrl3TriggerTypeBox.Enabled = false;
                }
                else
                {
                    Properties.Settings.Default.ctrl3CustomConfig = false;
                    ctrl3ConfigButton.Enabled = false;
                    ctrl3TriggerTypeBox.Enabled = true;
                }
            }
            else
            {
                if (((CheckBox)sender).Checked)
                {
                    Properties.Settings.Default.ctrl4CustomConfig = true;
                    ctrl4ConfigButton.Enabled = true;
                    ctrl4TriggerTypeBox.Enabled = false;
                }
                else
                {
                    Properties.Settings.Default.ctrl4CustomConfig = false;
                    ctrl4ConfigButton.Enabled = false;
                    ctrl4TriggerTypeBox.Enabled = true;
                }
            }
        }

        private void ctrl1ConfigButton_Click(object sender, EventArgs e)
        {
            showCtrlConfigDialog(1);
        }

        private void showCtrlConfigDialog(int ctrlnumber)
        {
            Custom_Settings_Window ctrldlg = new Custom_Settings_Window(ctrlnumber, this);
            ctrldlg.ShowDialog();
            if (ctrldlg.DialogResult == DialogResult.OK)
            {
                switch (ctrldlg.controllerNumber) {
                    case 1:
                        Properties.Settings.Default.ctrl1Profile = ctrldlg.profilePath;
                        break;
                    case 2:
                        Properties.Settings.Default.ctrl2Profile = ctrldlg.profilePath;
                        break;
                    case 3:
                        Properties.Settings.Default.ctrl3Profile = ctrldlg.profilePath;
                        break;
                    case 4:
                        Properties.Settings.Default.ctrl4Profile = ctrldlg.profilePath;
                        break;
                }
                if (ctrldlg.profilePath != "")
                {
                    switch (ctrldlg.controllerNumber)
                    {
                        case 1:
                            xboxControllers[0].configureGamepad(Properties.Settings.Default.ctrl1Profile);
                            ctrl1TriggerTypeBox.Enabled = false;
                            break;
                        case 2:
                            xboxControllers[1].configureGamepad(Properties.Settings.Default.ctrl2Profile);
                            ctrl2TriggerTypeBox.Enabled = false;
                            break;
                        case 3:
                            xboxControllers[2].configureGamepad(Properties.Settings.Default.ctrl3Profile);
                            ctrl3TriggerTypeBox.Enabled = false;
                            break;
                        case 4:
                            xboxControllers[3].configureGamepad(Properties.Settings.Default.ctrl4Profile);
                            ctrl3TriggerTypeBox.Enabled = false;
                            break;
                    }
                }
                else
                {
                    switch (ctrldlg.controllerNumber)
                    {
                        case 1:
                            xboxControllers[0].configureGamepad(Properties.Settings.Default.ctrl1TriggerAsButton);
                            break;
                        case 2:
                            xboxControllers[1].configureGamepad(Properties.Settings.Default.ctrl2TriggerAsButton);
                            break;
                        case 3:
                            xboxControllers[2].configureGamepad(Properties.Settings.Default.ctrl3TriggerAsButton);
                            break;
                        case 4:
                            xboxControllers[3].configureGamepad(Properties.Settings.Default.ctrl4TriggerAsButton);
                            break;
                    }
                }
            }
            ctrldlg.Dispose();
        }

        private void ctrl2ConfigButton_Click(object sender, EventArgs e)
        {
            showCtrlConfigDialog(2);
        }

        private void ctrl3ConfigButton_Click(object sender, EventArgs e)
        {
            showCtrlConfigDialog(3);
        }

        private void ctrl4ConfigButton_Click(object sender, EventArgs e)
        {
            showCtrlConfigDialog(4);
        }
    }
}
