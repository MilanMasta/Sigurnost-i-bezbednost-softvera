using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;

namespace ClientApp
{
    class CommunicationClient : ChannelFactory<ICommunication>, ICommunication, IDisposable
    {
        ICommunication factory;

        public CommunicationClient(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            var cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate =
                CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }
        public void SendMessage(string msg)
        {
            try
            {
                factory.SendMessage(msg);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
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
    }
}
