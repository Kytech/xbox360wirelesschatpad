using System;

namespace Xbox360WirelessChatpad.Core
{
    /// <summary>
    /// A virtual game controller presented to the operating system. Abstracts
    /// the ViGEmBus-backed implementation so controller logic can be tested
    /// with a fake.
    /// </summary>
    public interface IVirtualGamepad : IDisposable
    {
        /// <summary>Raised when a game requests force-feedback (rumble) or the
        /// bus assigns a player LED. May fire on a worker thread.</summary>
        event EventHandler<GamepadFeedback> FeedbackReceived;

        /// <summary>Plugs the virtual controller into the system.</summary>
        void Connect();

        /// <summary>Unplugs the virtual controller from the system.</summary>
        void Disconnect();

        /// <summary>Reports a complete gamepad state as one atomic update.</summary>
        void SubmitState(GamepadState state);
    }
}
