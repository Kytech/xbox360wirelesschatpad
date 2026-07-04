using Xbox360WirelessChatpad.Core;
using Xunit;

namespace Xbox360WirelessChatpad.Tests
{
    public class GamepadStateMapperDeadzoneTests
    {
        [Fact]
        public void InsideCircularDeadzone_BothComponentsZeroed()
        {
            // Distance of (3000, 3000) is ~4243, inside a radius-5000 deadzone
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(leftX: 3000, leftY: 3000, rightX: 3000, rightY: 3000),
                5000, 5000);

            Assert.Equal(0, state.LeftStickX);
            Assert.Equal(0, state.LeftStickY);
            Assert.Equal(0, state.RightStickX);
            Assert.Equal(0, state.RightStickY);
        }

        [Fact]
        public void OutsideCircle_SmallAxialComponentZeroed()
        {
            // Distance of (100, 8000) is ~8001, outside the radius-5000 circle,
            // but the X component alone is under the threshold so it is zeroed
            // (exact port of the original vJoy-era filter semantics).
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(leftX: 100, leftY: 8000), 5000, 0);

            Assert.Equal(0, state.LeftStickX);
            Assert.Equal(8000, state.LeftStickY);
        }

        [Fact]
        public void OutsideCircle_LargeComponentsUntouched()
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(leftX: 6000, leftY: 7000), 5000, 0);

            Assert.Equal(6000, state.LeftStickX);
            Assert.Equal(7000, state.LeftStickY);
        }

        [Fact]
        public void ZeroDeadzone_PassesValuesThrough()
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(leftX: 1, leftY: -1, rightX: 2, rightY: -2), 0, 0);

            Assert.Equal(1, state.LeftStickX);
            Assert.Equal(-1, state.LeftStickY);
            Assert.Equal(2, state.RightStickX);
            Assert.Equal(-2, state.RightStickY);
        }

        [Fact]
        public void DeadzonesApplyToTheirOwnStickOnly()
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(leftX: 3000, leftY: 0, rightX: 3000, rightY: 0),
                5000, 0);

            Assert.Equal(0, state.LeftStickX);
            Assert.Equal(3000, state.RightStickX);
        }
    }
}
