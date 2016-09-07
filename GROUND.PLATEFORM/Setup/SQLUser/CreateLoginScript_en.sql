IF  EXISTS (SELECT name FROM sys.server_principals WHERE name = 'NT AUTHORITY\NETWORK SERVICE') 
BEGIN DROP LOGIN [NT AUTHORITY\NETWORK SERVICE]; 
END;
CREATE LOGIN [NT AUTHORITY\NETWORK SERVICE] 
FROM WINDOWS WITH DEFAULT_DATABASE = [master], DEFAULT_LANGUAGE = [us_english];
EXEC master..sp_addsrvrolemember @loginame = [NT AUTHORITY\NETWORK SERVICE], @rolename = N'bulkadmin';
EXEC master..sp_addsrvrolemember @loginame = [NT AUTHORITY\NETWORK SERVICE], @rolename = N'dbcreator';
EXEC master..sp_addsrvrolemember @loginame = [NT AUTHORITY\NETWORK SERVICE], @rolename = N'diskadmin';
EXEC master..sp_addsrvrolemember @loginame = [NT AUTHORITY\NETWORK SERVICE], @rolename = N'processadmin';
EXEC master..sp_addsrvrolemember @loginame = [NT AUTHORITY\NETWORK SERVICE], @rolename = N'securityadmin';
EXEC master..sp_addsrvrolemember @loginame = [NT AUTHORITY\NETWORK SERVICE], @rolename = N'serveradmin';
EXEC master..sp_addsrvrolemember @loginame = [NT AUTHORITY\NETWORK SERVICE], @rolename = N'setupadmin';
EXEC master..sp_addsrvrolemember @loginame = [NT AUTHORITY\NETWORK SERVICE], @rolename = N'sysadmin';