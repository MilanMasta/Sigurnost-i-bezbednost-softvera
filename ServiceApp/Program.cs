using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using System.ServiceModel.Security;
using Manager;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Org.BouncyCastle.Crypto;

namespace ServiceApp
{
	public class Program
	{
		static void Main(string[] args)
		{
            /// srvCertCN.SubjectName should be set to the service's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string srvCertCN = "wcfservice";

            NetTcpBinding binding = new NetTcpBinding();

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            var cert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, "TestingCA");
            if (cert == null)
            {
                WCFService.certCA = CertManager.GenerateCACertificate("CN=TestingCA");
            }
            WCFService.certCA = CertManager.ReadAsymmetricKeyParameter();

            string address = "net.tcp://localhost:9999/Receiver";
            ServiceHost host = new ServiceHost(typeof(WCFService));
            host.AddServiceEndpoint(typeof(IWCFContract), binding, address);

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            //host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

            /////If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            //host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /////Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            //host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            try
            {
                host.Open();
                Console.WriteLine("WCFService is started.\nPress <enter> to stop ...");
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
