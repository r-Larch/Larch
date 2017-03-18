@echo off

set found=false
for %%f in (choco.exe) do if exist "%%~$PATH:f" set found=true

if %found%==false (
	echo Install chocolatey
    @powershell -NoProfile -ExecutionPolicy Bypass -Command "iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"
)

choco pack %1 --outputdirectory %2