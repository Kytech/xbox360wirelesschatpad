using System;
using Xbox360WirelessChatpad.Core;
using Xunit;

namespace Xbox360WirelessChatpad.Tests
{
    public class GamepadStateMapperButtonTests
    {
        [Theory]
        [InlineData(0x01, "DpadUp")]
        [InlineData(0x02, "DpadDown")]
        [InlineData(0x04, "DpadLeft")]
        [InlineData(0x08, "DpadRight")]
        [InlineData(0x10, "Start")]
        [InlineData(0x20, "Back")]
        [InlineData(0x40, "LeftThumb")]
        [InlineData(0x80, "RightThumb")]
        public void Byte6Bits_MapToExpectedButtons(byte mask, string buttonName)
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(buttons6: mask), 0, 0);

            Assert.True(GetButton(state, buttonName));
            Assert.Equal(1, CountPressedButtons(state));
        }

        [Theory]
        [InlineData(0x01, "LeftShoulder")]
        [InlineData(0x02, "RightShoulder")]
        [InlineData(0x04, "Guide")]
        [InlineData(0x10, "A")]
        [InlineData(0x20, "B")]
        [InlineData(0x40, "X")]
        [InlineData(0x80, "Y")]
        public void Byte7Bits_MapToExpectedButtons(byte mask, string buttonName)
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(buttons7: mask), 0, 0);

            Assert.True(GetButton(state, buttonName));
            Assert.Equal(1, CountPressedButtons(state));
        }

        [Fact]
        public void NoBitsSet_NoButtonsPressed()
        {
            GamepadState state = GamepadStateMapper.Map(PacketBuilder.Gamepad(), 0, 0);

            Assert.Equal(0, CountPressedButtons(state));
        }

        [Fact]
        public void AllBitsSet_AllButtonsPressed()
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(buttons6: 0xFF, buttons7: 0xF7), 0, 0);

            Assert.Equal(15, CountPressedButtons(state));
        }

        [Theory]
        [InlineData(0x05, "DpadUp", "DpadLeft")]
        [InlineData(0x06, "DpadDown", "DpadLeft")]
        [InlineData(0x09, "DpadUp", "DpadRight")]
        [InlineData(0x0A, "DpadDown", "DpadRight")]
        public void DpadDiagonals_SetBothDirections(byte mask, string first, string second)
        {
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(buttons6: mask), 0, 0);

            Assert.True(GetButton(state, first));
            Assert.True(GetButton(state, second));
            Assert.Equal(2, CountPressedButtons(state));
        }

        [Fact]
        public void SimultaneousButtons_AllReported()
        {
            // LB + RB + Back, the mouse-mode toggle combo
            GamepadState state = GamepadStateMapper.Map(
                PacketBuilder.Gamepad(buttons6: 0x20, buttons7: 0x03), 0, 0);

            Assert.True(state.LeftShoulder);
            Assert.True(state.RightShoulder);
            Assert.True(state.Back);
            Assert.Equal(3, CountPressedButtons(state));
        }

        private static bool GetButton(GamepadState state, string name)
        {
            switch (name)
            {
                case "A": return state.A;
                case "B": return state.B;
                case "X": return state.X;
                case "Y": return state.Y;
                case "Start": return state.Start;
                case "Back": return state.Back;
                case "Guide": return state.Guide;
                case "LeftShoulder": return state.LeftShoulder;
                case "RightShoulder": return state.RightShoulder;
                case "LeftThumb": return state.LeftThumb;
                case "RightThumb": return state.RightThumb;
                case "DpadUp": return state.DpadUp;
                case "DpadDown": return state.DpadDown;
                case "DpadLeft": return state.DpadLeft;
                case "DpadRight": return state.DpadRight;
                default: throw new ArgumentException("Unknown button: " + name);
            }
        }

        private static int CountPressedButtons(GamepadState state)
        {
            bool[] buttons =
            {
                state.A, state.B, state.X, state.Y,
                state.Start, state.Back, state.Guide,
                state.LeftShoulder, state.RightShoulder,
                state.LeftThumb, state.RightThumb,
                state.DpadUp, state.DpadDown, state.DpadLeft, state.DpadRight
            };

            int count = 0;
            foreach (bool pressed in buttons)
                if (pressed)
                    count++;
            return count;
        }
    }
}
