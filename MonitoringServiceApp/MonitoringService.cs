using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;

namespace MonitoringServiceApp
{
    class MonitoringService: IMonitoringContract
    {
        public void SendMessageToLogs(string message, byte[] sign)
        {
            throw new NotImplementedException();
        }

        public void TestCommunication()
        {
            Console.WriteLine("Hello from monitoring service");
        }
    }
}
