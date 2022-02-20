using Contracts;
using Manager;
using System;
using System.IO;
using System.Threading;

namespace ClientApp
{
    internal class CommunicationProvider : ICommunication
    {
        public void SendMessage(string msg)
        {
            var receiver = Formatter.ParseName(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
            var sender = Thread.CurrentPrincipal.Identity.Name.Split(';')[0];


            Console.WriteLine($"Message received {msg}");

            var msg1 = $"{sender} --> {receiver}. Content: {msg}";

            // read aes key first
            var keyPath = receiver + ".key";
            Console.WriteLine(receiver);
            if (File.Exists(keyPath))
            {
                var key = File.ReadAllBytes(keyPath);
                var iv = File.ReadAllBytes(receiver + ".IV");

                var encrypted = AESManager.Encrypt(msg1, key, iv);

                var monitorClient = new MonitoringClient();
                monitorClient.SendMessageToLogs(encrypted);
            }
        }
    }
}