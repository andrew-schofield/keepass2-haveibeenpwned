@echo off
set base=%~dp0
set findnet=powershell -command "$([System.Runtime.InteropServices.RuntimeEnvironment]::GetRuntimeDirectory())"

for /f "tokens=* USEBACKQ" %%I in (`%findnet%`) DO (
  set netframework=%%I
)

pushd ""%base%
cd /d ""%base%
del HaveIBeenPwned.plgx
del mono\HaveIBeenPwned.dll
rd /s /q HaveIBeenPwned\bin
rd /s /q HaveIBeenPwned\obj
"%ProgramFiles(x86)%\KeePass Password Safe 2\KeePass.exe" --plgx-create "%base%HaveIBeenPwned"
%netframework%MSBuild.exe /target:clean HaveIBeenPwned.sln
%netframework%MSBuild.exe /p:Configuration=Release HaveIBeenPwned.sln
copy /y HaveIBeenPwned\bin\Release\HaveIBeenPwned.dll mono
popd