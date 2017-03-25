

# Hosts
##### Hosts is a CMD commandline tool to manage your hosts file entries.

```
 Usage: hosts [OPTIONS] VALUE
 Shorthand for add: hosts VALUE

Copyright 2017 René Larch

  -e, --edit      Edit the hosts file in editor. set %EDITOR% to use your
                  favorite editor.
  -l, --list      List using wildcards or regex
  -a, --add       Add to hosts file
  -r, --remove    Remove from hosts file
  -f, --force     Use force (e.g. force remove)
  -i, --ip        Filter by ip address
  -n, --line      Filter by line number
  -R, --regex     Use regex for filter
  -d, --debug     Enables debuging
  --help          Display this help screen.
```

### Install
If you have the chocolatey packagemanager installed, you can just run:
```CMD
choco install hosts -version 1.1.0
```

or on Windows 10 with OneGet aka PackageManager you can run:
```PowerShell
PowerShell
Install-Package hosts -version 1.1.0
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
Install-Package hosts -version 1.1.0

# reset ExecutionPolicy
Set-ExecutionPolicy $old
```

