# xbox360wirelesschatpad
Project moved from code.google.com/p/xbox360wirelesschatpad

This application will allow you to use your Xbox 360 Wireless Controllers + Attached Chatpads on a Windows PC cordless through an Xbox 360 Wireless Receiver. Its been tested on Windows XP and Windows 7 64-Bit with decent success.

Special Thanks to [Ryan](http://thepocketofresistance.wordpress.com) and [Brandon](http://brandonw.net) who's source code this project is based on.

Disclaimer: As this is free, open-source software I hold no liability for any damages it may cause to you or your equipment. There isn't too much being installed, and I believe everything can be un-done but, use at your own risk. See the Readme.txt inside the archive for more details.

**Previous Updates**
  * Improved error handling.
  * Fixed issue with controller connection and multiple navigation shortcuts executing.
  * Updated with closer Xbox button mappings. Hopefully this help until custom button mapings can be implemented (soon-ish)
  * Added Mouse Mode to be remembered as a setting over multiple launches.
  * Added LED indicators for Mouse Mode. Green LED when Mouse Mode turns On, Red LED when Mouse Mode turns Off.
  * Reversed direction of Log messages in the textbox.
  * Fixed an issue where Ctrl1 Trigger As Button was not being remembered properly.
  * New Feature: Special Command - Disable Controller by holding down Left Trigger + Right Trigger + Back for more than 5 seconds.
  * New Feature: Special Command - Toggle Mouse Mode by holding down Left Bumper + Right Bumper + Back for more than 3 seconds. See below for further details.
  * Added Multiple Controller Support. This is preliminary and is VERY difficult for me to test properly with the lack of controllers. I make no guarantees that it will work flawlessly but please open an issue if you're having problems. For multiple controllers to work you must have multiple vJoy devices configured using the vJoyConf utility like described below. There is a counter on the bottom right of the utility you can increment to specify additional devices.
  * Changed FFXIV Compatibility mode to Trigger As Button mode. The biggest difference the compatibility mode did was make the left and right triggers react as normal buttons instead of a full ranged axis. This was necessary for FFXIV to properly address them. Note that the non-FFXIV button mapping no longer exist as an option without compiling the source individually. I'm looking into making the customization more flexible (there has been much outcry over it) but it will take some time.
  * Massive reorganization and cleanup of the code and repository as a whole.
  * Attempted having the application create the vJoy configuration, skipping a full install step. However this will take a bit more time.

**Known Functional Limitations**

-The native XInput driver is replaced with a non XInput driver. This may cause issues when using the controller in newer games that are expecting XInput functionality. **It is planned to add a feature that allows for the switching between the chatpad driver and native XInput driver to work around this issue.** Another workaround is to use the [x360ce](http://www.x360ce.com/) program which allows a non-XInput device to emulate an XInput device.

-Headset not supported. The scope of this application is not to create a driver but drive existing drivers (vJoy). To implement this I would need to find a virtual audio driver or create my own, and it may not work great due to latency issues anyway. Also, because this is windows, there are many ways to attach a headset/microphone other than through the Xbox 360 Controller. In short, I don't see a huge need for this functionality, but feel free to post an issue if you'd really like to see it added. With enough support, I may try to explore it.

-Force Feedback not supported. This can be done now since vJoy supports force feedback using the latest version, but I don't know how to send the commands to the controller since the developer who knew how to send these signals has not had any activity on this project since the Google Code Shutdown.

-Not all 3rd party receivers work very well with this project. Microsoft receivers are recommended to be used with this project. Some 3rd party receivers work fine and some don't work at all. Most connection issues are due to the use of a 3rd party receiver or an installation error. If you are using a 3rd party receiver and cannot get the chatpad or controller to connect and you are sure everything is installed correctly, unfortunately there is nothing I can do to fix this. If you are having issues with a 3rd party receiver that is not connection-based, or a connection issue with a Microsoft-branded receiver, post an issue and please specify if you are using a 3rd party receiver.

## Installation Directions ##
There's a unique method of getting this driver to work which requires two other applications to be installed. Follow these installation instructions exactly before submitting any issues.

_Installing the LibUSB Driver_
  1. Plug-In and Install the Native Drivers for your XBOX 360 Wireless Receiver.
  1. Download LibUSB [here](http://sourceforge.net/projects/libusb-win32/files). (v1.2.6)
  1. Extract the Archive to a Directory.
  1. Execute the following as an Admin: _Directory_/bin/_Architecture_/install-filter-win.exe
  1. Select "Install a Device Filter".
  1. Select the item with Description "Xbox 360 Wireless Receiver for Windows".
  1. Select "Install" then after the confirmation box select Cancel.
  1. Execute the following path: _Directory_/bin/inf-wizard.exe
  1. Select "Next".
  1. Select the item with Description "Xbox 360 Wireless Receiver for Windows".
  1. Select "Next" then save the new .inf somewhere.
  1. Select "Install Now" to install the driver.
  1. Select OK at the confirmation, the LibUSB driver should now be installed.

_Installing the vJoy Driver_
  1. Download vJoy [here](http://sourceforge.net/projects/vjoystick/files/Beta%202). (v2.0.2)
  1. Install as an Admin with at least the vJoy Configuration Application.
  1. Run "Configure vJoy" from the newly created Start Menu folder.
  1. Match the following selection then hit OK:
    * Basic Axes Selected: X, Y, Z, R/Rz/Rudder
    * Additional Axes Selected: Rx, Ry
    * POV Hat Switch: Continuous
    * POVs: 1
    * Number of Buttons: 11  
  1. The Configuration utility will disappear, the vJoy Driver should now be installed.
  1. Note: For multiple controller support you can specify additional configurations with this utility. Use the counter in the bottom right of the program to change between 1, 2, 3, and 4 controllers.

_Running the Xbox 360 Wireless Chatpad Application_
  1. Download the latest version of the application [here](https://github.com/KytechN24/xbox360wirelesschatpad/releases).
  1. Extract all of the files in the archive to a single directory.
  1. Execute "Xbox 360 Wireless Chatpad.exe"
  1. Follow in instruction in the application to connect your controller.

Note: The first time you connect a controller, you should Calibrate it using the Windows Game Controllers utility in the Control Panel (Windows XP). The following describes different Axes during calibration (Xbox Stick: Windows Name)
  * Left Stick: Primary Axis
  * Left Trigger: Z Axis
  * Right Trigger: Z Rotation
  * Right Stick X: X Rotation
  * Right Stick Y: Y Rotation
  
## Final Fantasy XIV Users ##
In order to use the Final Fantasy XIV compatibility mode, the vJoystick should be configured with the following settings instead of above:
  * Basic Axes Selected: X, Y, Z, R/Rz/Rudder
  * POV Hat Switch: Continuous
  * POVs: 1
  * Number of Buttons: 13  
It is fine if you've already configured above, just re-run the utility and use these settings. Afterwards, launch the program like normal but before selection the Connect Controller button, check the Final Fantasy XIV box. After connecting, calibrate like normal in Windows, then you "should" be good to go in Final Fantasy XIV. In-game, you'll definitely want to check the controller calibration options to validate the settings are working properly.

## Special Commands ##
Mouse Mode will allow you to move the mouse cursor, perform both left and right clicks, and scroll a window vertically. To enable Mouse Mode simply hold down the following button combination for 3 seconds:
  * Left Bumper + Right Bumper + Back  
Once in Mouse Mode use the left stick to move the mouse cursor. Holding down the Left Trigger will scale how fast the cursor moves. Use the A Button for Left Click and B Button for Right Click. Use the Right Joystick Y Axis to scroll a window vertically, similar to a mouse wheel.

To shut down a controller while leaving the application running hold down the following button combination for 5 seconds:
  * Left Trigger + Right Trigger + Back  
To reconnect the controller, simply press the Guide button like normal.
