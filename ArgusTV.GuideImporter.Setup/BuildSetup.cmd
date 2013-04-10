set makensis="C:\Program Files\NSIS\makensis.exe"
IF EXIST %makensis% goto buildSetup
set makensis="C:\Program Files (x86)\NSIS\makensis.exe"
IF NOT EXIST %makensis% goto end
:buildSetup
%makensis% /V3 /DTARGET=%2 %1
:end
exit 0
