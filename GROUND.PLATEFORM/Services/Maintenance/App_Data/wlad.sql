

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[sp_wlad]') AND type in (N'P', N'PC'))
  DROP PROCEDURE [dbo].[sp_wlad]
GO
CREATE PROCEDURE [dbo].[sp_wlad] 
	AS
BEGIN
  	select * from WladimirPerrad;
END
GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[sp_wlad2]') AND type in (N'P', N'PC'))
  DROP PROCEDURE [dbo].[sp_wlad2]
GO
CREATE PROCEDURE [dbo].[sp_wlad2] 
	AS
BEGIN
  	select * from WladimirPerrad where id=2;
END

GO
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[sp_GetTrainBaselineStatus]') AND type in (N'P', N'PC'))
  DROP PROCEDURE [dbo].[sp_GetTrainBaselineStatus]
GO
CREATE PROCEDURE [dbo].[sp_GetTrainBaselineStatus] 
	@BaselineProgressStatus int = NULL
AS
BEGIN
	IF @BaselineProgressStatus IS NOT NULL
	BEGIN
		-- Insert statements for procedure here
		SELECT TrainBaselineStatus.TrainID as TrainID,
			   TrainBaselineStatus.RequestId as RequestId,
               TrainBaselineStatus.TaskId as TaskId,
			   TrainBaselineStatus.TrainNumber as TrainNumber,
			   TrainBaselineStatus.OnlineStatus as OnlineStatus,
			   TrainBaselineStatus.BaselineProgressStatus as BaselineProgressStatus,
			   TrainBaselineStatus.CurrentBaselineVersion as CurrentBaselineVersion,
			   TrainBaselineStatus.FutureBaselineVersion as FutureBaselineVersion,
			   TrainBaselineStatus.PISOnBoardVersion as PISOnBoardVersion
		FROM TrainBaselineStatus
		WHERE TrainBaselineStatus.BaselineProgressStatus = @BaselineProgressStatus
        order by TrainBaselineStatus.TrainID;
	END
	ELSE
	BEGIN
		-- Insert statements for procedure here
		SELECT TrainBaselineStatus.TrainID as TrainID,
			   TrainBaselineStatus.RequestId as RequestId,
               TrainBaselineStatus.TaskId as TaskId,
			   TrainBaselineStatus.TrainNumber as TrainNumber,
			   TrainBaselineStatus.OnlineStatus as OnlineStatus,
			   TrainBaselineStatus.BaselineProgressStatus as BaselineProgressStatus,
			   TrainBaselineStatus.CurrentBaselineVersion as CurrentBaselineVersion,
			   TrainBaselineStatus.FutureBaselineVersion as FutureBaselineVersion,
			   TrainBaselineStatus.PISOnBoardVersion as PISOnBoardVersion
		FROM TrainBaselineStatus order by TrainBaselineStatus.TrainID;
	END
END
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TrainBaselineStatus]') AND type in (N'U'))
DROP TABLE [dbo].[TrainBaselineStatus]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TrainBaselineStatus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TrainBaselineStatus](
	[TrainId] [nvarchar](50) NOT NULL,
	[RequestId] [nvarchar](50) NOT NULL,
    [TaskId] [int] NOT NULL,
	[TrainNumber] [nvarchar](50) NOT NULL,
	[OnlineStatus] [bit] NOT NULL,
	[BaselineProgressStatus] [int] NOT NULL,
	[CurrentBaselineVersion] [nvarchar](50) NOT NULL,
	[FutureBaselineVersion] [nvarchar](50) NOT NULL,
	[PISOnBoardVersion] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TrainBaselineStatus] PRIMARY KEY CLUSTERED 
(
	[TrainId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

CREATE TABLE [dbo].[WladimirPerrad](id int NOT NULL, nom nvarchar(50), prenom nvarchar(50));
insert into WladimirPerrad(id, nom, prenom) values('1','perrad','wladimir');
insert into WladimirPerrad(id, nom, prenom) values('2','chokapic','De Nestle');
insert into WladimirPerrad(id, nom, prenom) values('3','Perrad-Sema','Hayden');
insert into WladimirPerrad(id, nom, prenom) values('4','Sema','Toungoulout');


/*
select * from WladimirPerrad;
-- Pour appeler la procedure stockee : 
exec sp_wlad;


-- Suppression :
drop TABLE [dbo].[WladimirPerrad];
drop PROCEDURE [dbo].[sp_wlad] ;
*/

