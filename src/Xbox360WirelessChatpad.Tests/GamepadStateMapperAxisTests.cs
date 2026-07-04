using Xbox360WirelessChatpad.Core;
using Xunit;

namespace Xbox360WirelessChatpad.Tests
{
    public class GamepadStateMapperAxisTests
    {
        [Theory]
        [InlineData(12345)]
        [InlineData(-12345)]
        [InlineData(1)]
        [InlineData(-1)]
        public void StickValues_AssembledLittleEndian(short value)
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(leftX: value, leftY: value, rightX: value, rightY: value),
                0, 0);

            Assert.Equal(value, state.LeftStickX);
            Assert.Equal(value, state.LeftStickY);
            Assert.Equal(value, state.RightStickX);
            Assert.Equal(value, state.RightStickY);
        }

        [Fact]
        public void StickExtremes_PassThroughUnmodified()
        {
            // Pins the design decision that receiver packets are already in
            // XInput convention: no negation of any axis (the old vJoy backend
            // negated left Y). Also proves short.MinValue cannot overflow.
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(
                    leftX: short.MinValue, leftY: short.MinValue,
                    rightX: short.MaxValue, rightY: short.MaxValue),
                0, 0);

            Assert.Equal(short.MinValue, state.LeftStickX);
            Assert.Equal(short.MinValue, state.LeftStickY);
            Assert.Equal(short.MaxValue, state.RightStickX);
            Assert.Equal(short.MaxValue, state.RightStickY);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(50)]
        [InlineData(255)]
        public void Triggers_PassThroughRawByteValues(byte value)
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(leftTrigger: value, rightTrigger: value), 0, 0);

            Assert.Equal(value, state.LeftTrigger);
            Assert.Equal(value, state.RightTrigger);
        }

        [Fact]
        public void LeftAndRightTriggers_AreIndependent()
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(leftTrigger: 200, rightTrigger: 15), 0, 0);

            Assert.Equal(200, state.LeftTrigger);
            Assert.Equal(15, state.RightTrigger);
        }
    }
}
