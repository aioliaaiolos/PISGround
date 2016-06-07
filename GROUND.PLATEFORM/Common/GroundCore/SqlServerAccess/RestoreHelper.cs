// <copyright file="RestoreHelper.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
namespace PIS.Ground.Core.SqlServerAccess
{
    using System;
    using System.Data.SqlClient;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;

    /// <summary>
    /// Class for Restoring database
    /// </summary>
    public class RestoreHelper
    {        
        /// <summary>
        /// Initializes a new instance of the RestoreHelper class.
        /// </summary>
        public RestoreHelper()
        {
        }

        /// <summary>
        /// Event PercentComplete decleration
        /// </summary>
        public event EventHandler<PercentCompleteEventArgs> PercentComplete;

        /// <summary>
        /// Event Complete decleration
        /// </summary>
        public event EventHandler<ServerMessageEventArgs> Complete;

        /// <summary>
        /// Method used to restore the database
        /// </summary>
        /// <param name="databaseName">database name</param>
        /// <param name="filePath">file path of the database</param>
        /// <param name="connectionString">sql connection</param>
        /// <param name="dataFilePath">mdf file path</param>
        /// <param name="logFilePath">ldf file path</param>
        /// <param name="dataBaseFileName">database file name of the restored.</param>
        public void RestoreDatabase(string databaseName, string filePath, SqlConnection connectionString, string dataFilePath, string logFilePath, string dataBaseFileName)
        {
            Restore sqlRestore = new Restore();

            BackupDeviceItem deviceItem = new BackupDeviceItem(filePath, DeviceType.File);
            sqlRestore.Devices.Add(deviceItem);
            sqlRestore.Database = databaseName;

            ServerConnection connection = new ServerConnection(connectionString);
            Server sqlServer = new Server(connection);

            Database db = sqlServer.Databases[databaseName];
            sqlRestore.Action = RestoreActionType.Database;
            string dataFileLocation = dataFilePath + dataBaseFileName + ".mdf";
            string logFileLocation = logFilePath + dataBaseFileName + "_Log.ldf";
            db = sqlServer.Databases[databaseName];

            sqlRestore.RelocateFiles.Add(new RelocateFile(databaseName, dataFileLocation));
            sqlRestore.RelocateFiles.Add(new RelocateFile(databaseName + "_log", logFileLocation));
            sqlRestore.ReplaceDatabase = true;
            sqlRestore.Complete += new ServerMessageEventHandler(this.SqlRestoreComplete);
            sqlRestore.PercentCompleteNotification = 10;
            sqlRestore.PercentComplete += new PercentCompleteEventHandler(this.SqlRestorePercentComplete);

            sqlRestore.SqlRestore(sqlServer);

            //db = sqlServer.Databases[databaseName];

            //db.SetOnline();

            sqlServer.Refresh();
        }

        /// <summary>
        /// raises to notify the complete percentage
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">PercentCompleteEventArgs argument</param>
        private void SqlRestorePercentComplete(object sender, PercentCompleteEventArgs e)
        {
            if (this.PercentComplete != null)
            {
                this.PercentComplete(sender, e);
            }
        }

        /// <summary>
        /// Event fired on complete
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">ServerMessageEventArgs argument</param>
        private void SqlRestoreComplete(object sender, ServerMessageEventArgs e)
        {
            if (this.Complete != null)
            {
                this.Complete(sender, e);
            }
        }
    }
}
