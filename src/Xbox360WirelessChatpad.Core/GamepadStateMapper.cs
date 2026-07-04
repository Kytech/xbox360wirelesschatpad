using System;

namespace Xbox360WirelessChatpad.Core
{
    /// <summary>
    /// Decodes gamepad data packets from the Xbox 360 Wireless Receiver into
    /// a <see cref="GamepadState"/>. Pure function of the packet bytes and
    /// deadzone settings, so it is unit-testable without hardware.
    /// </summary>
    public static class GamepadStateMapper
    {
        /// <summary>
        /// Maps a receiver gamepad data packet to a gamepad state.
        /// </summary>
        /// <param name="dataPacket">The raw USB packet. Button bits live in bytes
        /// 6 and 7, triggers in bytes 8 and 9, stick axes in bytes 10-17 as
        /// little-endian signed shorts.</param>
        /// <param name="deadzoneL">Circular deadzone radius for the left stick.</param>
        /// <param name="deadzoneR">Circular deadzone radius for the right stick.</param>
        public static GamepadState Map(byte[] dataPacket, int deadzoneL, int deadzoneR)
        {
            GamepadState state = new GamepadState();

            // Directional pad, bits of byte 6
            state.DpadUp = (dataPacket[6] & 0x01) > 0;
            state.DpadDown = (dataPacket[6] & 0x02) > 0;
            state.DpadLeft = (dataPacket[6] & 0x04) > 0;
            state.DpadRight = (dataPacket[6] & 0x08) > 0;

            // Remaining buttons of byte 6
            state.Start = (dataPacket[6] & 0x10) > 0;
            state.Back = (dataPacket[6] & 0x20) > 0;
            state.LeftThumb = (dataPacket[6] & 0x40) > 0;
            state.RightThumb = (dataPacket[6] & 0x80) > 0;

            // Buttons of byte 7
            state.LeftShoulder = (dataPacket[7] & 0x01) > 0;
            state.RightShoulder = (dataPacket[7] & 0x02) > 0;
            state.Guide = (dataPacket[7] & 0x04) > 0;
            state.A = (dataPacket[7] & 0x10) > 0;
            state.B = (dataPacket[7] & 0x20) > 0;
            state.X = (dataPacket[7] & 0x40) > 0;
            state.Y = (dataPacket[7] & 0x80) > 0;

            // Triggers pass through as raw 0-255 values
            state.LeftTrigger = dataPacket[8];
            state.RightTrigger = dataPacket[9];

            // Stick axes are little-endian signed shorts, already in XInput
            // convention (positive Y is up), so they pass through unmodified.
            // The old vJoy backend negated left Y to correct a HID quirk; the
            // XInput target must NOT do that. Verify direction on first
            // hardware test.
            short leftX = (short)(dataPacket[10] | (dataPacket[11] << 8));
            short leftY = (short)(dataPacket[12] | (dataPacket[13] << 8));
            short rightX = (short)(dataPacket[14] | (dataPacket[15] << 8));
            short rightY = (short)(dataPacket[16] | (dataPacket[17] << 8));

            // Filter the left stick X and Y values based on the left circular deadzone
            double leftDistance = Math.Sqrt((double)(leftX * leftX) + (double)(leftY * leftY));
            if (leftDistance < deadzoneL)
            {
                leftX = 0;
                leftY = 0;
            }
            else
            {
                if (Math.Abs((int)leftX) < deadzoneL)
                    leftX = 0;
                if (Math.Abs((int)leftY) < deadzoneL)
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
                if (Math.Abs((int)rightX) < deadzoneR)
                    rightX = 0;
                if (Math.Abs((int)rightY) < deadzoneR)
                    rightY = 0;
            }

            state.LeftStickX = leftX;
            state.LeftStickY = leftY;
            state.RightStickX = rightX;
            state.RightStickY = rightY;

            return state;
        }
    }
}
