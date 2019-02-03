# Builds the extensions and copies the plgx file into the KeePass plugin folder

# Build
# Remarks: Pipe '@()|' skips 'pause' in batch file
@()|.\build.bat

# Copy plgx file
# Remarks: 1. -Verb runAs starts an elevated powershell instance
#          2. $((${env:ProgramFiles(x86)},${env:ProgramFiles} -ne $null)[0]) gets the program folder for x86 on Windows x86 and x64
#          3. \`" escapes double quotes in the argument list of start-process
Start-Process powershell.exe -Verb runAs -ArgumentList "-Command cd \`"${PSScriptRoot}\`";cp \`".\HaveIBeenPwned\bin\ReleasePlgx\HaveIBeenPwned.plgx\`" \`"$((${env:ProgramFiles(x86)},${env:ProgramFiles} -ne $null)[0])\KeePass Password Safe 2\Plugins\`""