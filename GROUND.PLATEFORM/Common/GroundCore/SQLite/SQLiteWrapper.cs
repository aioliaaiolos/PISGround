/// 
namespace PIS.Ground.Core.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Globalization;
    using System.Text;

    public class SQLiteWrapperClass : IDisposable
    {
        SQLiteConnection aDbConnection;
        bool disposed;

        /// <summary>
        ///     Default Constructor for SQLiteWrapper Class.
        /// </summary>
        public SQLiteWrapperClass()
        {
            aDbConnection = new SQLiteConnection("Data Source=db.sqlite");
        }

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="pInputFile">The File containing the DB</param>
        public SQLiteWrapperClass(string pInputFile)
        {
            try
            {
                aDbConnection = new SQLiteConnection(string.Format(CultureInfo.InvariantCulture, "Data Source={0}", pInputFile));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        ///     Single Param Constructor for specifying advanced connection options.
        /// </summary>
        /// <param name="pConnectionOpts">A dictionary containing all desired options and their values</param>
        public SQLiteWrapperClass(Dictionary<string, string> pConnectionOpts)
        {
            string lStr = "";
            foreach (KeyValuePair<string, string> lRow in pConnectionOpts)
            {
                lStr += string.Format(CultureInfo.InvariantCulture, "{0}={1}; ", lRow.Key, lRow.Value);
            }

            lStr = lStr.Trim().Substring(0, lStr.Length - 1);
            aDbConnection = new SQLiteConnection(lStr);
        }

        /// <summary>
        /// Execute the Query which needs Transactions
        /// </summary>
        /// <param name="plstSql">list of queries to be executed</param>
        /// <returns>true if the transactions is committed else false</returns>
        public void mExecuteTransactionQuery(List<string> plstSql)
        {
            SQLiteTransaction trans = null;
            try
            {
                aDbConnection.Open();
                trans = aDbConnection.BeginTransaction();
                foreach (string SqlQry in plstSql)
                {
                    SQLiteCommand lMycommand = new SQLiteCommand(aDbConnection);
                    lMycommand.CommandText = SqlQry;
                    lMycommand.ExecuteNonQuery();
                }

                trans.Commit();
            }
            catch (SQLiteException lSQLiteException)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }

                throw new Exception(lSQLiteException.ErrorCode.ToString(), lSQLiteException);
            }
            finally
            {
                aDbConnection.Close();
            }
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="pSql">The SQL to run</param>
        /// <param name="pDt">The DataTable that will get the result</param>
        public void mExecuteQuery(string pSql, DataTable pDt)
        {
            try
            {
                aDbConnection.Open();
				using (SQLiteCommand lMycommand = new SQLiteCommand(aDbConnection))
				{
					lMycommand.CommandText = pSql;
					using (SQLiteDataReader lReader = lMycommand.ExecuteReader())
					{
						pDt.Load(lReader);
					}
				}
            }
            catch (SQLiteException lSQLiteException)
            {
                throw new Exception(lSQLiteException.ErrorCode.ToString(), lSQLiteException);
            }
            finally
            {
                aDbConnection.Close();
            }
        }

        /// <summary>
        ///     Allows the programmer to interact with the database with sql query.
        /// </summary>
        /// <param name="pSql">The SQL to be run.</param>
        public void mExecuteNonQuery(string pSql)
        {
            try
            {
                aDbConnection.Open();
				using (SQLiteCommand lMycommand = new SQLiteCommand(aDbConnection))
				{
					lMycommand.CommandText = pSql;
					lMycommand.ExecuteNonQuery();
				}
            }
            catch (SQLiteException lSQLiteException)
            {
                throw new Exception(lSQLiteException.ErrorCode.ToString(), lSQLiteException);
            }
            finally
            {
                aDbConnection.Close();
            }
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="pSql">The query to run.</param>
        /// <param name="pValue">The string that will get the result</param>
        public void mExecuteScalar(string pSql, ref string pValue)
        {
            try
            {
                aDbConnection.Open();
				using (SQLiteCommand lMycommand = new SQLiteCommand(aDbConnection))
				{
					lMycommand.CommandText = pSql;
					object lValue = lMycommand.ExecuteScalar();
					if (lValue != null)
					{
						pValue = lValue.ToString();
					}
				}
            }
            catch (SQLiteException lSQLiteException)
            {
                throw new Exception(lSQLiteException.ErrorCode.ToString(), lSQLiteException);
            }
            finally
            {
                aDbConnection.Close();
            }
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="pTableName">The table to update.</param>
        /// <param name="pData">A dictionary containing Column names and their new values.</param>
        /// <param name="pWhere">The where clause for the update statement.</param>
        public void mUpdate(string pTableName, Dictionary<string, string> pData, string pWhere)
        {
            if (pData == null || pData.Count < 1)
            {
                throw new ArgumentNullException("pData");
            }

            StringBuilder lVals = new StringBuilder(20 * pData.Count);

            bool isFirst = true;
            foreach (KeyValuePair<string, string> lVal in pData)
            {
                if (!isFirst)
                {
                    lVals.AppendLine(",");
                }
                else
                {
                    isFirst = false;
                }

                lVals.Append(lVal.Key);
                lVals.Append(" = '");
                lVals.Append(lVal.Value.Replace("'", "''"));
                lVals.Append("'");
            }

            this.mExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, "update {0} set {1} where {2};", pTableName, lVals.ToString(), pWhere));
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="pTableName">The table from which to delete.</param>
        /// <param name="pWhere">The where clause for the delete.</param>
        public void mDelete(string pTableName, string pWhere)
        {
            this.mExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, "delete from {0} where {1};", pTableName, pWhere));
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="pTableName">The table into which we insert the pData.</param>
        /// <param name="pData">A dictionary containing the column names and pData for the insert.</param>
        public void mInsert(string pTableName, Dictionary<string, string> pData)
        {
            string lColumns = "";
            string lValues = "";

            foreach (KeyValuePair<string, string> lVal in pData)
            {
                lColumns += System.String.Format(CultureInfo.InvariantCulture, " {0},", lVal.Key);
                lValues += System.String.Format(CultureInfo.InvariantCulture, " '{0}',", lVal.Value.Replace("'", "''"));
            }

            lColumns = lColumns.Substring(0, lColumns.Length - 1);
            lValues = lValues.Substring(0, lValues.Length - 1);

            try
            {
                this.mExecuteNonQuery(System.String.Format(CultureInfo.InvariantCulture, "insert into {0}({1}) values({2});", pTableName, lColumns, lValues));
            }
            catch (SQLiteException lSQLiteException)
            {
                throw new Exception(lSQLiteException.ErrorCode.ToString(), lSQLiteException);
            }
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        public void mClearDB()
        {
            using (DataTable lTables = new DataTable())
            {
                lTables.Locale = CultureInfo.InvariantCulture;
                this.mExecuteQuery("select NAME from SQLITE_MASTER where type='table' order by NAME;", lTables);

                foreach (DataRow lTable in lTables.Rows)
                {
                    this.mClearTable(lTable["NAME"].ToString());
                }
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="pTable">The name of the table to clear.</param>
        public void mClearTable(string pTable)
        {
            this.mExecuteNonQuery(System.String.Format(CultureInfo.InvariantCulture, "delete from {0};", pTable));
        }

        /// <summary>
        ///     Allows the user to test if an entry already exists.
        /// </summary>
        /// <param name="pTable">The name of the table to clear.</param>
        /// <param name="pEntry">A dictionary containing the column names and pData to look for.</param>
        /// <returns>A boolean if the entry already exists or not.</returns>
        public bool mEntryExists(string pTable, Dictionary<String, String> pEntry)
        {
            string lQuery = string.Empty;
            using (DataTable lDt = new DataTable())
            {
                lDt.Locale = CultureInfo.InvariantCulture;
                foreach (KeyValuePair<String, String> lVal in pEntry)
                {
                    lQuery += System.String.Format(CultureInfo.InvariantCulture, " {0} = '{1}' AND", lVal.Key, lVal.Value.Replace("'", "''"));
                }

                lQuery = lQuery.Substring(0, lQuery.Length - 3);

                this.mExecuteQuery(System.String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE {1}", pTable, lQuery), lDt);

                return (lDt.Rows.Count != 0);
            }
        }

        /// <summary>
        ///  Use SQLiteConnection tool to create a new db file
        /// </summary>
        /// <param name="pFile">The new file path</param>
        public void mCreateFile(string pFile)
        {
            if (!System.IO.File.Exists(pFile))
            {
                SQLiteConnection.CreateFile(pFile);
            }
        }

        #region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (aDbConnection != null)
                    {
                        aDbConnection.Dispose();
                    }
                }
                //// Indicate that the instance has been disposed.
                disposed = true;
            }
        }
        #endregion
    }
}
