using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using InfotainmentJournalingTestClient.InfotainmentJournalingClientService;

namespace InfotainmentJournalingTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (InfotainmentJournalingServiceClient lClient = new InfotainmentJournalingServiceClient())
            {
                try
                {
                    lClient.GetReport(Guid.NewGuid(), "TRAIN1", 1000 );
                    lClient.
                }
                finally
                {
                    if (lClient.State == CommunicationState.Faulted)
                    {
                        lClient.Abort();
                    }
                }
            }
        }
    }
}
