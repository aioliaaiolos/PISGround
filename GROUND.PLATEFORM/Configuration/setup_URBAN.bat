::=====================================================================================
:: File name      : setup_URBAN.bat
:: Description    : Script that install PIS-GROUND for URBAN platform
::				  : 	
:: Update         :	2016-09-09			
::=====================================================================================
SETLOCAL
SET APPPOOLNAME="PIS-GROUND"
start /wait msiexec /i SessionSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i MaintenanceSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i InstantMessageSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i InfotainmentJournalingSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i DataPackageSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i RealTimeSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i MissionSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i LiveVideoControlSetup.msi /qb TARGETAPPPOOL=%APPPOOLNAME%
start /wait msiexec /i RemoteDataStoreSetup.msi /qb
