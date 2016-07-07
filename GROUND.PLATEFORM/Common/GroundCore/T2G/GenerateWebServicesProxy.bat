@echo off
SETLOCAL

SET OUTFILE=%~dp0generated\NotificationServiceInterfaces.cs
SET TEMPOUTDIR=%~dp0generated\temp
SET TEMPOUTFILE=%TEMPOUTDIR%\NotificationServiceInterfaces.cs

IF "%1"=="" GOTO USE_ENV_VAR_PATH
set T2G_WSDL_PATH=%1

:USE_ENV_VAR_PATH
REM vcvars32.bat sets the appropriate environment variables to enable command line builds
SET VCVARSFILE=%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat

IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=%ProgramFiles%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" (
	echo "Visual Studio 2008 is required to build this application. vcvars32.bat file cannot be found"
	exit /B 10 
)

cd %~dp0
CALL GenerateServiceProxy.bat PIS.Ground.Core.T2G.WebServices.FileTransfer "%T2G_WSDL_PATH%\T2GFileTransfer.wsdl" FileTransferClient
CALL GenerateServiceProxy.bat PIS.Ground.Core.T2G.WebServices.Identification "%T2G_WSDL_PATH%\T2GIdentification.wsdl" IdentificationClient
CALL GenerateServiceProxy.bat PIS.Ground.Core.T2G.WebServices.Maintenance "%T2G_WSDL_PATH%\T2GMaintenance.wsdl" MaintenanceClient
CALL GenerateServiceProxy.bat PIS.Ground.Core.T2G.WebServices.VehicleInfo "%T2G_WSDL_PATH%\T2GVehicleInfo.wsdl" VehiculeInfoClient

CALL "%VCVARSFILE%"
cd %~dp0

echo "Generate T2GNotification"

IF NOT EXIST "%TEMPOUTDIR%" MKDIR "%TEMPOUTDIR%"
@echo ON
call wsdl.exe "%T2G_WSDL_PATH%\T2GNotification.wsdl" /out:%TEMPOUTDIR% /l:cs /serverInterface /n:PIS.Ground.Core.T2G.WebServices.Notification
@IF ERRORLEVEL 1 (
		@echo off
		echo Failed to generate T2GNotification proxy
		goto :End
	)

@echo off
	
IF NOT EXIST "%OUTFILE%" goto :ReplaceFile
	
FC /B "%OUTFILE%" "%TEMPOUTFILE%" >NUL
IF NOT ERRORLEVEL 1 GOTO :End

:ReplaceFile
IF EXIST "%OUTFILE%" echo File "%OUTFILE%" replaced
IF NOT EXIST "%OUTFILE%" echo File "%OUTFILE%" created
IF EXIST "%OUTFILE%" ATTRIB -R "%OUTFILE%"

echo F | XCOPY /V /Y "%TEMPOUTFILE%" "%OUTFILE%"
IF ERRORLEVEL 1 echo Cannot copy "%TEMPOUTFILE%" to "%OUTFILE%"

:End
if EXIST "%TEMPOUTDIR%" RMDIR /S /Q "%TEMPOUTDIR%"