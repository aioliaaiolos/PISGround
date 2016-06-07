::=====================================================================================
:: File name      : 		setup_SIVENG.bat
:: MakeFile name  : 
:: Description    : Script that install PIS-GROUND for SIVENG platform
::				  : 	
:: Update         :			   2016-02-26			
::=====================================================================================
SETLOCAL
SET APPPOOLNAME="PIS-GROUND"
start /wait msiexec /i SessionSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i MaintenanceSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i InstantMessageSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i InfotainmentJournalingSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i DataPackageSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i MissionSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i LiveVideoControlSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i RemoteDataStoreSetup.msi /qb

