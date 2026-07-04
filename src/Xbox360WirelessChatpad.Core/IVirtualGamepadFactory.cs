using System;

namespace Xbox360WirelessChatpad.Core
{
    /// <summary>
    /// Creates virtual gamepads. The implementation owns the underlying driver
    /// connection (one ViGEmBus client shared by all gamepads).
    /// </summary>
    public interface IVirtualGamepadFactory : IDisposable
    {
        IVirtualGamepad CreateGamepad();
    }
}
