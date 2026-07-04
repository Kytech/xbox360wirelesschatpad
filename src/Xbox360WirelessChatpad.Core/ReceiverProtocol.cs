namespace Xbox360WirelessChatpad.Core
{
    /// <summary>
    /// Builders for outbound USB command packets understood by the Xbox 360
    /// Wireless Receiver. Pure functions, unit-testable without hardware.
    /// </summary>
    public static class ReceiverProtocol
    {
        /// <summary>
        /// Builds a rumble (force-feedback) command for a wireless controller.
        /// Packet layout comes from the Linux kernel xpad driver's Xbox 360
        /// Wireless (XTYPE_XBOX360W) rumble output; verify on first hardware
        /// test with a rumble-capable game.
        /// </summary>
        /// <param name="largeMotor">Low-frequency motor strength, 0-255.</param>
        /// <param name="smallMotor">High-frequency motor strength, 0-255.</param>
        public static byte[] BuildRumbleCommand(byte largeMotor, byte smallMotor)
        {
            return new byte[12]
            {
                0x00, 0x01, 0x0F, 0xC0, 0x00,
                largeMotor, smallMotor,
                0x00, 0x00, 0x00, 0x00, 0x00
            };
        }
    }
}
