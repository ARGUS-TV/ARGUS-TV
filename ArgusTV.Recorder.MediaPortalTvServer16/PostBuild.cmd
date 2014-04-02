rem if %3 == Release goto reallyEnd
  set pluginDir="C:\Program Files\Team MediaPortal\MediaPortal TV Server\Plugins"
IF NOT EXIST %pluginDir% goto end
  net stop TvService
  copy %1ArgusTV.ServiceContracts.dll %pluginDir%\..
  copy %1ArgusTV.ServiceAgents.dll %pluginDir%\..
  copy %1ArgusTV.DataContracts.dll %pluginDir%\..
  copy %1ArgusTV.Common.dll %pluginDir%\..
  copy %1ArgusTV.Common.Recorders.dll %pluginDir%\..
  copy %2 %pluginDir%
:end
  set pluginDir="C:\Program Files (x86)\Team MediaPortal\MediaPortal TV Server\Plugins"
IF NOT EXIST %pluginDir% goto reallyEnd
  net stop TvService
  copy %1ArgusTV.ServiceContracts.dll %pluginDir%\..
  copy %1ArgusTV.ServiceAgents.dll %pluginDir%\..
  copy %1ArgusTV.DataContracts.dll %pluginDir%\..
  copy %1ArgusTV.Common.dll %pluginDir%\..
  copy %1ArgusTV.Common.Recorders.dll %pluginDir%\..
  copy %2 %pluginDir%
:reallyEnd
  exit 0
