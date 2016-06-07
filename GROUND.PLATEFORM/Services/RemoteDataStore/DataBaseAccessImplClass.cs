using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.ServiceModel;
using PIS.Ground.Core.SQLite;
using System.Data.SQLite;
using System.Globalization;

namespace PIS.Ground.RemoteDataStore
{
    /// <summary>
    /// Interface between DataPackage WebService and DataBase.
    /// </summary>
    public sealed class DatabaseAccessImplClass
    {
        /// <summary>
        /// The instance of SQLLite Wrapper to do query to database.
        /// </summary>
        public SQLiteWrapperClass _SQLWrapper { get; set; }

        /// <summary>
        /// List of databases names.
        /// </summary>
        private string _baselinesDataStore;
        private string _dataPackagesDataStore;
        private string _elementsDataStore;
		private string _baselineDistributingTasksDataStore;

        /// <summary>
        /// The only instance of DataBaseAccessImplClass (thread safe).
        /// </summary>
        public static readonly DatabaseAccessImplClass asInstance = new DatabaseAccessImplClass();

        /// <summary>
        /// Constructor.
        /// Init DB from config file, using sql instructions if needed.
        /// </summary>
        public DatabaseAccessImplClass()
        {
            //Read paths and DB names from app.config
            _baselinesDataStore = ConfigurationSettings.AppSettings["BaselinesDataStore"];
            _dataPackagesDataStore = ConfigurationSettings.AppSettings["DataPackagesDataStore"];
            _elementsDataStore = ConfigurationSettings.AppSettings["ElementsDataStore"];
			_baselineDistributingTasksDataStore = "BaselineDistributingTasksDataStore";
            string lSQLInitFile = ConfigurationSettings.AppSettings["SqlInstructionFile"];
            string lDBFile = ConfigurationSettings.AppSettings["DataBaseFile"];

            _SQLWrapper = new SQLiteWrapperClass();
            string lSQLInstruction = File.Exists(lSQLInitFile) ? File.ReadAllText(lSQLInitFile) : Properties.Resource.InitDataBaseSQLInstruction;

            //Check if a file is define
            if (String.IsNullOrEmpty(lDBFile))
            {
                //If not defined, create a temporary one
                string lDBNewFile = Path.GetTempPath() + "DataBaseRepositoryExample\\DataPackageDB" + Path.GetRandomFileName().Replace(".", "") + ".db";
                _SQLWrapper.mCreateFile(lDBNewFile);
                _SQLWrapper = new PIS.Ground.Core.SQLite.SQLiteWrapperClass(lDBNewFile);
                _SQLWrapper.mExecuteNonQuery(lSQLInstruction);
            }
            else
            {
                //If defined but not exists, create it
                if (!System.IO.File.Exists(lDBFile))
                {
                    _SQLWrapper.mCreateFile(lDBFile);
                    _SQLWrapper = new SQLiteWrapperClass(lDBFile);
                    _SQLWrapper.mExecuteNonQuery(lSQLInstruction);
                }
                else
                {
                    //Else, everything is right
                    _SQLWrapper = new SQLiteWrapperClass(lDBFile);
                    
                    //Check, if "ElementsDataStore" table contains columns which store the undefined baseline packages version
                    // If it doesn't - add these columns to it
                    using (DataTable lElementsTable = new DataTable("Elements"))
                    {
                        lElementsTable.Locale = CultureInfo.InvariantCulture;

                        _SQLWrapper.mExecuteQuery(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} LIMIT 1", _elementsDataStore), lElementsTable);

                        if (!lElementsTable.Columns.Contains("UndefinedBaselinePISBaseVersion"))
                        {
                            string lUpdateTableSQLInstruction = Properties.Resource.UpdateElementsDatastoreSQLInstruction;
                            _SQLWrapper.mExecuteNonQuery(lUpdateTableSQLInstruction);
                        }
                    }
                }
            }

			if (System.IO.File.Exists(lDBFile))
			{
				try
				{
                    using (DataTable lBaselineDistributingTasksTable = new DataTable("BaselineDistributingTasks"))
                    {
                        lBaselineDistributingTasksTable.Locale = CultureInfo.InvariantCulture;
                        _SQLWrapper.mExecuteQuery(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0}", _baselineDistributingTasksDataStore), lBaselineDistributingTasksTable);
                    }
				}
				catch (Exception)
				{
					string lUpdateTableSQLInstruction = Properties.Resource.AddBaselineDistributingTasksDataStoreSQLInstruction;
					_SQLWrapper.mExecuteNonQuery(lUpdateTableSQLInstruction);
				}
			}
        }

		/// <summary>Gets the baselines data store.</summary>
		/// <value>The baselines data store.</value>
		public string BaselinesDataStore
        {
			get { return _baselinesDataStore; }
        }

        /// <summary>Gets the data packages data store.</summary>
        /// <value>The data packages data store.</value>
        public string DataPackagesDataStore
        {
            get { return _dataPackagesDataStore; }
        }

        /// <summary>Gets the elements data store.</summary>
        /// <value>The elements data store.</value>
        public string ElementsDataStore
        {
            get { return _elementsDataStore; }
        }

		/// <summary>Gets the baseline distributing tasks data store.</summary>
		/// <value>The baseline distributing tasks data store.</value>
		public string BaselineDistributingTasksDataStore
		{
			get { return _baselineDistributingTasksDataStore; }
		}

        /// <summary>
        /// This function allows the Remote Data Store to return the baselines definitions list.
        /// </summary>
        /// <returns>The list of baselines already defined in the baselines definitions data store, an empty list when no baselines are defined in the baselines definitions data store.</returns>
        public DataContainer mGetBaselinesDefinitions()
        {
            using (DataTable lBaselinesTable = new DataTable("BaselinesDefinitions"))
            {
                lBaselinesTable.Locale = CultureInfo.InvariantCulture;
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0}", _baselinesDataStore), lBaselinesTable);
                }
                DataContainer lBLContainer = new DataContainer();
                lBLContainer.initFromDataTable(lBaselinesTable);
                return lBLContainer;
            }
        }

        /// <summary>
        /// This function allows the Remote Data Store to return a baseline definition.
        /// </summary>
        /// <param name="pBLVersion">The version number of the baseline to look after.</param>
        /// <returns>The definition of the baseline already defined in the baselines definitions data store. This baseline is referenced by its versions number.</returns>
        public DataContainer mGetBaselineDefinition(string pBLVersion)
        {
            using (DataTable lBaselineTable = new DataTable("BaselineDefinition"))
            {
                lBaselineTable.Locale = CultureInfo.InvariantCulture;
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE BaselineVersion == '{1}'", _baselinesDataStore, pBLVersion), lBaselineTable);
                }
                DataContainer lBLContainer = new DataContainer();
                lBLContainer.initFromDataTable(lBaselineTable);
                return lBLContainer;
            }
        }

        /// <summary>
        /// This function allows the Remote Data Store to return the assigned baselines versions.
        /// </summary>
        /// <returns>The assigned baselines versions.</returns>
        public DataContainer mGetAssignedBaselinesVersions()
        {
            using (DataTable lAssignedBaselinesVersionTable = new DataTable("BaselinesVersions"))
            {
                lAssignedBaselinesVersionTable.Locale = CultureInfo.InvariantCulture;
                lock (_SQLWrapper)
                {
                    string lSqlCommand = "SELECT ElementID, AssignedCurrentBaseline, AssignedFutureBaseline FROM {0} WHERE " +
                        "AssignedCurrentBaseline IS NOT NULL OR " +
                        "AssignedFutureBaseline IS NOT NULL";

                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, lSqlCommand, _elementsDataStore), lAssignedBaselinesVersionTable);
                }

                DataContainer lBLContainer = new DataContainer();
                lBLContainer.initFromDataTable(lAssignedBaselinesVersionTable);

                return lBLContainer;
            }
        }

        /// <summary>
        /// Check if a data package exists in the data base.
        /// </summary>
        /// <param name="pDPType">The DataPackage type as a string.</param>
        /// <param name="pDPVersion">The DataPackage Version.</param>
        /// <returns>A boolean if the package is available.</returns>
        public bool mDataPackageExists(string pDPType, string pDPVersion)
        {
            bool lResult = false;
            Dictionary<String, String> lExistDico = new Dictionary<String, String>();
            lExistDico["DataPackageType"] = pDPType;
            lExistDico["DataPackageVersion"] = pDPVersion;
            lock (_SQLWrapper)
            {
                lResult = _SQLWrapper.mEntryExists(_dataPackagesDataStore, lExistDico);
            }
            return lResult;
        }

        /// <summary>
        /// Check if an Element exists in the data base.
        /// </summary>
        /// <param name="pEID">The Element ID.</param>
        /// <returns>A boolean if the element is available.</returns>>
        public bool mElementExists(string pEID)
        {
            bool lResult = false;
            Dictionary<String, String> lExistDico = new Dictionary<String, String>();
            lExistDico["ElementID"] = pEID;
            lock (_SQLWrapper)
            {
                lResult = _SQLWrapper.mEntryExists(_elementsDataStore, lExistDico);
            }
            return lResult;
        }

        /// <summary>
        /// Check if a baseline exists in the data base.
        /// </summary>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <returns>A boolean if the baseline is available.</returns>
        public bool mBaselineExists(string pBLVersion)
        {
            bool lResult = false;
            Dictionary<String, String> lExistDico = new Dictionary<String, String>();
            lExistDico["BaselineVersion"] = pBLVersion;
            lock (_SQLWrapper)
            {
                lResult = _SQLWrapper.mEntryExists(_baselinesDataStore, lExistDico);
            }
            return lResult;
        }

        /// <summary>
        /// Returns the list of all data packages from Remote Data Store.
        /// </summary>        
        /// <returns>The data packages list.</returns>
        public DataContainer mGetDataPackagesList()
        {
            using (DataTable lDataPackageTable = new DataTable("DataPackagesVersionsList"))
            {
                lDataPackageTable.Locale = CultureInfo.InvariantCulture;

                lock (_SQLWrapper)
                {
                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, "SELECT DataPackageType, DataPackageVersion, DataPackagePath FROM {0}", _dataPackagesDataStore), lDataPackageTable);
                }
                DataContainer lDPCharCont = new DataContainer();
                lDPCharCont.initFromDataTable(lDataPackageTable);
                return lDPCharCont;
            }
        }
        
        /// <summary>
        /// Return a data package depending of type and version.
        /// </summary>
        /// <param name="pDPType">the data package type.</param>
        /// <param name="pDPVersion">the data package version.</param>
        /// <returns>The data package.</returns>
        public DataContainer mGetDataPackageCharacteristics(string pDPType, string pDPVersion)
        {
            using (DataTable lDataPackageTable = new DataTable("DataPackageCharacteristics"))
            {
                lDataPackageTable.Locale = CultureInfo.InvariantCulture;

                lock (_SQLWrapper)
                {
                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, "SELECT DataPackageType, DataPackageVersion, DataPackagePath FROM {0} WHERE DataPackageType == '{1}' AND DataPackageVersion == '{2}'", _dataPackagesDataStore, pDPType, pDPVersion), lDataPackageTable);
                }
                DataContainer lDPCharCont = new DataContainer();
                lDPCharCont.initFromDataTable(lDataPackageTable);
                return lDPCharCont;
            }
        }

        /// <summary>
        /// This function allows the "Remote Data Store" to return an element's assigned baselines definition.
        /// </summary>
        /// <param name="pEID">The element ID to get the baselines list.</param>
        /// <returns>The list of baselines assigned to element ID.</returns>
        public DataContainer mGetElementAssignedBaselines(string pEID)
        {
            using (DataTable lElementTable = new DataTable("ElementAssignedBaselines"))
            {
                lElementTable.Locale = CultureInfo.InvariantCulture;
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE ElementID == '{1}'", _elementsDataStore, pEID), lElementTable);
                }

                DataContainer lElAssBLCont = new DataContainer();
                lElAssBLCont.initFromDataTable(lElementTable);
                return lElAssBLCont;
            }
        }

        /// <summary>
        /// This function allows the "Remote Data Store" to return an element's chosen baseline definition.
        /// </summary>
        /// <param name="pEID">The element ID to get the baselines list.</param>
        /// <param name="pWhich">The baseline you want the definition (AssignedCurrent, AssignedFuture) .</param>
        /// <returns>The list of baselines assigned to elementID.</returns>
        public DataContainer mGetElementSpecificBaselineDefinition(string pEID, string pWhich)
        {
            using (DataTable lElementTable = new DataTable("ElementSpecificBaselineDefinition"))
            {
                lElementTable.Locale = CultureInfo.InvariantCulture;
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE ElementID == '{1}'", _elementsDataStore, pEID), lElementTable);
                }
                DataTableReader lElTableReader = new DataTableReader(lElementTable);
                lElTableReader.Read();
                string lBLVersion = string.Empty;
                for (int i = 0; i < lElTableReader.FieldCount; i++)
                {
                    if (lElTableReader.GetName(i) == pWhich + "Baseline")
                    {
                        lBLVersion = lElTableReader.GetValue(i).ToString().Trim();
                        break;
                    }
                }
                return mGetBaselineDefinition(lBLVersion);
            }
        }

        /// <summary>
        /// Return the list of data packages characteristics from a baseline definition
        /// </summary>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <returns>The list of data packages characteristics</returns>
        public DataContainer mGetDataPackages(string pBLVersion)
        {
            DataContainer lBLDef = mGetBaselineDefinition(pBLVersion);
            Dictionary<string, string> lDPVersions = new Dictionary<string,string>();
            lDPVersions["PISBASE"] = lBLDef.getStrValue("PISBaseDataPackageVersion");
            lDPVersions["PISMISSION"] = lBLDef.getStrValue("PISMissionDataPackageVersion");
            lDPVersions["PISINFOTAINMENT"] = lBLDef.getStrValue("PISInfotainmentDataPackageVersion");
            lDPVersions["LMT"] = lBLDef.getStrValue("LMTDataPackageVersion");
            DataContainer lDPkgsCont = new DataContainer();
            lDPkgsCont.Columns.Add("DataPackageType");
            lDPkgsCont.Columns.Add("DataPackageVersion");
            lDPkgsCont.Columns.Add("DataPackagePath");
            foreach (KeyValuePair<string, string> lPkg in lDPVersions)
	        {
                lDPkgsCont.merge(mGetDataPackageCharacteristics(lPkg.Key.ToString(), lPkg.Value.ToString().Trim()));
	        }
            return lDPkgsCont;
        }

        /// <summary>
        /// This function allows the Remote Data Store to add a new data package version to its data package files data store.
        /// </summary>
        /// <param name="pNDPkg">The new data package description.</param>
        public void mSetNewDataPackage(DataContainer pNDPkg)
        {
            //create dictionnary for insert operation
            Dictionary<String, String> lDictio = new Dictionary<String, String>();
            pNDPkg.restart();
            pNDPkg.read();
            for (int i = 0; i < pNDPkg.fieldCount(); i++)
                lDictio[pNDPkg.getName(i)] = pNDPkg.getValue(i);

            lock (_SQLWrapper)
            {
                _SQLWrapper.mInsert(_dataPackagesDataStore, lDictio);
            }
        }

        /// <summary>
        /// This function allows the Remote Data Store to update an open (i.e. decompressed) data package information.
        /// </summary>
        /// <param name="pDPType">The existing data package type.</param>
        /// <param name="pDPVersion">The existing data package version.</param>
        /// <param name="pDPLastOpenDate">The value of last open date of the identified data package.</param>
        public void mUpdateDataPackageLastOpenInfo(string pDPType, string pDPVersion, string pDPLastOpenDate)
        {
            lock (_SQLWrapper)
            {
                string lSqlCommand =
                    "UPDATE {0} SET DataPackageLastOpenDate='{1}' " +
                    "WHERE DataPackageType='{2}' AND DataPackageVersion='{3}'";

                _SQLWrapper.mExecuteNonQuery(String.Format(CultureInfo.InvariantCulture, lSqlCommand, _dataPackagesDataStore, pDPLastOpenDate, pDPType, pDPVersion));
            }
        }

        /// <summary>
        /// This function allows the Remote Data Store to add a new baseline definition to its baselines list.
        /// </summary>
        /// <param name="pBLDef">The new baseline.</param>
        public void mSetNewBaselineDefinition(DataContainer pBLDef)
        {
            Dictionary<String, String> lDictio = new Dictionary<String, String>();
            pBLDef.restart();
            pBLDef.read();
            for (int i = 0; i < pBLDef.fieldCount(); i++)
            {
                lDictio[pBLDef.getName(i)] = pBLDef.getValue(i);
            }
            lock (_SQLWrapper)
            {
                    _SQLWrapper.mInsert(_baselinesDataStore, lDictio);
            }
        }

		/// <summary>Saves a baseline distributing task.</summary>
		/// <param name="baselineDistributingTask">The baseline distributing task.</param>
		public void mSaveBaselineDistributingTask(DataContainer baselineDistributingTask)
		{
			string elementId = string.Empty;
			Dictionary<string, string> newValuesDictionnary = new Dictionary<string, string>();
			baselineDistributingTask.restart();
			baselineDistributingTask.read();
			for (int i = 0; i < baselineDistributingTask.fieldCount(); i++)
			{
				newValuesDictionnary[baselineDistributingTask.getName(i)] = baselineDistributingTask.getValue(i);
				if (baselineDistributingTask.getName(i).Equals("ElementID"))
				{
					elementId = baselineDistributingTask.getValue(i);
				}
			}

			mDeleteBaselineDistributingTask(elementId);

			lock (_SQLWrapper)
			{
				_SQLWrapper.mInsert(_baselineDistributingTasksDataStore, newValuesDictionnary);
			}
		}

		/// <summary>
		/// Deletes the baseline distributing task described by baselineDistributingTask.
		/// </summary>
		/// <param name="baselineDistributingTask">The baseline distributing task.</param>
		public void mDeleteBaselineDistributingTask(string elementId)
		{
			Dictionary<string, string> elementIdDictionnary = new Dictionary<string, string>();
			elementIdDictionnary.Add("ElementID", elementId);

			lock (_SQLWrapper)
			{
				if (_SQLWrapper.mEntryExists(_baselineDistributingTasksDataStore, elementIdDictionnary))
				{
					string whereClause = "ElementID = \'" + elementIdDictionnary["ElementID"] + "\'";
					_SQLWrapper.mDelete(_baselineDistributingTasksDataStore, whereClause);
				}				
			}
		}

        /// <summary>This function allows to deletes a data package from Remote Data Store.</summary>
        /// <param name="pType">The data package type.</param>
        /// <param name="pVersion">The data package version.</param>
        public void mDeleteDataPackage(string pType, string pVersion)
        {
            string whereCondition = "DataPackageType=\'" + pType + "\'" + " AND DataPackageVersion=\'" + pVersion + "\'";

            lock (_SQLWrapper)
            {
                _SQLWrapper.mDelete(_dataPackagesDataStore, whereCondition);
            }
        }

        /// <summary>Delete the baseline definition described by version.</summary>
        /// <param name="pVersion">The version. Could contains wildcars.</param>
        /// <returns>The list of assigned baselines, that weren't removed</returns>
        public void mDeleteBaselineDefinition(string pVersion)
        {
            string whereCondition = "BaselineVersion=\'" + pVersion + "\'";

            lock (_SQLWrapper)
            {
                _SQLWrapper.mDelete(_baselinesDataStore, whereCondition);
            }                                    
        }

        /// <summary>
        /// This function allows the Remote Data Store to assign a future baseline to an element.
        /// </summary>
        /// <param name="pEID">The ElementID to assign the baseline.</param>
        /// <param name="pBLVersion">The baseline version to assign.</param>
        /// <param name="pActDate">The activation date of the baseline.</param>
        /// <param name="pExpDate">The expiration date of the baseline.</param>
        public void mAssignFutureBaselineToElement(string pEID, string pBLVersion, DateTime pActDate, DateTime pExpDate)
        {
            Dictionary<String, String> lDictio = new Dictionary<String, String>();
            lDictio["AssignedFutureBaseline"] = pBLVersion;
            lDictio["AssignedFutureBaselineActivationDate"] = pActDate.ToString();
            lDictio["AssignedFutureBaselineExpirationDate"] = pExpDate.ToString();
            if (mElementExists(pEID))
            {
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mUpdate(_elementsDataStore, lDictio, String.Format(CultureInfo.InvariantCulture, "ElementID == '{0}'", pEID));
                }
            }
            else
            {
                lock (_SQLWrapper)
                {
                    lDictio["ElementID"] = pEID;
                    _SQLWrapper.mInsert(_elementsDataStore, lDictio);
                }
            }
        }

        /// <summary>
        /// This function allows the Remote Data Store to assign a current baseline to an element.
        /// </summary>
        /// <param name="pEID">The ElementID to assign the baseline.</param>
        /// <param name="pBLVersion">The baseline version to assign.</param>
        /// <param name="pExpDate">The expiration date of the baseline.</param>
        public void mAssignCurrentBaselineToElement(string pEID, string pBLVersion, DateTime pExpDate)
        {
            Dictionary<String, String> lDictio = new Dictionary<String, String>();
            lDictio["AssignedCurrentBaseline"] = pBLVersion;
            lDictio["AssignedCurrentBaselineExpirationDate"] = pExpDate.ToString();
            if (mElementExists(pEID))
            {
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mUpdate(_elementsDataStore, lDictio, String.Format(CultureInfo.InvariantCulture, "ElementID == '{0}'", pEID));
                }
            }
            else
            {
                lock (_SQLWrapper)
                {
                    lDictio["ElementID"] = pEID;
                    _SQLWrapper.mInsert(_elementsDataStore, lDictio);
                }
            }
        }

        /// <summary>This functions allows to set the undefined baseline parameters of an element.</summary>        
        /// <param name="pEID">The ElementID to set the undefined baseline parameters.</param>
        /// <param name="pPisBasePackageVersion">The pis base package version.</param>
        /// <param name="pPisMissionPackageVersion">The pis mission package version.</param>
        /// <param name="pPisInfotainmentPackageVersion">The pis infotainment package version.</param>
        /// <param name="pLmtPackageVersion">The lmt package version.</param>
        public void mSetElementUndefinedBaselineParams(
            string pEID,
            string pPisBasePackageVersion,
            string pPisMissionPackageVersion,
            string pPisInfotainmentPackageVersion,
            string pLmtPackageVersion)
        {
            Dictionary<String, String> lDictio = new Dictionary<String, String>();
            lDictio["UndefinedBaselinePisBaseVersion"] = pPisBasePackageVersion;
            lDictio["UndefinedBaselinePisMissionVersion"] = pPisMissionPackageVersion;
            lDictio["UndefinedBaselinePisInfotainmentVersion"] = pPisInfotainmentPackageVersion;
            lDictio["UndefinedBaselineLmtVersion"] = pLmtPackageVersion;

            if (mElementExists(pEID))
            {
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mUpdate(_elementsDataStore, lDictio, String.Format(CultureInfo.InvariantCulture, "ElementID == '{0}'", pEID));
                }
            }
            else
            {
                lock (_SQLWrapper)
                {
                    lDictio["ElementID"] = pEID;
                    _SQLWrapper.mInsert(_elementsDataStore, lDictio);
                }
            }
        }

        /// <summary>
        /// This function allows the Remote Data Store to return the elements description list.
        /// </summary>
        /// <returns>The list of elements already stored in the elements data store, an empty list when no elements are stored in the elements data store.</returns>
        public DataContainer mGetElementsDescription()
        {
            using (DataTable lElementsTable = new DataTable("ElementsDataStore"))
            {
                lElementsTable.Locale = CultureInfo.InvariantCulture;
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0}", _elementsDataStore), lElementsTable);
                }
                DataContainer lElementsContainer = new DataContainer();
                lElementsContainer.initFromDataTable(lElementsTable);
                return lElementsContainer;
            }
        }

        /// <summary>
        /// This function allows the Remote Data Store to return the undefined baselines list.
        /// </summary>
        /// <returns>The list of undefined baselines parameters stored in the elements data store,
        ///           an empty list when no elements are stored in the elements data store or 
        ///           there is no any element that is waiting for a baseline confirmation.</returns>
        public DataContainer mGetUndefinedBaselinesList()
        {
            using (DataTable lBaselinesTable = new DataTable("BaselinesDataStore"))
            {
                lBaselinesTable.Locale = CultureInfo.InvariantCulture;
                lock (_SQLWrapper)
                {
                    string lSqlCommand = "SELECT DISTINCT UndefinedBaselinePISBaseVersion, UndefinedBaselinePISMissionVersion, " +
                        "UndefinedBaselinePISInfotainmentVersion, UndefinedBaselineLmtVersion FROM {0} WHERE " +
                        "UndefinedBaselinePISBaseVersion IS NOT NULL AND " +
                        "UndefinedBaselinePISMissionVersion IS NOT NULL AND " +
                        "UndefinedBaselinePISInfotainmentVersion IS NOT NULL AND " +
                        "UndefinedBaselineLmtVersion IS NOT NULL";

                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, lSqlCommand, _elementsDataStore), lBaselinesTable);
                }

                DataContainer lBaselinesContainer = new DataContainer();
                lBaselinesContainer.initFromDataTable(lBaselinesTable);

                return lBaselinesContainer;
            }
        }

		/// <summary>Gets baseline distributing tasks.</summary>
		/// <returns>The baseline distributing tasks.</returns>
		public DataContainer mGetBaselineDistributingTasks()
		{
            using (DataTable baselineDistributingTasksTable = new DataTable("BaselineDistributingTasks"))
            {
                baselineDistributingTasksTable.Locale = CultureInfo.InvariantCulture;
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mExecuteQuery(String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0}", _baselineDistributingTasksDataStore), baselineDistributingTasksTable);
                }
                DataContainer lbaselineDistributingTasksContainer = new DataContainer();
                lbaselineDistributingTasksContainer.initFromDataTable(baselineDistributingTasksTable);
                return lbaselineDistributingTasksContainer;
            }
		}

        /// <summary>
        /// This function allows the Remote Data Store to unassign the future baseline from an element.
        /// </summary>
        /// <param name="pEID">The ElementID to unassign the baseline.</param>
        public void mUnassignFutureBaselineFromElement(string pEID)
        {
            Dictionary<String, String> lDictio = new Dictionary<String, String>();
            lDictio["AssignedFutureBaseline"] = "";
            lDictio["AssignedFutureBaselineActivationDate"] = "";
            lDictio["AssignedFutureBaselineExpirationDate"] = "";
            if (mElementExists(pEID))
            {
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mUpdate(_elementsDataStore, lDictio, String.Format(CultureInfo.InvariantCulture, "ElementID == '{0}'", pEID));
                }
            }
            else
            {
                // nothing to do
            }
        }

        /// <summary>
        /// This function allows the Remote Data Store to unassigns the current baseline from an element.
        /// </summary>
        /// <param name="pEID">The ElementID to unassign the baseline.</param>        
        public void mUnassignCurrentBaselineFromElement(string pEID)
        {
            Dictionary<String, String> lDictio = new Dictionary<String, String>();
            lDictio["AssignedCurrentBaseline"] = "";
            lDictio["AssignedCurrentBaselineExpirationDate"] = "";
            if (mElementExists(pEID))
            {
                lock (_SQLWrapper)
                {
                    _SQLWrapper.mUpdate(_elementsDataStore, lDictio, String.Format(CultureInfo.InvariantCulture, "ElementID == '{0}'", pEID));
                }
            }
            else
            {
                // nothing to do
            }
        }
    }
}