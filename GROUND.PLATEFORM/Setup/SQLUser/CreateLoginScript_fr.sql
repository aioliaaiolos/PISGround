IF  EXISTS (SELECT name FROM sys.server_principals WHERE name = 'AUTORITE NT\Système') 
BEGIN DROP LOGIN [AUTORITE NT\Système]; 
END;
CREATE LOGIN [AUTORITE NT\Système] 
FROM WINDOWS WITH DEFAULT_DATABASE = [master], DEFAULT_LANGUAGE = [us_english];
EXEC master..sp_addsrvrolemember @loginame = [AUTORITE NT\Système], @rolename = N'bulkadmin';
EXEC master..sp_addsrvrolemember @loginame = [AUTORITE NT\Système], @rolename = N'dbcreator';
EXEC master..sp_addsrvrolemember @loginame = [AUTORITE NT\Système], @rolename = N'diskadmin';
EXEC master..sp_addsrvrolemember @loginame = [AUTORITE NT\Système], @rolename = N'processadmin';
EXEC master..sp_addsrvrolemember @loginame = [AUTORITE NT\Système], @rolename = N'securityadmin';
EXEC master..sp_addsrvrolemember @loginame = [AUTORITE NT\Système], @rolename = N'serveradmin';
EXEC master..sp_addsrvrolemember @loginame = [AUTORITE NT\Système], @rolename = N'setupadmin';
EXEC master..sp_addsrvrolemember @loginame = [AUTORITE NT\Système], @rolename = N'sysadmin';