using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using InputManager;
using vJoyInterfaceWrap;

using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace Xbox360WirelessChatpad
{
    class Controller
    {
        // Tracks if the Wireless Controller is attached
        public bool controllerAttached = false;

        // Tracks if the trigger will behave like a button or axis
        private string controllerTrigger;

        // The Controllers associated endpoint writer in the receiver
        private UsbEndpointWriter epWriter;
        
        // Parent Window object necessary to communicate with form controls
        private Window_Main parentWindow;

        // Keep-Alive Thread, this will execute keep-alive commands periodically
        private System.Threading.Thread threadKeepAlive = null;

        // Button Combo Thread, this will execute to monitor for special button
        // combinations like Mouse Mode and Shutdown
        private System.Threading.Thread threadButtonCombo = null;

        // MouseMode Thread, this will execute periodically to move the mouse cursor
        // and scroll vertical when inidcated by joystick data
        private System.Threading.Thread mouseModeThread = null;

        // Determines if the chatpad needs initialization/handshake command.
        public bool chatpadInitNeeded = true;

        // Mapping for useful device commands
        public Dictionary<string, byte[]> controllerCommands = new Dictionary<string, byte[]>()
            {
                // General Device Commands
                { "RefreshConnection",  new byte[4] {0x08, 0x00, 0x00, 0x00} },
                { "KeepAlive1",         new byte[4] {0x00, 0x00, 0x0C, 0x1F} },
                { "KeepAlive2",         new byte[4] {0x00, 0x00, 0x0C, 0x1E} },
                { "ChatpadInit",        new byte[4] {0x00, 0x00, 0x0C, 0x1B} },
                { "SetControllerNum1",  new byte[4] {0x00, 0x00, 0x08, 0x42} },
                { "SetControllerNum2",  new byte[4] {0x00, 0x00, 0x08, 0x43} },
                { "SetControllerNum3",  new byte[4] {0x00, 0x00, 0x08, 0x44} },
                { "SetControllerNum4",  new byte[4] {0x00, 0x00, 0x08, 0x45} },
                { "DisableController",  new byte[4] {0x00, 0x00, 0x08, 0xC0} },

                // Chatpad LED Commands
                { "GreenOn",       new byte[4] {0x00, 0x00, 0x0C, 0x09} },
                { "GreenOff",      new byte[4] {0x00, 0x00, 0x0C, 0x01} },
                { "OrangeOn",      new byte[4] {0x00, 0x00, 0x0C, 0x0A} },
                { "OrangeOff",     new byte[4] {0x00, 0x00, 0x0C, 0x02} },
                { "MessengerOn",   new byte[4] {0x00, 0x00, 0x0C, 0x0B} },
                { "MessengerOff",  new byte[4] {0x00, 0x00, 0x0C, 0x03} },
                { "CapslockOn",    new byte[4] {0x00, 0x00, 0x0C, 0x08} },
                { "CapslockOff",   new byte[4] {0x00, 0x00, 0x0C, 0x00} }
            };

        // -----------------
        // Chatpad Variables
        // -----------------

        // Contains the mapping of Chatpad Buttons, Green Modifiers, and
        // Orange Modifiers respectively.
        Dictionary<int, Keys> keyMap = new Dictionary<int, Keys>();
        Dictionary<int, string> greenMap = new Dictionary<int, string>();
        Dictionary<int, string> orangeMap = new Dictionary<int, string>();

        // Identifies which Chatpad Modifiers are active
        Dictionary<string, bool> chatpadMod = new Dictionary<string, bool>()
            {
                { "Green", false },
                { "Orange", false },
                { "Shift", false },
                { "Capslock", false },
                { "Messenger", false }
            };

        // Identifies which keys are currently being held down, used to
        // determine if a keystroke should be sent or not
        private List<byte> chatpadKeysHeld = new List<byte>();

        // Identifies which keyboard keys are down, used to track if a
        // KeyUp command needs to be sent or not
        private List<Keys> keyboardKeysDown = new List<Keys>();

        // Identifies if the sent key data should be upper case or lower case
        bool flagUpperCase = false;

        // Identifies if Alt-Tab cycling has begun
        bool altTabActive = false;

        // Used to determine if the data has changed since the last packet
        private byte[] dataPacketLast = new byte[3]; 

        // -----------------
        // Gamepad Variables
        // -----------------

        // The vJoy virtual joystick
        vJoy vJoystick;

        // Deadzone variables for the joysticks on the gamepad
        public int deadzoneL = 0;
        public int deadzoneR = 0;

        // Global Mouse Mode Flag for use by data packet processing
        public bool mouseModeFlag = false;

        // Relative Mouse Data based on Joystick location. This will
        // be used by a higher level timer function to continually move
        // the mouse.
        public int mouseVelX, mouseVelY;

        // Direction Data for the Right Joystick location. This will
        // be used by a higher level timer function to continually hold
        // down an arrow key, allowing for scrolling or other fast navigation.
        // 0 = Neutral, 
        public int rightStickDir;

        // Special Command booleans used to detect when special button
        // combinations are pressed
        public bool cmdKillController = false;
        public bool cmdMouseModeToggle = false;

        // Identifies if the left or right mouse buttons are depressed
        // Only used in Mouse Mode.
        bool leftButtonDown = false;
        bool rightButtonDown = false;

        // Shortcut keystrokes for Navigating Forward and Back
        Keys[] navBack = new Keys[] { Keys.LMenu, Keys.Left };
        Keys[] navForward = new Keys[] { Keys.LMenu, Keys.Right };

        // Contains mappings of controller buttons, directional pad, and axes
        Dictionary<String, uint> buttonMap = new Dictionary<String, uint>();
        Dictionary<String, int> directionMap = new Dictionary<String, int>();
        Dictionary<String, HID_USAGES> axisMap = new Dictionary<String, HID_USAGES>();

        public Controller(Window_Main window)
        {
            // Stores the passed window as parentWindow for furtue use
            parentWindow = window;

            // Instantiate the virtual joystick
            vJoystick = new vJoy();

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!vJoystick.vJoyEnabled())
            {
                parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                    "ERROR: vJoy Driver Not Enabled.\r\n");
                return;
            }

            // Retreives the virtual joystick status
            VjdStat vJoystickStatus = vJoystick.GetVJDStatus(1);

            // Acquire the virtual joystick
            if ((vJoystickStatus == VjdStat.VJD_STAT_OWN) ||
                ((vJoystickStatus == VjdStat.VJD_STAT_FREE) && (!vJoystick.AcquireVJD(1))))
                    parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                        "ERROR: Failed to Acquire vJoy Gamepad Number 1.\r\n"); 
        }

        public void registerEndpoint(UsbEndpointWriter writer)
        {
            epWriter = writer;
        }

        public void processDataPacket(object sender, EndpointDataEventArgs e)
        {
            if (e.Buffer[0] == 0x08)
            {
                // This is a status packet, determine if the controller is connected
                bool controllerConnected = ((e.Buffer[1] & 0x80) > 0);

                if (!controllerConnected)
                {
                    // If the controller is not connected but used to be, report the error
                    if (!controllerAttached)
                    {
                        parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                            "Xbox 360 Wireless Controller Disconnected.\r\n");

                        // Disables mouse mode and kills the mouse mode thread
                        toggleMouseMode(false);

                        // Clean up the Keep-Alive thread
                        killKeepAlive();

                        // Clean up the Button Combo thread
                        killButtonCombo();

                        // Refresh the form due to a disconnection
                        parentWindow.Invoke(new controllerDisconnectCallback(parentWindow.controllerDisconnected));
                    }

                    controllerAttached = false;
                }
                else
                {
                    // Flag that the controller has connected
                    controllerAttached = true;

                    // Set the LED to Controller 1
                    sendData(controllerCommands["SetControllerNum1"]);

                    // Create and start the Keep-Alive thread
                    threadKeepAlive = new System.Threading.Thread(new System.Threading.ThreadStart(tickKeepAlive));
                    threadKeepAlive.IsBackground = true;
                    threadKeepAlive.Start();

                    // Create and start the Special Button thread
                    threadButtonCombo = new System.Threading.Thread(new System.Threading.ThreadStart(tickButtonCombo));
                    threadButtonCombo.IsBackground = true;
                    threadButtonCombo.Start();

                    // Refresh the form due to a connection
                    parentWindow.Invoke(new controllerConnectCallback(parentWindow.controllerConnected));

                    // Reports the Controller is Connected
                    parentWindow.Invoke(new logCallback(parentWindow.logUpdate), "Xbox 360 Wireless Controller 1 Connected.\r\n");
                }
            }
            else if (e.Buffer[0] == 0x00 && e.Buffer[2] == 0x00 && e.Buffer[3] == 0xF0)
            {
                if (controllerAttached)
                {
                    switch (e.Buffer[1])
                    {
                        case 0x01: // This is Gamepad data
                            ProcessGamepadData(e.Buffer);
                            break;
                        case 0x02: // This is Chatpad data
                            ProcessChatpadData(e.Buffer);
                            break;
                        default:  // Unknown Data, do nothing with it
                            break;
                    }
                }
            }
        }

        public void ProcessChatpadData(byte[] dataPacket)
        {
            // This function is called anytime received data is identified as chatpad data
            // It will parse the data, and depending on the value, send a keyboard command,
            // adjust a modifier for later use, flag initialization, or note the LED status.
            if (dataPacket[24] == 0xF0)
            {
                if (dataPacket[25] == 0x03)
                    // This data represents handshake request, flag keep-alive to send
                    // chatpad initialization data.
                    chatpadInitNeeded = true;
                else if (dataPacket[25] == 0x04)
                {
                    // This data represents the LED status, no need for these.
                    // Note: This is commented out because we don't need them but the code
                    // is a good reference of how to parse the data
                    //      Green = (dataPacket[26] & 0x08) > 0;
                    //      Orange = (dataPacket[26] & 0x10) > 0;
                    //      Messenger = (dataPacket[26] & 0x01) > 0;
                    //      Capslock = (dataPacket[26] & 0x20) > 0;
                    //      Backlight = (dataPacket[26] & 0x80) > 0;
                }
                else
                    parentWindow.Invoke(new logCallback(parentWindow.logUpdate), "WARNING: Unknown Chatpad Status Data.\r\n");
            }
            else if (dataPacket[24] == 0x00)
            {
                // This data represents a key-press event
                // Check if anything has changed since the last dataPacket
                bool dataChanged = false;
                if (dataPacketLast != null)
                {
                    if (dataPacketLast[0] != dataPacket[25])
                        dataChanged = true;
                    else if (dataPacketLast[1] != dataPacket[26])
                        dataChanged = true;
                    else if (dataPacketLast[2] != dataPacket[27])
                        dataChanged = true;
                }
                else
                    dataChanged = true;

                // Store bits 25-27 of the data packet for later comparison
                dataPacketLast[0] = dataPacket[25];
                dataPacketLast[1] = dataPacket[26];
                dataPacketLast[2] = dataPacket[27];

                if (dataChanged)
                {
                    // Record the Modifier Statuses
                    chatpadMod["Green"] = (dataPacket[25] & 0x02) > 0;
                    chatpadMod["Orange"] = (dataPacket[25] & 0x04) > 0;
                    chatpadMod["Shift"] = (dataPacket[25] & 0x01) > 0;
                    chatpadMod["Messenger"] = (dataPacket[25] & 0x08) > 0;

                    // Toggle Capslock Modifier based on Orange and Shift Modifiers
                    if (chatpadMod["Orange"] && chatpadMod["Shift"])
                        chatpadMod["Capslock"] = !chatpadMod["Capslock"];

                    // Set LEDs based on Modifiers
                    if (chatpadMod["Green"])
                        sendData(controllerCommands["GreenOn"]);
                    else
                        sendData(controllerCommands["GreenOff"]);

                    if (chatpadMod["Orange"])
                        sendData(controllerCommands["OrangeOn"]);
                    else
                        sendData(controllerCommands["OrangeOff"]);

                    if (chatpadMod["Messenger"])
                        sendData(controllerCommands["MessengerOn"]);
                    else
                        sendData(controllerCommands["MessengerOff"]);

                    if (chatpadMod["Capslock"])
                        sendData(controllerCommands["CapslockOn"]);
                    else
                        sendData(controllerCommands["CapslockOff"]);

                    // Set the Upper-Case flag and Shift Key status based on the
                    // XOR of Shift and Capslock Modifiers.
                    flagUpperCase = chatpadMod["Shift"] ^ chatpadMod["Capslock"];
                    if (flagUpperCase)
                        Keyboard.KeyDown(Keys.LShiftKey);
                    else
                        Keyboard.KeyUp(Keys.LShiftKey);

                    // Set the Tab Key status based on the Messenger Modifier.
                    if (chatpadMod["Messenger"])
                        Keyboard.KeyDown(Keys.Tab);
                    else
                        Keyboard.KeyUp(Keys.Tab);

                    // Duplicates the Alt-Tab functionality with the Green and Orange
                    // Modifiers. Orange is Alt, Green is Tab
                    if (chatpadMod["Orange"])
                    {
                        if (chatpadMod["Green"])
                        {
                            if (altTabActive)
                                Keyboard.KeyPress(Keys.Tab);
                            else
                            {
                                altTabActive = true;
                                Keyboard.KeyDown(Keys.LMenu);
                                Keyboard.KeyPress(Keys.Tab);
                            }
                        }
                    }
                    else
                    {
                        if (altTabActive)
                        {
                            altTabActive = false;
                            Keyboard.KeyUp(Keys.LMenu);
                        }
                    }

                    // Process the two different possible keys that could be held down
                    ProcessKeypress(dataPacket[26]);
                    ProcessKeypress(dataPacket[27]);

                    // Compile the list of keys that were once held but no longer being held
                    // For each one, send the KeyUp command if it was down and remove from the list.
                    List<byte> keysToRemove = new List<byte>();
                    foreach (var key in chatpadKeysHeld)
                        if (key != dataPacket[26] && key != dataPacket[27])
                            keysToRemove.Add(key);
                    foreach (var key in keysToRemove)
                    {
                        if (keyboardKeysDown.Contains(keyMap[key]))
                        {
                            keyboardKeysDown.Remove(keyMap[key]);
                            Keyboard.KeyUp(keyMap[key]);
                        }
                        chatpadKeysHeld.Remove(key);
                    }
                }
            }
            else
                parentWindow.Invoke(new logCallback(parentWindow.logUpdate), "WARNING: Unknown Chatpad Data.\r\n");
        }

        public void ProcessGamepadData(byte[] dataPacket)
        {
            // This function is called anytime received data is identified as gamepad data
            // It will parse the data and feed it to the vJoy device as necessary

            // --------------------------
            // Directional Pad Processing
            // --------------------------

            // Set the POV hat based on the currently held direction
            switch (dataPacket[6])
            {
                case 0x01:
                    vJoystick.SetContPov(directionMap["Up"], 1, 1);
                    break;
                case 0x02:
                    vJoystick.SetContPov(directionMap["Down"], 1, 1);
                    break;
                case 0x04:
                    vJoystick.SetContPov(directionMap["Left"], 1, 1);
                    break;
                case 0x08:
                    vJoystick.SetContPov(directionMap["Right"], 1, 1);
                    break;
                case 0x05:
                    vJoystick.SetContPov(directionMap["UpLeft"], 1, 1);
                    break;
                case 0x06:
                    vJoystick.SetContPov(directionMap["DownLeft"], 1, 1);
                    break;
                case 0x09:
                    vJoystick.SetContPov(directionMap["UpRight"], 1, 1);
                    break;
                case 0x0A:
                    vJoystick.SetContPov(directionMap["DownRight"], 1, 1);
                    break;
                default:
                    vJoystick.SetContPov(directionMap["Neutral"], 1, 1);
                    break;
            }

            // -----------------
            // Button Processing
            // -----------------

            // All button data comes in on 2 bytes, specifically 6 and 7. The following code
            // uses a Bitwise AND for each button to determine if it is being held down

            // If in mouse mode use A and B as mouse clicks; otherwise set joystick buttons.
            if (mouseModeFlag)
            {

                // A Button - Left Mouse Button
                if ((dataPacket[7] & 0x10) > 0)
                {
                    if (!leftButtonDown)
                    {
                        Mouse.ButtonDown(Mouse.MouseKeys.Left);
                        leftButtonDown = true;
                    }
                }
                else
                {
                    if (leftButtonDown)
                    {
                        Mouse.ButtonUp(Mouse.MouseKeys.Left);
                        leftButtonDown = false;
                    }
                }

                // B Button - Right Mouse Button
                if ((dataPacket[7] & 0x20) > 0)
                {
                    if (!rightButtonDown)
                    {
                        Mouse.ButtonDown(Mouse.MouseKeys.Right);
                        rightButtonDown = true;
                    }
                }
                else
                {
                    if (rightButtonDown)
                    {
                        Mouse.ButtonUp(Mouse.MouseKeys.Right);
                        rightButtonDown = false;
                    }
                }
            }
            else
            {
                vJoystick.SetBtn((dataPacket[7] & 0x10) > 0, 1, buttonMap["A"]);
                vJoystick.SetBtn((dataPacket[7] & 0x20) > 0, 1, buttonMap["B"]);
            }

            vJoystick.SetBtn((dataPacket[7] & 0x40) > 0, 1, buttonMap["X"]);
            vJoystick.SetBtn((dataPacket[7] & 0x80) > 0, 1, buttonMap["Y"]);
            vJoystick.SetBtn((dataPacket[6] & 0x10) > 0, 1, buttonMap["Start"]);
            vJoystick.SetBtn((dataPacket[6] & 0x20) > 0, 1, buttonMap["Back"]);
            vJoystick.SetBtn((dataPacket[6] & 0x40) > 0, 1, buttonMap["LStick"]);
            vJoystick.SetBtn((dataPacket[6] & 0x80) > 0, 1, buttonMap["RStick"]);
            vJoystick.SetBtn((dataPacket[7] & 0x04) > 0, 1, buttonMap["Guide"]);

            // If in mouse mode use Left and Rught bumpers as navigationg shortcuts;
            // otherwise set joystick buttons.
            if (mouseModeFlag)
            {
                // Left Bumper - Navigate Back
                if ((dataPacket[7] & 0x01) > 0)
                    Keyboard.ShortcutKeys(navBack);

                // Right Bumper - Navigate Forward
                if ((dataPacket[7] & 0x02) > 0)
                    Keyboard.ShortcutKeys(navForward);

            }
            else
            {
                vJoystick.SetBtn((dataPacket[7] & 0x01) > 0, 1, buttonMap["LBump"]);
                vJoystick.SetBtn((dataPacket[7] & 0x02) > 0, 1, buttonMap["RBump"]);
            }

            // ---------------
            // Axis Processing
            // ---------------

            // Record the left stick and right stick X and Y values and left and right trigger values
            short leftX = (short)(dataPacket[10] | (dataPacket[11] << 8));
            short leftY = (short)(dataPacket[12] | (dataPacket[13] << 8));
            short rightX = (short)(dataPacket[14] | (dataPacket[15] << 8));
            short rightY = (short)(dataPacket[16] | (dataPacket[17] << 8));
            int leftTrig = dataPacket[8];
            int rightTrig = dataPacket[9];

            // Filter the left stick X and Y values based on the left circular deadzone
            double leftDistance = Math.Sqrt((double)(leftX * leftX) + (double)(leftY * leftY));
            if (leftDistance < deadzoneL)
            {
                leftX = 0;
                leftY = 0;
            }
            else
            {
                if (Math.Abs(Convert.ToInt32(leftX)) < deadzoneL)
                    leftX = 0;
                if (Math.Abs(Convert.ToInt32(leftY)) < deadzoneL)
                    leftY = 0;
            }

            // Filter the right stick X and Y values based on the right circular deadzone
            double rightDistance = Math.Sqrt((double)(rightX * rightX) + (double)(rightY * rightY));
            if (rightDistance < deadzoneR)
            {
                rightX = 0;
                rightY = 0;
            }
            else
            {
                if (Math.Abs(Convert.ToInt32(rightX)) < deadzoneR)
                    rightX = 0;
                if (Math.Abs(Convert.ToInt32(rightY)) < deadzoneR)
                    rightY = 0;
            }

            // If in Mouse Mode use the left stick and left trigger to determine the movement
            // of the mouse. Use the right stick and right trigger to determine the movement
            // of the scrollbars. If not in mouse mode, simply set the joysticks.
            if (mouseModeFlag)
            {
                // Sets the maximum velocity based on the amount of Left Trigger depressed.
                // A more depressed trigger yields a higher max velocity.
                int maxVelocity = 10;
                if (leftTrig >= 50)
                    maxVelocity = 20;

                // Sets the mouse X and Y velocity to a fraction of the maximum velocity based on
                // the position of the Left stick.
                mouseVelX = maxVelocity * leftX / 32767;
                mouseVelY = maxVelocity * leftY / 32767;

                // Sets the right stick direction based on the Y position of the Right stick.
                if (rightY < 0)
                    rightStickDir = -1;
                else if (rightY > 0)
                    rightStickDir = 1;
                else
                    rightStickDir = 0;
            }
            else
            {
                // Set the left stick X and Y values
                // Note: For some reason, the left stick Y axis is inverted, multiplied by -1 to fix
                vJoystick.SetAxis(leftX, 1, axisMap["LX"]);
                vJoystick.SetAxis(-leftY, 1, axisMap["LY"]);

                // Set the right stick X and Y values
                vJoystick.SetAxis(rightX, 1, axisMap["RX"]);
                vJoystick.SetAxis(rightY, 1, axisMap["RY"]);

                // If in FFXIV Mode the Left and Right Triggers are buttons otherwise
                // they are separate axes.
                if (controllerTrigger == "Button")
                {
                    // Left Trigger
                    if (leftTrig >= 50)
                        vJoystick.SetBtn(true, 1, buttonMap["LTrig"]);
                    else
                        vJoystick.SetBtn(false, 1, buttonMap["LTrig"]);

                    // Right Trigger
                    if (rightTrig >= 50)
                        vJoystick.SetBtn(true, 1, buttonMap["RTrig"]);
                    else
                        vJoystick.SetBtn(false, 1, buttonMap["RTrig"]);
                }
                else
                {
                    // Left Trigger
                    vJoystick.SetAxis(leftTrig, 1, axisMap["LTrig"]);

                    // Right Trigger
                    vJoystick.SetAxis(rightTrig, 1, axisMap["RTrig"]);
                }
            }

            // ------------------
            // Special Processing
            // ------------------

            // The special button combinations will be stored as booleans when they are
            // detected. A higher level timer function will use these booleans to determine
            // if the associated actions should be executed.

            // Indicate to toggle mouse mode based on the command sequence:
            // LBump + RBump + Back
            if (((dataPacket[7] & 0x01) > 0) && ((dataPacket[7] & 0x02) > 0) && ((dataPacket[6] & 0x20) > 0))
                cmdMouseModeToggle = true;
            else
                cmdMouseModeToggle = false;

            // Indicate to shutdown controller based on the command sequence:
            // LTrig + RTrig + Back
            if ((leftTrig >= 50) && (rightTrig >= 50) && ((dataPacket[6] & 0x20) > 0))
                cmdKillController = true;
            else
                cmdKillController = false;
        }

        private void sendData(byte[] dataToSend)
        {
            // This function sends the supplied data via the Endpoint Writer
            // to the Wireless Receiver as long as it is attached.
            int bytesWritten;

            ErrorCode ec = epWriter.Write(dataToSend, 2000, out bytesWritten);

            if (ec != ErrorCode.None)
                parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                    "ERROR: Problem Sending Controller Data.");
        }

        private void ProcessKeypress(byte key)
        {
            if (key != 0 && !chatpadKeysHeld.Contains(key))
            {
                // If key is non-zero and was not previously being held down,
                // record that is now being held
                chatpadKeysHeld.Add(key);

                // Process the keystroke for the character associated with the key
                // depending on the status of Orange and Green modifiers.
                if (chatpadMod["Orange"])
                {
                    if (flagUpperCase)
                        SendKeys.SendWait(orangeMap[key].ToUpper());
                    else
                        SendKeys.SendWait(orangeMap[key]);
                }
                else if (chatpadMod["Green"])
                {
                    if (flagUpperCase)
                        SendKeys.SendWait(greenMap[key].ToUpper());
                    else
                        SendKeys.SendWait(greenMap[key]);
                }
                else
                {
                    keyboardKeysDown.Add(keyMap[key]);
                    Keyboard.KeyDown(keyMap[key]);
                }
            }
        }

        public void configureChatpad(string keyboardType)
        {
            // Updates the controller dictionaries with keystrokes based on the
            // specified keyboard type.
            switch (keyboardType)
            {
                case "Q W E R T Y":
                    keyMap[23] = Keys.D1; greenMap[23] = ""; orangeMap[23] = "";
                    keyMap[22] = Keys.D2; greenMap[22] = ""; orangeMap[22] = "";
                    keyMap[21] = Keys.D3; greenMap[21] = ""; orangeMap[21] = "";
                    keyMap[20] = Keys.D4; greenMap[20] = ""; orangeMap[20] = "";
                    keyMap[19] = Keys.D5; greenMap[19] = ""; orangeMap[19] = "";
                    keyMap[18] = Keys.D6; greenMap[18] = ""; orangeMap[18] = "";
                    keyMap[17] = Keys.D7; greenMap[17] = ""; orangeMap[17] = "";
                    keyMap[103] = Keys.D8; greenMap[103] = ""; orangeMap[103] = "";
                    keyMap[102] = Keys.D9; greenMap[102] = ""; orangeMap[102] = "";
                    keyMap[101] = Keys.D0; greenMap[101] = ""; orangeMap[101] = "";

                    keyMap[39] = Keys.Q; greenMap[39] = "!"; orangeMap[39] = "¡";
                    keyMap[38] = Keys.W; greenMap[38] = "@"; orangeMap[38] = "å";
                    keyMap[37] = Keys.E; greenMap[37] = "€"; orangeMap[37] = "é";
                    keyMap[36] = Keys.R; greenMap[36] = "#"; orangeMap[36] = "$";
                    keyMap[35] = Keys.T; greenMap[35] = "{%}"; orangeMap[35] = "Þ";
                    keyMap[34] = Keys.Y; greenMap[34] = "{^}"; orangeMap[34] = "ý";
                    keyMap[33] = Keys.U; greenMap[33] = "&"; orangeMap[33] = "ú";
                    keyMap[118] = Keys.I; greenMap[118] = "*"; orangeMap[118] = "í";
                    keyMap[117] = Keys.O; greenMap[117] = "{(}"; orangeMap[117] = "ó";
                    keyMap[100] = Keys.P; greenMap[100] = "{)}"; orangeMap[100] = "=";

                    keyMap[55] = Keys.A; greenMap[55] = "{~}"; orangeMap[55] = "á";
                    keyMap[54] = Keys.S; greenMap[54] = "š"; orangeMap[54] = "ß";
                    keyMap[53] = Keys.D; greenMap[53] = "{{}"; orangeMap[53] = "ð";
                    keyMap[52] = Keys.F; greenMap[52] = "{}}"; orangeMap[52] = "£";
                    keyMap[51] = Keys.G; greenMap[51] = "¨"; orangeMap[51] = "¥";
                    keyMap[50] = Keys.H; greenMap[50] = "/"; orangeMap[50] = "\\";
                    keyMap[49] = Keys.J; greenMap[49] = "'"; orangeMap[49] = "\"";
                    keyMap[119] = Keys.K; greenMap[119] = "{[}"; orangeMap[119] = "☺";
                    keyMap[114] = Keys.L; greenMap[114] = "{]}"; orangeMap[114] = "ø";
                    keyMap[98] = Keys.Oemcomma; greenMap[98] = ":"; orangeMap[98] = ";";

                    keyMap[70] = Keys.Z; greenMap[70] = "`"; orangeMap[70] = "æ";
                    keyMap[69] = Keys.X; greenMap[69] = "«"; orangeMap[69] = "œ";
                    keyMap[68] = Keys.C; greenMap[68] = "»"; orangeMap[68] = "ç";
                    keyMap[67] = Keys.V; greenMap[67] = "-"; orangeMap[67] = "_";
                    keyMap[66] = Keys.B; greenMap[66] = "|"; orangeMap[66] = "{+}";
                    keyMap[65] = Keys.N; greenMap[65] = "<"; orangeMap[65] = "ñ";
                    keyMap[82] = Keys.M; greenMap[82] = ">"; orangeMap[82] = "µ";
                    keyMap[83] = Keys.OemPeriod; greenMap[83] = "?"; orangeMap[83] = "¿";
                    keyMap[99] = Keys.Enter; greenMap[99] = ""; orangeMap[99] = "";

                    keyMap[85] = Keys.Left; greenMap[85] = ""; orangeMap[85] = "";
                    keyMap[84] = Keys.Space; greenMap[84] = ""; orangeMap[84] = "";
                    keyMap[81] = Keys.Right; greenMap[81] = ""; orangeMap[81] = "";
                    keyMap[113] = Keys.Back; greenMap[113] = ""; orangeMap[113] = "";
                    break;

                case "Q W E R T Z":
                    keyMap[23] = Keys.D1; greenMap[23] = ""; orangeMap[23] = "";
                    keyMap[22] = Keys.D2; greenMap[22] = ""; orangeMap[22] = "";
                    keyMap[21] = Keys.D3; greenMap[21] = ""; orangeMap[21] = "";
                    keyMap[20] = Keys.D4; greenMap[20] = ""; orangeMap[20] = "";
                    keyMap[19] = Keys.D5; greenMap[19] = ""; orangeMap[19] = "";
                    keyMap[18] = Keys.D6; greenMap[18] = ""; orangeMap[18] = "";
                    keyMap[17] = Keys.D7; greenMap[17] = ""; orangeMap[17] = "";
                    keyMap[103] = Keys.D8; greenMap[103] = ""; orangeMap[103] = "";
                    keyMap[102] = Keys.D9; greenMap[102] = ""; orangeMap[102] = "";
                    keyMap[101] = Keys.D0; greenMap[101] = ""; orangeMap[101] = "";

                    keyMap[39] = Keys.Q; greenMap[39] = "!"; orangeMap[39] = "@";
                    keyMap[38] = Keys.W; greenMap[38] = "\""; orangeMap[38] = "¡";
                    keyMap[37] = Keys.E; greenMap[37] = "€"; orangeMap[37] = "é";
                    keyMap[36] = Keys.R; greenMap[36] = "$"; orangeMap[36] = "¥";
                    keyMap[35] = Keys.T; greenMap[35] = "{%}"; orangeMap[35] = "Þ";
                    keyMap[34] = Keys.Z; greenMap[34] = "&"; orangeMap[34] = "{^}";
                    keyMap[33] = Keys.U; greenMap[33] = "/"; orangeMap[33] = "ü";
                    keyMap[118] = Keys.I; greenMap[118] = "{(}"; orangeMap[118] = "í";
                    keyMap[117] = Keys.O; greenMap[117] = "{)}"; orangeMap[117] = "ö";
                    keyMap[100] = Keys.P; greenMap[100] = "="; orangeMap[100] = "\\";

                    keyMap[55] = Keys.A; greenMap[55] = "å"; orangeMap[55] = "ä";
                    keyMap[54] = Keys.S; greenMap[54] = "ß"; orangeMap[54] = "š";
                    keyMap[53] = Keys.D; greenMap[53] = "«"; orangeMap[53] = "ð";
                    keyMap[52] = Keys.F; greenMap[52] = "»"; orangeMap[52] = "£";
                    keyMap[51] = Keys.G; greenMap[51] = "¨"; orangeMap[51] = "☺";
                    keyMap[50] = Keys.H; greenMap[50] = "{{}"; orangeMap[50] = "`";
                    keyMap[49] = Keys.J; greenMap[49] = "{}}"; orangeMap[49] = "ø";
                    keyMap[119] = Keys.K; greenMap[119] = "{[}"; orangeMap[119] = "æ";
                    keyMap[114] = Keys.L; greenMap[114] = "{]}"; orangeMap[114] = "œ";
                    keyMap[98] = Keys.Oemcomma; greenMap[98] = "':"; orangeMap[98] = "#;";

                    keyMap[70] = Keys.Y; greenMap[70] = "<"; orangeMap[70] = "°";
                    keyMap[69] = Keys.X; greenMap[69] = ">"; orangeMap[69] = "|";
                    keyMap[68] = Keys.C; greenMap[68] = "{~}"; orangeMap[68] = "ç";
                    keyMap[67] = Keys.V; greenMap[67] = "-"; orangeMap[67] = "_";
                    keyMap[66] = Keys.B; greenMap[66] = "*"; orangeMap[66] = "{+}";
                    keyMap[65] = Keys.N; greenMap[65] = ";"; orangeMap[65] = "ñ";
                    keyMap[82] = Keys.M; greenMap[82] = ":"; orangeMap[82] = "µ";
                    keyMap[83] = Keys.OemPeriod; greenMap[83] = "?"; orangeMap[83] = "¿";
                    keyMap[99] = Keys.Enter; greenMap[99] = ""; orangeMap[99] = "";

                    keyMap[85] = Keys.Left; greenMap[85] = ""; orangeMap[85] = "";
                    keyMap[84] = Keys.Space; greenMap[84] = ""; orangeMap[84] = "";
                    keyMap[81] = Keys.Right; greenMap[81] = ""; orangeMap[81] = "";
                    keyMap[113] = Keys.Back; greenMap[113] = ""; orangeMap[113] = "";
                    break;

                case "A Z E R T Y":
                    keyMap[23] = Keys.D1; greenMap[23] = ""; orangeMap[23] = "";
                    keyMap[22] = Keys.D2; greenMap[22] = ""; orangeMap[22] = "";
                    keyMap[21] = Keys.D3; greenMap[21] = ""; orangeMap[21] = "";
                    keyMap[20] = Keys.D4; greenMap[20] = ""; orangeMap[20] = "";
                    keyMap[19] = Keys.D5; greenMap[19] = ""; orangeMap[19] = "";
                    keyMap[18] = Keys.D6; greenMap[18] = ""; orangeMap[18] = "";
                    keyMap[17] = Keys.D7; greenMap[17] = ""; orangeMap[17] = "";
                    keyMap[103] = Keys.D8; greenMap[103] = ""; orangeMap[103] = "";
                    keyMap[102] = Keys.D9; greenMap[102] = ""; orangeMap[102] = "";
                    keyMap[101] = Keys.D0; greenMap[101] = ""; orangeMap[101] = "";

                    keyMap[39] = Keys.A; greenMap[39] = "à"; orangeMap[39] = "&";
                    keyMap[38] = Keys.Z; greenMap[38] = "æ"; orangeMap[38] = "{~}";
                    keyMap[37] = Keys.E; greenMap[37] = "€"; orangeMap[37] = "\"";
                    keyMap[36] = Keys.R; greenMap[36] = "é"; orangeMap[36] = "$";
                    keyMap[35] = Keys.T; greenMap[35] = "#"; orangeMap[35] = "{(}";
                    keyMap[34] = Keys.Y; greenMap[34] = "ý"; orangeMap[34] = "-";
                    keyMap[33] = Keys.U; greenMap[33] = "ù"; orangeMap[33] = "`";
                    keyMap[118] = Keys.I; greenMap[118] = "ì"; orangeMap[118] = "_";
                    keyMap[117] = Keys.O; greenMap[117] = "œ"; orangeMap[117] = "@";
                    keyMap[100] = Keys.P; greenMap[100] = "ó"; orangeMap[100] = "{)}";

                    keyMap[55] = Keys.Q; greenMap[55] = "å"; orangeMap[55] = "☺";
                    keyMap[54] = Keys.S; greenMap[54] = "š"; orangeMap[54] = "«";
                    keyMap[53] = Keys.D; greenMap[53] = "ð"; orangeMap[53] = "»";
                    keyMap[52] = Keys.F; greenMap[52] = "Þ"; orangeMap[52] = "{{}";
                    keyMap[51] = Keys.G; greenMap[51] = "¨"; orangeMap[51] = "¥";
                    keyMap[50] = Keys.H; greenMap[50] = "|"; orangeMap[50] = "ø";
                    keyMap[49] = Keys.J; greenMap[49] = "µ"; orangeMap[49] = "¨";
                    keyMap[119] = Keys.K; greenMap[119] = "/"; orangeMap[119] = "\\";
                    keyMap[114] = Keys.L; greenMap[114] = "$"; orangeMap[114] = "£";
                    keyMap[98] = Keys.M; greenMap[98] = "*"; orangeMap[98] = "{^}";

                    keyMap[70] = Keys.W; greenMap[70] = "¡"; orangeMap[70] = "<";
                    keyMap[69] = Keys.X; greenMap[69] = "¿"; orangeMap[69] = ">";
                    keyMap[68] = Keys.C; greenMap[68] = "ç"; orangeMap[68] = "{[}";
                    keyMap[67] = Keys.V; greenMap[67] = "="; orangeMap[67] = "{]}";
                    keyMap[66] = Keys.B; greenMap[66] = "{+}"; orangeMap[66] = "{%}";
                    keyMap[65] = Keys.N; greenMap[65] = "?"; orangeMap[65] = "ñ";
                    keyMap[82] = Keys.Oemcomma; greenMap[82] = "!"; orangeMap[82] = "'";
                    keyMap[83] = Keys.OemPeriod; greenMap[83] = ":"; orangeMap[83] = ";";
                    keyMap[99] = Keys.Enter; greenMap[99] = ""; orangeMap[99] = "";

                    keyMap[85] = Keys.Left; greenMap[85] = ""; orangeMap[85] = "";
                    keyMap[84] = Keys.Space; greenMap[84] = ""; orangeMap[84] = "";
                    keyMap[81] = Keys.Right; greenMap[81] = ""; orangeMap[81] = "";
                    keyMap[113] = Keys.Back; greenMap[113] = ""; orangeMap[113] = "";
                    break;

                default:
                    parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                        "ERROR: Unknown Keyboard Type.\r\n");
                    break;
            }
        }

        public void configureGamepad(string triggerType)
        {
            // Updates the controller dictionaries and record the specified
            // trigger type of either Button or Axis.
            controllerTrigger = triggerType;

            switch (triggerType)
            {
                case "Button":
                    buttonMap["A"]      = 3;
                    buttonMap["B"]      = 4;
                    buttonMap["X"]      = 1;
                    buttonMap["Y"]      = 2;
                    buttonMap["LStick"] = 9;
                    buttonMap["RStick"] = 10;
                    buttonMap["LBump"]  = 5;
                    buttonMap["RBump"]  = 6;
                    buttonMap["LTrig"]  = 7;
                    buttonMap["RTrig"]  = 8;
                    buttonMap["Back"]   = 11;
                    buttonMap["Start"]  = 12;
                    buttonMap["Guide"]  = 13;

                    directionMap["Neutral"]     = -1;
                    directionMap["Up"]          = 0;
                    directionMap["UpRight"]     = 4500;
                    directionMap["Right"]       = 9000;
                    directionMap["DownRight"]   = 13500;
                    directionMap["Down"]        = 18000;
                    directionMap["DownLeft"]    = 22500;
                    directionMap["Left"]        = 27000;
                    directionMap["UpLeft"]      = 31500;

                    axisMap["LX"] = HID_USAGES.HID_USAGE_X;
                    axisMap["LY"] = HID_USAGES.HID_USAGE_Y;
                    axisMap["RX"] = HID_USAGES.HID_USAGE_Z;
                    axisMap["RY"] = HID_USAGES.HID_USAGE_RZ;
                    break;

                case "Axis":
                    buttonMap["A"]      = 3;
                    buttonMap["B"]      = 4;
                    buttonMap["X"]      = 1;
                    buttonMap["Y"]      = 2;
                    buttonMap["LStick"] = 9;
                    buttonMap["RStick"] = 10;
                    buttonMap["LBump"]  = 5;
                    buttonMap["RBump"]  = 6;
                    buttonMap["Back"]   = 7;
                    buttonMap["Start"]  = 8;
                    buttonMap["Guide"]  = 11;

                    directionMap["Neutral"]     = -1;
                    directionMap["Up"]          = 0;
                    directionMap["UpRight"]     = 4500;
                    directionMap["Right"]       = 9000;
                    directionMap["DownRight"]   = 13500;
                    directionMap["Down"]        = 18000;
                    directionMap["DownLeft"]    = 22500;
                    directionMap["Left"]        = 27000;
                    directionMap["UpLeft"]      = 31500;

                    axisMap["LX"]       = HID_USAGES.HID_USAGE_X;
                    axisMap["LY"]       = HID_USAGES.HID_USAGE_Y;
                    axisMap["RX"]       = HID_USAGES.HID_USAGE_RX;
                    axisMap["RY"]       = HID_USAGES.HID_USAGE_RY;
                    axisMap["LTrig"]    = HID_USAGES.HID_USAGE_Z;
                    axisMap["RTrig"]    = HID_USAGES.HID_USAGE_RZ;
                    break;

                default:
                    parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                        "ERROR: Unknown Trigger Type.\r\n");
                    break;
            }
        }

        public void startController()
        {
            // Sends command to begin polling for controller data
            // Note: This is intentionally redundant, things seem to work better as a result
            sendData(controllerCommands["RefreshConnection"]);
            sendData(controllerCommands["RefreshConnection"]);
        }

        public void killController()
        {
            // Sends command to disable the controller
            if (controllerAttached)
                sendData(controllerCommands["DisableController"]);
        }

        private void tickButtonCombo()
        {
            // This function is executed periodically to process special button combinations when they
            // are held down for a specific amount of time.
            int mouseModeTick = 0;
            int killControllerTick = 0;

            while (true)
            {
                // Increment mouse mode counter if key combination for toggling the mouse
                // mode is detected, otherwise reset the counter.
                if (cmdMouseModeToggle)
                    mouseModeTick++;
                else
                    mouseModeTick = 0;

                // Toggle the mouse mode if the mouse mode counter hits 3 (1.5 seconds)
                if (mouseModeTick == 3)
                    toggleMouseMode(!mouseModeFlag);

                // Increment kill controller counter if key combination for killing the
                // controller is detected, otherwise reset the counter.
                if (cmdKillController)
                    killControllerTick++;
                else
                    killControllerTick = 0;

                // Kills the controller if the kill controller counter hits 6 (3 seconds)
                if (killControllerTick == 6)
                    sendData(controllerCommands["DisableController"]);

                System.Threading.Thread.Sleep(500);
            }
        }

        public void killButtonCombo()
        {
            // Cleans up the Button Combo thread
            if (threadButtonCombo != null)
            {
                threadButtonCombo.Abort();
                threadButtonCombo = null;
            }
        }

        private void tickKeepAlive()
        {
            // This function loops every second on a separate background thread
            // as long as the controller is connected. It will send unique alternating
            // device commands in order to keep the device alive, and if necessary send
            // chatpad initialization commands.

            // Keep-Alive Toggle, this will determine which keep-alive command will be sent
            // during each execution cycle, True = Command 1, False = Commands 2a and 2b
            bool keepAliveToggle = false;

            while (true)
            {
                if (epWriter != null)
                {
                    if (keepAliveToggle)
                        sendData(controllerCommands["KeepAlive1"]);
                    else
                        sendData(controllerCommands["KeepAlive2"]);

                    keepAliveToggle = !keepAliveToggle;

                    if (chatpadInitNeeded)
                    {
                        // Initialize Chatpad Communication
                        sendData(controllerCommands["ChatpadInit"]);

                        // Set Initialization flag to False, no need to do it again
                        chatpadInitNeeded = false;
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        public void killKeepAlive()
        {
            // Cleans up the Keep-Alive thread
            if (threadKeepAlive != null)
            {
                threadKeepAlive.Abort();
                threadKeepAlive = null;
            }
        }

        private void toggleMouseMode(bool mouseMode)
        {
            // This function will be used to enable/disable Mouse Mode
            if (mouseMode)
            {
                // Set the Gamepad instantiation to Mouse Mode
                mouseModeFlag = true;

                // Start the Mouse Mode thread
                mouseModeThread = new System.Threading.Thread(new System.Threading.ThreadStart(tickMouseMode));
                mouseModeThread.IsBackground = true;
                mouseModeThread.Start();

                // Update to Mouse Mode Label to ON
                parentWindow.Invoke(new mouseModeLabelCallback(parentWindow.mouseModeLabelUpdate),
                    "Mouse Mode: ON");
            }
            else
            {
                // Set the Gamepad instantiation to Normal Mode
                mouseModeFlag = false;

                // Kill the Mouse Mode thread
                killMouseMode();

                // Update to Mouse Mode Label to OFF
                parentWindow.Invoke(new mouseModeLabelCallback(parentWindow.mouseModeLabelUpdate),
                    "Mouse Mode: OFF");
            }
        }

        private void tickMouseMode()
        {
            // This function is executed periodically to continually move the mouse based on
            // the left joystick as long as the relative coordinates are not (0,0). In addition
            // this will allow for vertical scrolling using the right joystick
            // Note: For some reason, the left stick Y axis is inverted, multiplied by -1 to fix
            int tickCount = 0;

            while (true)
            {
                if ((Math.Abs(mouseVelX) > 0) || ((Math.Abs(mouseVelY) > 0)))
                    Mouse.MoveRelative(mouseVelX, -mouseVelY);

                // The tickCount will get incremented each time this thread executes. The check for 5
                // will allow the resulting code to execute every 5th iteration. This necessary to
                // keep the cursor speed fluid while still having a usable scroll speed.
                if (tickCount == 4)
                {
                    if (rightStickDir == -1)
                        Mouse.Scroll(Mouse.ScrollDirection.Down);
                    else if (rightStickDir == 1)
                        Mouse.Scroll(Mouse.ScrollDirection.Up);

                    tickCount = 0;
                }

                tickCount++;

                System.Threading.Thread.Sleep(20);
            }
        }

        public void killMouseMode()
        {
            // Clean up the Mouse Mode thread
            if (mouseModeThread != null)
            {
                mouseModeThread.Abort();
                mouseModeThread = null;
            }
        }
    }
}
