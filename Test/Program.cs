using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Security;


namespace Test {
    public class Program {
        public static void Main(string[] args) {
            var credential = new NetworkCredential("user", "pass");


            var connectionInfo = new WSManConnectionInfo() {
                Credential = new PSCredential(credential.UserName, credential.SecurePassword),
                //ConnectionUri = new Uri("")
                ComputerName = "web2.inetcons.net"
            };
            var runspace = RunspaceFactory.CreateRunspace(connectionInfo);
            runspace.Open();
            using (var ps = PowerShell.Create()) {
                ps.Runspace = runspace;
                ps.AddScript("Get-Service");
                var results = ps.Invoke();
                // Do something with result ... 

                Console.WriteLine("results: " + results.Count);
            }
            runspace.Close();
        }
    }
}