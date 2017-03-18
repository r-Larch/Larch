

# Hosts
##### Hosts is a CMD commandline tool to manage your hosts file entries.

```
Usage: hosts.exe [OPTIONS]+

Options:
  -e, --edit                 Edit the hosts file
  -l, --list=VALUE           list host in hosts file
  -a, --add=VALUE            Add to hosts file
  -r, --remove=VALUE         Remove from hosts file
      --rf=VALUE             Force Remove from hosts file
  -i, --searchIp=VALUE       Search for IP in hosts file
  -h, --help, -?             show this message and exit
```

### Install
If you have the chocolatey packagemanager installed, you can just run:
```PowerShell
PowerShell
choco install hosts -y
```

or on Windows 10 with OneGet aka PackageManager you can run:
```PowerShell
PowerShell
Install-Package hosts
```

if `Install-Package hosts` fails you have to install the chocolatey package provider
and you need to set your `ExecutionPolicy` to `RemoteSigned`.
##### Use this to do that:

```PowerShell
PowerShell

# remember ExecutionPolicy
$old = Get-ExecutionPolicy
Set-ExecutionPolicy RemoteSigned

# install chocolatey
iwr https://chocolatey.org/install.ps1 -UseBasicParsing | iex

# setup provider
Install-PackageProvider -Name Chocolatey
Set-PackageSource -Name chocolatey

# install the package
Install-Package hosts

# reset ExecutionPolicy
Set-ExecutionPolicy $old
```

