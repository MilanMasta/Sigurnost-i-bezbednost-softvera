using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using Manager;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.ServiceModel;

namespace ServiceApp
{
	public class WCFService : IWCFContract
	{
       

        public void SendMessage(string message, byte[] sign)
        {
        }

        public void TestCommunication()
		{
			Console.WriteLine("Communication established.");
		}

        public void IssueCertificate()
        {
            var certCA = CertManager.GenerateCACertificate("CN=TestingCA");
            var cert = CertManager.GenerateSelfSignedCertificate("CN=Hekki", "CN=TestingCA", certCA);

            CertManager.ExportCertificate(cert);
        }

        public void IssueMonitoringPassword()
        {
            throw new NotImplementedException();
        }

        public string GetMonitoringPassword()
        {
            throw new NotImplementedException();
        }
    }
}
