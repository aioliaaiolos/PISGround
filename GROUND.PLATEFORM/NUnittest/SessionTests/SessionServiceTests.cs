//---------------------------------------------------------------------------------------------------
// <copyright file="SessionServiceTests.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using DataPackageTests.Stubs;
using Moq;
using NUnit.Framework;
using PIS.Ground.Core.SessionMgmt;
using PIS.Ground.Core.T2G;
using PIS.Ground.Core.Utility;

namespace SessionTests
{
	[TestFixture, Category("InstantMessage")]
	public class SessionServiceTests : AssertionHelper
    {
        #region Fields
        /// <summary>The train 2ground client mock.</summary>
        private Mock<IT2GManager> _train2groundClientMock;

        private NotificationSenderStub _notificationSender;

        private SessionManager _sessionManager;

        private string _machineIP;


        #endregion

        #region Test setup management

        /// <summary>
        /// Test fixture setup.
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureInit()
        {
            ConfigurationSettings.AppSettings["DataStorePath"] = Path.Combine(ServiceConfiguration.AppDataPath, "RemoteDataStore");
            ConfigurationSettings.AppSettings["RemoteDataStoreUrl"] = Path.Combine(ServiceConfiguration.AppDataPath, "RemoteDataStore");



            Console.Out.Write("ServiceConfiguration.SessionSqLiteDBPath=\"");
            Console.Out.Write(ServiceConfiguration.SessionSqLiteDBPath);
            Console.Out.WriteLine("\"");

            if (!File.Exists(ServiceConfiguration.SessionSqLiteDBPath))
            {
                throw new Exception("The session database does not exist at this location: \"" + ServiceConfiguration.SessionSqLiteDBPath + "\".");
            }

            // Remove the readonly attribute on session database table if set.
            FileAttributes attributes = File.GetAttributes(ServiceConfiguration.SessionSqLiteDBPath);
            if ((attributes & FileAttributes.ReadOnly) != 0)
            {
                attributes = attributes & (~FileAttributes.ReadOnly);
                File.SetAttributes(ServiceConfiguration.SessionSqLiteDBPath, attributes);
            }

            _machineIP = GetLocalIPAddress();
            Console.Out.Write("Machine.IP=\"");
            Console.Out.Write(_machineIP);
            Console.Out.WriteLine("\"");
        }

        /// <summary>Test fixture cleanup.</summary>
        [TestFixtureTearDown]
        public void MyCleanup()
        {
        }


        /// <summary>Setups called before each test to initialize variables.</summary>
        [SetUp]
        public void Setup()
        {
            TestContext currentContext = TestContext.CurrentContext;

            Console.Out.WriteLine("===================================");
            Console.Out.WriteLine("BEGIN TEST {0}", currentContext.Test.Name);
            Console.Out.WriteLine("===================================");

            _train2groundClientMock = new Mock<IT2GManager>();
            _notificationSender = new NotificationSenderStub(true);
            _sessionManager = new SessionManager();
            Assert.AreEqual(string.Empty, _sessionManager.RemoveAllSessions(), "Cannot remove all sessions in session database");
        }

        /// <summary>Tear down called after each test to clean.</summary>
        [TearDown]
        public void TearDown()
        {
            _train2groundClientMock = new Mock<IT2GManager>();
            T2GManagerContainer.T2GManager = null;

            if (_sessionManager != null)
            {
                _sessionManager.StopMonitoringSessions();
                _sessionManager = null;
            }
            TestContext currentContext = TestContext.CurrentContext;

            Console.Out.WriteLine("===================================");
            Console.Out.WriteLine("END TEST {0}", currentContext.Test.Name);
            Console.Out.WriteLine("===================================");
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip) )
                {
                    return ip.ToString();
                }
            }
            
            throw new Exception("Local IP Address Not Found!");
        }

        #endregion

        #region Tests

        /// <summary>
        /// Perform test on login method.
        /// </summary>
        [Test]
        public void LoginLogoutNominalTest()
        {
            using (SessionServiceStub sessionService = new SessionServiceStub(_sessionManager, _train2groundClientMock.Object, _notificationSender))
            {
                Guid sessionId = sessionService.Login("admin", "admin");
                Assert.AreNotEqual(Guid.Empty, sessionId, "login function of session service didn't returned a valid session id");
                Assert.IsTrue(sessionService.IsSessionValid(sessionId), "session id returned by login function isn't valid");
                Assert.IsTrue(sessionService.Logout(sessionId), "logout function failed while expecting not.");
                Assert.IsFalse(sessionService.IsSessionValid(sessionId), "session id still valid after logout");
            }
        }

        /// <summary>
        /// Verify the function SetNotificationInformation with nominal scenario.
        /// </summary>
        [Test]
        public void SetNotificationInformationNominal()
        {
			GroundAppNotificationServiceStub trainService = new GroundAppNotificationServiceStub();
            string notificationAddressString = "http://127.0.0.1:8200/";
            Uri notificationServiceAddress = new Uri(notificationAddressString);

            using (SessionServiceStub sessionService = new SessionServiceStub(_sessionManager, _train2groundClientMock.Object, _notificationSender))
            using (ServiceHost host = new ServiceHost(trainService, notificationServiceAddress))
            {
                try
                {
                    host.Open();

                    Guid sessionId = sessionService.Login("admin", "admin");
                    Assert.AreNotEqual(Guid.Empty, sessionId, "login function of session service didn't returned a valid session id");
                    Assert.IsTrue(sessionService.IsSessionValid(sessionId), "session id returned by login function isn't valid");
                    Assert.IsTrue(sessionService.SetNotificationInformation(sessionId, notificationAddressString), "SetNotificationInformation of session service didn't succeeded as expected");
                    
                    string storedUrl;
                    Assert.AreEqual(string.Empty, _sessionManager.GetNotificationUrlBySessionId(sessionId, out storedUrl), "GetNotificaitonUrlBySessionId didn't succeeded as expected");
                    StringAssert.AreEqualIgnoringCase(notificationAddressString, storedUrl, "SetNotificationInformation does not persist notification url as expected");
                    host.Close();
                }
                finally
                {
                    if (host.State == CommunicationState.Faulted)
                    {
                        host.Abort();
                    }
                }
            }
        }

        /// <summary>
        /// Verify the function SetNotificationInformation behave as expected when the notification url cannot be reached.
        /// </summary>
        [Test]
        public void SetNotificationInformationUrlNotResponding()
        {
            GroundAppNotificationServiceStub trainService = new GroundAppNotificationServiceStub();
            string notificationAddressString = "http://127.0.0.1:8200/";
            Uri notificationServiceAddress = new Uri(notificationAddressString);

            using (SessionServiceStub sessionService = new SessionServiceStub(_sessionManager, _train2groundClientMock.Object, _notificationSender))
            {
                Guid sessionId = sessionService.Login("admin", "admin");
                Assert.AreNotEqual(Guid.Empty, sessionId, "login function of session service didn't returned a valid session id");
                Assert.IsTrue(sessionService.IsSessionValid(sessionId), "session id returned by login function isn't valid");
                Assert.IsFalse(sessionService.SetNotificationInformation(sessionId, notificationAddressString), "SetNotificationInformation of session service didn't failed as expected");

                string storedUrl;
                Assert.AreEqual(string.Empty, _sessionManager.GetNotificationUrlBySessionId(sessionId, out storedUrl), "GetNotificaitonUrlBySessionId didn't succeeded as expected");
                StringAssert.AreEqualIgnoringCase(string.Empty, storedUrl, "SetNotificationInformation updated the notification url associated to the session while expecting not.");
            }
        }

        /// <summary>
        /// Verify the function SetNotificationInformation still working properly after 6 failures.
        /// </summary>
        /// <remarks>This test verify the fix for CR atvcm00750190 - Manque de robustesse liée à l'échec de notification.
        /// In this CR, the function SetNotificationInformation stop working with valid url after being called 5 times with invalid url.</remarks>
        [Test]
        public void SetNotificationInformationUrlRobustnessAfterFailure_atvcm00750190()
        {
            GroundAppNotificationServiceStub trainService = new GroundAppNotificationServiceStub();
            string notificationAddressInvalidString = "http://" + _machineIP + ":8200/notexist";
            string notificationAddressString = "http://" + _machineIP + ":8200/";
            Uri notificationServiceAddress = new Uri(notificationAddressString);

            using (SessionServiceStub sessionService = new SessionServiceStub(_sessionManager, _train2groundClientMock.Object, _notificationSender))
            {
                Guid sessionId = sessionService.Login("admin", "admin");
                Assert.AreNotEqual(Guid.Empty, sessionId, "login function of session service didn't returned a valid session id");
                Assert.IsTrue(sessionService.IsSessionValid(sessionId), "session id returned by login function isn't valid");

                for (int i = 0; i < 6; ++i)
                {
                    Assert.IsFalse(sessionService.SetNotificationInformation(sessionId, notificationAddressInvalidString), "SetNotificationInformation succeeded with invalid url '{0}'", notificationAddressInvalidString);
                }
                using (ServiceHost host = new ServiceHost(trainService, notificationServiceAddress))
                {
                    try
                    {
                        host.Open();

                        Assert.IsTrue(sessionService.SetNotificationInformation(sessionId, notificationServiceAddress.ToString()), "SetNotificationInformation of session service didn't succeeded as expected");
                        host.Close();
                    }
                    finally
                    {
                        if (host.State == CommunicationState.Faulted)
                        {
                            host.Abort();
                        }
                    }
                }
            }
        }

        #endregion


    }
}
