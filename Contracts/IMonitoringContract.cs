using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Contracts
{
    [ServiceContract]
    public interface IMonitoringContract
    {

        [OperationContract]
        void SendMessageToLogs(byte[] message);
    }
}
