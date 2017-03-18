. (Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) chocolateyInclude.ps1)


if (!(Test-Path "$projectDir")){
	mkdir "$projectDir"
}
if (!(Test-Path "$binDir")){
	mkdir "$binDir"
}

Copy-Item (Join-Path $PSScriptRoot *.exe) "$projectDir\"
Copy-Item (Join-Path $PSScriptRoot *.dll) "$projectDir\"

Invoke-Expression -Command "cmd /c mklink `"$binDir\$fileName.exe`" `"$projectDir\$fileName.exe`""


if (!"$env:path".Contains("$binDir")){
	$env:Path += ";$binDir"
	Install-ChocolateyEnvironmentVariable -VariableName "Path" -VariableValue "$env:Path" -VariableType Machine
}