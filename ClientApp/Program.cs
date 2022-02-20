using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Manager;
using System.Threading;

namespace ClientApp
{
	public class Program
	{
		static void Main(string[] args)
		{
            var username = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:9999/Receiver"),
                EndpointIdentity.CreateUpnIdentity(username));

            if(CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, username) == null)
            {
                // request certificate
                using (WCFCertIssuerSvcClient proxy = new WCFCertIssuerSvcClient(binding, address))
                {
                    proxy.IssueCertificate();
                }
                Console.WriteLine("Waiting to install certificate.");
                while(CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, username) == null)
                {
                    Thread.Sleep(200);
                }
            }
            int port = 0;
            using (WCFCertIssuerSvcClient proxy = new WCFCertIssuerSvcClient(binding, address))
            {
                Random rnd = new Random(System.DateTime.Now.Millisecond);
                port = rnd.Next(12000, 20000);
                proxy.Register(port);
            }
            var communicationSvc = new CommunicationService($"net.tcp://localhost:{port}/{username}");

            try
            {
                communicationSvc.Open();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            using (WCFCertIssuerSvcClient proxy = new WCFCertIssuerSvcClient(binding, address))
            {
                Console.WriteLine("Press enter to get active user list.");
                Console.ReadLine();
                Console.WriteLine("Active users list:");
                var list = proxy.GetAllActiveUsers();
                var i = 0;
                if (list != null)
                {
                    foreach(var user in list.Keys)
                    {
                        Console.WriteLine($"{++i}. {user}");
                    }

                    Console.WriteLine("Select user:");
                    var input = "";
                    var selected = 0;
                    do
                    {
                        input = Console.ReadLine();
                    } while (!int.TryParse(input, out selected) || selected < 1 || selected > i);

                    Console.WriteLine("Enter message for user: ");
                    var msg = Console.ReadLine();

                    NetTcpBinding tcpBinding = new NetTcpBinding();
                    tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
                    var svcName = list.Keys.ToList()[i-1];
                    Console.WriteLine("Waiting for service certificate...");
                    while (CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, svcName) == null) {
                        Thread.Sleep(200);
                    }
                    X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, svcName);
                    EndpointAddress address1 = new EndpointAddress(new Uri($"net.tcp://localhost:{list[svcName]}/" + svcName), new X509CertificateEndpointIdentity(srvCert));
                    using (CommunicationClient client = new CommunicationClient(tcpBinding, address1))
                    {
                        client.SendMessage(msg);
                    }
                }
            }


            Console.WriteLine("Press <enter> to continue ...");
            Console.ReadLine();

            communicationSvc.Close();

        }
    }
}
