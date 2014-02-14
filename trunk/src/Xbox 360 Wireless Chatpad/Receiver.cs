using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace Xbox360WirelessChatpad
{
    class Receiver
    {
        // Tracks if the Wireless Receiver is attached
        public bool receiverAttached = false;

        // Parent Window object necessary to communicate with form controls
        private Window_Main parentWindow;

        // Xbox Controllers object necessary to communicate with each controller
        private Controller xboxController;

        // USB Wireless Receiver to connect
        private IUsbDevice wirelessReceiver;

        // USB Endpoints to send/receive data from the Wireless Receiver
        private UsbEndpointWriter epWriter;
        private UsbEndpointReader epReader;        

        public Receiver(Window_Main window)
        {
            // Stores the passed window as parentWindow for furtue use
            parentWindow = window;
        }
        
        public void registerClasses(Controller controller)
        {
            // Stores the passed controller as xboxController for future use
            xboxController = controller;
        }

        public void connectReceiver()
        {
            // Connect to the Xbox Wireless Receiver and register the endpoint
            // readers/writers as necessary.
            try
            {
                // Open the Xbox Wireless Receiver as a USB device
                // VendorID 0x045e, ProductID 0x0719
                wirelessReceiver = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(0x045E, 0x0719)) as IUsbDevice;

                // If primary IDs not found attempt secondary IDs
                // VendorID 0x045e, Product ID 0x0291
                if (wirelessReceiver == null)
                    wirelessReceiver = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(0x045E, 0x0291)) as IUsbDevice;

                // If secondary IDs not found report the error
                if (wirelessReceiver == null)
                    parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                        "ERROR: Wireless Receiver Not Found.");
                else
                {
                    // Set the Configuration, Claim the Interface
                    wirelessReceiver.ClaimInterface(1);
                    wirelessReceiver.SetConfiguration(1);

                    // Log if the Wireless Receiver was connected to successfully
                    if (wirelessReceiver.IsOpen)
                    {
                        receiverAttached = true;
                        parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                            "Xbox 360 Wireless Receiver Connected.\r\n");

                        // Connect Endpoint Readers/Writers and register the receiving event handler
                        epReader = wirelessReceiver.OpenEndpointReader(ReadEndpointID.Ep01);
                        epWriter = wirelessReceiver.OpenEndpointWriter(WriteEndpointID.Ep01);
                        epReader.DataReceived += new EventHandler<EndpointDataEventArgs>(xboxController.processDataPacket);
                        epReader.DataReceivedEnabled = true;

                        parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                            "Searching for Controllers...Press the Guide Button Now.");
                    }
                }
            }
            catch
            {
                parentWindow.Invoke(new logCallback(parentWindow.logUpdate),
                    "ERROR: Problem Connecting to Wireless Receiver.");
            }
        }

        public void killReceiver()
        {
            // Turns off the Xbox controllers
            if (receiverAttached)
                xboxController.killController();

            // Clean up the controller's keep-alive threads
            xboxController.killKeepAlive();

            // Clean up the controller's mouse mode threads
            xboxController.killMouseMode();

            // Clean up the controller's button combo threads
            xboxController.killButtonCombo();

            // Cleans up the Wireless Receiver Data
            if (epWriter != null)
            {
                epWriter.Abort();
                epWriter.Dispose();
            }

            if (epReader != null)
            {
                epReader.Abort();
                epReader.Dispose();
            }

            if (wirelessReceiver != null)
            {
                if (receiverAttached)
                    wirelessReceiver.Close();
                wirelessReceiver = null;
            }
        }
    }
}
