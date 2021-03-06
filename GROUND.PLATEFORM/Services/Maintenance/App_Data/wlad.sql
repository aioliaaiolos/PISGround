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
drop table TrainBaselineStatus;
drop PROCEDURE [dbo].[sp_wlad] ;
*/

GO
/****** Object:  ForeignKey [FK_CommandStatus_CommandType]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CommandStatus_CommandType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CommandStatus]'))
ALTER TABLE [dbo].[CommandStatus] DROP CONSTRAINT [FK_CommandStatus_CommandType]
GO
/****** Object:  ForeignKey [FK_CommandStatus_StatusType]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CommandStatus_StatusType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CommandStatus]'))
ALTER TABLE [dbo].[CommandStatus] DROP CONSTRAINT [FK_CommandStatus_StatusType]
GO
/****** Object:  ForeignKey [FK_MessageRequest_CommandType]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageRequest_CommandType]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRequest]'))
ALTER TABLE [dbo].[MessageRequest] DROP CONSTRAINT [FK_MessageRequest_CommandType]
GO
/****** Object:  ForeignKey [FK_MessageRequest_MsgContext]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageRequest_MsgContext]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRequest]'))
ALTER TABLE [dbo].[MessageRequest] DROP CONSTRAINT [FK_MessageRequest_MsgContext]
GO
/****** Object:  ForeignKey [FK_MessageStatus_CommandStatus]    Script Date: 07/13/2012 14:26:24 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageStatus_CommandStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageStatus]'))
ALTER TABLE [dbo].[MessageStatus] DROP CONSTRAINT [FK_MessageStatus_CommandStatus]
GO
/****** Object:  ForeignKey [FK_MessageStatus_MessageRequest]    Script Date: 07/13/2012 14:26:24 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageStatus_MessageRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageStatus]'))
ALTER TABLE [dbo].[MessageStatus] DROP CONSTRAINT [FK_MessageStatus_MessageRequest]
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateMessageRequest]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateMessageRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_UpdateMessageRequest]
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateMessageStatus]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateMessageStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_UpdateMessageStatus]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertLogMessage]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertLogMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_InsertLogMessage]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNewestMessage]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetNewestMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetNewestMessage]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetOldestMessage]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetOldestMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetOldestMessage]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertMessageStatus]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertMessageStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_InsertMessageStatus]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteAllMessage]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteAllMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_DeleteAllMessage]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteMessage]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_DeleteMessage]
GO

/****** Object: StoredProcedure [dbo].[sp_GetPendingMessages]  Script Date: 22/10/2015 11:00:00 *****/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetPendingMessages]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetPendingMessages]
GO
/****** Object:  StoredProcedure [dbo].[sp_DeletePendingMessages]    Script Date: 11/05/2015 13:43:00 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeletePendingMessages]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_DeletePendingMessages]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllMessages]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllMessages]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetAllMessages]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetOldestMessageDateTime]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetOldestMessageDateTime]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetOldestMessageDateTime]
GO
/****** Object:  Table [dbo].[MessageStatus]    Script Date: 07/13/2012 14:26:24 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageStatus_CommandStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageStatus]'))
ALTER TABLE [dbo].[MessageStatus] DROP CONSTRAINT [FK_MessageStatus_CommandStatus]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageStatus_MessageRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageStatus]'))
ALTER TABLE [dbo].[MessageStatus] DROP CONSTRAINT [FK_MessageStatus_MessageRequest]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageStatus]') AND type in (N'U'))
DROP TABLE [dbo].[MessageStatus]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNewestMessageDateTime]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetNewestMessageDateTime]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetNewestMessageDateTime]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertMessageRequest]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertMessageRequest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_InsertMessageRequest]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertStatusToPendingMessages]    Script Date: 10/23/2015 12:00:00 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertStatusToPendingMessages]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_InsertStatusToPendingMessages]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetStatusIdByName]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetStatusIdByName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetStatusIdByName]
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdateDatabaseVersion]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertUpdateDatabaseVersion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_InsertUpdateDatabaseVersion]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetCommandIdByCommandName]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetCommandIdByCommandName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetCommandIdByCommandName]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDatabaseVersion]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetDatabaseVersion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_GetDatabaseVersion]
GO
/****** Object:  Table [dbo].[MessageRequest]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageRequest_CommandType]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRequest]'))
ALTER TABLE [dbo].[MessageRequest] DROP CONSTRAINT [FK_MessageRequest_CommandType]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageRequest_MsgContext]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRequest]'))
ALTER TABLE [dbo].[MessageRequest] DROP CONSTRAINT [FK_MessageRequest_MsgContext]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageRequest]') AND type in (N'U'))
DROP TABLE [dbo].[MessageRequest]
GO
/****** Object:  Table [dbo].[CommandStatus]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CommandStatus_CommandType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CommandStatus]'))
ALTER TABLE [dbo].[CommandStatus] DROP CONSTRAINT [FK_CommandStatus_CommandType]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CommandStatus_StatusType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CommandStatus]'))
ALTER TABLE [dbo].[CommandStatus] DROP CONSTRAINT [FK_CommandStatus_StatusType]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommandStatus]') AND type in (N'U'))
DROP TABLE [dbo].[CommandStatus]
GO
/****** Object:  Table [dbo].[CommandType]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommandType]') AND type in (N'U'))
DROP TABLE [dbo].[CommandType]
GO
/****** Object:  Table [dbo].[DataBaseVersion]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataBaseVersion]') AND type in (N'U'))
DROP TABLE [dbo].[DataBaseVersion]
GO
/****** Object:  Table [dbo].[MessageContext]    Script Date: 07/13/2012 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageContext]') AND type in (N'U'))
DROP TABLE [dbo].[MessageContext]
GO
/****** Object:  StoredProcedure [dbo].[sp_PopFirstWord]    Script Date: 07/13/2012 14:26:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PopFirstWord]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_PopFirstWord]
GO
/****** Object:  Table [dbo].[StatusType]    Script Date: 07/13/2012 14:26:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StatusType]') AND type in (N'U'))
DROP TABLE [dbo].[StatusType]
GO









/* Wlad flag */









/****** Object:  Table [dbo].[StatusType]    Script Date: 10/20/2015 17:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StatusType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StatusType](
	[StatusId] [int] IDENTITY(1,1) NOT NULL,
	[Status] [nvarchar](50) NOT NULL,
	[IsFinal] bit NOT NULL DEFAULT 0
 CONSTRAINT [PK_StatusType] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET IDENTITY_INSERT [dbo].[StatusType] ON
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (1,  N'NoStatus', 0)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (2,  N'MsgProcessing', 0)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (3,  N'MsgWaitingToSend', 0)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (4,  N'MsgReceived', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (5,  N'MsgSent', 0)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (6,  N'MsgTimedOut', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (7,  N'MsgCanceledByStartupError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (8,  N'MsgCanceled', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (9,  N'MsgInvalidTemplateError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (10, N'MsgInvalidScheduledPeriodError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (11, N'MsgInvalidRepetitionCountError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (12, N'MsgInvalidTemplateFileError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (13, N'MsgUnknownCarIdError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (14, N'MsgInvalidDelayError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (15, N'MsgInvalidDelayReasonError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (16, N'MsgInvalidHourError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (17, N'MsgUndefinedStationIdError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (18, N'MsgInvalidTextError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (19, N'MsgLimitExceededError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (20, N'MsgInhibited', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (21, N'MsgUnexpectedError', 1)
INSERT [dbo].[StatusType] ([StatusId], [Status], [IsFinal]) VALUES (22, N'CanceledGroundOnly', 1)
SET IDENTITY_INSERT [dbo].[StatusType] OFF

/****** Object:  StoredProcedure [dbo].[sp_PopFirstWord]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_PopFirstWord]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 8th June 2012
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_PopFirstWord] 
    @SourceString nvarchar(max) output,
    @FirstWord    nvarchar(4000) output
AS
BEGIN
	SET NOCOUNT ON;
	-- Procedure accepts a comma delimited string as the first parameter
    -- Procedure outputs the first word in the second parameter
    -- Procedure outputs the remainder of the delimeted string in the first parameter
    -- Procedure yields the length of the First Word as the return value

    DECLARE @Oldword        nvarchar(4000)
    DECLARE @Length         int
    DECLARE @CommaLocation  int

    SELECT @Oldword = @SourceString

    IF NOT @Oldword IS NULL
    BEGIN
        SELECT @CommaLocation = CHARINDEX('','',@Oldword)
        SELECT @Length = DATALENGTH(@Oldword)

        IF @CommaLocation = 0
        BEGIN
            SELECT @FirstWord = @Oldword
            SELECT @SourceString = NULL

            RETURN @Length
        END

        SELECT @FirstWord = SUBSTRING(@Oldword, 1, @CommaLocation -1)
        SELECT @SourceString = SUBSTRING(@Oldword, @CommaLocation + 1, @Length - @CommaLocation)

        RETURN @Length - @CommaLocation
    END

    RETURN 0
END
' 
END
GO
/****** Object:  Table [dbo].[MessageContext]    Script Date: 07/13/2012 14:26:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageContext]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MessageContext](
	[MessageContextId] [int] IDENTITY(1,1) NOT NULL,
	[RequestID] [nvarchar](50) NOT NULL,
	[Context] [nvarchar](max) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
 CONSTRAINT [PK_MsgContext] PRIMARY KEY CLUSTERED 
(
	[MessageContextId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[DataBaseVersion]    Script Date: 07/13/2012 14:26:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataBaseVersion]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataBaseVersion](
	[Version] [nvarchar](12) NOT NULL
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[CommandType]    Script Date: 07/13/2012 14:26:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommandType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CommandType](
	[CommandID] [int] IDENTITY(1,1) NOT NULL,
	[Command] [nvarchar](50) NULL,
 CONSTRAINT [PK_CommandType] PRIMARY KEY CLUSTERED 
(
	[CommandID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET IDENTITY_INSERT [dbo].[CommandType] ON
INSERT [dbo].[CommandType] ([CommandID], [Command]) VALUES (1, N'SendScheduledMsg')
INSERT [dbo].[CommandType] ([CommandID], [Command]) VALUES (2, N'CancelScheduledMsg')
SET IDENTITY_INSERT [dbo].[CommandType] OFF
/****** Object:  Table [dbo].[CommandStatus]    Script Date: 07/13/2012 14:26:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommandStatus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CommandStatus](
	[CommandStatusId] [int] IDENTITY(1,1) NOT NULL,
	[StatusId] [int] NOT NULL,
	[CommandId] [int] NOT NULL,
 CONSTRAINT [PK_CommandStatus] PRIMARY KEY CLUSTERED 
(
	[CommandStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
INSERT INTO CommandStatus (StatusId, CommandId)
SELECT st.StatusId, ct.CommandId FROM StatusType st
CROSS JOIN CommandType AS ct
/****** Object:  Table [dbo].[MessageRequest]    Script Date: 07/13/2012 14:26:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageRequest]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MessageRequest](
	[MessageRequestID] [int] IDENTITY(1,1) NOT NULL,
	[MessageContextId] [int] NOT NULL,
	[TrainID] [nvarchar](50) NOT NULL,
	[DateTime] [smalldatetime] NOT NULL,
	[CommandID] [int] NOT NULL,
	[RequestID] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_MessageRequest] PRIMARY KEY CLUSTERED 
(
	[MessageRequestID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDatabaseVersion]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetDatabaseVersion]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		BV Vishwanath
-- Create date: 
-- Description:	Select database version
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetDatabaseVersion] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [Version] from  DataBaseVersion
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetCommandIdByCommandName]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetCommandIdByCommandName]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 13th June 2012
-- Description:	Get the CommandID based on CommandName
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetCommandIdByCommandName] 
	-- Add the parameters for the stored procedure here
	@CommandName nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
   DECLARE @CommandID int
   SELECT @CommandID=CommandType.CommandID from CommandType where CommandType.Command=@CommandName
   RETURN @CommandID
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdateDatabaseVersion]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertUpdateDatabaseVersion]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Bv Vishwanath
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertUpdateDatabaseVersion] 
	-- Add the parameters for the stored procedure here
	@dbVersion nvarchar(12) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
DECLARE @Version nvarchar(12);
DECLARE @Length         int

    -- Insert statements for procedure here
	SELECT @Version=[Version] from  DataBaseVersion
	SELECT @Length = DATALENGTH(@Version)
	IF @Length > 0
		BEGIN
		update DataBaseVersion set [Version]=@dbVersion
		end
		else
		begin
		Insert INTO DataBaseVersion ([Version]) values (@dbVersion)
		end
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetStatusIdByName]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetStatusIdByName]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 13th June 2012
-- Description:	Get the Status ID based on Status Name
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetStatusIdByName] 
	-- Add the parameters for the stored procedure here
	@StatusName nvarchar(50) 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @StatusId int
	SELECT @StatusId=StatusType.StatusId from StatusType where StatusType.[Status]=@StatusName
	RETURN @StatusId
END
' 
END
GO



-- Procedure qui pose probleme : sp_InsertMessageRequest

/* ***** Object:  StoredProcedure [dbo].[sp_InsertMessageRequest]    Script Date: 07/13/2012 14:26:18 ***** */
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertMessageRequest]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 13th June 2012
-- Description:	Get the Status ID based on Status Name
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertMessageRequest] 
	-- Add the parameters for the stored procedure here
	@StatusName nvarchar(50),

	@MessageContextId int, 
	@RequestID nvarchar(50) = 0, 
	@TrainID nvarchar(50) = 0,
	@CmdTypeID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	DECLARE @StatusId int
	SELECT @StatusId=StatusType.StatusId from StatusType where StatusType.[Status]=@StatusName
	
	INSERT INTO MessageRequest (MessageContextId, TrainID, DateTime, CommandID, RequestID) VALUES ( @MessageContextId, @TrainID, GETUTCDATE(), @CmdTypeID, @RequestId );
	
	RETURN @StatusId
END
' 
END
GO




/****** Object:  StoredProcedure [dbo].[sp_GetTrainBaselineStatus]    Script Date: 04/24/2014 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetTrainBaselineStatus]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Yoshiaki Date
-- Create date: 24th April 2014
-- Description:	Storeprocedure to get Train Baseline
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetTrainBaselineStatus] 
	@BaselineProgressStatus int = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

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
' 
END
GO


/****** Object:  StoredProcedure [dbo].[sp_GetNewestMessageDateTime]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetNewestMessageDateTime]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 7th June 2012
-- Description:	Storeprocedure to get the oldest and newest message
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetNewestMessageDateTime] 
	@CmdType nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @CmdTypeID int;
	DECLARE @Length INT;
    DECLARE @Firstcmd NVARCHAR(4000);
    DECLARE @ContextID int;
    --DECLARE @CommandExe NVARCHAR(4000);
    SELECT @Length = DATALENGTH(@CmdType);
    --DECLARE @CommaLocation  INT
    --SELECT @Commands='''';
    CREATE TABLE #CommandTyp (ID int)
    
    WHILE @Length > 0
	BEGIN
		EXECUTE @Length = sp_PopFirstWord @CmdType OUTPUT, @Firstcmd OUTPUT

		IF @Length > 0
		BEGIN
			EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @Firstcmd
			Insert INTO #CommandTyp (ID) values (@CmdTypeID)
			--SELECT @Commands=@Commands+CONVERT(NVARCHAR(10),@CmdTypeID)+'','';
		END 
	END
	
	SELECT TOP (1) @ContextID=MsgCntx.MessageContextId
	FROM 
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	WHERE MsgReq.[DateTime] = (Select Max(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID in (SELECT ID from #CommandTyp))
	
	print (@ContextID)
	
	--Msg Command
	SELECT MsgReq.TrainID, MsgReq.RequestID, MsgReq.MessageRequestID, MsgReq.MessageContextId, CmdTyp.Command, MsgReq.[DateTime]
	FROM 
	MessageContext MsgCntx 
	INNER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	INNER JOIN
		CommandType CmdTyp ON MsgReq.CommandID=CmdTyp.CommandID
	WHERE MsgCntx.MessageContextId = @ContextID
	
	
	drop TABLE #CommandTyp

END
' 
END
GO


/* Wlad flag : error */


/****** Object:  Table [dbo].[MessageStatus]    Script Date: 07/13/2012 14:26:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MessageStatus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MessageStatus](
	[MessageRequestID] [int] NOT NULL,
	[CommandStatusId] [int] NOT NULL,
	[DateTime] [smalldatetime] NOT NULL
) ON [PRIMARY]
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetOldestMessageDateTime]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetOldestMessageDateTime]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 7th June 2012
-- Description:	Storeprocedure to get the oldest and newest message
-- =============================================
create PROCEDURE [dbo].[sp_GetOldestMessageDateTime] 
	@CmdType nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @CmdTypeID int;
	DECLARE @Length INT;
    DECLARE @Firstcmd NVARCHAR(4000);
    DECLARE @ContextID int;
    --DECLARE @CommandExe NVARCHAR(4000);
    SELECT @Length = DATALENGTH(@CmdType);
    --DECLARE @CommaLocation  INT
    --SELECT @Commands='''';
    CREATE TABLE #CommandTyp (ID int)
    
    WHILE @Length > 0
	BEGIN
		EXECUTE @Length = sp_PopFirstWord @CmdType OUTPUT, @Firstcmd OUTPUT

		IF @Length > 0
		BEGIN
			EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @Firstcmd
			Insert INTO #CommandTyp (ID) values (@CmdTypeID)
			--SELECT @Commands=@Commands+CONVERT(NVARCHAR(10),@CmdTypeID)+'','';
		END 
	END
	
	SELECT TOP (1) @ContextID=MsgCntx.MessageContextId
	FROM 
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	WHERE MsgReq.[DateTime] = (Select Min(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID in (SELECT ID from #CommandTyp))
	
	print (@ContextID)
	
	--Msg Command
	SELECT MsgReq.TrainID, MsgReq.RequestID, MsgReq.MessageRequestID, MsgReq.MessageContextId, CmdTyp.Command, MsgReq.[DateTime]
	FROM 
	MessageContext MsgCntx 
	INNER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	INNER JOIN
		CommandType CmdTyp ON MsgReq.CommandID=CmdTyp.CommandID
	WHERE MsgCntx.MessageContextId = @ContextID
	
	
	drop TABLE #CommandTyp

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAllMessages]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAllMessages]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanth
-- Create date: 6th June 2012
-- Description:	storeprocedure to select message between dates
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetAllMessages] 
	@StartDate datetime , 
	@EndDate datetime,
	@CmdType nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @CmdTypeID int;
	DECLARE @Length INT;
    DECLARE @Firstcmd NVARCHAR(4000);
    SELECT @Length = DATALENGTH(@CmdType);

    CREATE TABLE #CommandTyp (ID int)
    
    WHILE @Length > 0
	BEGIN
		EXECUTE @Length = sp_PopFirstWord @CmdType OUTPUT, @Firstcmd OUTPUT

		IF @Length > 0
		BEGIN
			EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @Firstcmd
			Insert INTO #CommandTyp (ID) values (@CmdTypeID)
		END 
	END
	
    --Msg Context
	SELECT distinct MsgCntx.MessageContextId, MsgCntx.Context, MsgCntx.RequestID, MsgCntx.StartDate, MsgCntx.EndDate
	FROM 
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	WHERE MsgReq.[DateTime] >= @StartDate and MsgReq.[DateTime] <= @EndDate and MsgReq.CommandID in (SELECT ID from #CommandTyp)
	
	--Msg Request
	--SELECT MsgReq.TrainID,MsgReq.MessageContextId,MsgReq.RequestID
	--FROM 
	--MessageRequest MsgReq 
	--WHERE MsgReq.[DateTime] >= @StartDate and MsgReq.[DateTime] <= @EndDate and MsgReq.CommandID=@CmdTypeID
	
	--Msg Command
	SELECT distinct MsgReq.TrainID, MsgReq.RequestID, MsgReq.MessageRequestID, MsgReq.MessageContextId, CmdTyp.Command, MsgReq.[DateTime]
	FROM  
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	INNER JOIN
		CommandType CmdTyp ON MsgReq.CommandID=CmdTyp.CommandID
	WHERE MsgReq.[DateTime] >= @StartDate and MsgReq.[DateTime] <= @EndDate and MsgReq.CommandID in (SELECT ID from #CommandTyp)
	
	--Msg Status
	SELECT distinct MsgReq.TrainID, MsgReq.MessageRequestID, MsgSts.[DateTime], Stat.[Status]
	FROM 
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	LEFT OUTER JOIN
		MessageStatus MsgSts ON MsgReq.MessageRequestID = MsgSts.MessageRequestID  
	INNER JOIN
		CommandStatus CmdStsTyp ON MsgSts.CommandStatusId=CmdStsTyp.CommandStatusId
	INNER JOIN
		CommandType CmdTyp ON CmdStsTyp.CommandID=CmdTyp.CommandID
	INNER JOIN
		StatusType Stat ON CmdStsTyp.StatusId=Stat.StatusId
	WHERE MsgReq.[DateTime] >= @StartDate and MsgReq.[DateTime] <= @EndDate and MsgReq.CommandID in (SELECT ID from #CommandTyp)
		
END
' 
END
GO


/* Wlad flag : error */


/******* Object: Stored Procedure [dbo].[sp_GetPendingMessages]    Script date: 10/26/2015 11:00:00 *****/

-- =================================================================
-- Author:		C. Mailloux
-- Create date: 22th October, 2015
-- Description:	Stored procedure that get the message context identifier
--              to all messages that are not in final state.
-- =================================================================
CREATE PROCEDURE [dbo].[sp_GetPendingMessages] 
AS
BEGIN
	SELECT MR.MessageContextId FROM MessageRequest as MR
	INNER JOIN MessageStatus as MS ON MS.MessageRequestID = MR.MessageRequestID
	INNER JOIN CommandStatus as CS ON CS.CommandStatusId = MS.CommandStatusId
	INNER JOIN  StatusType as ST ON ST.StatusId = CS.StatusId
	GROUP BY (MR.MessageContextId)
	HAVING COALESCE(SUM(CAST(ST.IsFinal as INT)), 0) = 0
	UNION
	SELECT  MR.MessageContextId FROM MessageRequest as MR
	LEFT OUTER JOIN MessageStatus as MS ON MS.MessageRequestID = MR.MessageRequestID
	WHERE MS.MessageRequestID IS NULL
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertStatusToPendingMessages]    Script Date: 10/22/2015 11:40:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =================================================================
-- Author:		C. Mailloux
-- Create date: 22th October, 2015
-- Description:	Stored procedure that insert a status record 
--              to all messages that are not in final state
-- =================================================================
CREATE PROCEDURE [dbo].sp_InsertStatusToPendingMessages	
	@Status nvarchar(50)
AS
BEGIN
		SET NOCOUNT ON

		DECLARE @StatusId int

		EXECUTE @StatusId = [dbo].sp_GetStatusIdByName @Status
		CREATE TABLE #MessageContextIdTbl (ContextId int)
		
		Insert INTO #MessageContextIdTbl (ContextId) 
		exec [dbo].sp_GetPendingMessages

		SET NOCOUNT OFF

		INSERT INTO [dbo].MessageStatus (MessageRequestID, CommandStatusId, [DateTime])
		SELECT MessageRequestID, @StatusId, GETUTCDATE() 
		FROM MessageRequest 
		INNER JOIN #MessageContextIdTbl ON MessageContextID = ContextId 
		GROUP BY MessageRequestID
END
GO

/****** Object:  StoredProcedure [dbo].[sp_DeleteMessage]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 9th June
-- Description:	Store procedure to delete message
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteMessage] 
	@RecordCount int,
	@PercentageToCleanUpInLogDatabase int
AS
BEGIN

	SET NOCOUNT ON;
	BEGIN TRANSACTION
	DECLARE @MessagesContextCount int;
	DECLARE @DelCount int;
	--declare @CmdTypeID int;
	
	Select @MessagesContextCount=COUNT(MessageContext.MessageContextId) from MessageContext;
	IF @MessagesContextCount > @RecordCount
	BEGIN
	    set @DelCount = @MessagesContextCount*@PercentageToCleanUpInLogDatabase/100;
	    
		--EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @CmdType
		
		CREATE TABLE #MessageContextIdTbl (ContextId int)

		Insert INTO #MessageContextIdTbl SELECT TOP (@DelCount) MessageContextId FROM MessageContext
		
		delete from MessageStatus where MessageRequestID in (SELECT MessageRequestID FROM MessageRequest where MessageContextId in (SELECT ContextId from #MessageContextIdTbl))
		
		delete from MessageRequest where MessageContextId in ((SELECT ContextId from #MessageContextIdTbl))
		
		delete from MessageContext where MessageContextId in (SELECT ContextId from #MessageContextIdTbl)
		
		IF @@error<>0 
		BEGIN
			ROLLBACK TRANSACTION
			RETURN @@error
		END
		ELSE	
		BEGIN
			COMMIT TRANSACTION
			RETURN 0
		END
	END
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeletePendingMessages]    Script Date: 11/05/2015 13:43:00 ******/
/******																Update Script Date: 22/10/2015 11:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeletePendingMessages]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		S.Barbot
-- Create date: 9th June
-- Description:	Store procedure to delete pending messages
-- =======================================================
CREATE PROCEDURE [dbo].[sp_DeletePendingMessages] 
AS
BEGIN

	SET NOCOUNT ON;
	BEGIN TRANSACTION

	BEGIN
		CREATE TABLE #MessageContextIdTbl (ContextId int)
		
		Insert INTO #MessageContextIdTbl exec [dbo].sp_GetPendingMessages
		
		delete from MessageStatus where MessageRequestID in (SELECT MessageRequestID FROM MessageRequest where MessageContextId in (SELECT ContextId from #MessageContextIdTbl))
		
		delete from MessageRequest where MessageContextId in ((SELECT ContextId from #MessageContextIdTbl))
		
		delete from MessageContext where MessageContextId in (SELECT ContextId from #MessageContextIdTbl)
		
		IF @@error<>0 
		BEGIN
			ROLLBACK TRANSACTION
			RETURN @@error
		END
		ELSE	
		BEGIN
			COMMIT TRANSACTION
			RETURN 0
		END
	END
END
' 
END
GO








/* Wlad flag : error */








/****** Object:  StoredProcedure [dbo].[sp_DeleteAllMessage]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteAllMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 9th June
-- Description:	Store procedure to delete message
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteAllMessage] 
	@CmdType nvarchar(50)
AS
BEGIN
	SET NOCOUNT OFF;
	BEGIN TRANSACTION
	declare @CmdTypeID int;
	
		EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @CmdType
		
		CREATE TABLE #MessageContextIdTbl (ContextId int)

		Insert INTO #MessageContextIdTbl SELECT Distinct(MessageContextId) FROM MessageRequest where CommandID=@CmdTypeID
		
		delete from MessageStatus where MessageRequestID in (SELECT MessageRequestID FROM MessageRequest where MessageContextId in (SELECT ContextId from #MessageContextIdTbl))
				
		delete FROM MessageRequest where MessageContextId in (SELECT ContextId from #MessageContextIdTbl)
		
		delete from MessageContext where MessageContextId in (SELECT ContextId from #MessageContextIdTbl)
		
		drop TABLE #MessageContextIdTbl
		
		IF @@error<>0 
		BEGIN
			ROLLBACK TRANSACTION
			RETURN @@error
		END
		ELSE	
		BEGIN
			COMMIT TRANSACTION
			RETURN 0
		END
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertMessageStatus]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_InsertMessageStatus]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 7th June 2012
-- Description:	Storeprocedure to insert into message status
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertMessageStatus] 
	-- Add the parameters for the stored procedure here
	@MessageRequestID int, 
	@CommandStatusId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	INSERT INTO MessageStatus (MessageRequestID, CommandStatusId , DateTime)
	VALUES (@MessageRequestID, @CommandStatusId, GETUTCDATE());
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetOldestMessage]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetOldestMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 7th June 2012
-- Description:	Storeprocedure to get the oldest and newest message
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetOldestMessage] 
	@CmdType nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @CmdTypeID int;
	DECLARE @Length INT;
    DECLARE @Firstcmd NVARCHAR(4000);
    SELECT @Length = DATALENGTH(@CmdType);

    CREATE TABLE #CommandTyp (ID int)
    
    WHILE @Length > 0
	BEGIN
		EXECUTE @Length = sp_PopFirstWord @CmdType OUTPUT, @Firstcmd OUTPUT

		IF @Length > 0
		BEGIN
			EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @Firstcmd
			Insert INTO #CommandTyp (ID) values (@CmdTypeID)
		END 
	END
	
	 --Msg Context
	SELECT TOP (1) MsgCntx.Context, MsgCntx.RequestID, MsgCntx.MessageContextId, MsgCntx.StartDate, MsgCntx.EndDate
	FROM 
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	WHERE MsgReq.[DateTime] = (Select Min(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID in (SELECT ID from #CommandTyp))
	
	--Msg Request
	--SELECT MsgReq.TrainID,MsgReq.MessageContextId,MsgReq.RequestID
	--FROM 
	--MessageRequest MsgReq 
	--WHERE MsgReq.[DateTime] = (Select Min(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID=@CmdTypeID)
	
	--Msg Command
	SELECT MsgReq.TrainID, MsgReq.RequestID, MsgReq.MessageRequestID, MsgReq.MessageContextId, CmdTyp.Command, MsgReq.[DateTime]
	FROM 
	MessageRequest MsgReq 
	INNER JOIN
		CommandType CmdTyp ON MsgReq.CommandID=CmdTyp.CommandID
	WHERE MsgReq.[DateTime] = (Select Min(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID in (SELECT ID from #CommandTyp))
	
	--Msg Status
	SELECT MsgReq.TrainID, MsgReq.MessageRequestID, MsgSts.[DateTime], Stat.[Status]
	FROM 
	MessageRequest MsgReq 
	LEFT OUTER JOIN
		MessageStatus MsgSts ON MsgReq.MessageRequestID = MsgSts.MessageRequestID  
	INNER JOIN
		CommandStatus CmdStsTyp ON MsgSts.CommandStatusId=CmdStsTyp.CommandStatusId
	INNER JOIN
		CommandType CmdTyp ON CmdStsTyp.CommandID=CmdTyp.CommandID
	INNER JOIN
		StatusType Stat ON CmdStsTyp.StatusId=Stat.StatusId
	WHERE MsgReq.[DateTime] = (Select Min(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID in (SELECT ID from #CommandTyp))
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetNewestMessage]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetNewestMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		B.V.Vishwanath
-- Create date: 7th June 2012
-- Description:	Storeprocedure to get the oldest and newest message
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetNewestMessage] 
	@CmdType nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @CmdTypeID int;
	DECLARE @Length INT;
    DECLARE @Firstcmd NVARCHAR(4000);
    DECLARE @ContextID int;
    --DECLARE @CommandExe NVARCHAR(4000);
    SELECT @Length = DATALENGTH(@CmdType);
    --DECLARE @CommaLocation  INT
    --SELECT @Commands='''';
    CREATE TABLE #CommandTyp (ID int)
    
    WHILE @Length > 0
	BEGIN
		EXECUTE @Length = sp_PopFirstWord @CmdType OUTPUT, @Firstcmd OUTPUT

		IF @Length > 0
		BEGIN
			EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @Firstcmd
			Insert INTO #CommandTyp (ID) values (@CmdTypeID)
			--SELECT @Commands=@Commands+CONVERT(NVARCHAR(10),@CmdTypeID)+'','';
		END 
	END
	
	--SELECT @CommaLocation = CHARINDEX('','' , @Commands)	
	
	--Set @CommandExe=''''
	--IF @CommaLocation > 0
 --   BEGIN
	--	print (@Commands)
	--	print (@CommaLocation)
	--	SELECT @Length = DATALENGTH(@Commands);
	--	print (@Length)
	--	SELECT @CommandExe = SUBSTRING(@Commands, 1, @Length -5)
	--END
	
	--print (@CommandExe)
	SELECT TOP (1) @ContextID=MsgCntx.MessageContextId
	FROM 
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	WHERE MsgReq.[DateTime] = (Select Max(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID in (SELECT ID from #CommandTyp))
	
	print (@ContextID)
	 --Msg Context
	SELECT TOP (1) MsgCntx.Context, MsgCntx.RequestID, MsgCntx.MessageContextId, MsgCntx.StartDate, MsgCntx.EndDate
	FROM 
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	WHERE MsgReq.[DateTime] = (Select Max(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID in (SELECT ID from #CommandTyp))
	
	--Msg Request
	--SELECT MsgReq.TrainID,MsgReq.MessageContextId,MsgReq.RequestID
	--FROM 
	--MessageRequest MsgReq 
	--WHERE MsgReq.[DateTime] = (Select Max(MessageRequest.[DateTime]) from MessageRequest where MessageRequest.CommandID=@CmdTypeID)
	
	--Msg Command
	SELECT MsgReq.TrainID, MsgReq.RequestID, MsgReq.MessageRequestID, MsgReq.MessageContextId, CmdTyp.Command, MsgReq.[DateTime]
	FROM 
	MessageContext MsgCntx 
	INNER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	INNER JOIN
		CommandType CmdTyp ON MsgReq.CommandID=CmdTyp.CommandID
	WHERE MsgCntx.MessageContextId = @ContextID
	
	--Msg Status
	SELECT MsgReq.TrainID, MsgReq.MessageRequestID, MsgSts.[DateTime], Stat.[Status]
	FROM 
	MessageContext MsgCntx 
	LEFT OUTER JOIN
		MessageRequest MsgReq ON MsgCntx.MessageContextId = MsgReq.MessageContextId 
	LEFT OUTER JOIN
		MessageStatus MsgSts ON MsgReq.MessageRequestID = MsgSts.MessageRequestID  
	INNER JOIN
		CommandStatus CmdStsTyp ON MsgSts.CommandStatusId=CmdStsTyp.CommandStatusId
	INNER JOIN
		CommandType CmdTyp ON CmdStsTyp.CommandID=CmdTyp.CommandID
	INNER JOIN
		StatusType Stat ON CmdStsTyp.StatusId=Stat.StatusId
	WHERE MsgCntx.MessageContextId = @ContextID
	
	drop TABLE #CommandTyp

END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertLogMessage]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		B.V.Vishwanath, Carl Mailloux
-- Create date: 7th June 2012
-- Last update: 26th November 2015
-- Description:	Store procedure to insert schedule message
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertLogMessage] 
	@RequestID nvarchar(50), 
	@Context nvarchar(4000),
	@TrainId nvarchar(50),
	@CmdType nvarchar(50),
	@StartDate datetime,
	@EndDate datetime,
	@RecordCount int,
	@PercentageToCleanUpInLogDatabase int,
	@Status nvarchar(50),
	@AllowUpdate BIT = FALSE
AS
BEGIN
	SET NOCOUNT OFF;
	 
    DECLARE @MessagesRequestID INT;
    DECLARE @MessagesContextID int;
    declare @CmdTypeID int
    declare @StatusId int
    declare @CmdStatusId int
    DECLARE @MessagesContextCount int;
    
    BEGIN TRANSACTION
    
    IF @RecordCount IS NOT NULL AND @PercentageToCleanUpInLogDatabase IS NOT NULL
    BEGIN
		SELECT @MessagesContextCount=COUNT(MessageContext.MessageContextId) from MessageContext;
		IF @MessagesContextCount > @RecordCount
		BEGIN
			EXECUTE sp_DeleteMessage @RecordCount, @PercentageToCleanUpInLogDatabase
		END
	END
	    
    SELECT @MessagesContextID = 0;
    
    SELECT @MessagesContextID = MessageContext.MessageContextId
        FROM MessageContext WHERE MessageContext.RequestID=@RequestID;
   
    IF @MessagesContextID = 0
    BEGIN
		INSERT INTO MessageContext (RequestID, Context, StartDate, EndDate)
		VALUES (@RequestId, @Context, @StartDate, @EndDate);
		
		SELECT @MessagesContextID = @@IDENTITY    
	END
	ELSE
	BEGIN
		IF @AllowUpdate = 1
		BEGIN
			UPDATE MessageContext
			SET
				Context = @Context,
				StartDate = @StartDate,
				EndDate = @EndDate
			WHERE 
				MessageContextId = @MessagesContextID;
		END
    END

	EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @CmdType
	print @CmdTypeID
	EXECUTE @StatusId = sp_GetStatusIdByName @Status
	SELECT @CmdStatusId = CommandStatus.CommandStatusId
		FROM CommandStatus where CommandStatus.CommandId=@CmdTypeID and CommandStatus.StatusId=@StatusId;
		
	SELECT @MessagesRequestID = 0;	
	EXECUTE @MessagesRequestID = sp_InsertMessageRequest @MessagesContextID, @RequestID, @TrainId, @CmdTypeID
    
	IF @MessagesRequestID > 0
	BEGIN
		EXECUTE sp_InsertMessageStatus @MessagesRequestID, @CmdStatusId
	END 
	
	IF @@error<>0 
	BEGIN
		ROLLBACK TRANSACTION
		RETURN @@error
	END
	ELSE	
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateMessageStatus]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Authors:		B.V.Vishwanath, C. Mailloux
-- Create date: 7th June 2012
-- Update date: 25th november 2015
-- Description:	Storeprocedure to update message status
-- Return value: -1 on error
--               0 if no message request is associated to @RequestID
--               1 if record was added
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateMessageStatus] 
	-- Add the parameters for the stored procedure here
	@TrainId nvarchar(50), 
	@RequestID nvarchar(50) ,
	@Status nvarchar(50),
	@CmdType nvarchar(50)

AS
BEGIN
	SET NOCOUNT OFF;
	DECLARE @MessageRequestID int;
	declare @CmdTypeID int
    declare @StatusId int
    declare @CmdStatusId int
    declare @ReturnValue int
    
    SET @ReturnValue=-1;
    
    SET @MessageRequestID =0
    SET @CmdTypeID =0
    
    SELECT @MessageRequestID = MessageRequest.MessageRequestID, @CmdTypeID = MessageRequest.CommandID
        FROM MessageRequest 
		INNER JOIN CommandType CT ON CT.CommandID = MessageRequest.CommandID 
		WHERE MessageRequest.RequestID = @RequestID AND MessageRequest.TrainID=@TrainId AND CT.Command = @CmdType;
        
    EXECUTE @StatusId = sp_GetStatusIdByName @Status
	SELECT @CmdStatusId = CommandStatus.CommandStatusId
					FROM CommandStatus where CommandStatus.CommandId=@CmdTypeID and CommandStatus.StatusId=@StatusId;
        
    IF @MessageRequestID > 0 and @CmdStatusId > 0
	BEGIN
		EXECUTE sp_InsertMessageStatus @MessageRequestID, @CmdStatusId
		SET @ReturnValue = 1
	END 
	ELSE
	BEGIN
		IF @MessageRequestId < 1
		BEGIN
			SET @ReturnValue = 0
		END
	END

	return @ReturnValue
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateMessageRequest]    Script Date: 07/13/2012 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		B.V.Vishwanath, C. Mailloux
-- Create date: 7th June 2012
-- Last update: 26th november 2015
-- Description:	Storeprocedure to update MessageRequest
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateMessageRequest] 
	-- Add the parameters for the stored procedure here
	@RequestId nvarchar(50), 
	@TrainID nvarchar(50),
	@CmdType nvarchar(50),
	@Status nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	declare @CmdTypeID int
    declare @StatusId int
    declare @CmdStatusId int
    DECLARE @MessagesContextID int;
    DECLARE @MessagesRequestID INT;
    
    BEGIN TRANSACTION
		
	SET @MessagesContextID = 0;
	SELECT @MessagesContextID = MessageContext.MessageContextId
        FROM MessageContext where MessageContext.RequestID=@RequestID;
   
    if @MessagesContextID > 0
    BEGIN		
		EXECUTE @CmdTypeID = sp_GetCommandIdByCommandName @CmdType
		EXECUTE @StatusId = sp_GetStatusIdByName @Status
		SELECT @CmdStatusId = CommandStatus.CommandStatusId
			FROM CommandStatus where CommandStatus.CommandId=@CmdTypeID and CommandStatus.StatusId=@StatusId;
    
		EXECUTE @MessagesRequestID = sp_InsertMessageRequest @MessagesContextID, @RequestID, @TrainId, @CmdTypeID		          
		IF @MessagesRequestID > 0
		BEGIN
			EXECUTE sp_InsertMessageStatus @MessagesRequestID, @CmdStatusId
		END 
	END
	ELSE
	BEGIN
		EXEC sp_InsertLogMessage 	@RequestId,  
									'_AutoGenerated_Message_To_Track_CancelScheduledMessage_Request_', -- Context
									@TrainId, -- Train id
									@CmdType, -- CommandType
									'20000101', -- StartDate
									'20000101', -- EndDate
									NULL,		-- RecordCount
									NULL,		-- PercentageToCleanUpInLogDatabase
									@Status,	-- Status
									FALSE;		-- Allow Update
	END
	IF @@error<>0 
	BEGIN
		ROLLBACK TRANSACTION
		RETURN @@error
	END
	ELSE	
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END
END
GO

/****** Object:  ForeignKey [FK_CommandStatus_CommandType]    Script Date: 07/13/2012 14:26:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CommandStatus_CommandType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CommandStatus]'))
ALTER TABLE [dbo].[CommandStatus]  WITH CHECK ADD  CONSTRAINT [FK_CommandStatus_CommandType] FOREIGN KEY([CommandId])
REFERENCES [dbo].[CommandType] ([CommandID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CommandStatus_CommandType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CommandStatus]'))
ALTER TABLE [dbo].[CommandStatus] CHECK CONSTRAINT [FK_CommandStatus_CommandType]
GO
/****** Object:  ForeignKey [FK_CommandStatus_StatusType]    Script Date: 07/13/2012 14:26:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CommandStatus_StatusType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CommandStatus]'))
ALTER TABLE [dbo].[CommandStatus]  WITH CHECK ADD  CONSTRAINT [FK_CommandStatus_StatusType] FOREIGN KEY([StatusId])
REFERENCES [dbo].[StatusType] ([StatusId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_CommandStatus_StatusType]') AND parent_object_id = OBJECT_ID(N'[dbo].[CommandStatus]'))
ALTER TABLE [dbo].[CommandStatus] CHECK CONSTRAINT [FK_CommandStatus_StatusType]
GO
/****** Object:  ForeignKey [FK_MessageRequest_CommandType]    Script Date: 07/13/2012 14:26:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageRequest_CommandType]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRequest]'))
ALTER TABLE [dbo].[MessageRequest]  WITH CHECK ADD  CONSTRAINT [FK_MessageRequest_CommandType] FOREIGN KEY([CommandID])
REFERENCES [dbo].[CommandType] ([CommandID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageRequest_CommandType]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRequest]'))
ALTER TABLE [dbo].[MessageRequest] CHECK CONSTRAINT [FK_MessageRequest_CommandType]
GO
/****** Object:  ForeignKey [FK_MessageRequest_MsgContext]    Script Date: 07/13/2012 14:26:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageRequest_MsgContext]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRequest]'))
ALTER TABLE [dbo].[MessageRequest]  WITH CHECK ADD  CONSTRAINT [FK_MessageRequest_MsgContext] FOREIGN KEY([MessageContextId])
REFERENCES [dbo].[MessageContext] ([MessageContextId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageRequest_MsgContext]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageRequest]'))
ALTER TABLE [dbo].[MessageRequest] CHECK CONSTRAINT [FK_MessageRequest_MsgContext]
GO
/****** Object:  ForeignKey [FK_MessageStatus_CommandStatus]    Script Date: 07/13/2012 14:26:24 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageStatus_CommandStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageStatus]'))
ALTER TABLE [dbo].[MessageStatus]  WITH CHECK ADD  CONSTRAINT [FK_MessageStatus_CommandStatus] FOREIGN KEY([CommandStatusId])
REFERENCES [dbo].[CommandStatus] ([CommandStatusId])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageStatus_CommandStatus]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageStatus]'))
ALTER TABLE [dbo].[MessageStatus] CHECK CONSTRAINT [FK_MessageStatus_CommandStatus]
GO
/****** Object:  ForeignKey [FK_MessageStatus_MessageRequest]    Script Date: 07/13/2012 14:26:24 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageStatus_MessageRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageStatus]'))
ALTER TABLE [dbo].[MessageStatus]  WITH CHECK ADD  CONSTRAINT [FK_MessageStatus_MessageRequest] FOREIGN KEY([MessageRequestID])
REFERENCES [dbo].[MessageRequest] ([MessageRequestID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageStatus_MessageRequest]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageStatus]'))
ALTER TABLE [dbo].[MessageStatus] CHECK CONSTRAINT [FK_MessageStatus_MessageRequest]
GO
/****** Object:  Table [dbo].[TrainBaselineStatus]   Script Date: 04/24/2014 14:26:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TrainBaselineStatus]') AND type in (N'U'))
DROP TABLE [dbo].[TrainBaselineStatus]
GO
/****** Object:  Table [dbo].[TrainBaselineStatus]   Script Date: 04/24/2014 14:26:23 ******/
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
/****** Object:  StoredProcedure [dbo].[sp_DeleteTrainBaselineStatus]    Script Date: 04/24/2014 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_DeleteTrainBaselineStatus]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Yoshiaki Date
-- Create date: 24th April 2014
-- Description:	Storeprocedure to delete /// TrainBaselineStatus
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteTrainBaselineStatus] 
	-- Add the parameters for the stored procedure here
	@TrainId nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE TrainBaselineStatus
	WHERE TrainBaselineStatus.TrainId = @TrainId;
END
' 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateTrainBaselineStatus]    Script Date: 04/24/2014 14:26:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateTrainBaselineStatus]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'-- =============================================
-- Author:		Yoshiaki Date
-- Create date: 24th April 2014
-- Description:	/// Stored Procedure to insert/update TrainBaselineStatusTable
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateTrainBaselineStatus] 
	-- Add the parameters for the stored procedure here
	@TrainId nvarchar(50),
	@RequestId nvarchar(50), 
    @TaskId int, 
	@TrainNumber nvarchar(50), 
	@OnlineStatus bit,
	@BaselineProgressStatus int,
	@CurrentBaselineVersion nvarchar(50), 
	@FutureBaselineVersion nvarchar(50), 
	@PISOnBoardVersion nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;
	declare @AuxTrainId nvarchar(50);
    declare @AuxRequestId nvarchar(50); 
    declare @AuxTaskId int;
	declare @AuxTrainNumber nvarchar(50); 
	declare @AuxOnlineStatus bit;
	declare @AuxBaselineProgressStatus int;
	declare @AuxCurrentBaselineVersion nvarchar(50); 
	declare @AuxFutureBaselineVersion nvarchar(50); 
	declare @AuxPISOnBoardVersion nvarchar(50);
    
    BEGIN TRANSACTION
    
	SELECT @AuxTrainId = TrainBaselineStatus.TrainID,
		   @AuxRequestId = TrainBaselineStatus.RequestId,
           @AuxTaskId = TrainBaselineStatus.TaskId,
		   @AuxTrainNumber = TrainBaselineStatus.TrainNumber,
		   @AuxOnlineStatus = TrainBaselineStatus.OnlineStatus,
		   @AuxBaselineProgressStatus = TrainBaselineStatus.BaselineProgressStatus,
		   @AuxCurrentBaselineVersion = TrainBaselineStatus.CurrentBaselineVersion,
		   @AuxFutureBaselineVersion = TrainBaselineStatus.FutureBaselineVersion,
		   @AuxPISOnBoardVersion = TrainBaselineStatus.PISOnBoardVersion
		FROM TrainBaselineStatus where TrainBaselineStatus.TrainID=@TrainId;
		
	IF @@RowCount > 0
	BEGIN
		IF ( @RequestId IS NOT NULL ) AND ((LEN(LTRIM(RTRIM(@RequestId))) > 0))
		BEGIN
			SET @AuxRequestId = LTRIM(RTRIM(@RequestId));
		END
        
        IF ( @TaskId IS NOT NULL )
		BEGIN
			SET @AuxTaskId = LTRIM(RTRIM(@TaskId));
		END
		
		IF ( @TrainNumber IS NOT NULL ) AND ((LEN(LTRIM(RTRIM(@TrainNumber))) > 0))
		BEGIN
			SET @AuxTrainNumber = LTRIM(RTRIM(@TrainNumber));
		END
		
		IF ( @OnlineStatus IS NOT NULL ) 
		BEGIN
			SET @AuxOnlineStatus = @OnlineStatus;
		END
		
		IF ( @BaselineProgressStatus IS NOT NULL ) 
		BEGIN
			SET @AuxBaselineProgressStatus = @BaselineProgressStatus;
		END
		
		IF ( @CurrentBaselineVersion IS NOT NULL ) 
		BEGIN
			SET @AuxCurrentBaselineVersion = LTRIM(RTRIM(@CurrentBaselineVersion));
		END
		
		IF ( @FutureBaselineVersion IS NOT NULL ) 
		BEGIN
			SET @AuxFutureBaselineVersion = LTRIM(RTRIM(@FutureBaselineVersion));
		END
		
		IF ( @PISOnBoardVersion IS NOT NULL ) 
		BEGIN
			SET @AuxPISOnBoardVersion = LTRIM(RTRIM(@PISOnBoardVersion));
		END
		
		UPDATE TrainBaselineStatus set
			RequestId = @AuxRequestId, 
            TaskId = @AuxTaskId,
			TrainNumber = @AuxTrainNumber, 
			OnlineStatus = @AuxOnlineStatus, 
			BaselineProgressStatus = @AuxBaselineProgressStatus, 
			CurrentBaselineVersion = @AuxCurrentBaselineVersion, 
			FutureBaselineVersion = @AuxFutureBaselineVersion, 
			PISOnBoardVersion = @AuxPISOnBoardVersion
		WHERE
			TrainId = @TrainId;
	END
	ELSE
	BEGIN
		INSERT INTO TrainBaselineStatus (TrainId, 
										 RequestId, 
                                         TaskId,
										 TrainNumber, 
										 OnlineStatus, 
										 BaselineProgressStatus, 
										 CurrentBaselineVersion, 
										 FutureBaselineVersion, 
										 PISOnBoardVersion)
		VALUES (@TrainId, 
				@RequestId, 
                @TaskId, 
				@TrainNumber, 
				@OnlineStatus, 
				@BaselineProgressStatus,
				@CurrentBaselineVersion, 
				@FutureBaselineVersion, 
				@PISOnBoardVersion );
	END
	
	
	IF @@error<>0 
	BEGIN
		ROLLBACK TRANSACTION
		RETURN @@error
	END
	ELSE	
	BEGIN
		COMMIT TRANSACTION
		RETURN 0
	END
END
' 
END
GO
