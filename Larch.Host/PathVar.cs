using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace EnvVarHelper {
    [RunInstaller(true)]
    public partial class PathVar : System.Configuration.Install.Installer {
        public PathVar() {
            InitializeComponent();

            this.AfterInstall += OnAfterInstall;
        }

        private void OnAfterInstall(object sender, InstallEventArgs installEventArgs) {
            


        }

        public override void Install(IDictionary stateSaver) {
            base.Install(stateSaver);

            var targetdir = Context.Parameters["AssemblyPath"];
            var path = StripDir(targetdir);
            if (string.IsNullOrEmpty(path)) {
                throw new Exception("could not get 'TARGETDIR' in installerClass");
            }


            var keys = (from object key in stateSaver.Keys select key.ToString()).ToList();
            var values = (from object value in stateSaver.Values select value.ToString()).ToList();
            var sb = new StringBuilder();
            for (int i = 0; i < keys.Count; i++) {
                sb.AppendLine($"{keys[i]}: '{values[i]}'");
            }

            throw new Exception($"Targetdir: '{sb.ToString()}'");

            const string keyName = @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment";
            var oldPath = (string) Registry.LocalMachine.CreateSubKey(keyName)?.GetValue("Path", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
            if (oldPath == null || oldPath.Contains(path)) {
                return;
            }
            // update %PATH%
            oldPath = $";{path}";
            Registry.LocalMachine.CreateSubKey(keyName)?.SetValue("Path", oldPath, RegistryValueKind.ExpandString);
        }

        private string StripDir(string fullPath) {
            var retValue = default(string);

            if (!string.IsNullOrEmpty(fullPath)) {
                retValue = fullPath.Substring(0, fullPath.LastIndexOf(@"\", StringComparison.Ordinal));
            }

            return retValue;
        }
    }
}