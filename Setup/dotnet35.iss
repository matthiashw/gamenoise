#include "isxdl.iss"
#include "dotnet35_strings.iss"

[Files]
Source: "isxdl\german.ini"; Flags: dontcopy

[Run]
Filename: "{ini:{tmp}\dep.ini,install,msi31}"; Description: "{cm:MSI31Title}"; StatusMsg: "{cm:MSI31InstallMsg}"; Parameters: "/quiet /norestart"; Flags: skipifdoesntexist
Filename: "{ini:{tmp}\dep.ini,install,dotnet}"; Description: "{cm:DOTNETTitle}"; StatusMsg: "{cm:DOTNETInstallMsg}"; Parameters: "/Q /T:{tmp}\dotnetfx /C:""install /q"""; Flags: skipifdoesntexist

[Code]
var
  dotnetPath, msi31Path: string;
  downloadNeeded: boolean;
  dependenciesDownloadMemo: string;
  dependenciesInstallMemo: string;
  dependenciesDownloadMsg: string;

const
  msi31URL = 'http://download.microsoft.com/download/1/4/7/147ded26-931c-4daf-9095-ec7baf996f46/WindowsInstaller-KB893803-v2-x86.exe';
  dotnetURL = 'http://download.microsoft.com/download/6/0/f/60fc5854-3cb8-4892-b6db-bd4f42510f28/dotnetfx35.exe';
  memoWhiteSpace = '      ';

// get Windows Installer version
procedure DecodeVersion(const Version: cardinal; var a, b : word);
begin
  a := word(Version shr 16);
  b := word(Version and not $ffff0000);
end;

function IsMinMSIAvailable(HV:Integer; NV:Integer ): boolean;
var  Version,  dummy     : cardinal;
     MsiHiVer,  MsiLoVer  : word;

begin
    Result:=(FileExists(ExpandConstant('{sys}\msi.dll'))) and
        (GetVersionNumbers(ExpandConstant('{sys}\msi.dll'), Version, dummy));
    DecodeVersion(Version, MsiHiVer, MsiLoVer);
    Result:= (Result) and (MsiHiVer >= HV) and (MsiLoVer >= NV);
end;

function InitializeDOTNetCheck(): Boolean;
var
  SoftwareVersion: string;
  WindowsVersion: TWindowsVersion;
  ResultCode: integer;
  isInstalled: Cardinal;
  netFrameWorkInstalled : Boolean;


begin
  GetWindowsVersionEx(WindowsVersion);
  Result := true;

  // Check for Windows XP and SP < 2
  if (WindowsVersion.Major = 5) and
     (WindowsVersion.Minor = 1) and
     (WindowsVersion.ServicePackMajor < 2) then
  begin
    MsgBox(CustomMessage('WinXPSp2Msg'), mbError, MB_OK);
    Result := false;
    exit;
  end;

  // Check for incompatible Windows 2000
  if (WindowsVersion.Major = 5) and
     (WindowsVersion.Minor = 0) then
  begin
    MsgBox('Windows 2000 is not supported', mbError, MB_OK);
    Result := false;
    exit;
  end;
  
  // Check for incompatible Windows <=98
  if (not WindowsVersion.NTPlatform) then
  begin
    MsgBox('Windows 98 and lower not supported', mbError, MB_OK);
    Result := false;
    exit;
  end;

  // Check for required Windows Installer 3.0 on XP, Server 2003, Vista or higher
  if  (fileversion(ExpandConstant('{sys}{\}msi.dll')) < '3.1') then
  begin
    dependenciesInstallMemo := dependenciesInstallMemo + memoWhiteSpace + CustomMessage('MSI31Title') + #13;
    msi31Path := ExpandConstant('{src}') + '\' + CustomMessage('DependenciesDir') + '\WindowsInstaller-KB893803-v2-x86.exe';
    if not FileExists(msi31Path) then begin
      msi31Path := ExpandConstant('{tmp}\msi31.exe');
      if not FileExists(msi31Path) then begin
        dependenciesDownloadMemo := dependenciesDownloadMemo + memoWhiteSpace + CustomMessage('MSI31Title') + #13;
        dependenciesDownloadMsg := dependenciesDownloadMsg + CustomMessage('MSI31Title') + ' (' + CustomMessage('MSI31DownloadSize') + ')' + #13;
        isxdl_AddFile(msi31URL, msi31Path);
        downloadNeeded := true;
      end;
    end;
    SetIniString('install', 'msi31', msi31Path, ExpandConstant('{tmp}\dep.ini'));
  end;

  isInstalled := 0;
  netFrameworkInstalled := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5', 'Install', isInstalled);

  if ((not netFrameworkInstalled)  or (isInstalled <> 1)) then begin
      // dotnetfx 3.5 is not installed
      dependenciesInstallMemo := dependenciesInstallMemo + memoWhiteSpace + CustomMessage('DOTNETTitle') + #13;
      dotnetPath := ExpandConstant('{src}') + '\' + CustomMessage('DependenciesDir') + '\dotnetfx.exe';
			if not FileExists(dotnetPath) then begin
				dotnetPath := ExpandConstant('{tmp}\dotnetfx.exe');
				if not FileExists(dotnetPath) then begin
					dependenciesDownloadMemo := dependenciesDownloadMemo + memoWhiteSpace + CustomMessage('DOTNETTitle') + #13;
					dependenciesDownloadMsg := dependenciesDownloadMsg + CustomMessage('DOTNETTitle') + ' (' + CustomMessage('DOTNETDownloadSize') + ')' + #13;
					isxdl_AddFile(dotnetURL, dotnetPath);
					downloadNeeded := true;
				end;
			end;
			SetIniString('install', 'dotnet', dotnetPath, ExpandConstant('{tmp}\dep.ini'));
  end;
end;


function NextButtonClick(CurPage: Integer): Boolean;
var
  hWnd: Integer;

begin
  Result := true;

  if CurPage = wpReady then begin

    hWnd := StrToInt(ExpandConstant('{wizardhwnd}'));

    if downloadNeeded then begin
      // default isxdl language is already english
      if CompareStr(ActiveLanguage(), 'en') <> 0 then begin
        ExtractTemporaryFile(CustomMessage('ISXDLLanguageFile'));
        isxdl_SetOption('language', ExpandConstant('{tmp}\') + CustomMessage('ISXDLLanguageFile'));
      end;
      isxdl_SetOption('title', FmtMessage(SetupMessage(msgSetupWindowTitle), [CustomMessage('AppName')]));
      
      if MsgBox(FmtMessage(CustomMessage('DownloadMsg'), [dependenciesDownloadMsg]), mbConfirmation, MB_YESNO) = IDNO then Result := false
      else if isxdl_DownloadFiles(hWnd) = 0 then Result := false;
    end;
  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  s: string;

begin
  if dependenciesDownloadMemo <> '' then s := s + CustomMessage('DependenciesDownloadTitle') + ':' + NewLine + dependenciesDownloadMemo + NewLine;
  if dependenciesInstallMemo <> '' then s := s + CustomMessage('DependenciesInstallTitle') + ':' + NewLine + dependenciesInstallMemo + NewLine;

  s := s + MemoDirInfo + NewLine + NewLine + MemoGroupInfo
  if MemoTasksInfo <> '' then  s := s + NewLine + NewLine + MemoTasksInfo;

  Result := s
end;




