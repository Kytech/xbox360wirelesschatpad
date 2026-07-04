# xbox360wirelesschatpad
Project moved from code.google.com/p/xbox360wirelesschatpad

This application will allow you to use your Xbox 360 Wireless Controllers + Attached Chatpads on a Windows PC cordless through an Xbox 360 Wireless Receiver. Its been tested on Windows XP, Windows 7 64-Bit, and Windows 8 64-bit with decent success.

Special Thanks to [Ryan](http://thepocketofresistance.wordpress.com) and [Brandon](http://brandonw.net) who's source code this project is based on. Additional thanks to [SkyAG/ksbarnes](https://github.com/ksbarnes), who first wrote and maintained this codebase prior to the Google Code shutdown.

Disclaimer: As this is free, open-source software I hold no liability for any damages it may cause to you or your equipment. There isn't too much being installed, and I believe everything can be un-done but, use at your own risk. See the Readme.txt inside the archive for more details.

## Project Status Update

**Oct 26 2021** - In a fortunate turn of events, I managed to reconnect with the original author of this project, SkyAG/ksbarnes when he reached out on this repository. He is now added as a collaborator to this repository and will be able to contribute with full access. Please feel free to tag him in any pull requests as well so that we can get to it sooner and/or so that we can benefit from his input as the original author. I have also been in recent contact with Ryan and Brandon, whose code/findings serve as the basis of this program. This has helped give some further information as to where some additional improvements can be made until I can find the time to write this as more of an actual driver component, compared to relying on intermediary libraries like libusb and vjoy. Additionally, this line of contact will likely be helpful when I can get around to this. Ryan was able to share with me some of his findings and code while Brandon may be able to help point me in the right direction when I encounter roadblocks. ksbarnes probably knows this application better than anyone else, so keep an eye out for his comments on issues/PRs. After touching base with ksbarnes/SkyAG, it sounds like I will continue to oversee maintenance of this repository going forward. - Kytech

Presently, there are many random bugs that are very hard to track down due to the current implementation of the app as of now. I am beginning a large rewrite of the application which will change its underlying architecture and simplify the number of pieces involved in making this thing work. This should eliminate many of the functional limitations as well as reduce the likelihood of difficult or obscure bugs. These rewrites are targeted for version 1.0 of the program. The version 1.0 roadmap can be viewed [here](https://github.com/Kytech/xbox360wirelesschatpad/projects/1#card-57891164)

A few important points concerning my roadmap for this project once I can get to it:
* As version 1.0 will be a major rewrite, I anticipate that certain features will not be fully ported over on the initial release. If I feel that these features are still useful or relevant, I will reintroduce them in a future version.
  - Those who wish to use any features that are not ported at the time version 1.0 is released are welcome to use an older version pre-1.0
* I will not be officially supporting Windows XP or Vista in v1.0. I do not plan to change the version of the .NET framework away from v4 at the time, nor do I expect that I will make changes that will break XP/Vista support. However, if an issue occurs when using the application on one of these OSes that requires a bugfix, I will not spend the time to implement it. These OSes are not supported by Microsoft, so I do not feel a need to go out of my way to support them when I cannot test the application against these platforms.
  - If someone feels inclined enough to submit a PR for a fix of such an issue, so long as it does not affect the application on recent Windows versions, I will accept such Pull Requests.

Please note that this is a side project for me that I work on during my leisure, so development progress will likely be sporatic between my other demands. I am still happy to try and help troubleshoot issues with the current version, though please be aware that many issues which are reported are often very difficult for me to replicate, so the more detail in your issues, the better.

## Recent Changes

  * Replaced the vJoy virtual joystick backend with [ViGEmBus](https://github.com/nefarius/ViGEmBus). Controllers now appear to Windows and games as genuine Xbox 360 (XInput) controllers — no vJoy configuration, no calibration, and no compatibility workarounds like x360ce are needed anymore.
  * Force feedback (rumble) is now passed through from games to the physical controller.
  * Removed the "Triggers as Buttons" (Final Fantasy XIV compatibility) mode. With native XInput emulation, games address the triggers correctly without it.
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

## Known Functional Limitations

-Headset not supported. The scope of this application is not to create a driver but drive existing drivers (ViGEmBus). To implement this I would need to find a virtual audio driver or create my own, and it may not work great due to latency issues anyway. Also, because this is windows, there are many ways to attach a headset/microphone other than through the Xbox 360 Controller. In short, I don't see a huge need for this functionality, but feel free to post an issue if you'd really like to see it added. With enough support, I may try to explore it.

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

_Installing the ViGEmBus Driver_
  1. Download the latest ViGEmBus setup [here](https://github.com/nefarius/ViGEmBus/releases).
  1. Run the installer as an Admin and follow the prompts.
  1. That's it — no per-device configuration is needed. Virtual Xbox 360 controllers are created automatically as your physical controllers connect, so multiple controllers work out of the box.

_Running the Xbox 360 Wireless Chatpad Application_
  1. Download the latest version of the application [here](https://github.com/KytechN24/xbox360wirelesschatpad/releases).
  1. Extract all of the files in the archive to a single directory.
  1. Execute "Xbox 360 Wireless Chatpad.exe"
  1. Follow in instruction in the application to connect your controller.

Note: Controllers appear to Windows as standard Xbox 360 (XInput) controllers, so no calibration is required.

Having trouble getting things set up? Head on over to the GitHub issues. Try searching through the issues to see if someone has had a similar problem (don't forget to also search closed issues). If you don't see an answer there, open a new issue and someone will help.

## Special Commands ##
Mouse Mode will allow you to move the mouse cursor, perform both left and right clicks, and scroll a window vertically. To enable Mouse Mode simply hold down the following button combination for 3 seconds:
  * Left Bumper + Right Bumper + Back
Once in Mouse Mode use the left stick to move the mouse cursor. Holding down the Left Trigger will scale how fast the cursor moves. Use the A Button for Left Click and B Button for Right Click. Use the Right Joystick Y Axis to scroll a window vertically, similar to a mouse wheel.

To shut down a controller while leaving the application running hold down the following button combination for 5 seconds:
  * Left Trigger + Right Trigger + Back
To reconnect the controller, simply press the Guide button like normal.
