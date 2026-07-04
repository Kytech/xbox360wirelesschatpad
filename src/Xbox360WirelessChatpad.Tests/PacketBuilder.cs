namespace Xbox360WirelessChatpad.Tests
{
    /// <summary>
    /// Builds synthetic wireless receiver gamepad data packets for tests.
    /// Real packets are 29 bytes; the mapper reads buttons from bytes 6-7,
    /// triggers from bytes 8-9, and stick axes from bytes 10-17.
    /// </summary>
    internal static class PacketBuilder
    {
        public static byte[] Gamepad(
            byte buttons6 = 0,
            byte buttons7 = 0,
            byte leftTrigger = 0,
            byte rightTrigger = 0,
            short leftX = 0,
            short leftY = 0,
            short rightX = 0,
            short rightY = 0)
        {
            byte[] packet = new byte[29];
            packet[1] = 0x01; // gamepad data marker
            packet[3] = 0xF0;
            packet[6] = buttons6;
            packet[7] = buttons7;
            packet[8] = leftTrigger;
            packet[9] = rightTrigger;
            WriteShort(packet, 10, leftX);
            WriteShort(packet, 12, leftY);
            WriteShort(packet, 14, rightX);
            WriteShort(packet, 16, rightY);
            return packet;
        }

        private static void WriteShort(byte[] packet, int offset, short value)
        {
            packet[offset] = (byte)(value & 0xFF);
            packet[offset + 1] = (byte)((value >> 8) & 0xFF);
        }
    }
}
