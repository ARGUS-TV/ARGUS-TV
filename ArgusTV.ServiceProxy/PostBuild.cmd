IF NOT %1 == Release goto end
CD "%~2"
IF NOT EXIST merged MKDIR merged
..\..\..\packages\ILRepack.1.25.0\tools\ILRepack.exe /verbose /internalize /out:merged\ArgusTV.ServiceProxy.dll ArgusTV.ServiceProxy.dll RestSharp.dll
:end
exit 0
