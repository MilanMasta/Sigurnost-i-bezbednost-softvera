using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Contracts;
using Manager;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace ClientApp
{
	public class WCFCertIssuerSvcClient : ChannelFactory<IWCFContract>, IWCFContract, IDisposable
	{
		IWCFContract factory;

		public WCFCertIssuerSvcClient(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            var cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            factory = this.CreateChannel();
        }

        public void IssueCertificate()
        {
            try
            {
                factory.IssueCertificate();
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
            }
        }

        public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

        public Dictionary<string, int> GetAllActiveUsers()
        {
            try
            {
                return factory.GetAllActiveUsers();
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
            }
            return null;
        }

        public void Register(int port)
        {
            try
            {   
                factory.Register(port);
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
            }
        }
    }
}
