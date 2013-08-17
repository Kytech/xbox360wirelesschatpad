using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace Xbox360WirelessChatpad
{
    public class Chatpad
    {
        // Contains the mapping of Chatpad buttons
        Dictionary<int, List<string>> keyMap = new Dictionary<int, List<string>>();

        // Identifies which Chatpad LEDs are active
        Dictionary<string, bool> chatpadLED = new Dictionary<string, bool>()
            {
                { "Green", false },
                { "Orange", false },
                { "Messenger", false },
                { "Capslock", false },
                { "Backlight", false }
            };

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

        private byte[] dataPacketLast = new byte[3];

        public Chatpad(string keyboardType)
        {
            // Map the Xbox chatpad buttons to characters
            // The format of the list is: ( NormalKey, GreenShiftKey, OrangeShiftKey }
            switch (keyboardType)
            {
                case "Q W E R T Y":
                    keyMap.Add(17, new List<string> { "7", "", "" });
                    keyMap.Add(18, new List<string> { "6", "", "" });
                    keyMap.Add(19, new List<string> { "5", "", "" });
                    keyMap.Add(20, new List<string> { "4", "", "" });
                    keyMap.Add(21, new List<string> { "3", "", "" });
                    keyMap.Add(22, new List<string> { "2", "", "" });
                    keyMap.Add(23, new List<string> { "1", "", "" });
                    keyMap.Add(33, new List<string> { "u", "&", "ú" });
                    keyMap.Add(34, new List<string> { "y", "{^}", "ý" });
                    keyMap.Add(35, new List<string> { "t", "{%}", "Þ" });
                    keyMap.Add(36, new List<string> { "r", "#", "$" });
                    keyMap.Add(37, new List<string> { "e", "€", "é" });
                    keyMap.Add(38, new List<string> { "w", "@", "å" });
                    keyMap.Add(39, new List<string> { "q", "!", "¡" });
                    keyMap.Add(49, new List<string> { "j", "'", "\"" });
                    keyMap.Add(50, new List<string> { "h", "/", "\\" });
                    keyMap.Add(51, new List<string> { "g", "¨", "¥" });
                    keyMap.Add(52, new List<string> { "f", "{}}", "£" });
                    keyMap.Add(53, new List<string> { "d", "{{}", "ð" });
                    keyMap.Add(54, new List<string> { "s", "š", "ß" });
                    keyMap.Add(55, new List<string> { "a", "{~}", "á" });
                    keyMap.Add(65, new List<string> { "n", "<", "ñ" });
                    keyMap.Add(66, new List<string> { "b", "|", "{+}" });
                    keyMap.Add(67, new List<string> { "v", "-", "_" });
                    keyMap.Add(68, new List<string> { "c", "»", "ç" });
                    keyMap.Add(69, new List<string> { "x", "«", "œ" });
                    keyMap.Add(70, new List<string> { "z", "`", "æ" });
                    keyMap.Add(81, new List<string> { "{Right}", "", "" });
                    keyMap.Add(82, new List<string> { "m", ">", "µ" });
                    keyMap.Add(83, new List<string> { ".", "?", "¿" });
                    keyMap.Add(84, new List<string> { " ", "", "" });
                    keyMap.Add(85, new List<string> { "{Left}", "", "" });
                    keyMap.Add(98, new List<string> { ",", ":", ";" });
                    keyMap.Add(99, new List<string> { "{Enter}", "", "" });
                    keyMap.Add(100, new List<string> { "p", "{)}", "=" });
                    keyMap.Add(101, new List<string> { "0", "", "" });
                    keyMap.Add(102, new List<string> { "9", "", "" });
                    keyMap.Add(103, new List<string> { "8", "", "" });
                    keyMap.Add(113, new List<string> { "{Backspace}", "", "" });
                    keyMap.Add(114, new List<string> { "l", "{]}", "ø" });
                    keyMap.Add(117, new List<string> { "o", "{(}", "ó" });
                    keyMap.Add(118, new List<string> { "i", "*", "í" });
                    keyMap.Add(119, new List<string> { "k", "{[}", "☺" });
                    break;

                case "Q W E R T Z":
                    keyMap.Add(17, new List<string> { "7", "", "" });
                    keyMap.Add(18, new List<string> { "6", "", "" });
                    keyMap.Add(19, new List<string> { "5", "", "" });
                    keyMap.Add(20, new List<string> { "4", "", "" });
                    keyMap.Add(21, new List<string> { "3", "", "" });
                    keyMap.Add(22, new List<string> { "2", "", "" });
                    keyMap.Add(23, new List<string> { "1", "", "" });
                    keyMap.Add(33, new List<string> { "u", "/", "ü" });
                    keyMap.Add(34, new List<string> { "z", "&", "º" });
                    keyMap.Add(35, new List<string> { "t", "{%}", "Þ" });
                    keyMap.Add(36, new List<string> { "r", "$", "¥" });
                    keyMap.Add(37, new List<string> { "e", "€", "é" });
                    keyMap.Add(38, new List<string> { "w", "\"", "¡" });
                    keyMap.Add(39, new List<string> { "q", "!", "@" });
                    keyMap.Add(49, new List<string> { "j", "{}}", "ø" });
                    keyMap.Add(50, new List<string> { "h", "{{}", "`" });
                    keyMap.Add(51, new List<string> { "g", "¨", "☺" });
                    keyMap.Add(52, new List<string> { "f", "»", "£" });
                    keyMap.Add(53, new List<string> { "d", "«", "ð" });
                    keyMap.Add(54, new List<string> { "s", "ß", "š" });
                    keyMap.Add(55, new List<string> { "a", "å", "ä" });
                    keyMap.Add(65, new List<string> { "n", ";", "ñ" });
                    keyMap.Add(66, new List<string> { "b", "*", "{+}" });
                    keyMap.Add(67, new List<string> { "v", "-", "_" });
                    keyMap.Add(68, new List<string> { "c", "{~}", "ç" });
                    keyMap.Add(69, new List<string> { "x", ">", "|" });
                    keyMap.Add(70, new List<string> { "y", "<", "º" });
                    keyMap.Add(81, new List<string> { "{Right}", "", "" });
                    keyMap.Add(82, new List<string> { "m", ":", "µ" });
                    keyMap.Add(83, new List<string> { ".", "?", "¿" });
                    keyMap.Add(84, new List<string> { " ", "", "" });
                    keyMap.Add(85, new List<string> { "{Left}", "", "" });
                    keyMap.Add(98, new List<string> { ",", "'", "#" });
                    keyMap.Add(99, new List<string> { "{Enter}", "", "" });
                    keyMap.Add(100, new List<string> { "p", "=", "\\" });
                    keyMap.Add(101, new List<string> { "0", "", "" });
                    keyMap.Add(102, new List<string> { "9", "", "" });
                    keyMap.Add(103, new List<string> { "8", "", "" });
                    keyMap.Add(113, new List<string> { "{Backspace}", "", "" });
                    keyMap.Add(114, new List<string> { "l", "{]}", "œ" });
                    keyMap.Add(117, new List<string> { "o", "{)}", "ö" });
                    keyMap.Add(118, new List<string> { "i", "{(}", "í" });
                    keyMap.Add(119, new List<string> { "k", "{[}", "æ" });
                    break;

                case "A Z E R T Y":
                    keyMap.Add(17, new List<string> { "7", "", "" });
                    keyMap.Add(18, new List<string> { "6", "", "" });
                    keyMap.Add(19, new List<string> { "5", "", "" });
                    keyMap.Add(20, new List<string> { "4", "", "" });
                    keyMap.Add(21, new List<string> { "3", "", "" });
                    keyMap.Add(22, new List<string> { "2", "", "" });
                    keyMap.Add(23, new List<string> { "1", "", "" });
                    keyMap.Add(33, new List<string> { "u", "ù", "`" });
                    keyMap.Add(34, new List<string> { "y", "ý", "-" });
                    keyMap.Add(35, new List<string> { "t", "#", "{(}" });
                    keyMap.Add(36, new List<string> { "r", "é", "$" });
                    keyMap.Add(37, new List<string> { "e", "€", "\"" });
                    keyMap.Add(38, new List<string> { "z", "æ", "{~}" });
                    keyMap.Add(39, new List<string> { "a", "à", "&" });
                    keyMap.Add(49, new List<string> { "j", "µ", "¨" });
                    keyMap.Add(50, new List<string> { "h", "|", "ø" });
                    keyMap.Add(51, new List<string> { "g", "¨", "¥" });
                    keyMap.Add(52, new List<string> { "f", "Þ", "{{}" });
                    keyMap.Add(53, new List<string> { "d", "ð", "»" });
                    keyMap.Add(54, new List<string> { "s", "š", "«" });
                    keyMap.Add(55, new List<string> { "q", "å", "☺" });
                    keyMap.Add(65, new List<string> { "n", "?", "ñ" });
                    keyMap.Add(66, new List<string> { "b", "{+}", "{%}" });
                    keyMap.Add(67, new List<string> { "v", "=", "{]}" });
                    keyMap.Add(68, new List<string> { "c", "ç", "{[}" });
                    keyMap.Add(69, new List<string> { "x", "¿", ">" });
                    keyMap.Add(70, new List<string> { "w", "¡", "<" });
                    keyMap.Add(81, new List<string> { "{Right}", "", "" });
                    keyMap.Add(82, new List<string> { ",", "!", "'" });
                    keyMap.Add(83, new List<string> { ".", ":", ";" });
                    keyMap.Add(84, new List<string> { " ", "", "" });
                    keyMap.Add(85, new List<string> { "{Left}", "", "" });
                    keyMap.Add(98, new List<string> { "m", "*", "{^}" });
                    keyMap.Add(99, new List<string> { "{Enter}", "", "" });
                    keyMap.Add(100, new List<string> { "p", "ó", "{)}" });
                    keyMap.Add(101, new List<string> { "0", "", "" });
                    keyMap.Add(102, new List<string> { "9", "", "" });
                    keyMap.Add(103, new List<string> { "8", "", "" });
                    keyMap.Add(113, new List<string> { "{Backspace}", "", "" });
                    keyMap.Add(114, new List<string> { "l", "$", "£" });
                    keyMap.Add(117, new List<string> { "o", "œ", "@" });
                    keyMap.Add(118, new List<string> { "i", "ì", "_" });
                    keyMap.Add(119, new List<string> { "k", "/", "\\" });
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
                    // This data represents the LED status, store them for later use
                    chatpadLED["Green"] = (dataPacket[26] & 0x08) > 0;
                    chatpadLED["Orange"] = (dataPacket[26] & 0x10) > 0;
                    chatpadLED["Messenger"] = (dataPacket[26] & 0x01) > 0;
                    chatpadLED["Capslock"] = (dataPacket[26] & 0x20) > 0;
                    chatpadLED["Backlight"] = (dataPacket[26] & 0x80) > 0;
                }
                else
                    Trace.WriteLine("WARNING: Unknown Chatpad Status Data.");
            }
            else if (dataPacket[24] == 0x00)
            {
                // This data represents a key-press event
                // Check if any keys or modifiers have changed since the last dataPacket
                bool dataChanged = false;
                if (dataPacketLast != null)
                {
                    if (dataPacketLast[0] != dataPacket[25])
                        dataChanged = true;
                    if (dataPacketLast[1] != dataPacket[26])
                        dataChanged = true;
                    if (dataPacketLast[2] != dataPacket[27])
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

                    // Special Handling if Shift and Orange Modifer, Toggle Capslock Modifier
                    if (chatpadMod["Orange"] && chatpadMod["Shift"])
                        chatpadMod["Capslock"] = !chatpadMod["Capslock"];

                    // Set LED for Capslock Button based on Modifier
                    if (chatpadMod["Capslock"])
                        parentWindow.sendData(parentWindow.deviceCommands["Capslock_On"]);
                    else
                        parentWindow.sendData(parentWindow.deviceCommands["Capslock_Off"]);

                    // Record the two different possible keys that could be held down
                    byte key1 = dataPacket[26];
                    byte key2 = dataPacket[27];

                    if (key1 != 0 && !chatpadKeysHeld.Contains(key1))
                    {
                        // If key 1 is non-zero and was not previously being held down,
                        // record that is now being held
                        chatpadKeysHeld.Add(key1);

                        // Process the keystroke for the character associated with the key
                        ProcessKeystroke(GetChatPadKeyValue(key1, chatpadMod["Orange"], chatpadMod["Green"]));
                    }

                    if (key2 != 0 && !chatpadKeysHeld.Contains(key2))
                    {
                        // If key 2 is non-zero and was not previously being held down,
                        // record that is now being held
                        chatpadKeysHeld.Add(key2);

                        // Process the keystroke for the character associated with the key
                        ProcessKeystroke(GetChatPadKeyValue(key2, chatpadMod["Orange"], chatpadMod["Green"]));
                    }

                    // Remove the keys from the list of held keys that are not being held
                    List<byte> keysToRemove = new List<byte>();
                    foreach (var key in chatpadKeysHeld)
                        if (key != key1 && key != key2)
                            keysToRemove.Add(key);
                    foreach (var key in keysToRemove)
                        chatpadKeysHeld.Remove(key);
                }
            }
            else
                Trace.WriteLine("WARNING: Unknown Chatpad Data.");
        }

        private string GetChatPadKeyValue(int value, bool orangeModifer, bool greenModifer)
        {
            // Returns the String associated with the value supplied
            if (orangeModifer)
                return keyMap[value][2];
            else if (greenModifer)
                return keyMap[value][1];
            else
                return keyMap[value][0];
        }

        private void ProcessKeystroke(string key)
        {
            // If "CapsLock" is active and "Shift" is active, send the key as lowercase
            // If "CapsLock" is active and "Shift" is inactive, send the key as uppercase
            // If "CapsLock" is inactive and "Shift" is active, send the key as uppercase
            // If "CapsLock" is inactive and "Shift" is inactive, send the key as lowercase
            if (chatpadMod["Capslock"] && chatpadMod["Shift"])
                SendKeys.SendWait(key);
            else if (chatpadMod["Capslock"] && !chatpadMod["Shift"])
                SendKeys.SendWait(key.ToUpper());
            else if (!chatpadMod["Capslock"] && chatpadMod["Shift"])
                SendKeys.SendWait(key.ToUpper());
            else
                SendKeys.SendWait(key);
        }
    }
}
