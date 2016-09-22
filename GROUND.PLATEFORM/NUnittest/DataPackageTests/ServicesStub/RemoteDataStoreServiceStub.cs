﻿//---------------------------------------------------------------------------------------------------
// <copyright file="RemoteDataStoreServiceStub.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataPackageTests.Data;
using PIS.Ground.RemoteDataStore;
using Moq;
using PIS.Ground.DataPackage;
using System.ServiceModel;

namespace DataPackageTests.ServicesStub
{
    /// <summary>
    /// Simulate the implementation of RemoteDataStore service.
    /// </summary>
    class RemoteDataStoreServiceStub : IDisposable
    {
        #region Fields

        private Mock<IRemoteDataStore> _remoteDataStoreMock;

        private DataPackageCallbackService _datapackageCallbackService = new DataPackageCallbackService();

        private object _dataStoreLock = new object();

        /// <summary>
        /// The content of table ElementsDataStore in remoteDataStore indexed by system id
        /// </summary>
        private Dictionary<string, ElementsDataStoreData> _dataStoreElementsData = new Dictionary<string, ElementsDataStoreData>(10, StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The content of table BaselinesDataStore in remoteDataStore indexed by baseline version.
        /// </summary>
        private Dictionary<string, BaselinesDataStoreData> _dateStoreBaselinesData = new Dictionary<string, BaselinesDataStoreData>(10, StringComparer.Ordinal);

        #endregion

        #region Properties

        /// <summary>
        /// Gets the mock object that implement the remote data store..
        /// </summary>
        public Mock<IRemoteDataStore> Mock
        {
            get
            {
                return _remoteDataStoreMock;
            }
        }

        /// <summary>
        /// Gets the interface on the remote data store.
        /// </summary>
        public IRemoteDataStore Interface
        {
            get
            {
                return _remoteDataStoreMock.Object;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDataStoreServiceStub"/> class.
        /// </summary>
        public RemoteDataStoreServiceStub()
        {
            /* No logic body */
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether if the specified package type is valid.
        /// </summary>
        /// <param name="type">The type name to verify.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidPackageType(string type)
        {
            return Enum.IsDefined(typeof(DataPackageType), type);
        }


        /// <summary>
        /// Initializes the remote data store mock with default behavior.
        /// </summary>
        public void InitializeRemoteDataStoreMockWithDefaultBehavior()
        {
            _remoteDataStoreMock.Setup(r => r.getAllBaselineDistributingSavedRequests()).Returns(new DataContainer());
            _remoteDataStoreMock.Setup(r => r.getElementBaselinesDefinitions(It.IsAny<string>())).Returns<string>(GetElementBaselineDefinitionImplementation);
            _remoteDataStoreMock.Setup(r => r.getBaselineDefinition(It.IsAny<string>())).Returns<string>(GetBaselineDefinitionImplementation);
            _remoteDataStoreMock.Setup(r => r.checkIfBaselineExists(It.IsAny<string>())).Returns<string>(CheckIfBaselineExistsImplementation);
            _remoteDataStoreMock.Setup(r => r.checkDataPackagesAvailability(It.IsAny<Guid>(), It.IsAny<DataContainer>()));
            _remoteDataStoreMock.Setup(r => r.getDataPackageCharacteristics(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>(GetDataPackageCharacteristicsImplementation);
            _remoteDataStoreMock.Setup(r => r.checkIfElementExists(It.IsAny<string>())).Returns<string>(CheckIfElementExistsImplementation);
            _remoteDataStoreMock.Setup(r => r.createBaselineFile(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns<Guid, string, string, string, string>(CreateBaselineFileImplementation);
            _remoteDataStoreMock.Setup(r => r.checkIfDataPackageExists(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>(CheckIfDataPackageExistsImplementation);
        }

        /// <summary>
        /// Adds the baseline to remote data store.
        /// </summary>
        /// <param name="baselineVersion">The baseline version.</param>
        public void AddBaselineToRemoteDataStore(string baselineVersion)
        {
            BaselinesDataStoreData baseline = new BaselinesDataStoreData(baselineVersion);
            lock (_dataStoreLock)
            {
                _dateStoreBaselinesData.Add(baselineVersion, baseline);
            }
        }

        /// <summary>
        /// Updates the information on a specific element into the data store.
        /// </summary>
        /// <param name="data">The element data.</param>
        public void UpdateDataStore(ElementsDataStoreData data)
        {
            lock (_dataStoreLock)
            {
                _dataStoreElementsData[data.ElementID] = new ElementsDataStoreData(data);
            }
        }


        #endregion

        #region Private methods


        private string CreateBaselineFileImplementation(Guid requestId, string elementId, string baselineVersion, string activationDate, string expirationDate)
        {
            if (!CheckIfElementExistsImplementation(elementId))
            {
                throw new FaultException("Unknow ElementID", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString()));
            }

            BaselinesDataStoreData definition = GetBaselineDefinitionImplementation(baselineVersion);

            string outputPath = "/BaselinesDefinitions/" + requestId.ToString() + "/"
                + elementId + "/" + "baseline-" + definition.BaselineVersion + ".xml";
            return outputPath;

        }

        private bool CheckIfDataPackageExistsImplementation(string packageType, string version)
        {
            if (!IsValidPackageType(packageType))
            {
                throw new FaultException("Unknown DataPackage type", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_DATAPACKAGE_TYPE.ToString()));
            }

            return CheckIfBaselineExistsImplementation(version);
        }


        private DataContainer GetDataPackageCharacteristicsImplementation(string packageType, string version)
        {
            if (!IsValidPackageType(packageType))
            {
                throw new FaultException("Unknown DataPackage type", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_DATAPACKAGE_TYPE.ToString()));
            }
            else if (!CheckIfBaselineExistsImplementation(version))
            {
                throw new FaultException("Invalid DataPackage version", new FaultCode(RemoteDataStoreExceptionCodeEnum.INVALID_DATAPACKAGE_VERSION.ToString()));
            }

            return new DataPackagesDataStoreData((DataPackageType)Enum.Parse(typeof(DataPackageType), packageType), version);
        }

        private bool CheckIfElementExistsImplementation(string elementId)
        {
            lock (_dataStoreLock)
            {
                return _dataStoreElementsData.ContainsKey(elementId);
            }
        }

        private bool CheckIfBaselineExistsImplementation(string baseline)
        {
            lock (_dataStoreLock)
            {
                return _dateStoreBaselinesData.ContainsKey(baseline);
            }
        }

        private bool CheckDataPackagesAvailability(Guid requestId, DataContainer data)
        {
            BaselinesDataStoreData baselineData = new BaselinesDataStoreData(data);

            bool available = baselineData.BaselineVersion == baselineData.PISBaseDataPackageVersion &&
                baselineData.BaselineVersion == baselineData.PISInfotainmentDataPackageVersion &&
                baselineData.BaselineVersion == baselineData.PISMissionDataPackageVersion &&
                baselineData.BaselineVersion == baselineData.LMTDataPackageVersion &&
                CheckIfBaselineExistsImplementation(baselineData.BaselineVersion);

            if (!available)
            {
                Dictionary<string, string> restList = new Dictionary<string, string>(4);

                restList.Add("PISBASE", baselineData.PISBaseDataPackageVersion);
                restList.Add("PISMISSION", baselineData.PISMissionDataPackageVersion);
                restList.Add("PISINFOTAINMENT", baselineData.PISInfotainmentDataPackageVersion);
                restList.Add("LMT", baselineData.LMTDataPackageVersion);

                _datapackageCallbackService.missingDataPackageNotification(requestId, restList);
            }

            return available;
        }

        private ElementsDataStoreData GetElementBaselineDefinitionImplementation(string elementId)
        {
            lock (_dataStoreLock)
            {
                ElementsDataStoreData definition;
                if (_dataStoreElementsData.TryGetValue(elementId, out definition))
                {
                    return new ElementsDataStoreData(definition);
                }
            }

            throw new FaultException("Unknow ElementID", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_ELEMENT_ID.ToString()));
        }

        private BaselinesDataStoreData GetBaselineDefinitionImplementation(string baseline)
        {
            lock (_dataStoreLock)
            {
                BaselinesDataStoreData definition;
                if (_dateStoreBaselinesData.TryGetValue(baseline, out definition))
                {
                    return definition;
                }

                throw new FaultException("Unknown Baseline version", new FaultCode(RemoteDataStoreExceptionCodeEnum.UNKNOWN_BASELINE_VERSION.ToString()));
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _datapackageCallbackService = null;
                lock (_dataStoreLock)
                {
                    _dataStoreElementsData.Clear();
                    _dateStoreBaselinesData.Clear();
                }
            }
        }

        #endregion
    }
}
