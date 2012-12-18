goto reallyEnd

if %3 == Release goto reallyEnd
  set mpDir="C:\Program Files\Team MediaPortal\MediaPortal"
  set progDir="C:\ProgramData\Team MediaPortal\MediaPortal"
IF NOT EXIST %mpDir% goto end
  copy %1ArgusTV.ServiceContracts.dll %mpDir%\plugins\Windows
  copy %1ArgusTV.ServiceAgents.dll %mpDir%\plugins\Windows
  copy %1ArgusTV.Entities.dll %mpDir%\plugins\Windows
  copy %1ArgusTV.UI.Process.dll %mpDir%\plugins\Windows
  copy %1ArgusTV.UI.MediaPortal.* %mpDir%\plugins\Windows
  copy %1ArgusTV.Client.Common.* %mpDir%\plugins\Windows
  xcopy /S /Y /I %1..\..\skin %progDir%\skin
  xcopy /S /Y /I %1..\..\language %progDir%\language
:end
  set mpDir="C:\Program Files (x86)\Team MediaPortal\MediaPortal"
IF NOT EXIST %mpDir% goto reallyEnd
  copy %1ArgusTV.ServiceContracts.dll %mpDir%\plugins\Windows
  copy %1ArgusTV.ServiceAgents.dll %mpDir%\plugins\Windows
  copy %1ArgusTV.Entities.dll %mpDir%\plugins\Windows
  copy %1ArgusTV.UI.Process.dll %mpDir%\plugins\Windows
  copy %1ArgusTV.UI.MediaPortal.* %mpDir%\plugins\Windows
  copy %1ArgusTV.Client.Common.* %mpDir%\plugins\Windows
  xcopy /S /Y /I %1..\..\skin %progDir%\skin
  xcopy /S /Y /I %1..\..\language %progDir%\language
:reallyEnd
  REM IF EXIST "C:\ProgramData\Team MediaPortal\MediaPortal\Cache" DEL /S /Q "C:\ProgramData\Team MediaPortal\MediaPortal\Cache"
  REM IF EXIST "C:\Documents And Settings\All Users\Application Data\Team Mediaportal\Mediaportal\Cache" DEL /S /Q "C:\Documents And Settings\All Users\Application Data\Team Mediaportal\Mediaportal\Cache"
  exit 0
