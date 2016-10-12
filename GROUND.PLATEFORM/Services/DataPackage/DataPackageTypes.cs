using System;
using System.Collections.Generic;
using System.Globalization;
using PIS.Ground.RemoteDataStore;

namespace PIS.Ground.DataPackage
{
    public enum BaselineCommandType
    {
        FORCE_FUTURE = 0,
        FORCE_ARCHIVED = 1,
        CLEAR_FORCING = 2
    }

    /// <summary>
    /// Class implementing methods to convert from DataTable from Remote Data Store to Data Packages types
    /// </summary>
    public class DataTypeConversion
    {
        /// <summary>
        /// Converts DataTable into BaselineDefinition
        /// </summary>
        /// <param name="pBLDef">Data table to convert.</param>
        /// <returns>The converted BaselineDefinition.</returns>
        public static BaselineDefinition fromDataContainerToBaselineDefinition(DataContainer pBLDef)
        {
            BaselineDefinition lBLDef = new BaselineDefinition();
            pBLDef.restart();
            pBLDef.read();
            for (int i = 0; i < pBLDef.fieldCount(); i++)
            {
                switch (pBLDef.getName(i))
                {
                    case "BaselineVersion" :
                        lBLDef.BaselineVersion = pBLDef.getValue(i);
                        break;
                    case "BaselineDescription" :
                        lBLDef.BaselineDescription = pBLDef.getValue(i);
                        break;
                    case "BaselineCreationDate" :
                        DateTime lTmpDT = new DateTime();
                        DateTime.TryParse(pBLDef.getValue(i), out lTmpDT);
                        lBLDef.BaselineCreationDate = lTmpDT;
                        break;
                    case "PISBaseDataPackageVersion" :
                    case "UndefinedBaselinePISBaseVersion":
                        lBLDef.PISBaseDataPackageVersion = pBLDef.getValue(i);
                        break;
                    case "PISMissionDataPackageVersion" :
                    case "UndefinedBaselinePISMissionVersion":
                        lBLDef.PISMissionDataPackageVersion = pBLDef.getValue(i);
                        break;
                    case "PISInfotainmentDataPackageVersion" :
                    case "UndefinedBaselinePISInfotainmentVersion":
                        lBLDef.PISInfotainmentDataPackageVersion = pBLDef.getValue(i);
                        break;
                    case "LMTDataPackageVersion" :
                    case "UndefinedBaselineLmtVersion":
                        lBLDef.LMTDataPackageVersion = pBLDef.getValue(i);
                        break;
                    default:
                        break;
                }
            }
            return lBLDef;
        }

        /// <summary>
        /// Converts DataTable into list of BaselineDefinition
        /// </summary>
        /// <param name="pBLDefs">Data table to convert</param>
        /// <returns>The converted BaselineDefinition list.</returns>
        public static List<BaselineDefinition> fromDataContainerToBaselinesDefinitionsList(DataContainer pBLDefs)
        {
            List<BaselineDefinition> lBLDefs = new List<BaselineDefinition>();
            pBLDefs.restart();
            while (pBLDefs.read())
            {
                BaselineDefinition lBLDef = new BaselineDefinition();
                for (int i = 0; i < pBLDefs.fieldCount(); i++)
                {
                    switch (pBLDefs.getName(i))
                    {
                        case "BaselineVersion":
                            lBLDef.BaselineVersion = pBLDefs.getValue(i);
                            break;
                        case "BaselineDescription":
                            lBLDef.BaselineDescription = pBLDefs.getValue(i);
                            break;
                        case "BaselineCreationDate":
                            DateTime lTmpDT = new DateTime();
                            DateTime.TryParse(pBLDefs.getValue(i), out lTmpDT);
                            lBLDef.BaselineCreationDate = lTmpDT;
                            break;
                        case "PISBaseDataPackageVersion":
                        case "UndefinedBaselinePISBaseVersion":
                            lBLDef.PISBaseDataPackageVersion = pBLDefs.getValue(i);
                            break;
                        case "PISMissionDataPackageVersion":
                        case "UndefinedBaselinePISMissionVersion":
                            lBLDef.PISMissionDataPackageVersion = pBLDefs.getValue(i);
                            break;
                        case "PISInfotainmentDataPackageVersion":
                        case "UndefinedBaselinePISInfotainmentVersion":
                            lBLDef.PISInfotainmentDataPackageVersion = pBLDefs.getValue(i);
                            break;
                        case "LMTDataPackageVersion":
                        case "UndefinedBaselineLmtVersion":
                            lBLDef.LMTDataPackageVersion = pBLDefs.getValue(i);
                            break;
                        default:
                            break;
                    }
                }
                lBLDefs.Add(lBLDef);   
            }
            return lBLDefs;
        }

        /// <summary>
        /// Converts DataTable into DataPackagesCharacteristics
        /// </summary>
        /// <param name="pDPChar">Data table to convert</param>
        /// <returns>The converted DataPackagesCharacteristics.</returns>
        public static DataPackagesCharacteristics fromDataContainerToDataPackagesCharacteristics(DataContainer pDPChar)
        {
            DataPackagesCharacteristics lDPChar = new DataPackagesCharacteristics();
            pDPChar.restart();
            pDPChar.read();
            for (int i = 0; i < pDPChar.fieldCount(); i++)
            {
                switch (pDPChar.getName(i))
                {
                    case "DataPackageType":
                        lDPChar.DataPackageType = (DataPackageType)Enum.Parse(typeof(DataPackageType), pDPChar.getValue(i));
                        break;
                    case "DataPackageVersion":
                        lDPChar.DataPackageVersion = pDPChar.getValue(i);
                        break;
                    case "DataPackagePath":
                        lDPChar.DataPackagePath = pDPChar.getValue(i);
                        break;
                    default:
                        break;
                }
            }
            return lDPChar;        
        }

        /// <summary>
        /// Converts DataTable into DataPackagesCharacteristics list
        /// </summary>
        /// <param name="pDPChars">Data table to convert</param>
        /// <returns>The converted DataPackagesCharacteristics list.</returns>
        public static List<DataPackagesCharacteristics> fromDataContainerToDataPackagesCharacteristicsList(DataContainer pDPChars)
        {
            List<DataPackagesCharacteristics> lDPChars = new List<DataPackagesCharacteristics>();
            pDPChars.restart();
            while (pDPChars.read())
            {
                DataPackagesCharacteristics lDPChar = new DataPackagesCharacteristics();
                for (int i = 0; i < pDPChars.fieldCount(); i++)
                {
                    switch (pDPChars.getName(i))
                    {
                        case "DataPackageType":
                            lDPChar.DataPackageType = (DataPackageType)Enum.Parse(typeof(DataPackageType), pDPChars.getValue(i));
                            break;
                        case "DataPackageVersion":
                            lDPChar.DataPackageVersion = pDPChars.getValue(i);
                            break;
                        case "DataPackagePath":
                            lDPChar.DataPackagePath = pDPChars.getValue(i);
                            break;
                        default:
                            break;
                    }
                }
                lDPChars.Add(lDPChar);
            }
            return lDPChars;
        }

        /// <summary>
        /// Converts from DataContainer to ElementDescription.
        /// </summary>
        /// <param name="pElDescrCont">The Element description as DataContainer.</param>
        /// <returns>The ElementDescription instance.</returns>
        public static ElementDescription fromDataContainerToElementDescription(DataContainer pElDescrCont)
        {
            ElementDescription lElDescr = new ElementDescription();
            pElDescrCont.restart();
            pElDescrCont.read();
            for (int i = 0; i < pElDescrCont.fieldCount(); i++)
            {
                switch (pElDescrCont.getName(i))
                {
                    case "ElementID":
                        lElDescr.ElementID = pElDescrCont.getValue(i);
                        break;
                    case "AssignedCurrentBaseline":
                        lElDescr.AssignedCurrentBaseline = pElDescrCont.getValue(i);
                        break;
                    case "AssignedCurrentBaselineExpirationDate":
                        DateTime ltmpdt = new DateTime();
                        DateTime.TryParse(pElDescrCont.getValue(i), out ltmpdt);
                        lElDescr.AssignedCurrentBaselineExpirationDate = ltmpdt;
                        break;
                    case "AssignedFutureBaseline":
                        lElDescr.AssignedFutureBaseline = pElDescrCont.getValue(i);
                        break;
                    case "AssignedFutureBaselineActivationDate":
                        ltmpdt = new DateTime();
                        DateTime.TryParse(pElDescrCont.getValue(i), out ltmpdt);
                        lElDescr.AssignedFutureBaselineActivationDate = ltmpdt;
                        break;
                    case "AssignedFutureBaselineExpirationDate":
                        ltmpdt = new DateTime();
                        DateTime.TryParse(pElDescrCont.getValue(i), out ltmpdt);
                        lElDescr.AssignedFutureBaselineExpirationDate = ltmpdt;
                        break;
                    case "UndefinedBaselinePISBaseVersion":
                        lElDescr.UndefinedBaselinePisBaseVersion = pElDescrCont.getValue(i);
                        break;
                    case "UndefinedBaselinePISMissionVersion":
                        lElDescr.UndefinedBaselinePisMissionVersion = pElDescrCont.getValue(i);
                        break;
                    case "UndefinedBaselinePISInfotainmentVersion":
                        lElDescr.UndefinedBaselinePisInfotainmentVersion = pElDescrCont.getValue(i);
                        break;
                    case "UndefinedBaselineLmtVersion":
                        lElDescr.UndefinedBaselineLmtVersion = pElDescrCont.getValue(i);
                        break;
                    default:
                        break;
                }
            }
            return lElDescr;
        }

        /// <summary>
        /// Converts from DataContainer to List of ElementDescription
        /// </summary>
        /// <param name="pElDescrCont">The list as DataContainer</param>
        /// <returns>The list of ElementDescription</returns>
        public static List<ElementDescription> fromDataContainerToElementDescriptionList(DataContainer pElDescrCont)
        {
            List<ElementDescription> lElDescrs = new List<ElementDescription>();
            pElDescrCont.restart();
            while (pElDescrCont.read())
            {
                ElementDescription lElDescr = new ElementDescription();
                for (int i = 0; i < pElDescrCont.fieldCount(); i++)
                {
                    switch (pElDescrCont.getName(i))
                    {
                        case "ElementID":
                            lElDescr.ElementID = pElDescrCont.getValue(i);
                            break;
                        case "AssignedCurrentBaseline":
                            lElDescr.AssignedCurrentBaseline = pElDescrCont.getValue(i);
                            break;
                        case "AssignedCurrentBaselineExpirationDate":
                            DateTime ltmpdt = new DateTime();
                            DateTime.TryParse(pElDescrCont.getValue(i), out ltmpdt);
                            lElDescr.AssignedCurrentBaselineExpirationDate = ltmpdt;
                            break;
                        case "AssignedFutureBaseline":
                            lElDescr.AssignedFutureBaseline = pElDescrCont.getValue(i);
                            break;
                        case "AssignedFutureBaselineActivationDate":
                            ltmpdt = new DateTime();
                            DateTime.TryParse(pElDescrCont.getValue(i), out ltmpdt);
                            lElDescr.AssignedFutureBaselineActivationDate = ltmpdt;
                            break;
                        case "AssignedFutureBaselineExpirationDate":
                            ltmpdt = new DateTime();
                            DateTime.TryParse(pElDescrCont.getValue(i), out ltmpdt);
                            lElDescr.AssignedFutureBaselineExpirationDate = ltmpdt;
                            break;
                        case "UndefinedBaselinePISBaseVersion":
                            lElDescr.UndefinedBaselinePisBaseVersion = pElDescrCont.getValue(i);
                            break;
                        case "UndefinedBaselinePISMissionVersion":
                            lElDescr.UndefinedBaselinePisMissionVersion = pElDescrCont.getValue(i);
                            break;
                        case "UndefinedBaselinePISInfotainmentVersion":
                            lElDescr.UndefinedBaselinePisInfotainmentVersion = pElDescrCont.getValue(i);
                            break;
                        case "UndefinedBaselineLmtVersion":
                            lElDescr.UndefinedBaselineLmtVersion = pElDescrCont.getValue(i);
                            break;
                        default:
                            break;
                    }                    
                }
                lElDescrs.Add(lElDescr);
            }
            return lElDescrs;
        }
        
        /// <summary>
        /// Converts from BaselineDefinition to DataContainer.
        /// </summary>
        /// <param name="pBLDef">The BaselineDefinition instance</param>
        /// <returns>The resulting DataContainer</returns>
        public static DataContainer fromBaselineDefinitionToDataContainer(BaselineDefinition pBLDef)
        {
            DataContainer lBlDefCont = new DataContainer();
            lBlDefCont.Columns = new List<string>();
            lBlDefCont.Rows = new List<string>();
            lBlDefCont.Columns.Add("BaselineVersion");
            lBlDefCont.Columns.Add("BaselineDescription");
            lBlDefCont.Columns.Add("BaselineCreationDate");
            lBlDefCont.Columns.Add("PISBaseDataPackageVersion");
            lBlDefCont.Columns.Add("PISMissionDataPackageVersion");
            lBlDefCont.Columns.Add("PISInfotainmentDataPackageVersion");
            lBlDefCont.Columns.Add("LMTDataPackageVersion");

            lBlDefCont.Rows.Add(pBLDef.BaselineVersion);
            lBlDefCont.Rows.Add(pBLDef.BaselineDescription);
            lBlDefCont.Rows.Add(pBLDef.BaselineCreationDate.ToString());
            lBlDefCont.Rows.Add(pBLDef.PISBaseDataPackageVersion);
            lBlDefCont.Rows.Add(pBLDef.PISMissionDataPackageVersion);
            lBlDefCont.Rows.Add(pBLDef.PISInfotainmentDataPackageVersion);
            lBlDefCont.Rows.Add(pBLDef.LMTDataPackageVersion);
            
            return lBlDefCont;

        }

        /// <summary>
        /// Converts from List of BaselineDefinition to DataContainer.
        /// </summary>
        /// <param name="pBLDef">The BaselineDefinition list.</param>
        /// <returns>The resulting DataContainer</returns>
        public static DataContainer fromBaselinesDefinitionsListToDataContainer(List<BaselineDefinition> pBLDefs)
        {
            DataContainer lBlDefsCont = new DataContainer();
            lBlDefsCont.Columns = new List<string>();
            lBlDefsCont.Rows = new List<string>();
            lBlDefsCont.Columns.Add("BaselineVersion");
            lBlDefsCont.Columns.Add("BaselineDescription");
            lBlDefsCont.Columns.Add("BaselineCreationDate");
            lBlDefsCont.Columns.Add("PISBaseDataPackageVersion");
            lBlDefsCont.Columns.Add("PISMissionDataPackageVersion");
            lBlDefsCont.Columns.Add("PISInfotainmentDataPackageVersion");
            lBlDefsCont.Columns.Add("LMTDataPackageVersion");

            foreach (BaselineDefinition lBLDef in pBLDefs)
            {
                lBlDefsCont.Rows.Add(lBLDef.BaselineVersion);
                lBlDefsCont.Rows.Add(lBLDef.BaselineDescription);
                lBlDefsCont.Rows.Add(lBLDef.BaselineCreationDate.ToString());
                lBlDefsCont.Rows.Add(lBLDef.PISBaseDataPackageVersion);
                lBlDefsCont.Rows.Add(lBLDef.PISMissionDataPackageVersion);
                lBlDefsCont.Rows.Add(lBLDef.PISInfotainmentDataPackageVersion);
                lBlDefsCont.Rows.Add(lBLDef.LMTDataPackageVersion);
            }

            return lBlDefsCont;
        }

        /// <summary>
        /// Converts from DataPackageCharacteristics to DataContainer.
        /// </summary>
        /// <param name="pDPChar">The DataPackageCharacteristics instance.</param>
        /// <returns>The resulting DataContainer.</returns>
        public static DataContainer fromDataPackagesCharacteristicsToDataContainer(DataPackagesCharacteristics pDPChar)
        {
            DataContainer lDPCharCont = new DataContainer();
            lDPCharCont.Columns = new List<string>();
            lDPCharCont.Rows = new List<string>();
            lDPCharCont.Columns.Add("DataPackageType");
            lDPCharCont.Columns.Add("DataPackageVersion");
            lDPCharCont.Columns.Add("DataPackagePath");

            lDPCharCont.Rows.Add(pDPChar.DataPackageType.ToString());
            lDPCharCont.Rows.Add(pDPChar.DataPackageVersion);
            lDPCharCont.Rows.Add(pDPChar.DataPackagePath);

            return lDPCharCont;
        }

        /// <summary>
        /// Converts from DataPackageCharacteristics to DataContainer.
        /// </summary>
        /// <param name="pDPChar">The DataPackageCharacteristics instance.</param>
        /// <returns>The resulting DataContainer.</returns>
        public static DataContainer fromDataPackagesCharacteristicsListToDataContainer(List<DataPackagesCharacteristics> pDPChars)
        {
            DataContainer lDPCharsCont = new DataContainer();
            lDPCharsCont.Columns = new List<string>();
            lDPCharsCont.Rows = new List<string>();
            lDPCharsCont.Columns.Add("DataPackageType");
            lDPCharsCont.Columns.Add("DataPackageVersion");
            lDPCharsCont.Columns.Add("DataPackagePath");

            foreach (DataPackagesCharacteristics lDPChar in pDPChars)
            {

                lDPCharsCont.Rows.Add(lDPChar.DataPackageType.ToString());
                lDPCharsCont.Rows.Add(lDPChar.DataPackageVersion);
                lDPCharsCont.Rows.Add(lDPChar.DataPackagePath);

            }
            return lDPCharsCont;
        }

        /// <summary>
        /// Converts from ElementDescription to DataContainer.
        /// </summary>
        /// <param name="pElDescr">The ElementDescription instance.</param>
        /// <returns>The resulting DataContainer.</returns>
        public static DataContainer fromElementDescriptionToDataContainer(ElementDescription pElDescr)
        {
            DataContainer lElDescrCont = new DataContainer();
            lElDescrCont.Columns = new List<string>();
            lElDescrCont.Rows = new List<string>();
            lElDescrCont.Columns.Add("ElementID");
            lElDescrCont.Columns.Add("AssignedCurrentBaseline");
            lElDescrCont.Columns.Add("AssignedCurrentBaselineExpirationDate");
            lElDescrCont.Columns.Add("AssignedFutureBaseline");
            lElDescrCont.Columns.Add("AssignedFutureBaselineActivationDate");
            lElDescrCont.Columns.Add("AssignedFutureBaselineExpirationDate");
            lElDescrCont.Columns.Add("UndefinedBaselinePISBaseVersion");
            lElDescrCont.Columns.Add("UndefinedBaselinePISMissionVersion");
            lElDescrCont.Columns.Add("UndefinedBaselinePISInfotainmentVersion");
            lElDescrCont.Columns.Add("UndefinedBaselineLmtVersion");

            lElDescrCont.Rows.Add(pElDescr.ElementID);
            lElDescrCont.Rows.Add(pElDescr.AssignedCurrentBaseline);
            lElDescrCont.Rows.Add(pElDescr.AssignedCurrentBaselineExpirationDate.ToString());
            lElDescrCont.Rows.Add(pElDescr.AssignedFutureBaseline);
            lElDescrCont.Rows.Add(pElDescr.AssignedFutureBaselineActivationDate.ToString());
            lElDescrCont.Rows.Add(pElDescr.AssignedFutureBaselineExpirationDate.ToString());
            lElDescrCont.Rows.Add(pElDescr.UndefinedBaselinePisBaseVersion.ToString());
            lElDescrCont.Rows.Add(pElDescr.UndefinedBaselinePisMissionVersion.ToString());
            lElDescrCont.Rows.Add(pElDescr.UndefinedBaselinePisInfotainmentVersion.ToString());
            lElDescrCont.Rows.Add(pElDescr.UndefinedBaselineLmtVersion.ToString());
        
            return lElDescrCont;
        }

        /// <summary>
        /// Converts from List of ElementDescription to DataContainer.
        /// </summary>
        /// <param name="pElDescr">The ElementDescription list.</param>
        /// <returns>The resulting DataContainer.</returns>
        public static DataContainer fromElementDescriptionListToDataContainer(List<ElementDescription> pElDescrs)
        {
            DataContainer lElDescrCont = new DataContainer();
            lElDescrCont.Columns = new List<string>();
            lElDescrCont.Rows = new List<string>();
            lElDescrCont.Columns.Add("ElementID");
            lElDescrCont.Columns.Add("AssignedCurrentBaseline");
            lElDescrCont.Columns.Add("AssignedCurrentBaselineExpirationDate");
            lElDescrCont.Columns.Add("AssignedFutureBaseline");
            lElDescrCont.Columns.Add("AssignedFutureBaselineActivationDate");
            lElDescrCont.Columns.Add("AssignedFutureBaselineExpirationDate");
            lElDescrCont.Columns.Add("UndefinedBaselinePISBaseVersion");
            lElDescrCont.Columns.Add("UndefinedBaselinePISMissionVersion");
            lElDescrCont.Columns.Add("UndefinedBaselinePISInfotainmentVersion");
            lElDescrCont.Columns.Add("UndefinedBaselineLmtVersion");

            foreach (ElementDescription lElDescr in pElDescrs)
            {
                lElDescrCont.Rows.Add(lElDescr.ElementID);
                lElDescrCont.Rows.Add(lElDescr.AssignedCurrentBaseline);
                lElDescrCont.Rows.Add(lElDescr.AssignedCurrentBaselineExpirationDate.ToString());
                lElDescrCont.Rows.Add(lElDescr.AssignedFutureBaseline);
                lElDescrCont.Rows.Add(lElDescr.AssignedFutureBaselineActivationDate.ToString());
                lElDescrCont.Rows.Add(lElDescr.AssignedFutureBaselineExpirationDate.ToString());
                lElDescrCont.Rows.Add(lElDescr.UndefinedBaselinePisBaseVersion.ToString());
                lElDescrCont.Rows.Add(lElDescr.UndefinedBaselinePisMissionVersion.ToString());
                lElDescrCont.Rows.Add(lElDescr.UndefinedBaselinePisInfotainmentVersion.ToString());
                lElDescrCont.Rows.Add(lElDescr.UndefinedBaselineLmtVersion.ToString());
            }
            return lElDescrCont;
        }

		/// <summary>
		/// Initializes this object from the given from baseline distributing request to data
		/// container.
		/// </summary>
		/// <param name="processBaselineDistributingRequest">The process baseline distributing request.</param>
		/// <returns>Data container instance filled with converted data</returns>
		public static DataContainer fromBaselineDistributingRequestToDataContainer(RequestMgt.BaselineDistributingRequestContext processBaselineDistributingRequest)
		{
			DataContainer baselineDistributingTask = new DataContainer();
			baselineDistributingTask.Columns = new List<string>();
			baselineDistributingTask.Rows = new List<string>();
			baselineDistributingTask.Columns.Add("ElementID");
			baselineDistributingTask.Columns.Add("RequestID");
			baselineDistributingTask.Columns.Add("TransferMode");
			baselineDistributingTask.Columns.Add("FileCompression");
			baselineDistributingTask.Columns.Add("TransferDate");
			baselineDistributingTask.Columns.Add("TransferExpirationDate");
			baselineDistributingTask.Columns.Add("Priority");
			baselineDistributingTask.Columns.Add("Incremental");
			baselineDistributingTask.Columns.Add("BaselineVersion");
			baselineDistributingTask.Columns.Add("BaselineActivationDate");
			baselineDistributingTask.Columns.Add("BaselineExpirationDate");

			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.ElementId);
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.RequestId.ToString());
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.DistributionAttributes.TransferMode.ToString());
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.DistributionAttributes.fileCompression.ToString());
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.DistributionAttributes.transferDate.ToString());
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.DistributionAttributes.transferExpirationDate.ToString());
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.DistributionAttributes.priority.ToString());
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.IsIncremental.ToString());
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.BaselineVersion);
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.BaselineActivationDate.ToString());
			baselineDistributingTask.Rows.Add(processBaselineDistributingRequest.BaselineExpirationDate.ToString());
			
			return baselineDistributingTask;
		}

		/// <summary>From data container to baseline distributing saved requests list.</summary>
		/// <param name="baselineDistributingSavedRequestsContainer">The baseline distributing saved
		/// requests container.</param>
		/// <param name="requestContextFactory">The request context factory.</param>
		/// <returns>List of requests created from data container data.</returns>
		internal static List<Core.Data.IRequestContext> fromDataContainerToBaselineDistributingSavedRequestsList(DataContainer baselineDistributingSavedRequestsContainer, RequestMgt.IRequestContextFactory requestContextFactory)
		{
			List<Core.Data.IRequestContext> baselineDistributingSavedRequestsList = new List<Core.Data.IRequestContext>();
			baselineDistributingSavedRequestsContainer.restart();

			while (baselineDistributingSavedRequestsContainer.read())
			{
				string elementId = string.Empty;
				Guid requestId = Guid.Empty;
				Core.Data.FileTransferMode transferMode = PIS.Ground.Core.Data.FileTransferMode.AnyBandwidth;
				bool fileCompression = false, isIncremental = false;
				DateTime transferDate = DateTime.MinValue;
				DateTime transferExpirationDate = DateTime.MaxValue;
				sbyte priority = 0;
				string baselineVersion = string.Empty;
				DateTime baselineActivationDate = DateTime.MinValue;
				DateTime baselineExpirationDate = DateTime.MaxValue;

				for (int i = 0; i < baselineDistributingSavedRequestsContainer.fieldCount(); i++)
				{
					switch (baselineDistributingSavedRequestsContainer.getName(i))
					{
						case "ElementID":
							elementId = baselineDistributingSavedRequestsContainer.getValue(i);
							break;
						case "RequestID":
							requestId = new Guid(baselineDistributingSavedRequestsContainer.getValue(i));
							break;
						case "TransferMode":
							transferMode = (Core.Data.FileTransferMode)Enum.Parse(typeof(Core.Data.FileTransferMode), baselineDistributingSavedRequestsContainer.getValue(i));
							break;
						case "FileCompression":
							fileCompression = bool.Parse(baselineDistributingSavedRequestsContainer.getValue(i));
							break;
						case "TransferDate":
							DateTime.TryParse(baselineDistributingSavedRequestsContainer.getValue(i), out transferDate);
							break;
						case "TransferExpirationDate":
							DateTime.TryParse(baselineDistributingSavedRequestsContainer.getValue(i), out transferExpirationDate);
							break;
						case "Priority":
							priority = sbyte.Parse(baselineDistributingSavedRequestsContainer.getValue(i));
							break;
						case "Incremental":
							isIncremental = bool.Parse(baselineDistributingSavedRequestsContainer.getValue(i));
							break;
						case "BaselineVersion":
							baselineVersion = baselineDistributingSavedRequestsContainer.getValue(i);
							break;
						case "BaselineActivationDate":
							DateTime.TryParse(baselineDistributingSavedRequestsContainer.getValue(i), out baselineActivationDate);
							break;
						case "BaselineExpirationDate":
							DateTime.TryParse(baselineDistributingSavedRequestsContainer.getValue(i), out baselineExpirationDate);
							break;
						default:
							break;
					}
				}

				baselineDistributingSavedRequestsList.Add(
					requestContextFactory.CreateBaselineDistributingRequestContext(
						elementId,
						requestId,
						transferMode,
						fileCompression,
						isIncremental,
						transferDate,
						transferExpirationDate,
						priority,
						baselineVersion,
						baselineActivationDate,
						baselineExpirationDate
					)
				);
			}

			return baselineDistributingSavedRequestsList;
		}
	}        
}