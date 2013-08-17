using System;
using System.Collections.Generic;
using System.Diagnostics;

using vJoyInterfaceWrap;

namespace Xbox360WirelessChatpad
{
    public class Gamepad
    {
        // The vJoy virtual joystick
        vJoy vJoystick;

        // Deadzone variables for the joysticks on the gamepad
        public int deadzoneL = 0;
        public int deadzoneR = 0;

        // Global FFXIV Flag for use by data packet processing
        public bool ffxivFlag;

        // Contains the mapping of Gamepad buttons
        Dictionary<String, uint> buttonMap;

        // Contains the mapping of Gamepad directional pad buttons
        Dictionary<String, int> directionMap;

        // Contains the mapping of Gamepad axes
        Dictionary<String, HID_USAGES> axisMap;

        public Gamepad(bool ffFlag)
        {
            // Populates the Dictionaries depending on Final Fantasy XIV Flag
            // This is done to retain full functionality of the controller app,
            // but also allow the controller to work with FF14.
            if (ffFlag)
            {
                ffxivFlag = ffFlag;

                buttonMap = new Dictionary<String, uint>()
                {
                    {"A", 3},         // A
                    {"B", 4},         // B
                    {"X", 1},         // X
                    {"Y", 2},         // Y
                    {"LStick", 9},    // Left Analog Stick Press
                    {"RStick", 10},   // Right Analog Stick Press
                    {"LBump", 5},     // Left Bumper
                    {"RBump", 6},     // Right Bumper
                    {"LTrig", 7},     // Left Trigger
                    {"RTrig", 8},     // Right Trigger
                    {"Back", 11},     // Back
                    {"Start", 12},    // Start
                    {"Guide", 13}     // Guide
                };

                directionMap = new Dictionary<String, int>()
                {
                    {"Neutral", -1},        // Neutral, Nothing Pressed
                    {"Up", 0},              // Up
                    {"UpRight", 4500},      // Up-Right
                    {"Right", 9000},        // Right
                    {"DownRight", 13500},   // Down-Right
                    {"Down", 18000},        // Down
                    {"DownLeft", 22500},    // Down-Left
                    {"Left", 27000},        // Left
                    {"UpLeft", 31500}       // Up-Left
                };

                axisMap = new Dictionary<String, HID_USAGES>()
                {
                    {"LX", HID_USAGES.HID_USAGE_X},     // Left Stick X Axis
                    {"LY", HID_USAGES.HID_USAGE_Y},     // Left Stick Y Axis
                    {"RX", HID_USAGES.HID_USAGE_Z},     // Right Stick X Axis
                    {"RY", HID_USAGES.HID_USAGE_RZ}     // Right Stick Y Axis
                };
            }
            else
            {
                buttonMap = new Dictionary<String, uint>()
                {
                    {"A", 1},         // A
                    {"B", 2},         // B
                    {"X", 3},         // X
                    {"Y", 4},         // Y
                    {"LStick", 5},    // Left Analog Stick Press
                    {"RStick", 6},    // Right Analog Stick Press
                    {"LBump", 7},     // Left Bumper
                    {"RBump", 8},     // Right Bumper
                    {"Guide", 9},     // Center/Power Button
                    {"Back", 10},     // Back
                    {"Start", 11}     // Start
                };

                directionMap = new Dictionary<String, int>()
                {
                    {"Neutral", -1},        // Neutral, Nothing Pressed
                    {"Up", 0},              // Up
                    {"UpRight", 4500},      // Up-Right
                    {"Right", 9000},        // Right
                    {"DownRight", 13500},   // Down-Right
                    {"Down", 18000},        // Down
                    {"DownLeft", 22500},    // Down-Left
                    {"Left", 27000},        // Left
                    {"UpLeft", 31500}       // Up-Left
                };

                axisMap = new Dictionary<String, HID_USAGES>()
                {
                    {"LTrig", HID_USAGES.HID_USAGE_Z},    // Left Trigger
                    {"RTrig", HID_USAGES.HID_USAGE_RZ},   // Right Trigger
                    {"LX", HID_USAGES.HID_USAGE_X},       // Left Stick X Axis
                    {"LY", HID_USAGES.HID_USAGE_Y},       // Left Stick Y Axis
                    {"RX", HID_USAGES.HID_USAGE_RX},      // Right Stick X Axis
                    {"RY", HID_USAGES.HID_USAGE_RY}       // Right Stick Y Axis
                };
            }

            // Instantiate the virtual joystick
            vJoystick = new vJoy();

            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!vJoystick.vJoyEnabled())
            {
                Trace.WriteLine("ERROR: vJoy Driver Not Enabled");
                return;
            }

            // Retreives the virtual joystick status
            VjdStat vJoystickStatus = vJoystick.GetVJDStatus(1);

            // Acquire the virtual joystick
            if ((vJoystickStatus == VjdStat.VJD_STAT_OWN) ||
                ((vJoystickStatus == VjdStat.VJD_STAT_FREE) && (!vJoystick.AcquireVJD(1))))
                Trace.WriteLine("ERROR: Failed to Acquire vJoy Gamepad Number 1.");
        }

        public void ProcessData(byte[] dataPacket)
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
            vJoystick.SetBtn((dataPacket[7] & 0x10) > 0, 1, buttonMap["A"]);
            vJoystick.SetBtn((dataPacket[7] & 0x20) > 0, 1, buttonMap["B"]);
            vJoystick.SetBtn((dataPacket[7] & 0x40) > 0, 1, buttonMap["X"]);
            vJoystick.SetBtn((dataPacket[7] & 0x80) > 0, 1, buttonMap["Y"]);
            vJoystick.SetBtn((dataPacket[6] & 0x10) > 0, 1, buttonMap["Start"]);
            vJoystick.SetBtn((dataPacket[6] & 0x20) > 0, 1, buttonMap["Back"]);
            vJoystick.SetBtn((dataPacket[6] & 0x40) > 0, 1, buttonMap["LStick"]);
            vJoystick.SetBtn((dataPacket[6] & 0x80) > 0, 1, buttonMap["RStick"]);
            vJoystick.SetBtn((dataPacket[7] & 0x01) > 0, 1, buttonMap["LBump"]);
            vJoystick.SetBtn((dataPacket[7] & 0x02) > 0, 1, buttonMap["RBump"]);
            vJoystick.SetBtn((dataPacket[7] & 0x04) > 0, 1, buttonMap["Guide"]);

            // ---------------
            // Axis Processing
            // ---------------

            // Record the left stick X and Y values
            short leftX = (short)(dataPacket[10] | (dataPacket[11] << 8));
            short leftY = (short)(dataPacket[12] | (dataPacket[13] << 8));

            // Filter the left stick X and Y values based on the circular deadzone
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

            // Set the left stick X and Y values
            // Note: For some reason, the left stick Y axis is inverted, multiplied by -1 to fix
            vJoystick.SetAxis(leftX, 1, axisMap["LX"]);
            vJoystick.SetAxis(-leftY, 1, axisMap["LY"]);

            // If we're in FFXIV Mode, the Left and Right Triggers are buttons and the R and Z
            // axes are used in a very peculiar manner.
            if (!ffxivFlag)
	        {
		        // Record the right stick X and Y values
                short rightX = (short)(dataPacket[14] | (dataPacket[15] << 8));
                short rightY = (short)(dataPacket[16] | (dataPacket[17] << 8));

                // Filter the right stick X and Y values based on the circular deadzone
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

                // Set the right stick X and Y values
                vJoystick.SetAxis(rightX, 1, axisMap["RX"]);
                vJoystick.SetAxis(rightY, 1, axisMap["RY"]);

                // Left Trigger
                vJoystick.SetAxis(dataPacket[8], 1, axisMap["LTrig"]);

                // Right Trigger
                vJoystick.SetAxis(dataPacket[9], 1, axisMap["RTrig"]); 
	        }
            else
            {
                // Record the right stick X and Y values
                short rightX = (short)(dataPacket[14] | (dataPacket[15] << 8));
                short rightY = (short)(dataPacket[16] | (dataPacket[17] << 8));

                // Filter the right stick X and Y values based on the circular deadzone
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

                // Set the right stick X and Y values
                vJoystick.SetAxis(rightX, 1, axisMap["RX"]);
                vJoystick.SetAxis(rightY, 1, axisMap["RY"]);

                // Left Trigger
                if (dataPacket[8] >= 50)
                    vJoystick.SetBtn(true, 1, buttonMap["LTrig"]);
                else
                    vJoystick.SetBtn(false, 1, buttonMap["LTrig"]);

                // Right Trigger
                if (dataPacket[9] >= 50)
                    vJoystick.SetBtn(true, 1, buttonMap["RTrig"]);
                else
                    vJoystick.SetBtn(false, 1, buttonMap["RTrig"]);
            }
        }
    }
}
