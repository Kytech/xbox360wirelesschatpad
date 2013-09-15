using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

using InputManager;

namespace Xbox360WirelessChatpad
{
    public class Chatpad
    {
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

        // Identifies which keys are currently being held down, used to determine
        // if a keystroke should be sent or not
        private List<byte> chatpadKeysHeld = new List<byte>();

        // Identifies which keyboard keys are down, used to track if a KeyUp command
        // needs to be sent or not
        private List<Keys> keyboardKeysDown = new List<Keys>();

        // Identifies if the sent key data should be upper case or lower case
        bool flagUpperCase = false;

        // Identifies if Alt-Tab cycling has begun
        bool altTabActive = false;

        // Used to determine if the data has changed since the last packet
        private byte[] dataPacketLast = new byte[3];

        public Chatpad(string keyboardType)
        {
            // Map the Xbox chatpad buttons to characters
            switch (keyboardType)
            {
                case "Q W E R T Y":
                    keyMap.Add(23, Keys.D1);        greenMap.Add(23, "");       orangeMap.Add(23, "");
                    keyMap.Add(22, Keys.D2);        greenMap.Add(22, "");       orangeMap.Add(22, "");
                    keyMap.Add(21, Keys.D3);        greenMap.Add(21, "");       orangeMap.Add(21, "");
                    keyMap.Add(20, Keys.D4);        greenMap.Add(20, "");       orangeMap.Add(20, "");
                    keyMap.Add(19, Keys.D5);        greenMap.Add(19, "");       orangeMap.Add(19, "");
                    keyMap.Add(18, Keys.D6);        greenMap.Add(18, "");       orangeMap.Add(18, "");
                    keyMap.Add(17, Keys.D7);        greenMap.Add(17, "");       orangeMap.Add(17, "");
                    keyMap.Add(103, Keys.D8);       greenMap.Add(103, "");      orangeMap.Add(103, "");
                    keyMap.Add(102, Keys.D9);       greenMap.Add(102, "");      orangeMap.Add(102, "");
                    keyMap.Add(101, Keys.D0);       greenMap.Add(101, "");      orangeMap.Add(101, "");

                    keyMap.Add(39, Keys.Q);         greenMap.Add(39, "!");      orangeMap.Add(39, "¡");
                    keyMap.Add(38, Keys.W);         greenMap.Add(38, "@");      orangeMap.Add(38, "å");
                    keyMap.Add(37, Keys.E);         greenMap.Add(37, "€");      orangeMap.Add(37, "é");
                    keyMap.Add(36, Keys.R);         greenMap.Add(36, "#");      orangeMap.Add(36, "$");
                    keyMap.Add(35, Keys.T);         greenMap.Add(35, "{%}");    orangeMap.Add(35, "Þ");
                    keyMap.Add(34, Keys.Y);         greenMap.Add(34, "{^}");    orangeMap.Add(34, "ý");
                    keyMap.Add(33, Keys.U);         greenMap.Add(33, "&");      orangeMap.Add(33, "ú");
                    keyMap.Add(118, Keys.I);        greenMap.Add(118, "*");     orangeMap.Add(118, "í");
                    keyMap.Add(117, Keys.O);        greenMap.Add(117, "{(}");   orangeMap.Add(117, "ó");
                    keyMap.Add(100, Keys.P);        greenMap.Add(100, "{)}");   orangeMap.Add(100, "=");

                    keyMap.Add(55, Keys.A);         greenMap.Add(55, "{~}");    orangeMap.Add(55, "á");
                    keyMap.Add(54, Keys.S);         greenMap.Add(54, "š");      orangeMap.Add(54, "ß");
                    keyMap.Add(53, Keys.D);         greenMap.Add(53, "{{}");    orangeMap.Add(53, "ð");
                    keyMap.Add(52, Keys.F);         greenMap.Add(52, "{}}");    orangeMap.Add(52, "£");
                    keyMap.Add(51, Keys.G);         greenMap.Add(51, "¨");      orangeMap.Add(51, "¥");
                    keyMap.Add(50, Keys.H);         greenMap.Add(50, "/");      orangeMap.Add(50, "\\");
                    keyMap.Add(49, Keys.J);         greenMap.Add(49, "'");      orangeMap.Add(49, "\"");
                    keyMap.Add(119, Keys.K);        greenMap.Add(119, "{[}");   orangeMap.Add(119, "☺");
                    keyMap.Add(114, Keys.L);        greenMap.Add(114, "{]}");   orangeMap.Add(114, "ø");
                    keyMap.Add(98, Keys.Oemcomma);  greenMap.Add(98, ":");      orangeMap.Add(98, ";");

                    keyMap.Add(70, Keys.Z);         greenMap.Add(70, "`");      orangeMap.Add(70, "æ");
                    keyMap.Add(69, Keys.X);         greenMap.Add(69, "«");      orangeMap.Add(69, "œ");
                    keyMap.Add(68, Keys.C);         greenMap.Add(68, "»");      orangeMap.Add(68, "ç");
                    keyMap.Add(67, Keys.V);         greenMap.Add(67, "-");      orangeMap.Add(67, "_");
                    keyMap.Add(66, Keys.B);         greenMap.Add(66, "|");      orangeMap.Add(66, "{+}");
                    keyMap.Add(65, Keys.N);         greenMap.Add(65, "<");      orangeMap.Add(65, "ñ");
                    keyMap.Add(82, Keys.M);         greenMap.Add(82, ">");      orangeMap.Add(82, "µ");
                    keyMap.Add(83, Keys.OemPeriod); greenMap.Add(83, "?");      orangeMap.Add(83, "¿");
                    keyMap.Add(99, Keys.Enter);     greenMap.Add(99, "");       orangeMap.Add(99, "");

                    keyMap.Add(85, Keys.Left);      greenMap.Add(85, "");       orangeMap.Add(85, "");
                    keyMap.Add(84, Keys.Space);     greenMap.Add(84, "");       orangeMap.Add(84, "");
                    keyMap.Add(81, Keys.Right);     greenMap.Add(81, "");       orangeMap.Add(81, "");
                    keyMap.Add(113, Keys.Back);     greenMap.Add(113, "");      orangeMap.Add(113, "");
                    break;

                case "Q W E R T Z":
                    keyMap.Add(23, Keys.D1);        greenMap.Add(23, "");       orangeMap.Add(23, "");
                    keyMap.Add(22, Keys.D2);        greenMap.Add(22, "");       orangeMap.Add(22, "");
                    keyMap.Add(21, Keys.D3);        greenMap.Add(21, "");       orangeMap.Add(21, "");
                    keyMap.Add(20, Keys.D4);        greenMap.Add(20, "");       orangeMap.Add(20, "");
                    keyMap.Add(19, Keys.D5);        greenMap.Add(19, "");       orangeMap.Add(19, "");
                    keyMap.Add(18, Keys.D6);        greenMap.Add(18, "");       orangeMap.Add(18, "");
                    keyMap.Add(17, Keys.D7);        greenMap.Add(17, "");       orangeMap.Add(17, "");
                    keyMap.Add(103, Keys.D8);       greenMap.Add(103, "");      orangeMap.Add(103, "");
                    keyMap.Add(102, Keys.D9);       greenMap.Add(102, "");      orangeMap.Add(102, "");
                    keyMap.Add(101, Keys.D0);       greenMap.Add(101, "");      orangeMap.Add(101, "");

                    keyMap.Add(39, Keys.Q);         greenMap.Add(39, "!");      orangeMap.Add(39, "@");
                    keyMap.Add(38, Keys.W);         greenMap.Add(38, "\"");     orangeMap.Add(38, "¡");
                    keyMap.Add(37, Keys.E);         greenMap.Add(37, "€");      orangeMap.Add(37, "é");
                    keyMap.Add(36, Keys.R);         greenMap.Add(36, "$");      orangeMap.Add(36, "¥");
                    keyMap.Add(35, Keys.T);         greenMap.Add(35, "{%}");    orangeMap.Add(35, "Þ");
                    keyMap.Add(34, Keys.Z);         greenMap.Add(34, "&");      orangeMap.Add(34, "{^}");
                    keyMap.Add(33, Keys.U);         greenMap.Add(33, "/");      orangeMap.Add(33, "ü");
                    keyMap.Add(118, Keys.I);        greenMap.Add(118, "{(}");   orangeMap.Add(118, "í");
                    keyMap.Add(117, Keys.O);        greenMap.Add(117, "{)}");   orangeMap.Add(117, "ö");
                    keyMap.Add(100, Keys.P);        greenMap.Add(100, "=");     orangeMap.Add(100, "\\");

                    keyMap.Add(55, Keys.A);         greenMap.Add(55, "å");      orangeMap.Add(55, "ä");
                    keyMap.Add(54, Keys.S);         greenMap.Add(54, "ß");      orangeMap.Add(54, "š");
                    keyMap.Add(53, Keys.D);         greenMap.Add(53, "«");      orangeMap.Add(53, "ð");
                    keyMap.Add(52, Keys.F);         greenMap.Add(52, "»");      orangeMap.Add(52, "£");
                    keyMap.Add(51, Keys.G);         greenMap.Add(51, "¨");      orangeMap.Add(51, "☺");
                    keyMap.Add(50, Keys.H);         greenMap.Add(50, "{{}");    orangeMap.Add(50, "`");
                    keyMap.Add(49, Keys.J);         greenMap.Add(49, "{}}");    orangeMap.Add(49, "ø");
                    keyMap.Add(119, Keys.K);        greenMap.Add(119, "{[}");   orangeMap.Add(119, "æ");
                    keyMap.Add(114, Keys.L);        greenMap.Add(114, "{]}");   orangeMap.Add(114, "œ");
                    keyMap.Add(98, Keys.Oemcomma);  greenMap.Add(98, "':");     orangeMap.Add(98, "#;");

                    keyMap.Add(70, Keys.Y);         greenMap.Add(70, "<");      orangeMap.Add(70, "°");
                    keyMap.Add(69, Keys.X);         greenMap.Add(69, ">");      orangeMap.Add(69, "|");
                    keyMap.Add(68, Keys.C);         greenMap.Add(68, "{~}");    orangeMap.Add(68, "ç");
                    keyMap.Add(67, Keys.V);         greenMap.Add(67, "-");      orangeMap.Add(67, "_");
                    keyMap.Add(66, Keys.B);         greenMap.Add(66, "*");      orangeMap.Add(66, "{+}");
                    keyMap.Add(65, Keys.N);         greenMap.Add(65, ";");      orangeMap.Add(65, "ñ");
                    keyMap.Add(82, Keys.M);         greenMap.Add(82, ":");      orangeMap.Add(82, "µ");
                    keyMap.Add(83, Keys.OemPeriod); greenMap.Add(83, "?");      orangeMap.Add(83, "¿");
                    keyMap.Add(99, Keys.Enter);     greenMap.Add(99, "");       orangeMap.Add(99, "");

                    keyMap.Add(85, Keys.Left);      greenMap.Add(85, "");       orangeMap.Add(85, "");
                    keyMap.Add(84, Keys.Space);     greenMap.Add(84, "");       orangeMap.Add(84, "");
                    keyMap.Add(81, Keys.Right);     greenMap.Add(81, "");       orangeMap.Add(81, "");
                    keyMap.Add(113, Keys.Back);     greenMap.Add(113, "");      orangeMap.Add(113, "");
                    break;

                case "A Z E R T Y":
                    keyMap.Add(23, Keys.D1);        greenMap.Add(23, "");       orangeMap.Add(23, "");
                    keyMap.Add(22, Keys.D2);        greenMap.Add(22, "");       orangeMap.Add(22, "");
                    keyMap.Add(21, Keys.D3);        greenMap.Add(21, "");       orangeMap.Add(21, "");
                    keyMap.Add(20, Keys.D4);        greenMap.Add(20, "");       orangeMap.Add(20, "");
                    keyMap.Add(19, Keys.D5);        greenMap.Add(19, "");       orangeMap.Add(19, "");
                    keyMap.Add(18, Keys.D6);        greenMap.Add(18, "");       orangeMap.Add(18, "");
                    keyMap.Add(17, Keys.D7);        greenMap.Add(17, "");       orangeMap.Add(17, "");
                    keyMap.Add(103, Keys.D8);       greenMap.Add(103, "");      orangeMap.Add(103, "");
                    keyMap.Add(102, Keys.D9);       greenMap.Add(102, "");      orangeMap.Add(102, "");
                    keyMap.Add(101, Keys.D0);       greenMap.Add(101, "");      orangeMap.Add(101, "");

                    keyMap.Add(39, Keys.A);         greenMap.Add(39, "à");      orangeMap.Add(39, "&");
                    keyMap.Add(38, Keys.Z);         greenMap.Add(38, "æ");      orangeMap.Add(38, "{~}");
                    keyMap.Add(37, Keys.E);         greenMap.Add(37, "€");      orangeMap.Add(37, "\"");
                    keyMap.Add(36, Keys.R);         greenMap.Add(36, "é");      orangeMap.Add(36, "$");
                    keyMap.Add(35, Keys.T);         greenMap.Add(35, "#");      orangeMap.Add(35, "{(}");
                    keyMap.Add(34, Keys.Y);         greenMap.Add(34, "ý");      orangeMap.Add(34, "-");
                    keyMap.Add(33, Keys.U);         greenMap.Add(33, "ù");      orangeMap.Add(33, "`");
                    keyMap.Add(118, Keys.I);        greenMap.Add(118, "ì");     orangeMap.Add(118, "_");
                    keyMap.Add(117, Keys.O);        greenMap.Add(117, "œ");     orangeMap.Add(117, "@");
                    keyMap.Add(100, Keys.P);        greenMap.Add(100, "ó");     orangeMap.Add(100, "{)}");

                    keyMap.Add(55, Keys.Q);         greenMap.Add(55, "å");      orangeMap.Add(55, "☺");
                    keyMap.Add(54, Keys.S);         greenMap.Add(54, "š");      orangeMap.Add(54, "«");
                    keyMap.Add(53, Keys.D);         greenMap.Add(53, "ð");      orangeMap.Add(53, "»");
                    keyMap.Add(52, Keys.F);         greenMap.Add(52, "Þ");      orangeMap.Add(52, "{{}");
                    keyMap.Add(51, Keys.G);         greenMap.Add(51, "¨");      orangeMap.Add(51, "¥");
                    keyMap.Add(50, Keys.H);         greenMap.Add(50, "|");      orangeMap.Add(50, "ø");
                    keyMap.Add(49, Keys.J);         greenMap.Add(49, "µ");      orangeMap.Add(49, "¨");
                    keyMap.Add(119, Keys.K);        greenMap.Add(119, "/");     orangeMap.Add(119, "\\");
                    keyMap.Add(114, Keys.L);        greenMap.Add(114, "$");     orangeMap.Add(114, "£");
                    keyMap.Add(98, Keys.M);         greenMap.Add(98, "*");      orangeMap.Add(98, "{^}");

                    keyMap.Add(70, Keys.W);         greenMap.Add(70, "¡");      orangeMap.Add(70, "<");
                    keyMap.Add(69, Keys.X);         greenMap.Add(69, "¿");      orangeMap.Add(69, ">");
                    keyMap.Add(68, Keys.C);         greenMap.Add(68, "ç");      orangeMap.Add(68, "{[}");
                    keyMap.Add(67, Keys.V);         greenMap.Add(67, "=");      orangeMap.Add(67, "{]}");
                    keyMap.Add(66, Keys.B);         greenMap.Add(66, "{+}");    orangeMap.Add(66, "{%}");
                    keyMap.Add(65, Keys.N);         greenMap.Add(65, "?");      orangeMap.Add(65, "ñ");
                    keyMap.Add(82, Keys.Oemcomma);  greenMap.Add(82, "!");      orangeMap.Add(82, "'");
                    keyMap.Add(83, Keys.OemPeriod); greenMap.Add(83, ":");      orangeMap.Add(83, ";");
                    keyMap.Add(99, Keys.Enter);     greenMap.Add(99, "");       orangeMap.Add(99, "");

                    keyMap.Add(85, Keys.Left);      greenMap.Add(85, "");       orangeMap.Add(85, "");
                    keyMap.Add(84, Keys.Space);     greenMap.Add(84, "");       orangeMap.Add(84, "");
                    keyMap.Add(81, Keys.Right);     greenMap.Add(81, "");       orangeMap.Add(81, "");
                    keyMap.Add(113, Keys.Back);     greenMap.Add(113, "");      orangeMap.Add(113, "");
                    break;

                default:
                    Trace.WriteLine("ERROR: Unknown Keyboard Type.");
                    break;
            }
        }

        public void ProcessData(byte[] dataPacket, Window_Main parentWindow)
        {
            // This function is called anytime received data is identified as chatpad data
            // It will parse the data, and depending on the value, send a keyboard command,
            // adjust a modifier for later use, flag initialization, or note the LED status.
            if (dataPacket[24] == 0xF0)
            {
                if (dataPacket[25] == 0x03)
                    // This data represents handshake request, flag keep-alive to send
                    // chatpad initialization data.
                    parentWindow.chatpadInitNeeded = true;
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
                    Trace.WriteLine("WARNING: Unknown Chatpad Status Data.");
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
                        parentWindow.sendData(parentWindow.deviceCommands["Green_On"]);
                    else
                        parentWindow.sendData(parentWindow.deviceCommands["Green_Off"]);

                    if (chatpadMod["Orange"])
                        parentWindow.sendData(parentWindow.deviceCommands["Orange_On"]);
                    else
                        parentWindow.sendData(parentWindow.deviceCommands["Orange_Off"]);

                    if (chatpadMod["Messenger"])
                        parentWindow.sendData(parentWindow.deviceCommands["Messenger_On"]);
                    else
                        parentWindow.sendData(parentWindow.deviceCommands["Messenger_Off"]);

                    if (chatpadMod["Capslock"])
                        parentWindow.sendData(parentWindow.deviceCommands["Capslock_On"]);
                    else
                        parentWindow.sendData(parentWindow.deviceCommands["Capslock_Off"]);

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
                Trace.WriteLine("WARNING: Unknown Chatpad Data.");
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
    }
}
