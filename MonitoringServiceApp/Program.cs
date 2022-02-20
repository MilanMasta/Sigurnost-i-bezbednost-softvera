using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using Contracts;
using Manager;

namespace MonitoringServiceApp
{
    class Program
    {
        static void Main(string[] args)
        {

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            string address = "net.tcp://localhost:9000/Monitor";
            ServiceHost host = new ServiceHost(typeof(MonitoringService));
            host.AddServiceEndpoint(typeof(IMonitoringContract), binding, address);

            try
            {
                host.Open();
                Console.WriteLine("Monitoring service is started.\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
