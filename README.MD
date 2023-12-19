FEngCli + FNGInjector Package v0.2.0-alpha (2022-12-09)

!!! THIS IS A TOOL FOR EXPERTS !!!

Supported games: UG2 (US 1.2), MW (US 1.3), Carbon (US 1.4)
FNGInjector requires Ultimate ASI Loader (https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases/download/Win32-latest/dinput8.zip)
FEngCli requires .NET 6.0 (https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.11-windows-x64-installer)

Installing FNGInjector for:
- UG2: copy fe_hotreload_ug2.asi to game's scripts folder
- MW: copy fe_hotreload_mw.asi to game's scripts folder
- Carbon: copy fe_hotreload_carbon.asi to game's scripts folder
- any other game: wait for a release that supports it :)

Extracting FNGs: use SATEditor (https://nfsmods.xyz/mod/143) to extract FNGs from BUN/LZC files.

Decompiling FNGs to JSON: FEngCli.exe decompile -i <path to FNG> -o <path to JSON file that should be created>

Editing FNGs: Just edit the JSON file(s) that you generate with FEngCli. Most of it is relatively straightforward and can be learned through experimentation. Note that "Rotation" is a quaternion, and using online tools (see: https://www.andre-gaschler.com/rotationconverter/) will make working with them easier.

Compiling FNGs from JSON: FEngCli.exe compile -i <path to FNG JSON file> -o <path to FNG file that should be created>

Loading custom FNGs into the game: make a folder called fngs in the game's scripts folder, then copy compiled FNG files there. Custom FNGs will be loaded when the game attempts to load the corresponding originals.

Questions? Ask in #modders-private, someone can probably help you