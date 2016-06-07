// <copyright file="DataCopyHelper.cs" company="Alstom Transport Telecite Inc.">
// Copyright by Alstom Transport Telecite Inc. 2011.  All rights reserved.
// 
// The informiton contained herein is confidential property of Alstom
// Transport Telecite Inc.  The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Telecite Inc.
namespace PIS.Ground.Core.SqlServerAccess
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.SqlServer.Management.Common;
    using Microsoft.SqlServer.Management.Smo;

    /// <summary>
    /// Class for Data copy within database
    /// </summary>
    public class DataCopyHelper
    {
        /// <summary>
        /// Variable to hold the sql server
        /// </summary>
        private Server sqlServer;

        /// <summary>
        /// Initializes a new instance of the DataCopyHelper class.
        /// </summary>
        /// <param name="serverName">name of the server</param>
        /// <param name="userName">user name of the database</param>
        /// <param name="password">password of the database</param>
        public DataCopyHelper(string serverName, string userName, string password)
        {
            this.sqlServer = new Server(new ServerConnection(serverName, userName, password));
        }

        /// <summary>
        /// Method is used to copy data from Source to Destination
        /// </summary>
        /// <param name="sourceDatabase">Source database name</param>
        /// <param name="destinationDatabase">Destination database name</param>
        public void CopyData(string sourceDatabase, string destinationDatabase)
        {
            Database dataBaseSource = this.sqlServer.Databases[sourceDatabase];
            Database dataBaseDestination = this.sqlServer.Databases[destinationDatabase];

            if (dataBaseDestination == null || dataBaseSource == null)
            {
                throw new Exception("Specified Database not found the server " + this.sqlServer.Name);
            }

            StringBuilder sqlScript = new StringBuilder(string.Empty);
            sqlScript.AppendLine("USE " + destinationDatabase + ";");
            sqlScript.AppendLine(string.Empty);

            foreach (Table dataTable in dataBaseSource.Tables)
            {
                if (!dataBaseDestination.Tables.Contains(dataTable.Name, dataTable.Schema))
                {
                    continue;
                }

                sqlScript.AppendFormat("INSERT INTO {0} \n SELECT * FROM {0}", dataTable.Name);
                sqlScript.AppendLine();
            }

            dataBaseDestination.ExecuteNonQuery(sqlScript.ToString());
        }

        /// <summary>
        /// Method used to Trancate data of the tables of a database
        /// </summary>
        /// <param name="tableNames">table names</param>
        /// <param name="databaseName"> database name</param>
        public void TrancateData(string[] tableNames, string databaseName)
        {
            StringBuilder sqlScript = new StringBuilder(string.Empty);
            sqlScript.AppendFormat("USE {0};", databaseName);
            sqlScript.AppendLine();
            foreach (string tableName in tableNames)
            {
                sqlScript.AppendFormat("TRUNCATE TABLE {0}", tableName);
                sqlScript.AppendLine();
            }

            Database db = this.sqlServer.Databases[databaseName];
            db.ExecuteNonQuery(sqlScript.ToString());
        }

        /// <summary>
        /// Method used to Trancate a database
        /// </summary>
        /// <param name="databaseName">database name</param>
        public void TruncateDatabase(string databaseName)
        {
            Database db = this.sqlServer.Databases[databaseName];
            if (db == null)
            {
                throw new Exception("Specified Database not found the server " + this.sqlServer.Name);
            }

            List<string> tables = new List<string>();
            foreach (Table dataTable in db.Tables)
            {
                tables.Add(dataTable.Name);
            }

            this.TrancateData(tables.ToArray(), databaseName);
        }
    }
}
