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