using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

[ServiceContract]
public interface ICommonService
{
    [OperationContract]
    Guid Login(string Username, string Password);

    [OperationContract]
    void Logout(Guid SessionIdentifier);

    [OperationContract]
    int SetNotificationInformation(Guid SessionId, string NotificationURL);

    [OperationContract]
    List<string> GetListOfElements(Guid SessionId);

    [OperationContract]
    TransferInfo GetTransferInformation(Guid SessionId, int FolderId);
 
}

// Use a data contract as illustrated in the sample below to add composite types to service operations.
[DataContract]
public class TransferInfo
{
    private int folderId;
    private string ftpServerIP = "127.0.0.1";
    private ushort ftpPortNumber = 21;
    private string ftpUsername = "admin";
    private string ftpPassword = "admin";

    [DataMember(Order=0)]
    public int FolderId
    {
        get { return folderId; }
        set { folderId = value; }
    }

    [DataMember(Order = 1)]
    public string FtpServerIP
    {
        get { return ftpServerIP; }
        set { ftpServerIP = value; }
    }

    [DataMember(Order = 2)]
    public ushort FtpPortNumber
    {
        get { return ftpPortNumber; }
        set { ftpPortNumber = value; }
    }

    [DataMember(Order = 3)]
    public string FtpUsername
    {
        get { return ftpUsername; }
        set { ftpUsername = value; }
    }

    [DataMember(Order = 4)]
    public string FtpPassword
    {
        get { return ftpPassword; }
        set { ftpPassword = value; }
    }
}
