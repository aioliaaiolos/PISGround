namespace PIS.Ground.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Text;

    /// <summary>
    /// Session service
    /// </summary>
    [ServiceContract(Namespace = "http://alstom.com/pacis/pis/ground/session/")]
    public interface ISessionService
    {
        /// <summary>
        /// Login to service
        /// </summary>
        /// <param name="username">user name </param>
        /// <param name="password">user password</param>
        /// <returns>session id</returns>
        [OperationContract]
        Guid Login(string username, string password);

        /// <summary>
        /// Logoff from service
        /// </summary>
        /// <param name="sessionId">valid session id</param>
        /// <returns>true if success else false</returns>
        [OperationContract]
        bool Logout(Guid sessionId);

        /// <summary>
        /// set the notification url
        /// </summary>
        /// <param name="sessionId">session id</param>
        /// <param name="notificationURL">notificaion url</param>
        /// <returns>true if success else false</returns>
        [OperationContract]
        bool SetNotificationInformation(Guid sessionId, string notificationURL);

        /// <summary>
        /// Used to Validate Session
        /// </summary>
        /// <param name="sessionId">input session Id</param>
        /// <returns>if Valid return true else false</returns>
        [OperationContract]
        bool IsSessionValid(Guid sessionId);
    }
}
