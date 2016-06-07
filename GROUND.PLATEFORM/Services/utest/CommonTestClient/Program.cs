using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonTestClient.CommonService;

namespace CommonTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonServiceClient client = new CommonServiceClient();

            // Utilisez la variable « client » pour appeler des opérations sur le service.

            Console.WriteLine("Calling The Service ...");
            client.Login("Admin", "Admin");
            // Fermez toujours le client.
            /*Guid tmp = Guid.NewGuid();
            TransferInfo lInfo = client.GetTransferInformation(tmp, 34);
            if (null != lInfo)
            {
                Console.WriteLine("FTP server IP: " + lInfo.FtpServerIP);
                Console.WriteLine("FTP server Port: " + lInfo.FtpPortNumber);
                Console.WriteLine("FTP username: " + lInfo.FtpUsername);
                Console.WriteLine("FTP password: " + lInfo.FtpPassword);
                Console.WriteLine("FTP folder Id: " + lInfo.FolderId);
            }*/
            client.Close();
            while (true) ;
        }
    }
}
