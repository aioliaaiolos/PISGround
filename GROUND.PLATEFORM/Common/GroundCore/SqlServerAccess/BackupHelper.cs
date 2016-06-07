// <copyright file="BackupHelper.cs" company="Alstom Transport Telecite Inc.">
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
    /// Class for backuping database
    /// </summary>
    internal sealed class BackupHelper
    {
        /// <summary>
        /// Prevents a default instance of the BackupHelper class from being created.
        /// </summary>
        private BackupHelper()
        {
        }

        /// <summary>
        /// Method used to backup database
        /// </summary>
        /// <param name="databaseName">name of the database</param>
        /// <param name="connectionString">sql connection</param>
        /// <param name="destinationPath">path to which database will be archived</param>
		public static void BackupDatabase(string databaseName, SqlConnection connectionString, string destinationPath)
		{
			Backup sqlBackup = new Backup();

			sqlBackup.Action = BackupActionType.Database;
			sqlBackup.BackupSetDescription = "ArchiveDataBase:" + DateTime.Now.ToShortDateString();
			sqlBackup.BackupSetName = "Archive";

			sqlBackup.Database = databaseName;

			BackupDeviceItem deviceItem = new BackupDeviceItem(destinationPath, DeviceType.File);
			ServerConnection connection = new ServerConnection(connectionString);
			Server sqlServer = new Server(connection);

			sqlBackup.Initialize = true;
			sqlBackup.Checksum = true;
			sqlBackup.ContinueAfterError = true;

			sqlBackup.Devices.Add(deviceItem);
			sqlBackup.Incremental = false;

			sqlBackup.ExpirationDate = DateTime.Now.AddDays(3);
			sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;

			sqlBackup.FormatMedia = false;

			sqlBackup.SqlBackup(sqlServer);
			SqlConnection.ClearAllPools();
		}
    }
}
