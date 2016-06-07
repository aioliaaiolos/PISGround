using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: If you change the interface name "IService" here, you must also update the reference to "IService" in Web.config.
[ServiceContract]
public interface IInfotainmentJournalingService
{
    [OperationContract]
    int GetReport(Guid SessionId, string AddresseeId, int RequestTimeout);
}

// Use a data contract as illustrated in the sample below to add composite types to service operations.

