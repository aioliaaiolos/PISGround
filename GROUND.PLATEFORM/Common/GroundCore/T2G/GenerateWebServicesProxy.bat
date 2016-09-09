::=====================================================================================
:: File name      : GenerateWebServicesProxy.bat
:: Description    : Generate the T2G clients proxy from T2G WSDL	
:: Updated        :	2016-09-09
::=====================================================================================
@echo off
SETLOCAL

SET "OUTFILE=%~dp0generated\NotificationServiceInterfaces.cs"
SET "TEMPOUTDIR=%~dp0generated\temp"
SET "TEMPOUTFILE=%TEMPOUTDIR%\NotificationServiceInterfaces.cs"
SET EXIT_CODE=0

IF "%1"=="" GOTO USE_ENV_VAR_PATH
SET "T2G_WSDL_PATH=%1"

:USE_ENV_VAR_PATH

if "%T2G_WSDL_PATH%"=="" for %%D in ("%~dp0..\..\..\..\T2G_DELIVERY\T2G.CLIENT.DELIVERY\WSDL") do SET "T2G_WSDL_PATH=%%~fD"

REM vcvars32.bat sets the appropriate environment variables to enable command line builds

SET "VCVARSFILE=%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"

IF NOT EXIST "%VCVARSFILE%" SET "VCVARSFILE=C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
IF NOT EXIST "%VCVARSFILE%" SET "VCVARSFILE=%ProgramFiles%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
IF NOT EXIST "%VCVARSFILE%" (
	echo "Visual Studio 2008 is required to build this application. vcvars32.bat file cannot be found"
	SET EXIT_CODE=1
	goto :End
)

cd /D "%~dp0" || echo Cannot move to directory "%~dp0" && SET EXIT_CODE=2 && goto :End

CALL ..\..\..\WSDL\GenerateOneProxy.bat "%~dp0generated" PIS.Ground.Core.T2G.WebServices.FileTransfer FileTransferClient.cs FileTransferClient.config "%T2G_WSDL_PATH%\T2GFileTransfer.wsdl" /a || echo Failed to generate client for T2G FileTranser. && SET EXIT_CODE=3 && goto :End
CALL ..\..\..\WSDL\GenerateOneProxy.bat "%~dp0generated" PIS.Ground.Core.T2G.WebServices.Identification IdentificationClient.cs IdentificationClient.config "%T2G_WSDL_PATH%\T2GIdentification.wsdl" /a || echo Failed to generate client for T2G Identification. && SET EXIT_CODE=4 && goto :End
CALL ..\..\..\WSDL\GenerateOneProxy.bat "%~dp0generated" PIS.Ground.Core.T2G.WebServices.Maintenance MaintenanceClient.cs MaintenanceClient.config "%T2G_WSDL_PATH%\T2GMaintenance.wsdl" /a || echo Failed to generate client for T2G Maintenance. && SET EXIT_CODE=5 && goto :End
CALL ..\..\..\WSDL\GenerateOneProxy.bat "%~dp0generated" PIS.Ground.Core.T2G.WebServices.VehicleInfo VehiculeInfoClient.cs VehiculeInfoClient.config "%T2G_WSDL_PATH%\T2GVehicleInfo.wsdl" /a || echo Failed to generate client for T2G Vehicle-Info. && SET EXIT_CODE=6 && goto :End


REM CALL GenerateServiceProxy.bat PIS.Ground.Core.T2G.WebServices.FileTransfer "%T2G_WSDL_PATH%\T2GFileTransfer.wsdl" FileTransferClient || echo Failed to generate client for T2G FileTranser. && SET EXIT_CODE=3 && goto :End
REM CALL GenerateServiceProxy.bat PIS.Ground.Core.T2G.WebServices.Identification "%T2G_WSDL_PATH%\T2GIdentification.wsdl" IdentificationClient || echo Failed to generate client for T2G Identification. && SET EXIT_CODE=4 && goto :End
REM CALL GenerateServiceProxy.bat PIS.Ground.Core.T2G.WebServices.Maintenance "%T2G_WSDL_PATH%\T2GMaintenance.wsdl" MaintenanceClient || echo Failed to generate client for T2G Maintenance. && SET EXIT_CODE=5 && goto :End
REM CALL GenerateServiceProxy.bat PIS.Ground.Core.T2G.WebServices.VehicleInfo "%T2G_WSDL_PATH%\T2GVehicleInfo.wsdl" VehiculeInfoClient || echo Failed to generate client for T2G Vehicle-Info. && SET EXIT_CODE=6 && goto :End

CALL "%VCVARSFILE%"
cd /D "%~dp0" || echo Cannot move to directory "%~dp0" && SET EXIT_CODE=7 && goto :End

echo "Generate T2GNotification"

IF NOT EXIST "%TEMPOUTDIR%" MKDIR "%TEMPOUTDIR%"
echo RUN: call wsdl.exe "%T2G_WSDL_PATH%\T2GNotification.wsdl" /out:"%TEMPOUTDIR%" /l:cs /serverInterface /n:PIS.Ground.Core.T2G.WebServices.Notification
call wsdl.exe "%T2G_WSDL_PATH%\T2GNotification.wsdl" /out:"%TEMPOUTDIR%" /l:cs /serverInterface /n:PIS.Ground.Core.T2G.WebServices.Notification
IF ERRORLEVEL 1 (
		echo Failed to generate T2GNotification proxy
		SET EXIT_CODE=8
		goto :End
	)

	
IF NOT EXIST "%OUTFILE%" goto :ReplaceFile
	
FC /B "%OUTFILE%" "%TEMPOUTFILE%" >NUL
IF NOT ERRORLEVEL 1 GOTO :End

:ReplaceFile
IF EXIST "%OUTFILE%" echo File "%OUTFILE%" replaced
IF NOT EXIST "%OUTFILE%" echo File "%OUTFILE%" created
IF EXIST "%OUTFILE%" ATTRIB -R "%OUTFILE%"

echo F | XCOPY /V /Y "%TEMPOUTFILE%" "%OUTFILE%"
IF ERRORLEVEL 1 echo Cannot copy "%TEMPOUTFILE%" to "%OUTFILE%" && SET EXIT_CODE=9 && goto :End

:End
if EXIST "%TEMPOUTDIR%" RMDIR /S /Q "%TEMPOUTDIR%"
if "%EXIT_CODE%" == "0" echo %~nx0 succeeded
if not "%EXIT_CODE%" == "0" echo %~nx0 failed with error code: %EXIT_CODE%

EXIT /B %EXIT_CODE%
