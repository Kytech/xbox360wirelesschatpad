namespace Xbox360WirelessChatpad.Core
{
    /// <summary>
    /// A complete snapshot of Xbox 360 gamepad state, decoded from a wireless
    /// receiver data packet and ready to be fed to a virtual XInput gamepad.
    /// </summary>
    public struct GamepadState
    {
        // Face and system buttons
        public bool A;
        public bool B;
        public bool X;
        public bool Y;
        public bool Start;
        public bool Back;
        public bool Guide;

        // Shoulder buttons and stick clicks
        public bool LeftShoulder;
        public bool RightShoulder;
        public bool LeftThumb;
        public bool RightThumb;

        // Directional pad
        public bool DpadUp;
        public bool DpadDown;
        public bool DpadLeft;
        public bool DpadRight;

        // Analog sticks, XInput convention (positive Y is up)
        public short LeftStickX;
        public short LeftStickY;
        public short RightStickX;
        public short RightStickY;

        // Analog triggers, 0-255
        public byte LeftTrigger;
        public byte RightTrigger;

        /// <summary>A state with no buttons pressed and all axes centered.</summary>
        public static GamepadState Neutral
        {
            get { return default(GamepadState); }
        }
    }
}
