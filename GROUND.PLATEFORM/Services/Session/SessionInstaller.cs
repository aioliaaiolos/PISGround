using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Diagnostics;
using System.Security;
using PIS.Ground.Core.Data;
using PIS.Ground.Core.LogMgmt;
using System.Windows.Forms;
using System.Web.Configuration;


namespace PIS.Ground.Session
{
    /// <summary>
    /// Session Installer Class
    /// </summary>
    [RunInstaller(true)]
    public partial class SessionInstaller : Installer
    {
        /// <summary>
        /// Session Installer Constructor
        /// </summary>
        public SessionInstaller()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Overriding OnCommiting to register PIS with Eventlog
        /// </summary>
        protected override void OnCommitting(IDictionary savedState)
        {
            base.OnCommitting(savedState);
            try
            {
                // Create the PIS source, if it does not already exist. 
                if (!EventLog.SourceExists("PIS"))
                {
                    //An event log source should not be created and immediately used. 
                    //There is a latency time to enable the source, it should be created 
                    //prior to executing the application that uses the source. 
                    EventLog.CreateEventSource("PIS", "Application");
                    // The source is created.  Exit the application to allow it to be registered. 
                }
            }
            catch (SecurityException ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Creation of PIS Source for Eventlog is failed, hence logging the messages to Eventlog is not possible. Manually create PIS Source in registry, refer the user manual for more details.", "Critical Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + "Creation of PIS Source for Eventlog is failed, hence logging the messages to Eventlog is not possible. Manually create PIS Source in registry, refer the user manual for more details.", "Critical Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }        
    }
}
