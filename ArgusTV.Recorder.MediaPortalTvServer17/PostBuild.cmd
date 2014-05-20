if %3 == Release goto mergePlugin

  set pluginDir="C:\Program Files\Team MediaPortal\MediaPortal TV Server\Plugins"
IF NOT EXIST %pluginDir% goto end
  net stop TvService
  copy %1ArgusTV.ServiceProxy.dll %pluginDir%\..
  copy %1ArgusTV.DataContracts.dll %pluginDir%\..
  copy %1ArgusTV.Common.dll %pluginDir%\..
  copy %1ArgusTV.Common.Recorders.dll %pluginDir%\..
  copy %1RestSharp.dll %pluginDir%\..
  copy %1Nancy.dll %pluginDir%\..
  copy %1Nancy.Hosting.Self.dll %pluginDir%\..
  copy %2 %pluginDir%
:end
  set pluginDir="C:\Program Files (x86)\Team MediaPortal\MediaPortal TV Server\Plugins"
IF NOT EXIST %pluginDir% goto reallyEnd
  net stop TvService
  copy %1ArgusTV.ServiceProxy.dll %pluginDir%\..
  copy %1ArgusTV.DataContracts.dll %pluginDir%\..
  copy %1ArgusTV.Common.dll %pluginDir%\..
  copy %1ArgusTV.Common.Recorders.dll %pluginDir%\..
  copy %1RestSharp.dll %pluginDir%\..
  copy %1Nancy.dll %pluginDir%\..
  copy %1Nancy.Hosting.Self.dll %pluginDir%\..
  copy %2 %pluginDir%
  goto reallyEnd

:mergePlugin
  CD "%~1"
  IF NOT EXIST merged MKDIR merged
  ..\..\..\packages\ilmerge.2.13.0307\ILMerge.exe /out:merged\ArgusTV.Recorder.MediaPortalTvServer.dll ArgusTV.Recorder.MediaPortalTvServer.dll ArgusTV.Common.dll ArgusTV.Common.Recorders.dll Nancy.dll Nancy.Hosting.Self.dll System.Net.Http.dll System.Net.Http.Extensions.dll System.Net.Http.Primitives.dll System.Net.Http.WebRequest.dll System.Runtime.dll System.IO.dll System.Threading.Tasks.dll

:reallyEnd
  exit 0
