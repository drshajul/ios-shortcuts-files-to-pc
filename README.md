# iPhone Copy to PC Shortcut Helper
Directly transfer your files, photos, video or any media to any PC or Linux wirelessly over the lan via IPhone's built-in Shortcuts application with Http Post request without any necessity of 3rd party drive/web/internet services. 
Also, one way **Clipboard** sync for text! 

## How To Make It Work
1. Your IPhone and the PC you want to transfer, both must be in the same network.

2. **Download** and run "ShortcutsListener" from [Releases](https://github.com/shajul/ios-shortcuts-files-to-pc/releases/latest/) for Windows users **OR** You can **Compile** the "ShortcutsListener" C# .net project and run the application(server)

3. **Install Shortcut** from [RoutineHub](https://routinehub.co/shortcut/17314/). Configure the shortcut with the ip address shown in the console on PC.

All selected files, photos and videos will be transfered to PC. Or you can use it from the share sheet.

- Note: Transfer stops as soon as iPhone screen goes off. You need to keep the screen on while you are transfering too many photos or huge videos.
- Note: To make large amount of photo/video transfer 'Allow Sharing Large Amounts of Data' needs to be turned on from 'Settings -> Shortcuts -> Advanced' screen.

## Compiling Hints
VS Code with [DotNet SDK](https://dotnet.microsoft.com/en-us/download) and `C# Dev Kit` extension is required.
Then from VS Code commands pallete, `.NET: Generate Assets for Build and Debug` command.
Confirm everything is in order with `dotnet --version`
You can then build debug with `dotnet build` and release with `dotnet publish`.
For different architecture (eg. Raspberry Pi 4 with arm64), you can build with `dotnet publish --runtime linux-arm64 --self-contained` and copy the `publish` directory with scp [like so](https://learn.microsoft.com/en-us/dotnet/iot/deployment) 

## Credits
Fork from [bariseksi](https://github.com/bariseksi/iphone-shortcuts-media-transfer), merge from [legend-is-alive](https://github.com/legend-is-alive/iphone-shortcuts-media-transfer)
