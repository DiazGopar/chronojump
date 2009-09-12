; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{69EC15A7-5D66-4D24-A15B-1C23C0F621E0}
AppName=Chronojump
AppVerName=Chronojump  0.8.9.8
AppPublisher=Chronojump
AppPublisherURL=http://www.chronojump.org/
AppSupportURL=http://www.chronojump.org/
AppUpdatesURL=http://www.chronojump.org/
DefaultDirName={pf}\Chronojump
DefaultGroupName=Chronojump
LicenseFile=.\gpl-2.0.txt
OutputDir=.
OutputBaseFilename=Chronojump-0.8.9.8-win32
SetupIconFile=.\chronojump_icon.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "basque"; MessagesFile: "compiler:Languages\Basque.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "slovak"; MessagesFile: "compiler:Languages\Slovak.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "../*"; DestDir: "{app}"; Flags: ignoreversion  recursesubdirs createallsubdirs ;   Excludes: "build"
Source: "./chronojump_icon.ico"; DestDir: "{app}\share\chronojump\images\"
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\Chronojump"; Filename: "{app}\bin\Chronojump.exe"; WorkingDir: "{app}\bin"
Name: "{group}\Chronojump mini"; Filename: "{app}\bin\Chronojump_mini.bat"; WorkingDir: "{app}\bin" ; IconFileName: "{app}\share\chronojump\images\chronojump_icon.ico"
Name: "{commondesktop}\Chronojump"; Filename: "{app}\bin\Chronojump.exe"; WorkingDir: "{app}\bin"; Tasks: desktopicon
Name: "{group}\Change theme - Cambiar tema"; Filename: "{app}\bin\gtk2_prefs.exe"; WorkingDir: "{app}\bin"
Name: "{group}\Chronojump Manual spanish"; Filename: "{app}\share\doc\chronojump\chronojump_manual_es.pdf"
Name: "{group}\{cm:UninstallProgram,Chronojump}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\drivers\CDM 2.04.16.exe";
Filename: "{app}\bin\Chronojump.exe"; Description: "{cm:LaunchProgram,ChronoJump}"; Flags: nowait postinstall skipifsilent

