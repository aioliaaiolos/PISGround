using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using GroundCore.SessionMgmt;
using GroundCore.LogMgmt;

/// <summary>
/// CommonService
/// </summary>
public class CommonService : ICommonService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Username"></param>
    /// <param name="Password"></param>
    /// <returns></returns>
    public Guid Login(string Username, string Password)
    {
        SessionManager objSessionMgr = new SessionManager();
        Guid objGuid;
        try
        {
            objGuid = objSessionMgr.Login(Username, Password);
            if (objGuid.ToString() == "0")
            {
                LogManager.WriteLog(TraceType.ERROR, "Error occured while calling SessionManager.Login() in the method CommonService.Login");
            }
            return objGuid;
        }
        catch
        {
            LogManager.WriteLog(TraceType.ERROR, "Exception occured while calling SessionManager.Login() in the method CommonService.Login");
            return new Guid();
        }
    }

    public void Logout(Guid SessionId)
    {
        SessionManager objSessionMgr = new SessionManager();
        try
        {
            objSessionMgr.RemoveSessionID(SessionId);
        }
        catch
        {
            LogManager.WriteLog(TraceType.ERROR, "Error occured while calling SessionManager.RemoveSessionID() in the method CommonService.Logout");
        }
    }

    public int SetNotificationInformation(Guid SessionId, string NotificationURL)
    {
         SessionManager objSessionMgr = new SessionManager();
         try
         {
             objSessionMgr.AddNotificationURL(SessionId, NotificationURL);            
         }
         catch
         {
             LogManager.WriteLog(TraceType.ERROR, "Error occured while calling SessionManager.AddNotificationURL() in the method CommonService.SetNotificationInformation");
         }
         return 0;
    }

    public List<string> GetListOfElements(Guid SessionId)
    {
        List<string> elementList = new List<string>();
        return elementList;
    }

    public TransferInfo GetTransferInformation(Guid SessionId, int FolderId)
    {
        TransferInfo transferInfo = new TransferInfo();

        transferInfo.FolderId = FolderId;

        return transferInfo;
    }
}
