using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

using LibUsbDotNet;
using LibUsbDotNet.Main;
using System.IO;

namespace Xbox360WirelessChatpad
{
    // Necessary to reset the deadzones in a thread-safe manner
    delegate void controllerDisconnectCallback();

    public partial class Window_Main : Form
    {
        // Necessary to write to TextBox from multiple threads
        Util_LogToTextbox appLog;

        // Issue 2: Log File
        StreamWriter debugLog;

        // The Gamepad, non-chatpad buttons, joysticks, and triggers.
        private Gamepad xboxGamepad;

        // The Chatpad, chatpad buttons
        private Chatpad xboxChatpad;

        // The Xbox Wireless Receiver, and Endpoint 1 reader and writer
        private IUsbDevice WirelessReceiver;
        private UsbEndpointWriter epWriter;
        private UsbEndpointReader epReader;

        // Identifies if the Wireless Receiver/Controller are attached
        private bool WirelessReceiverAttached = false;
        private bool WirelessControllerAttached = false;
        
        // Keep-Alive Thread, this will execute keep-alive commands periodically
        private System.Threading.Thread keepAliveThread = null;

        // Keep-Alive Toggle, this will determine which keep-alive command will be sent
        // during each execution cycle, True = Command 1, False = Commands 2a and 2b
        private bool keepAliveToggle = false;

        // Determines if the chatpad needs initialization/handshake commands.
        private bool chatpadInitNeeded = true;

        // Contains the mapping of useful device commands
        public Dictionary<string, byte[]> deviceCommands = new Dictionary<string, byte[]>()
            {
                // General Device Commands
                { "RefreshConnection",  new byte[4] {0x08, 0x00, 0x00, 0x00} },
                { "KeepAlive1",         new byte[4] {0x00, 0x00, 0x0C, 0x1F} },
                { "KeepAlive2a",        new byte[4] {0x00, 0x00, 0x0C, 0x1E} },
                { "KeepAlive2b",        new byte[4] {0x00, 0x00, 0x0C, 0x1B} },
                { "SetControllerNum1",  new byte[4] {0x00, 0x00, 0x08, 0x42} },
                { "SetControllerNum2",  new byte[4] {0x00, 0x00, 0x08, 0x43} },
                { "SetControllerNum3",  new byte[4] {0x00, 0x00, 0x08, 0x44} },
                { "SetControllerNum4",  new byte[4] {0x00, 0x00, 0x08, 0x45} },
                { "DisableController",  new byte[4] {0x00, 0x00, 0x08, 0xC0} },

                // Chatpad LED Commands
                { "Green_On",           new byte[8] {0x00, 0x00, 0x0C, 0x09, 0x02, 0x00, 0x00, 0x00} },
                { "Green_Off",          new byte[8] {0x00, 0x00, 0x0C, 0x01, 0x02, 0x00, 0x00, 0x00} },
                { "Orange_On",          new byte[4] {0x00, 0x00, 0x0C, 0x0A} },
                { "Orange_Off",         new byte[4] {0x00, 0x00, 0x0C, 0x02} },
                { "Messenger_On",       new byte[4] {0x00, 0x00, 0x0C, 0x0B} },
                { "Messenger_Off",      new byte[4] {0x00, 0x00, 0x0C, 0x03} },
                { "Capslock_On",        new byte[4] {0x00, 0x00, 0x0C, 0x08} },
                { "Capslock_Off",       new byte[4] {0x00, 0x00, 0x0C, 0x00} }
            };

        #region User Interface Events

        public Window_Main()
        {
            InitializeComponent();
        }

        private void Window_Main_Load(object sender, EventArgs e)
        {
            // Instansiates the appLog and adds it to the Trace listeners
            appLog = new Util_LogToTextbox(appLogTextbox);
            Trace.Listeners.Add(appLog);

            // Issue 2: Delete the log file so we can get a new one
            File.Delete("debugLog.txt");
            debugLog = File.AppendText("debugLog.txt");
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
                // Turns off the Xbox controller
                if (WirelessReceiverAttached)
                    sendData(deviceCommands["DisableController"]);

                // Cleans up the timer
                if (keepAliveThread != null)
                {
                    keepAliveThread.Abort();
                    keepAliveThread = null;
                }

                // Cleans up the Wireless Receiver Data
                if (epWriter != null)
                {
                    epWriter.Abort();
                    epWriter.Dispose();
                }

                if (epReader != null)
                {
                    epReader.Abort();
                    epReader.Dispose();
                }

                if (WirelessReceiver != null)
                {
                    if (WirelessReceiverAttached)
                        WirelessReceiver.Close();
                    WirelessReceiver = null;
                }
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

        private void connectButton_Click(object sender, EventArgs e)
        {
            // Connects Xbox Wireless Receiver
            connectReceiver();

            // Connects Xbox Wireless Controller 1
            connectController();
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
            xboxGamepad.deadzoneL = (int)Math.Round(leftDeadzone.Value * 327.67);

            // Set the left deadzone label to the new value
            leftDeadzonePercentLabel.Text = leftDeadzone.Value.ToString() + "%";
        }

        private void rightDeadzone_ValueChanged(object sender, EventArgs e)
        {
            // Set the right deadzone to slider percentage of 32767, the maximum
            // value of an analog stick
            xboxGamepad.deadzoneR = (int)Math.Round(rightDeadzone.Value * 327.67);

            // Set the right deadzone label to the new value
            rightDeadzonePercentLabel.Text = rightDeadzone.Value.ToString() + "%";
        }

        private void controllerDisconnect()
        {
            // Reset the deadzones in a thread safe manner
            leftDeadzone.Value = 0;
            rightDeadzone.Value = 0;
            deadzoneGroupBox.Enabled = false;

            // Reset the chatpad test box in a thread safe manner
            chatpadTextBox.Enabled = false;
            chatpadTextBox.TextAlign = HorizontalAlignment.Center;
            chatpadTextBox.Text = "-Test Chatpad Here-";
            chatpadTextBox.Enter += chatpadTextBox_Enter;
        }

        #endregion

        private void connectReceiver()
        {
            // This function connects the Xbox Wireless Receiver, registers the endpoint reader
            // and writer, and sets up the DataReceived event.
            // Note: For now this only supports Player 1, if there is a need to include multiple controllers
            // it can be done pretty easily, but the use-case for multiple chatpad-enabled controllers seems
            // pretty rare. It's recommended that the official driver is used for multiple controllers, the
            // only drawback is the lack of chatpad in that scenario.
            if (!WirelessReceiverAttached)
            {
                try
                {
                    // Open the Xbox Wireless Receiver as a USB device
                    // VendorID 0x045e, ProductID 0x0719
                    WirelessReceiver = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(0x045E, 0x0719)) as IUsbDevice;

                    // If primary IDs not found, report error and attempt secondary IDs
                    // VendorID 0x045e, Product ID 0x0291
                    if (WirelessReceiver == null)
                        WirelessReceiver = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(0x045E, 0x0291)) as IUsbDevice;

                    // If secondary IDs not found, report the error
                    if (WirelessReceiver == null)
                        Trace.WriteLine("ERROR: Wireless Receiver Not Found.");
                    else
                    {
                        // Set the Configuration, Claim the Interface
                        WirelessReceiver.ClaimInterface(1); // Note: This was originally 0, changed to 1, not sure why
                        WirelessReceiver.SetConfiguration(1);

                        // Connect Endpoint 1 Reader/Writer and register the receiving event handler
                        // Note: Endpoint 1 is assumed to be associated to Controller 1
                        epReader = WirelessReceiver.OpenEndpointReader(ReadEndpointID.Ep01);
                        epWriter = WirelessReceiver.OpenEndpointWriter(WriteEndpointID.Ep01);
                        epReader.DataReceived += new EventHandler<EndpointDataEventArgs>(receivedData);
                        epReader.DataReceivedEnabled = true;

                        // If the Wireless Receiver was connected successfully, report it
                        if (WirelessReceiver.IsOpen)
                        {
                            WirelessReceiverAttached = true;
                            Trace.WriteLine("Xbox 360 Wireless Receiver Connected.\r\n");
                        }
                    }
                }
                catch
                {
                    Trace.WriteLine("ERROR: Problem Connecting to Wireless Receiver.");
                }
            }
        }

        private void connectController()
        {
            if (xboxGamepad == null)
                xboxGamepad = new Gamepad();
            // This function connects the Xbox Wireless Controller, initializes the chatpad values,
            // and sets the LED status to indicate the controller number.
            // Note: For now this only supports Player 1, if there is a need to include multiple controllers
            // it can be done pretty easily, but the use-case for multiple chatpad-enabled controllers seems
            // pretty rare. It's recommended that the official driver is used for multiple controllers, the
            // only drawback is the lack of chatpad in that scenario.
            if (WirelessReceiverAttached)
            {
                // Instantiates the Gamepad and Chatpad, if it hasn't been done
                

                if (xboxChatpad == null)
                    xboxChatpad = new Chatpad();

                Trace.WriteLine("Searching for Controller...Press the Guide Button Now.");

                try
                {
                    // Request the connection to Controller 1
                    // Note: This is intentionally redundant, things seem to work better as a result
                    sendData(deviceCommands["RefreshConnection"]);
                    sendData(deviceCommands["RefreshConnection"]);

                    // Check for the connection every 10ms, wait at most 10 seconds
                    DateTime timeoutStart = DateTime.Now;
                    while (!WirelessControllerAttached && DateTime.Now <= timeoutStart.AddSeconds(10))
                    {
                        System.Threading.Thread.Sleep(10);
                    }

                    // If Controller 1 is attached, proceed with the initialization, otherwise report the error
                    if (WirelessControllerAttached)
                    {
                        // Create and start the Keep-Alive thread
                        keepAliveThread = new System.Threading.Thread(new System.Threading.ThreadStart(keepAliveTick));
                        keepAliveThread.IsBackground = true;
                        keepAliveThread.Start();

                        // Reports the Controller is Connected
                        Trace.WriteLine("Xbox 360 Wireless Controller 1 Connected.\r\n");

                        // Enable the chatpad test box
                        chatpadTextBox.Enabled = true;

                        // Set the Controller deadzones to 16% and enabled the deadzone toggles
                        leftDeadzone.Value = 18;
                        rightDeadzone.Value = 18;
                        deadzoneGroupBox.Enabled = true;
                    }
                    else
                        Trace.WriteLine("ERROR: Controller 1 Connection Timeout, Try Again.");
                }
                catch
                {
                    Trace.WriteLine("ERROR: Problem Connecting to Controller 1.");
                }
            }
            else
                Trace.WriteLine("ERROR: Wireless Receiver Not Connected.");
        }

        public void sendData(byte[] dataToSend)
        {
            // This function sends the supplied data via the Endpoint Writer
            // to the Wireless Receiver as long as it is attached.
            int bytesWritten;

            if (WirelessReceiverAttached)
            {
                ErrorCode ec = epWriter.Write(dataToSend, 2000, out bytesWritten);

                if (ec != ErrorCode.None)
                    Trace.WriteLine("ERROR: Problem Sending Controller Data.");

                //System.Threading.Thread.Sleep(15);
            }
        }

        private void receivedData(object sender, EndpointDataEventArgs e)
        {
            if (e.Buffer[0] == 0x08)
            {
                // This is a status packet, determine if the controller is connected
                bool controllerConnected = ((e.Buffer[1] & 0x80) > 0);

                if (!controllerConnected)
                {
                    // If the controller is not connected but used to be, report the error
                    if (WirelessControllerAttached)
                    {
                        Trace.WriteLine("Xbox 360 Wireless Controller Disconnected.\r\n");

                        // Reset the deadzones and disable the form control
                        Invoke(new controllerDisconnectCallback(controllerDisconnect));
                    }

                    WirelessControllerAttached = false;
                }
                else
                {
                    // If the controller is connected, set the LED to Controller 1
                    sendData(deviceCommands["SetControllerNum1"]);

                    WirelessControllerAttached = true;
                }
            }
            else if (e.Buffer[0] == 0x00 && e.Buffer[2] == 0x00 && e.Buffer[3] == 0xF0)
            {
                if (WirelessControllerAttached)
                {
                    switch (e.Buffer[1])
                    {
                        case 0x01: // This is Gamepad data
                            xboxGamepad.ProcessData(e.Buffer);
                            break;
                        case 0x02: // This is Chatpad data
                            if (e.Buffer[25] == 3)
                                // This is a handshake, flag chatpad for initialization
                                chatpadInitNeeded = true;
                            else
                                xboxChatpad.ProcessData(e.Buffer);
                            break;
                        case 0x0F:
                            sendData(deviceCommands["KeepAlive2b"]);
                            break;
                        default:
                            // Unknown Data, do nothing with it
                            break;
                    }
                }
            }

            // Issue 2: Outputting the chatpad data to determine why its not registering.
            byte[] debugString = new byte[40];
            for (int i = 0; i < debugString.Length; i++)
            {
                debugString[i] = e.Buffer[i];
            }
            debugLog.WriteLine("Controller Data: " + BitConverter.ToString(debugString));
        }

        private void keepAliveTick()
        {
            // This function is executed every second on a separate background thread
            // as long as the controller is connected. It will send unique alternating
            // device commands in order to keep the device alive.
            try
            {
                while (true)
                {
                    if (epWriter != null)
                    {
                        if (keepAliveToggle)
                            sendData(deviceCommands["KeepAlive1"]);
                        else
                        {
                            sendData(deviceCommands["KeepAlive2a"]);
                            sendData(deviceCommands["KeepAlive2b"]);
                        }

                        keepAliveToggle = !keepAliveToggle;

                        if (chatpadInitNeeded)
                        {
                            // Initialize Chatpad Communication
                            // Note: Not really sure where these commands came from, or what they mean. Some may
                            // be redundant but for now it increases the possibility of things working. For reference
                            // all of these commands have been un-necessary for my machine, but required for others.
                            sendData(new byte[] { 0x40, 0xA9, 0x0C, 0xA3, 0x23, 0x44, 0x00, 0x00 });
                            sendData(new byte[] { 0x40, 0xA9, 0x44, 0x23, 0x03, 0x7F, 0x00, 0x00 });
                            sendData(new byte[] { 0x40, 0xA9, 0x39, 0x58, 0x32, 0x08, 0x00, 0x00 });
                            sendData(new byte[] { 0xC0, 0xA1, 0x00, 0x00, 0x16, 0xE4, 0x02, 0x00 });
                            sendData(new byte[] { 0x40, 0xA1, 0x00, 0x00, 0x16, 0xE4, 0x02, 0x00 });
                            sendData(new byte[] { 0xC0, 0xA1, 0x00, 0x00, 0x16, 0xE4, 0x02, 0x00 });
                            sendData(deviceCommands["KeepAlive2b"]);

                            // Set chatpadInitNeeded to false, we just did it
                            chatpadInitNeeded = false;
                        }
                    }
                }
            }
            catch
            {
                Trace.WriteLine("ERROR: Problem With Keep-Alive Commands.");
            }

            System.Threading.Thread.Sleep(1000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!xboxChatpad.chatpadLED["Green"])
                sendData(deviceCommands["Green_On"]);
            else
                sendData(deviceCommands["Green_Off"]);
        }
    }
}
