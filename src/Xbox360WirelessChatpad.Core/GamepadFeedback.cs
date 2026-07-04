using System;

namespace Xbox360WirelessChatpad.Core
{
    /// <summary>
    /// Force-feedback data reported by the virtual gamepad (i.e. requested by a
    /// game through XInput), to be forwarded to the physical controller.
    /// </summary>
    public class GamepadFeedback : EventArgs
    {
        /// <summary>Low-frequency rumble motor strength, 0-255.</summary>
        public byte LargeMotor { get; private set; }

        /// <summary>High-frequency rumble motor strength, 0-255.</summary>
        public byte SmallMotor { get; private set; }

        /// <summary>Player LED number assigned by the virtual bus.</summary>
        public byte LedNumber { get; private set; }

        public GamepadFeedback(byte largeMotor, byte smallMotor, byte ledNumber)
        {
            LargeMotor = largeMotor;
            SmallMotor = smallMotor;
            LedNumber = ledNumber;
        }
    }
}
