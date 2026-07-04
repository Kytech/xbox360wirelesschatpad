using System;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;

namespace Xbox360WirelessChatpad.Core.ViGEm
{
    /// <summary>
    /// Creates ViGEmBus-backed virtual Xbox 360 controllers. Owns the single
    /// <see cref="ViGEmClient"/> connection to the bus driver, shared by all
    /// gamepads it creates.
    /// </summary>
    public class ViGEmGamepadFactory : IVirtualGamepadFactory
    {
        private readonly ViGEmClient client;

        public ViGEmGamepadFactory()
        {
            try
            {
                client = new ViGEmClient();
            }
            catch (VigemBusNotFoundException ex)
            {
                throw new VigemBusNotAvailableException(
                    "The ViGEmBus driver is not installed.", ex);
            }
            catch (VigemBusAccessFailedException ex)
            {
                throw new VigemBusNotAvailableException(
                    "The ViGEmBus driver could not be accessed.", ex);
            }
        }

        public IVirtualGamepad CreateGamepad()
        {
            return new ViGEmGamepad(client.CreateXbox360Controller());
        }

        public void Dispose()
        {
            if (client != null)
                client.Dispose();
        }
    }
}
