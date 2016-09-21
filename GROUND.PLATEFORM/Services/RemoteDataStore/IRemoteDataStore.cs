//---------------------------------------------------------------------------------------------------
// <copyright file="IRemoteDataStore.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace PIS.Ground.RemoteDataStore
{
    /// <summary>
    ///  Define a Service Contract to access the Remote Data Store
    /// </summary>
    [ServiceContract]
    public interface IRemoteDataStore : IDisposable
    {
        /// <summary>
        /// Gets or sets the operation timeout.
        /// </summary>
        TimeSpan OperationTimeout { get; set; }

        /// <summary>
        /// Download a file from an url in the Remote Data Store
        /// </summary>
        /// <param name="pReqID">The request id.</param>
        /// <param name="pFileURL">The url of the file to download.</param>
        /// <remarks>Throw updatePackageUploadStatus notification.</remarks>
        [OperationContract (IsOneWay = true)]
        void moveTheNewDataPackageFiles(Guid pReqID, string pFileURL);

        /// <summary>
        /// Open a data package locally
        /// </summary>
        /// <param name="packageType">data package type</param>
        /// <param name="packageVersion">package version</param>
        /// <param name="fileRegexMatchPattern">regex pattern to match requested package elements (when empty, the whole package is opened)</param>
        /// <param name="force">when true, do not use any cached, previously-opened package elements</param>
        /// <returns><see cref="OpenDataPackageResult"/></returns>
        [OperationContract]
        OpenDataPackageResult openLocalDataPackage(
            string packageType,
            string packageVersion,
            string fileRegexMatchPattern);
        
        [OperationContract]
        bool checkUrl(Guid pReqID, string pUrl);

        /// <summary>
        /// Check if data packages from a baseline definition.
        /// </summary>
        /// <param name="pRequestID">The request id.</param>
        /// <param name="pBLDef">The baseline definition.</param>
        /// <remarks>Send missingDataPackageNotification if at least one data package is missing.</remarks>
        [OperationContract (IsOneWay = true)]
        void checkDataPackagesAvailability(Guid pRequestID, DataContainer pBLDef);

        /// <summary>
        /// Check if a data package exists in the data base.
        /// </summary>
        /// <param name="pDPType">The DataPackage type as a string.</param>
        /// <param name="pDPVersion">The DataPackage Version.</param>
        /// <returns>A boolean if the package is available.</returns>
        [OperationContract]
        bool checkIfDataPackageExists(string pDPType, string pDPVersion);

        /// <summary>
        /// Check if a baseline exists in the data base.
        /// </summary>
        /// <param name="pBLVersion">the baseline version.</param>
        /// <returns>A boolean if the baseline is available.</returns>
        [OperationContract]
        bool checkIfBaselineExists(string pBLVersion);

        /// <summary>
        /// Check if an Element exists in the data base.
        /// </summary>
        /// <param name="pEID">The Element ID.</param>
        /// <returns>A boolean if the element is available.</returns>
        [OperationContract]
        bool checkIfElementExists(string pEID);

        /// <summary>
        /// Add a new data package to the data base.
        /// </summary>
        /// <param name="pNDPkg">A DataContainer representing the data package characteristics.</param>
        [OperationContract(IsOneWay = true)]
        void setNewDataPackage(DataContainer pNDPkg);

        /// <summary>
        /// Add a new baseline definition to the data base.
        /// </summary>
        /// <param name="pRequestID">The request ID.</param>
        /// <param name="pNBLDef">A table representing the baseline definition.</param>
        /// <remarks>Send updateBaselineDefinitionStatus notification.</remarks>
        [OperationContract(IsOneWay = true)]
        void setNewBaselineDefinition(Guid pRequestID, DataContainer pNBLDef);

		/// <summary>Saves a baseline distributing request.</summary>
		/// <param name="baselineDistributingTask">The baseline distributing task.</param>
		[OperationContract(IsOneWay = true)]
		void saveBaselineDistributingRequest(DataContainer baselineDistributingTask);

        /// <summary>
        /// Delete the baseline definition described by version.
        /// </summary>
        /// <param name="pVersion">The version. Could contains wildcars.</param>
        [OperationContract]
        void deleteBaselineDefinition(string pVersion);

        /// <summary>
        /// get all the baselines definitions from the data base.
        /// </summary>
        /// <returns>A DataContainer representing the list of baselines definitions.</returns>
        [OperationContract]
        DataContainer getBaselinesDefinitions();

        /// <summary>
        /// get a baseline definition from the data base
        /// </summary>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <returns>A DataContainer representing the baseline definition</returns>
        [OperationContract]
        DataContainer getBaselineDefinition(string pBLVersion);

        /// <summary>
        /// Get the list of assigned baselines versions from the data base.
        /// </summary>        
        /// <returns>A DataContainer representing the list of baselines versions</returns>
        [OperationContract]
        DataContainer getAssignedBaselinesVersions();

        /// <summary>
        /// Get the assigned current baseline version of an element from the data base .
        /// </summary>
        /// <param name="pBLVersion">The element id you want the info.</param>
        /// <returns>The baseline version as a string.</returns>
        [OperationContract]
        string getAssignedCurrentBaselineVersion(string pEID);

        /// <summary>
        /// Get the assigned future baseline version of an element from the data base.
        /// </summary>
        /// <param name="pBLVersion">The element id you want the info.</param>
        /// <returns>The baseline version as a string.</returns>
        [OperationContract]
        string getAssignedFutureBaselineVersion(string pEID);

        /// <summary>
        /// Get the list of data packages characteristics corresponding to a baseline definition.
        /// </summary>
        /// <param name="pBLDef">The baseline definition.</param>
        /// <returns>The list of data packages characteristics as a DataContainer.</returns>
        [OperationContract]
        DataContainer getDataPackages(DataContainer pBLDef);

        /// <summary>
        /// Get the list of all data packages stored in Remote Data Store.
        /// </summary>        
        /// <returns>The list of data packages.</returns>
        [OperationContract]
        DataContainer getDataPackagesList();

        /// <summary>
        /// Return the data package characteristics as in data base.
        /// </summary>
        /// <param name="pDPType">The data package type.</param>
        /// <param name="pDPVersion">The data package version.</param>
        /// <returns>The data package characteristics</returns>
        [OperationContract]
        DataContainer getDataPackageCharacteristics(string pDPType, string pDPVersion);

        /// <summary>
        /// Return the definition of the current assigned baseline of an element
        /// </summary>
        /// <param name="pEID">The element ID.</param>
        /// <returns>The baseline definition</returns>
        [OperationContract]
        DataContainer getCurrentBaselineDefinition(string pEID);

        /// <summary>
        /// Return the definition of the assigned baselines of an element
        /// </summary>
        /// <param name="pEID">The element ID.</param>
        /// <returns>The baselines definitions (Assigned Current and Assigned Future).</returns>
        [OperationContract]
        DataContainer getElementBaselinesDefinitions(string pEID);

		/// <summary>Gets all baseline distributing saved requests.</summary>
		/// <returns>all baseline distributing saved requests.</returns>
		[OperationContract]
		DataContainer getAllBaselineDistributingSavedRequests();

        /// <summary>
        /// Create an xml file corresponding to the Assigned Future baseline of an element and return the path where it creates it.
        /// </summary>
        /// <param name="pReqId">The request Id.</param>
        /// <param name="pEID">The element id</param>
        /// <returns>The path to baseline.xml file.</returns>
        [OperationContract]
        string createBaselineFile(Guid pReqId, string pEID, string pBLVersion, string pActivationDate, string pExpirationDate);

        /// <summary>
        /// Construct a differential package between two data packages and return an url to this package.
        /// </summary>
        /// <param name="pReqID">The request Id.</param>
        /// <param name="pDPVersionOnBoard">The version of the data package currently on train.</param>
        /// <param name="pDPVersionOnGround">The version of the data package of the future baseline.</param>
        /// <returns>Url of the differential package</returns>
        [OperationContract]
        string getDiffDataPackageUrl(Guid pReqID, string pEID, string pDPType, string pDPVersionOnBoard, string pDPVersionOnGround);

        /// <summary>
        /// Assign a baseline to an element as a future baseline
        /// </summary>
        /// <param name="pEID">The element ID.</param>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <param name="pActDate">The activation date of the baseline.</param>
        /// <param name="pExpDate">The expiration date of the baseline.</param>
        /// <remarks>Send updateBaselineAssignmentStatus notification.
        ///          Call checkDataPackagesAvailability that could send missingDataPackageNotification.</remarks>
        [OperationContract(IsOneWay = true)]
        void assignAFutureBaselineToElement(Guid ReqID, string pEID, string pBLVersion, DateTime pActDate, DateTime pExpDate);

        /// <summary>
        /// Assign a baseline to an element as a current baseline
        /// </summary>
        /// <param name="pEID">The element ID.</param>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <param name="pExpDate">The expiration date of the baseline.</param>
        /// <remarks>Send updateBaselineAssignmentStatus notification.
        ///          Call checkDataPackagesAvailability that could send missingDataPackageNotification.</remarks>
        [OperationContract(IsOneWay = true)]
        void assignACurrentBaselineToElement(Guid ReqID, string pEID, string pBLVersion, DateTime pExpDate);

        [OperationContract(IsOneWay=true)]
        void deleteBaselineFile(Guid pReqId, string pEID);

        /// <summary>Delete the Data Package.</summary>
        /// <param name="pReqId">The request Id.</param>
        /// <param name="pDPType">The Data Package type as a string.</param>
        /// <param name="pDPVersion">The Data Package Version.</param>
        [OperationContract(IsOneWay = true)]
        void deleteDataPackage(Guid pReqId, string pDPType, string pDPVersion);

        /// <summary>Deletes the data package difference file.</summary>
        /// <param name="pReqId">The request Id.</param>
        /// <param name="pEID">The element ID.</param>
        [OperationContract(IsOneWay = true)]
        void deleteDataPackageDiffFile(Guid pReqId, string pEID);

		/// <summary>Deletes the baseline distributing request.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		[OperationContract(IsOneWay = true)]
		void deleteBaselineDistributingRequest(string elementId);

        /// <summary>Sets the undefined baseline parameters of an element.</summary>
        /// <param name="pEID">The element ID.</param>
        /// <param name="pPisBasePackageVersion">The Pis Base package version.</param>
        /// <param name="pPisMissionPackageVersion">The Pis Mission package version.</param>
        /// <param name="pPisInfotainmentPackageVersion">The Pis Infotainment package version.</param>
        /// <param name="pLmtPackageVersion">The Lmt package version.</param>
        [OperationContract(IsOneWay = true)]
        void setElementUndefinedBaselineParams(
            string pEID,
            string pPisBasePackageVersion,
            string pPisMissionPackageVersion,
            string pPisInfotainmentPackageVersion,
            string pLmtPackageVersion);

        /// <summary>Return all elements description form the "ElementsDataStore" table.</summary>
        /// <returns>A DataContainer representing the list of elements description.</returns>
        [OperationContract]
        DataContainer getElementsDescription();

        /// <summary>
        /// get undefined baselines list from the data base.
        /// </summary>
        /// <returns>A DataContainer representing the list of undefined baselines parameters.</returns>
        [OperationContract]
        DataContainer getUndefinedBaselinesList();

        /// <summary>
        /// Unassign future baseline from an element
        /// </summary>
        /// <param name="pEID">The element ID.</param>        
        [OperationContract]
        void unassignFutureBaselineFromElement(string pEID);

        /// <summary>
        /// Unassign current baseline from an element
        /// </summary>
        /// <param name="pEID">The element ID.</param>        
        [OperationContract]
        void unassignCurrentBaselineFromElement(string pEID);
    }

    /// <summary>
    /// Enum used to provide response code for openLocalDataPackage and openLocalDataPackageAsync methods
    /// </summary>
    [DataContract]
    public enum OpenDataPackageStatusEnum
    {
        [EnumMember]
        COMPLETED = 0,

        [EnumMember]
        FAILED_UNKNOWN_PACKAGE = 1
    }

    /// <summary>
    /// Operation result datatype used by openLocalDataPackage and openLocalDataPackageAsync methods
    /// </summary>
    [DataContract]
    public class OpenDataPackageResult
    {
        [DataMember]
        public string LocalPackagePath;

        [DataMember]
        public OpenDataPackageStatusEnum Status;
    }

    /// <summary>
    /// Enum used to provide exception code for all methods
    /// </summary>
    [DataContract]
    public enum RemoteDataStoreExceptionCodeEnum
    {
        [EnumMember]
        UNKNOWN_ELEMENT_ID = 0,

        [EnumMember]
        INVALID_BASELINE_DEFINITION = 1,

        [EnumMember]
        UNKNOWN_DATAPACKAGE_TYPE = 2,

        [EnumMember]
        INVALID_DATAPACKAGE_VERSION = 3,

        [EnumMember]
        INVALID_BASELINE_VERSION = 4,

        [EnumMember]
        INVALID_ELEMENT_ID = 5,

        [EnumMember]
        INVALID_DATAPACKAGE_DEFINITION = 6,

        [EnumMember]
        UNKNOWN_BASELINE_VERSION = 7,

        [EnumMember]
        INVALID_ONBOARD_PACKAGE_VERSION = 8,

        [EnumMember]
        INVALID_ONGROUND_PACKAGE_VERSION = 9,

        [EnumMember]
        BAD_ACTIVATION_DATE = 10,

        [EnumMember]
        BAD_EXPIRATION_DATE = 11,
    }

    /// <summary>
    /// Defines a data type to exchange data between DataPackage and RemoteDataStore usiong less bandwith than with a DataTable.
    /// </summary>
    [DataContract]
    [KnownType(typeof(string[]))]
    public class DataContainer
    {
        [DataMember]
        public List<string> Columns { get; set; }
        [DataMember]
        public List<string> Rows { get; set; }

        private int currentRowNumber { get; set; }

        public DataContainer()
        {
            Columns = new List<string>();
            Rows = new List<string>();
            currentRowNumber = -1;
        }

        [OperationContract]
        public void initFromDataTable(DataTable pDT)
        {
            this.currentRowNumber = -1;
            this.Columns = new List<string>();
            this.Rows = new List<string>();

            foreach (DataColumn lCo in pDT.Columns)
            {
                this.Columns.Add(lCo.ColumnName);
            }

            foreach (DataRow lRo in pDT.Rows)
            {
                for (int i = 0; i < lRo.ItemArray.GetLength(0); i++)
                {
                    if (lRo.ItemArray[i] is DateTime)
                    {
                        DateTime lValue = new DateTime();
                        DateTime.TryParse(lRo.ItemArray[i].ToString(), out lValue);
                        this.Rows.Add(lValue.ToString());
                    }
                    else
                    {
                        this.Rows.Add((lRo.ItemArray[i] ?? (Object)"").ToString());
                    }
                }
            }
        }

        [OperationContract]
        public void merge(DataContainer pIn)
        {
            if (this.Columns.Count != pIn.Columns.Count)
            {
                throw new InvalidExpressionException("Trying to merge two incompatible DataContainers");
            }
            else
            {
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    if (this.Columns[i] != pIn.Columns[i])
                    {
                        throw new InvalidExpressionException("Trying to merge two incompatible DataContainers");
                    }
                }
                foreach (string lVal in pIn.Rows)
                {
                    this.Rows.Add(lVal);
                }
            }
        }

        [OperationContract]
        public bool read()
        {
            if (((currentRowNumber + 1) * Columns.Count) < Rows.Count)
            {
                currentRowNumber++;
                return true;
            }
            return false;
        }

        [OperationContract]
        public void restart()
        {
            currentRowNumber = -1;
        }

        [OperationContract]
        public string getName(int i)
        {
            return Columns[i];
        }

        [OperationContract]
        public string getValue(int i)
        {
            int ind;
            if (currentRowNumber == -1)
	        {
               ind = 0;
	        }
            else
            {
                ind = currentRowNumber;
            }
            return Rows[ind * Columns.Count + i];
        }

        [OperationContract]
        public void setValue(int i, string pValue)
        {
            int ind;
            if (currentRowNumber == -1)
            {
                ind = 0;
            }
            else
            {
                ind = currentRowNumber;
            }
            Rows[ind * Columns.Count + i] = pValue;
        }

        [OperationContract]
        public int fieldCount()
        {
            return Columns.Count;
        }

        [OperationContract]
        public string getStrValue(string pColumnName)
        {
            int ind;
            if (currentRowNumber == -1)
            {
                ind = 0;
            }
            else
            {
                ind = currentRowNumber;
            }
            for (int j = 0; j < Columns.Count; j++)
			{
                if (Columns[j].Equals(pColumnName,StringComparison.OrdinalIgnoreCase))
                {
                    return Rows[ind * Columns.Count + j];
                }
			}
            return "";
        }

        [OperationContract]
        public void setStrValue(string pColumnName, string pValue)
        {
            int ind;
            if (currentRowNumber == -1)
            {
                ind = 0;
            }
            else
            {
                ind = currentRowNumber;
            }
            for (int j = 0; j < Columns.Count; j++)
            {
                if (Columns[j].Equals(pColumnName, StringComparison.OrdinalIgnoreCase))
                {
                    Rows[ind * Columns.Count + j] = pValue;
                }
            }
        }

        [OperationContract]
        public static bool operator ==(DataContainer pExpected, DataContainer pActual)
        {
            bool lResult = true;
            if (System.Object.ReferenceEquals(pExpected, pActual))
            {
                lResult = true;
            }
            else
            {
                if (((object)pExpected == null) || ((object)pActual == null))
                {
                    lResult = false;
                }
                else
                {
                    if (System.Object.ReferenceEquals(pExpected.Columns, pActual.Columns))
                    {
                        lResult = true;
                    }
                    else
                    {
                        if (((object)pExpected.Columns == null) || ((object)pActual.Columns == null))
                        {
                            lResult = false;
                        }
                        else
                        {
                            if (pExpected.Columns.Count != pActual.Columns.Count)
                            {
                                lResult = false;
                            }
                            else
                            {
                                if (System.Object.ReferenceEquals(pExpected.Rows, pActual.Rows))
                                {
                                    lResult = true;
                                }
                                else
                                {
                                    if (((object)pExpected.Rows == null) || ((object)pActual.Rows == null))
                                    {
                                        lResult = false;
                                    }
                                    else
                                    {
                                        if (pExpected.Rows.Count != pActual.Rows.Count)
                                        {
                                            lResult = false;
                                        }
                                        else
                                        {
                                            for (int i = 0; i < pExpected.Columns.Count; i++)
                                            {
                                                if (pExpected.Columns[i] != pActual.Columns[i])
                                                {
                                                    lResult = false;
                                                }
                                            }
                                            if (lResult)
                                            {
                                                pExpected.restart();
                                                pActual.restart();
                                                while (pExpected.read() && pActual.read())
                                                {
                                                    for (int i = 0; i < pExpected.fieldCount(); i++)
                                                    {
                                                        if (pExpected.getValue(i) != pActual.getValue(i))
                                                        {
                                                            lResult = false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lResult;
        }

        [OperationContract]
        public static bool operator !=(DataContainer pExpected, DataContainer pActual)
        {
            return !(pExpected == pActual);
        }

        [OperationContract]
        public override bool Equals(object obj)
        {
            DataContainer lDC = obj as DataContainer;
            if ((object)lDC == null)
            {
                return false;
            }

            bool lResult = true;
            if (System.Object.ReferenceEquals(this.Columns, lDC.Columns))
            {
                lResult = true;
            }
            else
            {
                if (((object)this.Columns == null) || ((object)lDC.Columns == null))
                {
                    lResult = false;
                }
                else
                {
                    if (this.Columns.Count != lDC.Columns.Count)
                    {
                        lResult = false;
                    }
                    else
                    {
                        if (System.Object.ReferenceEquals(this.Rows, this.Rows))
                        {
                            lResult = true;
                        }
                        else
                        {
                            if (((object)this.Rows == null) || ((object)this.Rows == null))
                            {
                                lResult = false;
                            }
                            else
                            {
                                if (this.Rows.Count != lDC.Rows.Count)
                                {
                                    lResult = false;
                                }
                                else
                                {
                                    for (int i = 0; i < this.Columns.Count; i++)
                                    {
                                        if (this.Columns[i] != lDC.Columns[i])
                                        {
                                            lResult = false;
                                        }
                                    }
                                    if (lResult)
                                    {
                                        this.restart();
                                        lDC.restart();
                                        while (this.read() && lDC.read())
                                        {
                                            for (int i = 0; i < this.fieldCount(); i++)
                                            {
                                                if (this.getValue(i) != lDC.getValue(i))
                                                {
                                                    lResult = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return base.Equals(obj) && lResult;
        }

        [OperationContract]
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.Columns.GetHashCode() ^ this.Rows.GetHashCode() ^ this.currentRowNumber.GetHashCode();
        }
    }
}
