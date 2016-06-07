--BaselineDataStore Table store informations for each Baseline definition
CREATE TABLE [BaselinesDataStore] (
    [BaselineVersion] text PRIMARY KEY NOT NULL,
    [BaselineDescription] text,
    [BaselineCreationDate] text NOT NULL,
    [PISBaseDataPackageVersion] text NOT NULL,
    [PISMissionDataPackageVersion] text NOT NULL,
    [PISInfotainmentDataPackageVersion] text NOT NULL,
    [LMTDataPackageVersion] text NOT NULL
);

--DataPackageDataStore store informations about each data package (PISBASE, PISMISSION,...)
CREATE TABLE [DataPackagesDataStore] (
    [DataPackageType] text NOT NULL,
    [DataPackageVersion] text NOT NULL,
    [DataPackagePath] text,
    [DataPackageLastOpenDate] text,
    CONSTRAINT [PK_DataPackagesDataStore] PRIMARY KEY ([DataPackageType], [DataPackageVersion])
);

--ElementsDataStore store data about assigned baselines for elementID
CREATE TABLE [ElementsDataStore] (
    [ElementID] text PRIMARY KEY NOT NULL,
    [AssignedCurrentBaseline] text,
    [AssignedCurrentBaselineExpirationDate] text,
    [AssignedFutureBaseline] text,
    [AssignedFutureBaselineActivationDate] text,
    [AssignedFutureBaselineExpirationDate] text,
    [UndefinedBaselinePISBaseVersion] text,
    [UndefinedBaselinePISMissionVersion] text,
    [UndefinedBaselinePISInfotainmentVersion] text,
    [UndefinedBaselineLmtVersion] text
);

--BaselineDistributingTasksDataStore store data about baselines distribution tasks for elementID
CREATE TABLE [BaselineDistributingTasksDataStore] (
    [ElementID] text PRIMARY KEY NOT NULL,
    [RequestID] text NOT NULL,
    [TransferMode] text NOT NULL,
    [FileCompression] text NOT NULL,
    [TransferDate] text NOT NULL,
    [TransferExpirationDate] text NOT NULL,
    [Priority] int NOT NULL,
    [Incremental] text NOT NULL,
    [BaselineVersion] text NOT NULL,
    [BaselineActivationDate] text,
    [BaselineExpirationDate] text
);