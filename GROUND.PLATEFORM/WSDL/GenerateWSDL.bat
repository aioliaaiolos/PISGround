rem This batch file generates the WSDL files corresponding to the PIS Ground WCF services

@cls

@echo .
@echo ============== DataPackage ==============
@echo .

@cd DataPackage
@del /Q /F .\*.wsdl
@del /Q /F .\*.xsd
..\svcutil.exe /t:metadata http://alstom-ground-pis-server/DataPackage/DataPackageService.svc
perl ..\GenerateWSDL.pl
@cd ..

@echo .
@echo ============== InfotainmentJournaling ==============
@echo .

@cd InfotainmentJournaling
@del /Q /F .\*.wsdl
@del /Q /F .\*.xsd
..\svcutil.exe /t:metadata http://alstom-ground-pis-server/InfotainmentJournaling/JournalingService.svc
perl ..\GenerateWSDL.pl
@cd ..

@echo .
@echo ============== InstantMessage ==============
@echo .

@cd InstantMessage
@del /Q /F .\*.wsdl
@del /Q /F .\*.xsd
..\svcutil.exe /t:metadata http://alstom-ground-pis-server/InstantMessage/InstantMessageService.svc
perl ..\GenerateWSDL.pl
@cd ..

@echo .
@echo ============== Maintenance ==============
@echo .

@cd Maintenance
@del /Q /F .\*.wsdl
@del /Q /F .\*.xsd
..\svcutil.exe /t:metadata http://alstom-ground-pis-server/Maintenance/MaintenanceService.svc
perl ..\GenerateWSDL.pl
cscript RemoveObsoleteMaintenanceMethods.wsf
@cd ..

@echo .
@echo ============== Session ==============
@echo .

@cd Session
@del /Q /F .\*.wsdl
@del /Q /F .\*.xsd
..\svcutil.exe /t:metadata http://alstom-ground-pis-server/Session/SessionService.svc
perl ..\GenerateWSDL.pl
@cd ..

@echo .
@echo ============== Mission ==============
@echo .

@cd Mission
@del /Q /F .\*.wsdl
@del /Q /F .\*.xsd
..\svcutil.exe /t:metadata http://alstom-ground-pis-server/Mission/MissionService.svc
perl ..\GenerateWSDL.pl
@cd ..

@echo .
@echo ============== LiveVideoControl ==============
@echo .

@cd LiveVideoControl
@del /Q /F .\*.wsdl
@del /Q /F .\*.xsd
..\svcutil.exe /t:metadata http://alstom-ground-pis-server/LiveVideoControl/LiveVideoControlService.svc
perl ..\GenerateWSDL.pl
@cd ..

@echo .
@echo ============== RealTime ==============
@echo .

@cd RealTime
@del /Q /F .\*.wsdl
@del /Q /F .\*.xsd
..\svcutil.exe /t:metadata http://alstom-ground-pis-server/RealTime/RealTimeService.svc
perl ..\GenerateWSDL.pl
@cd ..


@echo .
@echo ============== Done! ==============
@echo .

@pause