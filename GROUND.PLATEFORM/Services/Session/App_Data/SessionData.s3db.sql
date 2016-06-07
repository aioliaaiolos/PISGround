--SQLite Maestro 9.3.0.4
------------------------------------------
--Host     : localhost
--Database : D:\ClearcaseViews\PIS2G_V5.4\PACIS\PROD.PIS.GROUND\Services\Session\App_Data\SessionData.s3db


CREATE TABLE Session (
  SessionID         nvarchar(300) PRIMARY KEY NOT NULL,
  UserName          nvarchar(25) NOT NULL,
  NotificationURL   nvarchar(300),
  LoginTime         nvarchar(25) NOT NULL,
  LastAccessedTime  nvarchar(25) NOT NULL,
  Password          nvarchar(10) NOT NULL,
  UserTimeOut	    nvarchar(10) ,
  UserTimeOutSet    nvarchar(25) 
);

CREATE UNIQUE INDEX IDX_SESSION_SESSIONID
  ON Session
  (SessionID);

CREATE TABLE SessionData (
  SessionID  nvarchar NOT NULL,
  RequestID  nvarchar(50) NOT NULL,
  Status     nvarchar,
  /* Foreign keys */
  FOREIGN KEY (SessionID)
    REFERENCES "[Session]"(SessionID)
);

