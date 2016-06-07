@echo off
SETLOCAL
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


wsdl.exe "%T2G_WSDL_PATH%\T2GNotification.wsdl" /out:generated /l:cs /serverInterface /n:PIS.Ground.Core.T2G.WebServices.Notification
