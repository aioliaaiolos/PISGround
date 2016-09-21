//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteDataStoreProxy.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.ServiceModel;


namespace PIS.Ground.RemoteDataStore
{
    /// <summary>WCF created proxy for PIS.Ground.IRemoteDataStore using ClientBase.</summary>
    public class RemoteDataStoreProxy :
        //WCF create proxy for IRemoteDataStore using ClientBase
        ClientBase<IRemoteDataStore>,
        IRemoteDataStore,
        IDisposable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDataStoreProxy"/> class.
        /// </summary>
        public RemoteDataStoreProxy()
        {
            // No logic to apply
        }

        /// <summary>
        /// Gets or sets the operation timeout.
        /// </summary>
        public TimeSpan OperationTimeout
        {
            get
            {
                return InnerChannel.OperationTimeout;
            }

            set
            {
                InnerChannel.OperationTimeout = value;
            }
        }

        /// <summary>Download a file from an url in the Remote Data Store.</summary>
        /// <param name="pReqID">Identifier for the request.</param>
        /// <param name="pFileURL">URL of the file.</param>
        public virtual void moveTheNewDataPackageFiles(Guid pReqID, string pFileURL)
        {
            base.Channel.moveTheNewDataPackageFiles(pReqID, pFileURL);
        }        

        /// <summary>Open a data package locally.</summary>
        /// <param name="packageType">Type of the package.</param>
        /// <param name="packageVersion">The package version.</param>
        /// <param name="fileRegexMatchPattern">A pattern specifying the file regular expression match.</param>
        /// <returns><see cref="OpenDataPackageResult"/></returns>
        public virtual OpenDataPackageResult openLocalDataPackage(
            string packageType,
            string packageVersion,
            string fileRegexMatchPattern)
        {
            return base.Channel.openLocalDataPackage(
               packageType,
               packageVersion,
               fileRegexMatchPattern);
        }

        /// <summary>Check URL.</summary>
        /// <param name="pReqID">Identifier for the request.</param>
        /// <param name="pUrl">URL of the document.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public virtual bool checkUrl(Guid pReqID, string pUrl)
        {
            return base.Channel.checkUrl(pReqID, pUrl);
        }

        /// <summary>Check if data packages from a baseline definition.</summary>
        /// <param name="pRequestID">Identifier for the request.</param>
        /// <param name="pBLDef">The baseline definition.</param>
        public virtual void checkDataPackagesAvailability(Guid pRequestID, DataContainer pBLDef)
        {
            base.Channel.checkDataPackagesAvailability(pRequestID, pBLDef);
        }

        /// <summary>Check if a data package exists in the data base.</summary>
        /// <param name="pDPType">Type of the datapackage.</param>
        /// <param name="pDPVersion">The datapackage version.</param>
        /// <returns>A boolean if the package is available.</returns>
        public virtual bool checkIfDataPackageExists(string pDPType, string pDPVersion)
        {
            return base.Channel.checkIfDataPackageExists(pDPType, pDPVersion);
        }

        /// <summary>Check if a baseline exists in the data base.</summary>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <returns>A boolean if the baseline is available.</returns>
        public virtual bool checkIfBaselineExists(string pBLVersion)
        {
            return base.Channel.checkIfBaselineExists(pBLVersion);
        }

        /// <summary>Check if an Element exists in the data base.</summary>
        /// <param name="pEID">The element Id.</param>
        /// <returns>A boolean if the element is available.</returns>
        public virtual bool checkIfElementExists(string pEID)
        {
            return base.Channel.checkIfElementExists(pEID);
        }

        /// <summary>Add a new data package to the data base.</summary>
        /// <param name="pNDPkg">The nd package.</param>
        public virtual void setNewDataPackage(DataContainer pNDPkg)
        {
            base.Channel.setNewDataPackage(pNDPkg);
        }

        /// <summary>Add a new baseline deifnition to the data base.</summary>
        /// <param name="pRequestID">Identifier for the request.</param>
        /// <param name="pNBLDef">The nbl definition.</param>
        public virtual void setNewBaselineDefinition(Guid pRequestID, DataContainer pNBLDef)
        {
            base.Channel.setNewBaselineDefinition(pRequestID, pNBLDef);
        }

		/// <summary> Delete the baseline definition described by version.</summary>
		/// <param name="pVersion">The baseline version.</param>
        public virtual void deleteBaselineDefinition(string pVersion)
		{
            base.Channel.deleteBaselineDefinition(pVersion);
		}

        /// <summary>get all the baselines definitions from the data base.</summary>
        /// <returns>A DataContainer representing the list of baselines definitions.</returns>
        public virtual DataContainer getBaselinesDefinitions()
        {
            return base.Channel.getBaselinesDefinitions();
        }

        /// <summary>get a baseline definition from the data base.</summary>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <returns>A DataContainer representing the baseline definition.</returns>
        public virtual DataContainer getBaselineDefinition(string pBLVersion)
        {
            return base.Channel.getBaselineDefinition(pBLVersion);
        }

        /// <summary>Get the list of the assigned baselines from the data base.</summary>
        /// <returns>A DataContainer representing the list of assigned baselines versions.</returns>
        public virtual DataContainer getAssignedBaselinesVersions()
        {
            return base.Channel.getAssignedBaselinesVersions();
        }

        /// <summary>
        /// Get the assigned current baseline version of an element from the data base .
        /// </summary>
        /// <param name="pEID">The element Id.</param>
        /// <returns>The baseline version as a string.</returns>
        public virtual string getAssignedCurrentBaselineVersion(string pEID)
        {
            return base.Channel.getAssignedCurrentBaselineVersion(pEID);
        }

        /// <summary>Get the assigned future baseline version of an element from the data base.</summary>
        /// <param name="pEID">The element Id.</param>
        /// <returns>The baseline version as a string.</returns>
        public virtual string getAssignedFutureBaselineVersion(string pEID)
        {
            return base.Channel.getAssignedFutureBaselineVersion(pEID);
        }

        /// <summary>
        /// Get the list of data packages characteristics corresponding to a baseline definition.
        /// </summary>
        /// <param name="pBLDef">The bl definition.</param>
        /// <returns>The list of data packages characteristics as a DataContainer.</returns>
        public virtual DataContainer getDataPackages(DataContainer pBLDef)
        {
            return base.Channel.getDataPackages(pBLDef);
        }

        /// <summary>
        /// Get the list of all data packages characteristics
        /// </summary>        
        /// <returns>The list of data packages characteristics as a DataContainer.</returns>
        public virtual DataContainer getDataPackagesList()
        {
            return base.Channel.getDataPackagesList();
        }

        /// <summary>Return the data package characteristics as in data base.</summary>
        /// <param name="pDPType">Type of the datapackage.</param>
        /// <param name="pDPVersion">The datapackage version.</param>
        /// <returns>The data package characteristics.</returns>
        public virtual DataContainer getDataPackageCharacteristics(string pDPType, string pDPVersion)
        {
            return base.Channel.getDataPackageCharacteristics(pDPType, pDPVersion);
        }

        /// <summary>Return the definition of the current assigned baseline of an element.</summary>
        /// <param name="pEID">The element Id.</param>
        /// <returns>The baseline definition.</returns>
        public virtual DataContainer getCurrentBaselineDefinition(string pEID)
        {
            return base.Channel.getCurrentBaselineDefinition(pEID);
        }

        /// <summary>Return the definition of the assigned baselines of an element.</summary>
        /// <param name="pEID">The element Id.</param>
        /// <returns>The baselines definitions (Assigned Current and Assigned Future).</returns>
        public virtual DataContainer getElementBaselinesDefinitions(string pEID)
        {
            return base.Channel.getElementBaselinesDefinitions(pEID);
        }

        /// <summary>
        /// Create an xml file corresponding to the Assigned Future baseline of an element and return the
        /// path where it creates it.
        /// </summary>
        /// <param name="pReqId">Identifier for the request.</param>
        /// <param name="pEID">The element Id.</param>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <param name="pActivationDate">Date of the activation.</param>
        /// <param name="pExpirationDate">Date of the expiration.</param>
        /// <returns>The path to baseline.xml file.</returns>
        public virtual string createBaselineFile(Guid pReqId, string pEID, string pBLVersion, string pActivationDate, string pExpirationDate)
        {
            return base.Channel.createBaselineFile(pReqId, pEID, pBLVersion, pActivationDate, pExpirationDate);
        }

        /// <summary>
        /// Construct a differential package between two data packages and return an url to this package.
        /// </summary>
        /// <param name="pReqID">Identifier for the request.</param>
        /// <param name="pEID">The element Id.</param>
        /// <param name="pDPType">Type of the datapackage.</param>
        /// <param name="pDPVersionOnBoard">The datapackage version board.</param>
        /// <param name="pDPVersionOnGround">The datapackage version ground.</param>
        /// <returns>Url of the differential package.</returns>
        public virtual string getDiffDataPackageUrl(Guid pReqID, string pEID, string pDPType, string pDPVersionOnBoard, string pDPVersionOnGround)
        {
            return base.Channel.getDiffDataPackageUrl(pReqID, pEID, pDPType, pDPVersionOnBoard, pDPVersionOnGround);
        }

        /// <summary>Assign a baseline to an element as a future baseline.</summary>
        /// <param name="ReqID">Identifier for the request.</param>
        /// <param name="pEID">The element Id.</param>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <param name="pActDate">Date of the activation.</param>
        /// <param name="pExpDate">Date of the expiration.</param>
        public virtual void assignAFutureBaselineToElement(Guid ReqID, string pEID, string pBLVersion, DateTime pActDate, DateTime pExpDate)
        {
            base.Channel.assignAFutureBaselineToElement(ReqID, pEID, pBLVersion, pActDate, pExpDate);
        }

        /// <summary>Assign a baseline to an element as a current baseline.</summary>
        /// <param name="ReqID">Identifier for the request.</param>
        /// <param name="pEID">The element Id.</param>
        /// <param name="pBLVersion">The baseline version.</param>
        /// <param name="pExpDate">Date of the exponent.</param>
        public virtual void assignACurrentBaselineToElement(Guid ReqID,string pEID, string pBLVersion, DateTime pExpDate)
        {
            base.Channel.assignACurrentBaselineToElement(ReqID, pEID, pBLVersion, pExpDate);
        }

        /// <summary>Deletes the baseline file.</summary>
        /// <param name="pReqId">Identifier for the request.</param>
        /// <param name="pEID">The element Id.</param>
        public virtual void deleteBaselineFile(Guid pReqId, string pEID)
        {
            base.Channel.deleteBaselineFile(pReqId, pEID);
        }

        /// <summary>Deletes the Data Package.</summary>
        /// <param name="pReqId">Identifier for the request.</param>
        /// <param name="pDPType">Type of the datapackage.</param>
        /// <param name="pDPVersion">The datapackage version.</param>
        public virtual void deleteDataPackage(Guid pReqId, string pDPType, string pDPVersion)
        {
            base.Channel.deleteDataPackage(pReqId, pDPType, pDPVersion);
        }

        /// <summary>Deletes the data package difference file.</summary>
        /// <param name="pReqId">Identifier for the request.</param>
        /// <param name="pEID">The element Id.</param>
        public virtual void deleteDataPackageDiffFile(Guid pReqId, string pEID)
        {
            base.Channel.deleteDataPackageDiffFile(pReqId, pEID);
        }

        public virtual void setElementUndefinedBaselineParams(
            string pEID,
            string pPisBasePackageVersion,
            string pPisMissionPackageVersion,
            string pPisInfotainmentPackageVersion,
            string pLmtPackageVersion)
        {
            base.Channel.setElementUndefinedBaselineParams(
                pEID,
                pPisBasePackageVersion,
                pPisMissionPackageVersion,
                pPisInfotainmentPackageVersion,
                pLmtPackageVersion);
        }

        public virtual DataContainer getElementsDescription()
        {
            return base.Channel.getElementsDescription();
        }

        public virtual DataContainer getUndefinedBaselinesList()
        {
            return base.Channel.getUndefinedBaselinesList();
        }

        public virtual void unassignFutureBaselineFromElement(string pEID)
        {
            base.Channel.unassignFutureBaselineFromElement(pEID);
        }

        public virtual void unassignCurrentBaselineFromElement(string pEID)
        {
            base.Channel.unassignCurrentBaselineFromElement(pEID);
        }

		#region Baseline distribution requests processing

		/// <summary>Saves a baseline distributing request.</summary>
		/// <param name="baselineDistributingTask">The baseline distributing task.</param>
		public virtual void saveBaselineDistributingRequest(DataContainer baselineDistributingTask)
		{
			base.Channel.saveBaselineDistributingRequest(baselineDistributingTask);
		}

		/// <summary>Gets all baseline distributing saved requests.</summary>
		/// <returns>all baseline distributing saved requests.</returns>
		public virtual DataContainer getAllBaselineDistributingSavedRequests()
		{
			return base.Channel.getAllBaselineDistributingSavedRequests();
		}

		/// <summary>Deletes the baseline distributing request.</summary>
		/// <param name="elementId">Identifier for the element.</param>
		public virtual void deleteBaselineDistributingRequest(string elementId)
		{
			base.Channel.deleteBaselineDistributingRequest(elementId);
		}

		#endregion

        #region IDisposable Members

        /// <summary>
        /// Method that safely close the proxy.
        /// </summary>
        void IDisposable.Dispose()
        {
            bool success = false;
            try
            {
                if (State != CommunicationState.Faulted)
                {
                    Close();
                    success = true;
                }
            }
            finally
            {
            	if (!success)
                {
                    Abort();
                }
            }
        }

        #endregion
    }
}
