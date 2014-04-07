if %3 == Release goto mergePlugin
  set mpDir="C:\Program Files\Team MediaPortal\MediaPortal"
  set progDir="C:\ProgramData\Team MediaPortal\MediaPortal"
IF NOT EXIST %mpDir% goto end
  copy %1ArgusTV.UI.MediaPortal.* %mpDir%\plugins\Windows
  copy %1ArgusTV.ServiceProxy.dll %mpDir%
  copy %1ArgusTV.DataContracts.dll %mpDir%
  copy %1ArgusTV.UI.Process.dll %mpDir%
  copy %1ArgusTV.Client.Common.* %mpDir%
  xcopy /S /Y /I %1..\..\skin %progDir%\skin
  xcopy /S /Y /I %1..\..\language %progDir%\language
:end
  set mpDir="C:\Program Files (x86)\Team MediaPortal\MediaPortal"
IF NOT EXIST %mpDir% goto reallyEnd
  copy %1ArgusTV.UI.MediaPortal.* %mpDir%\plugins\Windows
  copy %1ArgusTV.ServiceProxy.dll %mpDir%
  copy %1ArgusTV.DataContracts.dll %mpDir%
  copy %1ArgusTV.UI.Process.dll %mpDir%
  copy %1ArgusTV.Client.Common.* %mpDir%
  xcopy /S /Y /I %1..\..\skin %progDir%\skin
  xcopy /S /Y /I %1..\..\language %progDir%\language
  goto reallyEnd

:mergePlugin
  CD "%~1"
  IF NOT EXIST merged MKDIR merged
  ..\..\..\packages\ilmerge.2.13.0307\ILMerge.exe /internalize /out:merged\ArgusTV.UI.MediaPortal.dll ArgusTV.UI.MediaPortal.dll ArgusTV.Client.Common.dll ArgusTV.UI.Process.dll

:reallyEnd
  REM IF EXIST "C:\ProgramData\Team MediaPortal\MediaPortal\Cache" DEL /S /Q "C:\ProgramData\Team MediaPortal\MediaPortal\Cache"
  REM IF EXIST "C:\Documents And Settings\All Users\Application Data\Team Mediaportal\Mediaportal\Cache" DEL /S /Q "C:\Documents And Settings\All Users\Application Data\Team Mediaportal\Mediaportal\Cache"
  exit 0
