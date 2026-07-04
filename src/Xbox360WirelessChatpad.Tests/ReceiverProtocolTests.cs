using Xbox360WirelessChatpad.Core;
using Xunit;

namespace Xbox360WirelessChatpad.Tests
{
    public class ReceiverProtocolTests
    {
        [Fact]
        public void RumbleCommand_HasExpectedLayout()
        {
            byte[] packet = ReceiverProtocol.BuildRumbleCommand(0xAB, 0xCD);

            Assert.Equal(
                new byte[] { 0x00, 0x01, 0x0F, 0xC0, 0x00, 0xAB, 0xCD, 0x00, 0x00, 0x00, 0x00, 0x00 },
                packet);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(255, 255)]
        [InlineData(255, 0)]
        [InlineData(0, 255)]
        public void RumbleCommand_MotorBoundaryValues(byte large, byte small)
        {
            byte[] packet = ReceiverProtocol.BuildRumbleCommand(large, small);

            Assert.Equal(12, packet.Length);
            Assert.Equal(large, packet[5]);
            Assert.Equal(small, packet[6]);
        }
    }
}
