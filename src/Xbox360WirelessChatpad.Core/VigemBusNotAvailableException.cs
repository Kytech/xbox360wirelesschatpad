using System;

namespace Xbox360WirelessChatpad.Core
{
    /// <summary>
    /// Thrown when the ViGEmBus driver is not installed or cannot be reached.
    /// Wraps the ViGEm client exceptions so callers outside this library never
    /// need to reference ViGEm types.
    /// </summary>
    public class VigemBusNotAvailableException : Exception
    {
        public VigemBusNotAvailableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
