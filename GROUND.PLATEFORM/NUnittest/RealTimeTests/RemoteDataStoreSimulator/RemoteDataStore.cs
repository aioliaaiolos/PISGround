//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteDataStoreNominalCase.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2014.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.IO;
using PIS.Ground.RemoteDataStore;

namespace PIS.Ground.RealTimeTests.RemoteDataStoreSimulator
{
	/// <summary>Remote data store nominal case.</summary>
	public class RemoteDataStore : RemoteDataStoreProxy
	{
		#region fields
		
		/// <summary>Full pathname of the local package file.</summary>
		private readonly string _localPackagePath = null;

		/// <summary>The status.</summary>
		private readonly OpenDataPackageStatusEnum _status = OpenDataPackageStatusEnum.FAILED_UNKNOWN_PACKAGE;

		#endregion

		#region Constructor

		/// <summary>Initializes a new instance of the RemoteDataStore class.</summary>
		/// <param name="localPackagePath">Full pathname of the local package file.</param>
		public RemoteDataStore(string localPackagePath)
		{
			_localPackagePath = localPackagePath;

			if (Directory.Exists(_localPackagePath))
			{
				_status = OpenDataPackageStatusEnum.COMPLETED;
			}
		}

		#endregion

		#region IRemoteDataStore Members

		/// <summary>Download a file from an url in the Remote Data Store.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pReqID">Identifier for the request.</param>
		/// <param name="pFileURL">URL of the file.</param>
		public override void moveTheNewDataPackageFiles(Guid pReqID, string pFileURL)
		{
			throw new NotImplementedException();
		}

		/// <summary>Open a data package locally.</summary>
		/// <param name="packageType">Type of the package.</param>
		/// <param name="packageVersion">The package version.</param>
		/// <param name="fileRegexMatchPattern">A pattern specifying the file regular expression match.</param>
		/// <returns><see cref="OpenDataPackageResult"/>Result of the open package operation.</returns>
		public override OpenDataPackageResult openLocalDataPackage(string packageType, string packageVersion, string fileRegexMatchPattern)
		{
			return new OpenDataPackageResult() { LocalPackagePath = _localPackagePath, Status = _status};
		}

		/// <summary>Check URL.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pReqID">Identifier for the request.</param>
		/// <param name="pUrl">URL of the document.</param>
		/// <returns>true if it succeeds, false if it fails.</returns>
		public override bool checkUrl(Guid pReqID, string pUrl)
		{
			throw new NotImplementedException();
		}

		/// <summary>Check if data packages from a baseline definition.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pRequestID">Identifier for the request.</param>
		/// <param name="pBLDef">The baseline definition.</param>
		public override void checkDataPackagesAvailability(Guid pRequestID, DataContainer pBLDef)
		{
			throw new NotImplementedException();
		}

		/// <summary>Check if a data package exists in the data base.</summary>
		/// <param name="pDPType">Type of the datapackage.</param>
		/// <param name="pDPVersion">The datapackage version.</param>
		/// <returns>A boolean if the package is available.</returns>
		public override bool checkIfDataPackageExists(string pDPType, string pDPVersion)
		{
			return true;
		}

		/// <summary>Check if a baseline exists in the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pBLVersion">The baseline version.</param>
		/// <returns>A boolean if the baseline is available.</returns>
		public override bool checkIfBaselineExists(string pBLVersion)
		{
			throw new NotImplementedException();
		}

		/// <summary>Check if an Element exists in the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pEID">The element Id.</param>
		/// <returns>A boolean if the element is available.</returns>
		public override bool checkIfElementExists(string pEID)
		{
			throw new NotImplementedException();
		}

		/// <summary>Add a new data package to the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pNDPkg">The nd package.</param>
		public override void setNewDataPackage(DataContainer pNDPkg)
		{
			throw new NotImplementedException();
		}

		/// <summary>Add a new baseline deifnition to the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pRequestID">Identifier for the request.</param>
		/// <param name="pNBLDef">The nbl definition.</param>
		public override void setNewBaselineDefinition(Guid pRequestID, DataContainer pNBLDef)
		{
			throw new NotImplementedException();
		}

		/// <summary>Delete the baseline definition described by version.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pVersion">The baseline version.</param>
		public override void deleteBaselineDefinition(string pVersion)
		{
			throw new NotImplementedException();
		}

		/// <summary>Get all the baselines definitions from the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <returns>A DataContainer representing the list of baselines definitions.</returns>
		public override DataContainer getBaselinesDefinitions()
		{
			throw new NotImplementedException();
		}

		/// <summary>Get a baseline definition from the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pBLVersion">The baseline version.</param>
		/// <returns>A DataContainer representing the baseline definition.</returns>
		public override DataContainer getBaselineDefinition(string pBLVersion)
		{
			throw new NotImplementedException();
		}

		/// <summary>Get the list of the assigned baselines from the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <returns>A DataContainer representing the list of assigned baselines versions.</returns>
		public override DataContainer getAssignedBaselinesVersions()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the assigned current baseline version of an element from the data base .
		/// </summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pEID">The element Id.</param>
		/// <returns>The baseline version as a string.</returns>
		public override string getAssignedCurrentBaselineVersion(string pEID)
		{
			throw new NotImplementedException();
		}

		/// <summary>Get the assigned future baseline version of an element from the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pEID">The element Id.</param>
		/// <returns>The baseline version as a string.</returns>
		public override string getAssignedFutureBaselineVersion(string pEID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Get the list of data packages characteristics corresponding to a baseline definition.
		/// </summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pBLDef">The bl definition.</param>
		/// <returns>The list of data packages characteristics as a DataContainer.</returns>
		public override DataContainer getDataPackages(DataContainer pBLDef)
		{
			throw new NotImplementedException();
		}

		/// <summary>Get the list of all data packages characteristics.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <returns>The list of data packages characteristics as a DataContainer.</returns>
		public override DataContainer getDataPackagesList()
		{
			throw new NotImplementedException();
		}

		/// <summary>Return the data package characteristics as in data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pDPType">Type of the datapackage.</param>
		/// <param name="pDPVersion">The datapackage version.</param>
		/// <returns>The data package characteristics.</returns>
		public override DataContainer getDataPackageCharacteristics(string pDPType, string pDPVersion)
		{
			throw new NotImplementedException();
		}

		/// <summary>Return the definition of the current assigned baseline of an element.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pEID">The element Id.</param>
		/// <returns>The baseline definition.</returns>
		public override DataContainer getCurrentBaselineDefinition(string pEID)
		{
			throw new NotImplementedException();
		}

		/// <summary>Return the definition of the assigned baselines of an element.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pEID">The element Id.</param>
		/// <returns>The baselines definitions (Assigned Current and Assigned Future).</returns>
		public override DataContainer getElementBaselinesDefinitions(string pEID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Create an xml file corresponding to the Assigned Future baseline of an element and return the
		/// path where it creates it.
		/// </summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pReqId">Identifier for the request.</param>
		/// <param name="pEID">The element Id.</param>
		/// <param name="pBLVersion">The baseline version.</param>
		/// <param name="pActivationDate">Date of the activation.</param>
		/// <param name="pExpirationDate">Date of the expiration.</param>
		/// <returns>The path to baseline.xml file.</returns>
		public override string createBaselineFile(Guid pReqId, string pEID, string pBLVersion, string pActivationDate, string pExpirationDate)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Construct a differential package between two data packages and return an url to this package.
		/// </summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pReqID">Identifier for the request.</param>
		/// <param name="pEID">The element Id.</param>
		/// <param name="pDPType">Type of the datapackage.</param>
		/// <param name="pDPVersionOnBoard">The datapackage version board.</param>
		/// <param name="pDPVersionOnGround">The datapackage version ground.</param>
		/// <returns>Url of the differential package.</returns>
		public override string getDiffDataPackageUrl(Guid pReqID, string pEID, string pDPType, string pDPVersionOnBoard, string pDPVersionOnGround)
		{
			throw new NotImplementedException();
		}

		/// <summary>Assign a baseline to an element as a future baseline.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="ReqID">Identifier for the request.</param>
		/// <param name="pEID">The element Id.</param>
		/// <param name="pBLVersion">The baseline version.</param>
		/// <param name="pActDate">Date of the activation.</param>
		/// <param name="pExpDate">Date of the expiration.</param>
		public override void assignAFutureBaselineToElement(Guid ReqID, string pEID, string pBLVersion, DateTime pActDate, DateTime pExpDate)
		{
			throw new NotImplementedException();
		}

		/// <summary>Assign a baseline to an element as a current baseline.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="ReqID">Identifier for the request.</param>
		/// <param name="pEID">The element Id.</param>
		/// <param name="pBLVersion">The baseline version.</param>
		/// <param name="pExpDate">Date of the exponent.</param>
		public override void assignACurrentBaselineToElement(Guid ReqID, string pEID, string pBLVersion, DateTime pExpDate)
		{
			throw new NotImplementedException();
		}

		/// <summary>Deletes the baseline file.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pReqId">Identifier for the request.</param>
		/// <param name="pEID">The element Id.</param>
		public override void deleteBaselineFile(Guid pReqId, string pEID)
		{
			throw new NotImplementedException();
		}

		/// <summary>Deletes the Data Package.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pReqId">Identifier for the request.</param>
		/// <param name="pDPType">Type of the datapackage.</param>
		/// <param name="pDPVersion">The datapackage version.</param>
		public override void deleteDataPackage(Guid pReqId, string pDPType, string pDPVersion)
		{
			throw new NotImplementedException();
		}

		/// <summary>Deletes the data package difference file.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pReqId">Identifier for the request.</param>
		/// <param name="pEID">The element Id.</param>
		public override void deleteDataPackageDiffFile(Guid pReqId, string pEID)
		{
			throw new NotImplementedException();
		}

		/// <summary>Sets the undefined baseline parameters of an element.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pEID">The element Id.</param>
		/// <param name="pPisBasePackageVersion">The pis base package version.</param>
		/// <param name="pPisMissionPackageVersion">The pis mission package version.</param>
		/// <param name="pPisInfotainmentPackageVersion">The pis infotainment package version.</param>
		/// <param name="pLmtPackageVersion">The lmt package version.</param>
		public override void setElementUndefinedBaselineParams(string pEID, string pPisBasePackageVersion, string pPisMissionPackageVersion, string pPisInfotainmentPackageVersion, string pLmtPackageVersion)
		{
			throw new NotImplementedException();
		}

		/// <summary>Return all elements description form the "ElementsDataStore" table.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <returns>A DataContainer representing the list of elements description.</returns>
		public override DataContainer getElementsDescription()
		{
			throw new NotImplementedException();
		}

		/// <summary>Get undefined baselines list from the data base.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <returns>A DataContainer representing the list of undefined baselines parameters.</returns>
		public override DataContainer getUndefinedBaselinesList()
		{
			throw new NotImplementedException();
		}

		/// <summary>Unassign future baseline from an element.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pEID">The element Id.</param>
		public override void unassignFutureBaselineFromElement(string pEID)
		{
			throw new NotImplementedException();
		}

		/// <summary>Unassign current baseline from an element.</summary>
		/// <exception cref="NotImplementedException">Thrown when the requested operation is unimplemented.</exception>
		/// <param name="pEID">The element Id.</param>
		public override void unassignCurrentBaselineFromElement(string pEID)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}