. (Join-Path (Split-Path -parent $MyInvocation.MyCommand.Definition) chocolateyInclude.ps1)


Remove-Item "$projectDir" -Force -Recurse
Remove-Item "$binDir\$fileName.exe"


if ("$env:path".Contains("$binDir")){
	$env:path = $env:path.Replace($binDir, "").Replace(";;", ";")
	Install-ChocolateyEnvironmentVariable -VariableName "Path" -VariableValue "$env:Path" -VariableType Machine
}