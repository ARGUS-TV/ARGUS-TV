!addplugindir ".\NSIS\Plugins"

; Constants
!define PRODUCT_NAME "ARGUS TV Guide Importer"
!define PRODUCT_VERSION "2.3 BETA 8"
!define PRODUCT_PUBLISHER "ARGUS TV"
!define PRODUCT_WEB_SITE "http://www.argus-tv.com"
!define PRODUCT_DIR_REGKEY "Software\ARGUS TV\Install"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

!ifndef TARGET
!define TARGET "Release"
!endif

; Variables

; MUI 1.67 compatible ------
!include "MUI.nsh"
!include "WinMessages.nsh"
!include "LogicLib.nsh"
!include "StrFunc.nsh"
!include ".\NSIS\Include\nsProcess.nsh"

; MUI Settings
!define MUI_WELCOMEFINISHPAGE_BITMAP ".\Bitmaps\Welcome.bmp"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_RIGHT
!define MUI_HEADERIMAGE_BITMAP ".\Bitmaps\Header.bmp"
!define MUI_HEADERIMAGE_UNBITMAP ".\Bitmaps\Header.bmp"
!define MUI_ABORTWARNING
!define MUI_ICON ".\Install.ico"
!define MUI_UNICON ".\Uninstall.ico"

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; UnInstall page
Page Custom RemovePreviousVersionPage RemovePreviousVersionPageLeave
; License page
!insertmacro MUI_PAGE_LICENSE ".\SoftwareLicence.rtf"
; Components page
!insertmacro MUI_PAGE_COMPONENTS

; Directory page
!insertmacro MUI_PAGE_DIRECTORY
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
;!define MUI_FINISHPAGE_RUN "Test.exe"
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_WELCOME
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_COMPONENTS
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_UNPAGE_FINISH

; Language files
!insertmacro MUI_LANGUAGE "English"

; MUI end ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "bin\${TARGET}\ARGUS TV Guide Importer.exe"
InstallDir "$PROGRAMFILES\ARGUS TV"
InstallDirRegKey HKLM "${PRODUCT_DIR_REGKEY}" ""
ShowInstDetails hide
ShowUnInstDetails hide
RequestExecutionLevel admin
BrandingText " "
UninstallCaption "${PRODUCT_NAME} Uninstall"

Section "Plugin Hoster" GuideImporterSection
  SectionIn RO
  SetShellVarContext all
  SetOverwrite try
  SetOutPath "$INSTDIR\Guide Importer"
  File "..\ArgusTV.GuideImporter\bin\${TARGET}\ArgusTV.Common.dll"
  File "..\ArgusTV.GuideImporter\bin\${TARGET}\ArgusTV.DataContracts.dll"
  File "..\ArgusTV.GuideImporter\bin\${TARGET}\ArgusTV.ServiceProxy.dll"
  File "..\ArgusTV.GuideImporter\bin\${TARGET}\ArgusTV.GuideImporter.Interfaces.dll"  
  File "..\ArgusTV.GuideImporter\bin\${TARGET}\ArgusTV.GuideImporter.exe"
  File "..\ArgusTV.GuideImporter\bin\${TARGET}\ArgusTV.GuideImporter.exe.config"
  File "..\ArgusTV.GuideImporter\bin\${TARGET}\log4net.dll"

  CreateDirectory "$SMPROGRAMS\ARGUS TV"
  CreateShortCut "$SMPROGRAMS\ARGUS TV\Guide Importer.lnk" "$INSTDIR\Guide Importer\ArgusTV.GuideImporter.exe"
  CreateShortCut "$DESKTOP\ARGUS TV Guide Importer.lnk" "$INSTDIR\Guide Importer\ArgusTV.GuideImporter.exe"
SectionEnd

SectionGroup /e "Plugins" GuideImporterSectionGroup

Section "ClickFinder" ClickFinderPluginSection

  SetShellVarContext all
  SetOverwrite try
  SetOutPath "$INSTDIR\Guide Importer\Plugins\ClickFinder"
  File "..\ArgusTV.GuideImporter.ClickFinder\bin\${TARGET}\ArgusTV.DataContracts.dll"
  File "..\ArgusTV.GuideImporter.ClickFinder\bin\${TARGET}\ArgusTV.GuideImporter.Interfaces.dll"
  File "..\ArgusTV.GuideImporter.ClickFinder\bin\${TARGET}\ArgusTV.GuideImporter.ClickFinder.dll"

SectionEnd

Section "SchedulesDirect" SchedulesDirectPluginSection

  SetShellVarContext all
  SetOverwrite try
  SetOutPath "$INSTDIR\Guide Importer\Plugins\SchedulesDirect"
  File "..\ArgusTV.GuideImporter.SchedulesDirect\bin\${TARGET}\ArgusTV.Common.dll"
  File "..\ArgusTV.GuideImporter.SchedulesDirect\bin\${TARGET}\ArgusTV.DataContracts.dll"
  File "..\ArgusTV.GuideImporter.SchedulesDirect\bin\${TARGET}\ArgusTV.GuideImporter.Interfaces.dll"
  File "..\ArgusTV.GuideImporter.SchedulesDirect\bin\${TARGET}\ArgusTV.GuideImporter.SchedulesDirect.dll"
  File "..\ArgusTV.GuideImporter.SchedulesDirect\bin\${TARGET}\ArgusTV.GuideImporter.SchedulesDirect.dll.config"
  File "..\ArgusTV.GuideImporter.SchedulesDirect\bin\${TARGET}\AvailableChannels.config"
  
SectionEnd

SectionGroupEnd

Section -AdditionalIcons
  SetOutPath $INSTDIR
  WriteIniStr "$INSTDIR\ARGUS TV.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\ARGUS TV\Website.lnk" "$INSTDIR\ARGUS TV.url"
  CreateShortCut "$SMPROGRAMS\ARGUS TV\Uninstall Guide Importer.lnk" "$INSTDIR\uninst.GuideImporter.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.GuideImporter.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.GuideImporter.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\ArgusTV.ico"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

; Section descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${GuideImporterSection} "ARGUS TV Guide Importer, host for all importer plugins."
  !insertmacro MUI_DESCRIPTION_TEXT ${ClickFinderPluginSection} "ClickFinder plugin, to import guide data from ClickFinder."
  !insertmacro MUI_DESCRIPTION_TEXT ${SchedulesDirectPluginSection} "SchedulesDirect plugin, to import guide data from SchedulesDirect."  
!insertmacro MUI_FUNCTION_DESCRIPTION_END

Function .onInit
FunctionEnd

Function un.onUninstSuccess
FunctionEnd

Function un.onInit
FunctionEnd

Section Uninstall

  SetShellVarContext all
  CALL un.CloseRunningArgusTVApplications

  ; Remove Guide Importer
  Delete "$INSTDIR\Guide Importer\ArgusTV.Common.dll"
  Delete "$INSTDIR\Guide Importer\ArgusTV.DataContracts.dll"
  Delete "$INSTDIR\Guide Importer\ArgusTV.ServiceProxy.dll"
  Delete "$INSTDIR\Guide Importer\ArgusTV.GuideImporter.Interfaces.dll"
  Delete "$INSTDIR\Guide Importer\ArgusTV.GuideImporter.exe"  
  Delete "$INSTDIR\Guide Importer\ArgusTV.GuideImporter.exe.config"
  Delete "$INSTDIR\Guide Importer\log4net.dll"

  ;remove Plugins
  Delete "$INSTDIR\Guide Importer\plugins\ClickFinder\ArgusTV.DataContracts.dll"
  Delete "$INSTDIR\Guide Importer\plugins\ClickFinder\ArgusTV.GuideImporter.Interfaces.dll"
  Delete "$INSTDIR\Guide Importer\plugins\ClickFinder\ArgusTV.GuideImporter.ClickFinder.dll"

  Delete "$INSTDIR\Guide Importer\plugins\SchedulesDirect\ArgusTV.Common.dll"
  Delete "$INSTDIR\Guide Importer\plugins\SchedulesDirect\ArgusTV.DataContracts.dll"
  Delete "$INSTDIR\Guide Importer\plugins\SchedulesDirect\ArgusTV.GuideImporter.Interfaces.dll"
  Delete "$INSTDIR\Guide Importer\plugins\SchedulesDirect\ArgusTV.GuideImporter.SchedulesDirect.dll"
  Delete "$INSTDIR\Guide Importer\plugins\SchedulesDirect\ArgusTV.GuideImporter.SchedulesDirect.dll.config"

  RMDir /r "$INSTDIR\Guide Importer"

  ; Shortcuts
  Delete "$SMPROGRAMS\ARGUS TV\Uninstall Guide Importer.lnk"
  Delete "$SMPROGRAMS\ARGUS TV\Guide Importer.lnk"

  ; Cleanup directories
  Delete "$INSTDIR\uninst.GuideImporter.exe"

  IfFileExists "$INSTDIR\uninst.ArgusTV.exe" ArgusTVInstalled 0
    ; Shortcut    
    Delete "$SMPROGRAMS\ARGUS TV\Website.lnk"    

    Delete "$INSTDIR\ARGUS TV.url"
    Delete "$INSTDIR\ArgusTV.ico"
    RMDir "$SMPROGRAMS\ARGUS TV"
    RMDir "$INSTDIR"

ArgusTVInstalled:

  ; Cleanup registry
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

Function RemovePreviousVersionPage
  ; discover if ARGUS TV is already installed
  IfFileExists "$INSTDIR\uninst.GuideImporter.exe" show 0
    Abort
show:
  ReserveFile "RemovePreviousVersionPage.ini"
  !insertmacro MUI_HEADER_TEXT "Another ARGUS TV Guide Importer installation detected" "An Un-Install is required."
  !insertmacro MUI_INSTALLOPTIONS_EXTRACT "RemovePreviousVersionPage.ini"
  !insertmacro MUI_INSTALLOPTIONS_DISPLAY "RemovePreviousVersionPage.ini"
FunctionEnd

Function RemovePreviousVersionPageLeave
  ; only launch un-installer when not already running  
  ${nsProcess::FindProcess} "Au_.exe" $R0
  ;MessageBox MB_OK "nsProcess::FindProcess$\n$\n\Errorlevel: [$R0]"
  StrCmp $R0 0 +2 0
  CopyFiles /SILENT "$INSTDIR\uninst.GuideImporter.exe" "$INSTDIR\uninstall.exe"
  ExecWait '"$INSTDIR\uninstall.exe" _?=$INSTDIR' $0
  Delete "$INSTDIR\uninstall.exe"
  IfFileExists "$INSTDIR\uninst.GuideImporter.exe" 0 end
    MessageBox MB_OK "Uninstall seems to have failed."
    Abort
end:
FunctionEnd

Function un.CloseRunningArgusTVApplications
  loopMMC:
  ${nsProcess::FindProcess} "ArgusTV.GuideImporter.exe" $R0
; MessageBox MB_OK "nsProcess::FindProcess$\n$\n\Errorlevel: [$R0]"
  StrCmp $R0 0 0 +2
  MessageBox MB_OKCANCEL|MB_ICONEXCLAMATION 'Please close ARGUS TV Guide Importer before uninstall can continue' IDOK loopMMC IDCANCEL DoAbort
  goto end

DoAbort:
  Abort

end:
  ${nsProcess::Unload}
FunctionEnd