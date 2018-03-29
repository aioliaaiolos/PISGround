// <copyright file="SqlHelper.cs" company="Alstom Transport Telecite Inc.">
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
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Xml;

    /// <summary>
    /// This enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that
    /// we can set the appropriate CommandBehavior when calling ExecuteReader()
    /// </summary>
    internal enum SqlConnectionOwnership
    {
        /// <summary>Connection is owned and managed by SqlHelper</summary>
        Internal,

        /// <summary>Connection is owned and managed by the caller</summary>
        External
    }

    /// <summary>
    /// The SqlHelper class is intended to encapsulate high performance, scalable best practices for 
    /// common uses of SqlClient.
    /// </summary>
    public sealed class SqlHelper
    {
        #region private utility methods & constructors

        /// <summary>
        /// Prevents a default instance of the SqlHelper class from being created, Since this class provides only static methods, make the default constructor private to prevent 
        /// instances from being created with "new SqlHelper()".
        /// </summary>
        private SqlHelper()
        {
        }

        #endregion private constructors
        /// <summary>
        /// Method to backup the database
        /// </summary>
        /// <param name="databaseName">name of the database to backup</param>
        /// <param name="connectionString">connection string</param>
        /// <param name="destinationPath">destination bath of backup</param>
        /// <returns>true if sucess else false</returns>
        public static bool BackupDatabase(string databaseName, string connectionString, string destinationPath)
        {
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				BackupHelper.BackupDatabase(databaseName, cn, destinationPath);
			}

            return true;
        }

        /// <summary>
        /// Method to restore the database
        /// </summary>
        /// <param name="databaseName">name of the database to backup</param>
        /// <param name="archveDatabasePath"> Archive database path</param>
        /// <param name="connectionString">connection string</param>
        /// <param name="destinationPath">destination path of database mdf file</param>
        /// <param name="destinationLogPath">destination path of database ldf file</param>
        /// <param name="dataBaseFileName">database file name of the restored.</param>
        /// <returns>true if success else false</returns>
        public static bool RestoreDatabase(string databaseName, string archveDatabasePath, string connectionString, string destinationPath, string destinationLogPath, string dataBaseFileName)
        {
			using (SqlConnection cn = new SqlConnection(connectionString))
			{
				RestoreHelper restore = new RestoreHelper();
				restore.RestoreDatabase(databaseName, archveDatabasePath, cn, destinationPath, destinationLogPath, dataBaseFileName);
			}

            return true;
        }
        #region ExecuteNonQuery

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                ////call the overload that takes a connection in place of the connection string
                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored prcedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }            
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

				////finally, execute the command.
                int retval = 0;
                try
                {
                    retval = cmd.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    string msg = e.Message;
                    msg = msg;
                }

				//// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();
				return retval;
			}
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connection.ConnectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

				////finally, execute the command.
				int retval = cmd.ExecuteNonQuery();

				//// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();
				return retval;
			}
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified 
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction transaction, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(transaction.Connection.ConnectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        #endregion ExecuteNonQuery

        #region ExecuteDataSet

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteDataSet(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                ////call the overload that takes a connection in place of the connection string
                return ExecuteDataSet(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(string connectionString, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteDataSet(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteDataSet(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(SqlConnection connection, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteDataSet(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

				////create the DataAdapter & DataSet
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				ds.Locale = CultureInfo.CurrentCulture;
				cmd.CommandTimeout = 400;
				////fill the DataSet using default values for DataTable names, etc.
				da.Fill(ds);

				//// detach the SqlParameters from the command object, so they can be used again.            
				cmd.Parameters.Clear();

				////return the dataset
				return ds;
			}
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(SqlConnection connection, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connection.ConnectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteDataSet(connection, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteDataSet(connection, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteDataSet(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

				////create the DataAdapter & DataSet
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				ds.Locale = CultureInfo.CurrentCulture;
				////fill the DataSet using default values for DataTable names, etc.
				da.Fill(ds);

				//// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();

				////return the dataset
				return ds;
			}
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static DataSet ExecuteDataSet(SqlTransaction transaction, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(transaction.Connection.ConnectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteDataSet(transaction, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteDataSet(transaction, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        #endregion ExecuteDataSet

        #region ExecuteReader

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
        /*public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
        }*/

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
        /*public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create & open a SqlConnection
            SqlConnection cn = new SqlConnection(connectionString);
            cn.Open();

            try
            {
                ////call the private overload that takes an internally owned connection in place of the connection string
                return ExecuteReader(cn, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                ////if we fail to return the SqlDatReader, we need to close the connection ourselves
                cn.Close();
                throw;
            }
        }*/

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
/*        public static SqlDataReader ExecuteReader(string connectionString, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteReader(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteReader(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }*/

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////pass through the call to the private overload using a null transaction value and an externally owned connection
            return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connection.ConnectionString, storedProcedureName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(connection, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteReader(connection, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///   SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////pass through to private overload, indicating that the connection is owned by the caller
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  SqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a SqlDataReader containing the resultset generated by the command</returns>
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(transaction.Connection.ConnectionString, storedProcedureName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteReader(transaction, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteReader(transaction, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        #endregion ExecuteReader

        #region ExecuteScalar

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in 
        /// the connection string. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create & open a SqlConnection, and dispose of it after we are done.
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                ////call the overload that takes a connection in place of the connection string
                return ExecuteScalar(cn, commandType, commandText, commandParameters);
            }
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the database specified in 
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(string connectionString, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteScalar(connectionString, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

				////execute the command & return the results
				object retval = cmd.ExecuteScalar();

				//// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();
				return retval;
			}
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(SqlConnection connection, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connection.ConnectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteScalar(connection, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteScalar(connection, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount");
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

				////execute the command & return the results
				object retval = cmd.ExecuteScalar();

				//// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();
				return retval;
			}
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an object containing the value in the 1x1 resultset generated by the command</returns>
        public static object ExecuteScalar(SqlTransaction transaction, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(transaction.Connection.ConnectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteScalar(transaction, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteScalar(transaction, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        #endregion ExecuteScalar

        #region ExecuteXmlReader

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

				////create the DataAdapter & DataSet
				XmlReader retval = cmd.ExecuteXmlReader();

				//// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();
				return retval;
			}
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection 
        /// using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="connection">a valid SqlConnection</param>
        /// <param name="storedProcedureName">the name of the stored procedure using "FOR XML AUTO"</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(SqlConnection connection, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(connection.ConnectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else
            {
                ////otherwise we can just call the SP without params
                return ExecuteXmlReader(connection, CommandType.StoredProcedure, storedProcedureName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders");
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            ////pass through the call providing null for the set of SqlParameters
            return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command using "FOR XML AUTO"</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an XmlReader containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

				////create the DataAdapter & DataSet
				XmlReader retval = cmd.ExecuteXmlReader();

				//// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();
				return retval;
			}
        }

        /// <summary>
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the 
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order.
        /// </summary>
        /// <remarks>
        /// This method provides no access to output parameters or the stored procedure's return value parameter.
        /// e.g.:  
        ///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36);
        /// </remarks>
        /// <param name="transaction">a valid SqlTransaction</param>
        /// <param name="storedProcedureName">the name of the stored procedure</param>
        /// <param name="parameterValues">an array of objects to be assigned as the input values of the stored procedure</param>
        /// <returns>a dataset containing the resultset generated by the command</returns>
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string storedProcedureName, List<object> parameterValues)
        {
            ////if we receive parameter values, we need to figure out where they go
            if ((parameterValues != null) && (parameterValues.Count > 0))
            {
                ////pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache)
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetStoredProcedureParameterSet(transaction.Connection.ConnectionString, storedProcedureName);

                ////assign the provided values to these parameters based on parameter order
                AssignParameterValues(commandParameters, parameterValues);

                ////call the overload that takes an array of SqlParameters
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, storedProcedureName, commandParameters);
            }
            else 
            {
                ////otherwise we can just call the SP without params
                return ExecuteXmlReader(transaction, CommandType.StoredProcedure, storedProcedureName);
            }
        }
        #endregion ExecuteXmlReader

        #region private ExecuteReader

        /// <summary>
        /// Create and prepare a SqlCommand, and call ExecuteReader with the appropriate CommandBehavior.
        /// </summary>
        /// <remarks>
        /// If we created and opened the connection, we want the connection to be closed when the DataReader is closed.
        /// If the caller provided the connection, we want to leave it to them to manage.
        /// </remarks>
        /// <param name="connection">a valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">a valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        /// <param name="connectionOwnership">indicates whether the connection parameter was provided by the caller, or created by SqlHelper</param>
        /// <returns>SqlDataReader containing the results of the command</returns>
        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            ////create a command and prepare it for execution
			using (SqlCommand cmd = new SqlCommand())
			{
				PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters);

				////create a reader
				SqlDataReader dr;

				//// call ExecuteReader with the appropriate CommandBehavior
				if (connectionOwnership == SqlConnectionOwnership.External)
				{
					dr = cmd.ExecuteReader();
				}
				else
				{
					dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
				}

				//// detach the SqlParameters from the command object, so they can be used again.
				cmd.Parameters.Clear();

				return dr;
			}
        }
        #endregion private ExecuteReader

        #region private utility methods & constructors

        /// <summary>
        /// This method is used to attach array of SqlParameters to a SqlCommand.
        /// This method will assign a value of DbNull to any parameter with a direction of
        /// InputOutput and a value of null.  
        /// This behavior will prevent default values from being used, but
        /// this will be the less common case than an intended pure output parameter (derived as InputOutput)
        /// where the user provided no input value.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">an array of SqlParameters tho be added to command</param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                ////check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        /// <summary>
        /// This method assigns an array of values to an array of SqlParameters.
        /// </summary>
        /// <param name="commandParameters">array of SqlParameters to be assigned values</param>
        /// <param name="parameterValues">array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, List<object> parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                ////do nothing if we get no data
                return;
            }

            ////we must have the same number of values as we pave parameters to put them in
            if (commandParameters.Length != parameterValues.Count)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            ////iterate through the SqlParameters, assigning the values from the corresponding position in the 
            ////value array
            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                commandParameters[i].Value = parameterValues[i];
            }
        }

        /// <summary>
        /// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
        /// to the provided command.
        /// </summary>
        /// <param name="command">the SqlCommand to be prepared</param>
        /// <param name="connection">a valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">a valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            ////if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            ////associate the connection with the command
            command.Connection = connection;

            ////set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            ////if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            ////set the command type
            command.CommandType = commandType;

            ////attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }

        #endregion private utility methods & constructors
    }
}