# Warbox
Warbox is a modding tool for KCD 2. 

[![GitHub release](https://img.shields.io/github/release/vawser/Warbox.svg)](https://github.com/vawser/Warbox/releases/latest)
[![Github All Releases](https://img.shields.io/github/downloads/vawser/Warbox/total.svg)](https://github.com/vawser/Warbox/releases/latest)

## Key Features
- Table Editor: editor for searching and editing all the configuration table data.
- Text Editor: editor for searching and editing the text localization.

## Requirements
* Windows 7/8/8.1/10/11 (64-bit only)
* [Visual C++ Redistributable x64](https://aka.ms/vs/16/release/vc_redist.x64.exe)
* For the error message "You must install or update .NET to run this application", use these exact download links. It is not enough to install the default .NET runtime.
  * [Microsoft .NET Core 7.0 Desktop Runtime](https://aka.ms/dotnet/7.0/windowsdesktop-runtime-win-x64.exe)
  * [Microsoft .NET Core 7.0 ASP.NET Core Runtime](https://aka.ms/dotnet/7.0/aspnetcore-runtime-win-x64.exe)
* A Vulkan 1.3 compatible graphics card with up to date graphics drivers: NVIDIA Maxwell (900 series) and newer or AMD Polaris (Radeon 400 series) and newer
* Intel GPUs currently don't seem to be working properly. At the moment you will need a dedicated NVIDIA or AMD GPU

## Usage Instructions
You will be prompted to create a project when first starting the program.
1. Name the project.
2. Select the Data directory. This is the top directory that contains all of the game data, i.e. G:\SteamLibrary\steamapps\common\KingdomComeDeliverance2
3. Select the Project directory. This is the directory your project files will reside in. i.e. G:\SteamLibrary\steamapps\common\KingdomComeDeliverance2\Mods\MyMod

## Credits
* Vawser
* Katalash, philiquaz, george, thefifthmatt, TKGP, Nordgaren, Pav, Meowmaritus, etc for the DSMapStudio tool, which this tool is based upon.
