using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using Contracts;
using Manager;

namespace MonitoringServiceApp
{
    class MonitoringService: IMonitoringContract
    {
        private string path = "messages.txt";
        public void SendMessageToLogs(byte[] message)
        {
            Console.WriteLine($"Message received.");
            var principal = OperationContext.Current.ServiceSecurityContext.WindowsIdentity;
            var username = Formatter.ParseName(principal.Name);
            // decrypt message
            var keyPath = username + ".key";
            if (File.Exists(keyPath))
            {
                var key = File.ReadAllBytes(keyPath);
                var iv = File.ReadAllBytes(username + ".IV");
                var msg = AESManager.Decrypt(message, key, iv);

                if (!File.Exists(path))
                {
                    File.WriteAllText(path, msg + ".\n  ");
                }
                else
                {
                    File.AppendAllText(path, msg + ".\n  ");
                }
            }
        }
    }
}
