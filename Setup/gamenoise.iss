#include "fileversion.iss"
#include "dotnet35.iss"

[Setup]
AppCopyright=Gamenoise
AppName=Gamenoise
AppVerName=Gamenoise Beta
PrivilegesRequired=admin
DefaultDirName={pf}\Gamenoise
WizardImageFile=Styles\left.bmp
WizardSmallImageFile=logo.bmp
OutputDir=..\..\SETUP
OutputBaseFilename=gamenoise-setup-beta
SetupIconFile=trayicon.ico
AlwaysShowGroupOnReadyPage=true
DefaultGroupName=Gamenoise
ChangesAssociations=true

[Languages]
Name: en; MessagesFile: compiler:Default.isl
Name: de; MessagesFile: compiler:Languages\German.isl

[Tasks]
Name: desktopicon; Description: Desktop-Icon erstellen; Flags: unchecked
Name: regfiles; Description: Audio-Dateien automatisch mit Gamenoise öffnen

[Icons]
Name: {group}\Gamenoise; Filename: {app}\Gamenoise.exe; Comment: Gamenoise - your ingame musicplayer; WorkingDir: {app}
Name: {userdesktop}\Gamenoise; Filename: {app}\Gamenoise.exe; Tasks: desktopicon; WorkingDir: {app}; IconIndex: 0; Comment: Gamenoise - your ingame musicplayer

[Run]
Filename: {app}\Gamenoise.exe; Flags: nowait postinstall skipifsilent; WorkingDir: {app}

[Files]
;============ Binaries ============
Source: ..\..\BIN\UserInterface.exe; DestDir: {app}; DestName: Gamenoise.exe; Flags: replacesameversion
;DLLs
Source: ..\..\BIN\bass.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\Bass.Net.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\bass_fx.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\bassmix.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\basswma.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\Interfaces.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\Observer.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\Organisation.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\Overlay.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\PlayControl.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\ResourceLibrary.dll; DestDir: {app}; Flags: replacesameversion
Source: ..\..\BIN\PluginManager.dll; DestDir: {app}; Flags: replacesameversion
;============ Settings ============
Source: ..\..\BIN\equalizer.set; DestDir: {userdocs}\Gamenoise
;============ Plugins ============
;RadioNoise
Source: ..\..\BIN\Plugins\RadioNoise\RadioNoise.dll; DestDir: {app}\Plugins\RadioNoise; Flags: replacesameversion
Source: ..\..\DEBUG\Plugins\RadioNoise\Skins\gamenoiseLight\plugin_radioNoise.xaml; DestDir: {app}\Plugins\RadioNoise\Skins\gamenoiseLight; Flags: replacesameversion
Source: ..\..\DEBUG\Plugins\RadioNoise\Skins\gamenoiseLight\Simple Styles.xaml; DestDir: {app}\Plugins\RadioNoise\Skins\gamenoiseLight; Flags: replacesameversion
;WebService
Source: ..\..\BIN\Plugins\Webservice\Webservice.dll; DestDir: {app}\Plugins\Webservice; Flags: replacesameversion
Source: ..\..\BIN\Plugins\Webservice\CookComputing.XmlRpcV2.dll; DestDir: {app}\Plugins\Webservice; Flags: replacesameversion
Source: ..\..\BIN\Plugins\Webservice\IStateName.dll; DestDir: {app}\Plugins\Webservice; Flags: replacesameversion
Source: ..\..\BIN\Plugins\Webservice\MathService.dll; DestDir: {app}\Plugins\Webservice; Flags: replacesameversion
Source: ..\..\DEBUG\Plugins\Webservice\Skins\gamenoiseLight\plugin_Webservice.xaml; DestDir: {app}\Plugins\Webservice\Skins\gamenoiseLight; Flags: replacesameversion
Source: ..\..\DEBUG\Plugins\Webservice\Skins\gamenoiseLight\Simple Styles.xaml; DestDir: {app}\Plugins\Webservice\Skins\gamenoiseLight; Flags: replacesameversion
;============ SKINS ============
;DarkNoise
Source: ..\..\DEBUG\Skins\TheDarkNoise\controls.xaml; DestDir: {app}\Skins\TheDarkNoise; Flags: replacesameversion
Source: ..\..\DEBUG\Skins\TheDarkNoise\styles.xaml; DestDir: {app}\Skins\TheDarkNoise; Flags: replacesameversion
Source: ..\..\DEBUG\Skins\TheDarkNoise\preview.jpg; DestDir: {app}\Skins\TheDarkNoise; Flags: replacesameversion
;Light
Source: ..\..\DEBUG\Skins\gamenoiseLight\controls.xaml; DestDir: {app}\Skins\gamenoiseLight; Flags: replacesameversion
Source: ..\..\DEBUG\Skins\gamenoiseLight\styles.xaml; DestDir: {app}\Skins\gamenoiseLight; Flags: replacesameversion
Source: ..\..\DEBUG\Skins\gamenoiseLight\preview.jpg; DestDir: {app}\Skins\gamenoiseLight; Flags: replacesameversion
;Global style
Source: ..\..\DEBUG\Skins\Simple Styles.xaml; DestDir: {app}\Skins; Flags: replacesameversion
;Language
Source: ..\..\DEBUG\en-US\UserInterface.resources.dll; DestDir: {app}\en-US; Flags: replacesameversion
Source: ..\..\DEBUG\de-DE\UserInterface.resources.dll; DestDir: {app}\de-DE; Flags: replacesameversion
;Source: ..\..\BIN\en-US\UserInterface.resources.dll.tmp; DestDir: {app}\en-US
;============ ICONS ============
Source: trayicon.ico; DestDir: {app}; Flags: replacesameversion
Source: logo.bmp; DestDir: {tmp}; Flags: dontcopy overwritereadonly
; Add the ISSkin DLL used for skinning Inno Setup installations.
Source: ISSkin.dll; DestDir: {app}; Flags: dontcopy overwritereadonly
; Add the Visual Style resource contains resources used for skinning,
; you can also use Microsoft Visual Styles (*.msstyles) resources.
Source: Styles\Vista.cjstyles; DestDir: {tmp}; Flags: dontcopy overwritereadonly

[InstallDelete]
Name: {app}\Skins\gamenoiseRetro; Type: filesandordirs; Tasks: ; Languages: 

[Registry]
;Link mp3 to Gamenoise
Root: HKCR; Subkey: .mp3; ValueType: string; ValueData: Gamenoise; Tasks: regfiles
;Set Subtext
Root: HKCR; Subkey: Gamenoise; ValueType: string; ValueData: Gamenoise - your ingame musicplayer; Flags: uninsdeletekey; Tasks: regfiles
;Set Default Icon
Root: HKCR; Subkey: Gamenoise\DefaultIcon; ValueType: string; ValueData: {app}\Gamenoise.exe,0; Flags: uninsdeletekey; Tasks: regfiles
;Set exe to open
Root: HKCR; Subkey: Gamenoise\shell\open\command; ValueType: string; ValueData: """{app}\Gamenoise.exe"" ""%1"""; Flags: uninsdeletekey; Tasks: regfiles
;Delete user defined prog
Root: HKCU; Subkey: Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.mp3\UserChoice; ValueType: none; Flags: deletekey; Tasks: regfiles

[Code]
// Importing LoadSkin API from ISSkin.DLL
procedure LoadSkin(lpszPath: String; lpszIniFileName: String);
external 'LoadSkin@files:isskin.dll stdcall';

// Importing UnloadSkin API from ISSkin.DLL
procedure UnloadSkin();
external 'UnloadSkin@files:isskin.dll stdcall';

// Importing ShowWindow Windows API from User32.DLL
function ShowWindow(hWnd: Integer; uType: Integer): Integer;
external 'ShowWindow@user32.dll stdcall';

function InitializeSetup(): Boolean;
var
  netFrameWorkInstalled2 : Boolean;
  isInstalled: Cardinal;
begin
	netFrameworkInstalled2 := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5', 'Install', isInstalled);
	ExtractTemporaryFile('Vista.cjstyles');
	LoadSkin(ExpandConstant('{tmp}\Vista.cjstyles'), '');
	Result := True;
	InitializeDOTNetCheck();
end;

procedure DeinitializeSetup();
begin
	// Hide Window before unloading skin so user does not get
	// a glimse of an unskinned window before it is closed.
	ShowWindow(StrToInt(ExpandConstant('{wizardhwnd}')), 0);
	UnloadSkin();
end;

