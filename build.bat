@echo off
set base=%~dp0
set findnet=powershell -command "$([System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory())"

for /f "tokens=* USEBACKQ" %%I in (`%findnet%`) DO (
  set netframework=%%I
)

pushd ""%base%
cd /d ""%base%
del HaveIBeenPwned.plgx
%netframework%MSBuild.exe /target:clean HaveIBeenPwned.sln
%netframework%MSBuild.exe /p:Configuration=ReleasePlgx /m HaveIBeenPwned.sln
copy /y HaveIBeenPwned\bin\ReleasePlgx\HaveIBeenPwned.dll mono
copy /y HaveIBeenPwned\bin\ReleasePlgx\HaveIBeenPwned.plgx .
popd

pause