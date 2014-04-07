IF NOT %1 == Release goto end
CD "%~2"
IF NOT EXIST merged MKDIR merged
..\..\..\..\packages\ilmerge.2.13.0307\ILMerge.exe /internalize /out:merged\ArgusTV.ServiceProxy.dll ArgusTV.ServiceProxy.dll System.Net.Http.dll System.Net.Http.Extensions.dll System.Net.Http.Primitives.dll System.Net.Http.WebRequest.dll System.Runtime.dll System.IO.dll System.Threading.Tasks.dll
:end
exit 0
