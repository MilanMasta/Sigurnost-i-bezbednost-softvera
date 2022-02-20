using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using Manager;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.ServiceModel;
using Org.BouncyCastle.Crypto;
using System.IO;

namespace ServiceApp
{
	public class WCFService : IWCFContract
	{
        public static AsymmetricKeyParameter certCA;
        private static Dictionary<string, int> activeUsers = new Dictionary<string, int>();
        public Dictionary<string, int> GetAllActiveUsers()
        {
            return activeUsers;
        }

        public void IssueCertificate()
        {
            var principal = OperationContext.Current.ServiceSecurityContext.WindowsIdentity;
            var username = Formatter.ParseName(principal.Name);

            var cert = CertManager.GenerateSelfSignedCertificate($"CN={username}", "CN=TestingCA", certCA);
            CertManager.ExportCertificate(cert);

            // generate AES key
            using (AesManaged aes = new AesManaged())
            {
                File.WriteAllBytes(username + ".key", aes.Key);
                File.WriteAllBytes(username + ".IV", aes.IV);
            }

                // log 
            Logger.Log($"Certificate generated for user {username}.", System.Diagnostics.EventLogEntryType.Information);
        }

        public void Register(int port)
        {
            var principal = OperationContext.Current.ServiceSecurityContext.WindowsIdentity;
            var username = Formatter.ParseName(principal.Name);
            if (activeUsers.ContainsKey(username))
            {
                activeUsers[username] = port;
            }
            else
            {
                activeUsers.Add(username, port);
            }
        }
    }
}
