if %3 == Release goto mergePlugin

  set pluginDir="C:\Program Files\Team MediaPortal\MediaPortal TV Server\Plugins"
IF EXIST %pluginDir% goto copyDlls
  set pluginDir="C:\Program Files (x86)\Team MediaPortal\MediaPortal TV Server\Plugins"
IF NOT EXIST %pluginDir% goto reallyEnd
:copyDlls
  net stop TvService
  copy %1ArgusTV.ServiceProxy.dll %pluginDir%\..
  copy %1ArgusTV.DataContracts.dll %pluginDir%\..
  copy %1ArgusTV.Common.dll %pluginDir%\..
  copy %1ArgusTV.Common.Recorders.dll %pluginDir%\..
  copy %1Newtonsoft.JSON.dll %pluginDir%\..
  copy %1System.IO.dll %pluginDir%\..
  copy %1System.Runtime.dll %pluginDir%\..
  copy %1System.Threading.Tasks.dll %pluginDir%\..
  copy %1System.Net.Http*.dll %pluginDir%\..
  copy %2 %pluginDir%
  goto reallyEnd

:mergePlugin
  CD "%~1"
  IF NOT EXIST merged MKDIR merged
  ..\..\..\packages\ilmerge.2.13.0307\ILMerge.exe /internalize /out:merged\ArgusTV.Recorder.MediaPortalTvServer.dll ArgusTV.Recorder.MediaPortalTvServer.dll ArgusTV.Common.dll ArgusTV.Common.Recorders.dll Newtonsoft.Json.dll

:reallyEnd
  exit 0
