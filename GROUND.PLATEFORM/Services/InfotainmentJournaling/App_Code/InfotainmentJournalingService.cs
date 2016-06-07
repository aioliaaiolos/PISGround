using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: If you change the class name "Service" here, you must also update the reference to "Service" in Web.config and in the associated .svc file.
public class InfotainmentJournalingService : IInfotainmentJournalingService
{
    public int GetReport(Guid SessionId, string AddresseeId, int RequestTimeout)
    {
        return 0;
    }
}
