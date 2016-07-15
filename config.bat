::=====================================================================================
:: File name      : 					config.bat
:: Description    : 	download all files necessary to build GROUND project
::				  : 				   copyVoute.bat
:: Created        :						  2015-10-07
:: Updated        :						  2016-07-14
::=====================================================================================
@echo off

if "%1"=="" SETLOCAL

::=====================================================================================
:: Updatable section - BEGIN
::=====================================================================================

::=====================================================================================
:: Fields that need to be updated on every new version - Begin
::=====================================================================================
set VERSION_NUMBER=5.16.8.0
set SETUP_VERSION=5.16.0800
SET SETUP_PACKAGE_CODE_GUID={C8192351-EC42-4750-B1B2-EC290F2F8B9C}
:: The setup product code does not need to be changed because PIS-Ground does not support
:: multiple installation on same machine.

::=====================================================================================
:: Fields that need to be updated on every new version - End
::=====================================================================================

:: Version of the pre-requisite files to deliver PIS-Ground
set REQUESITE_VERSION=5.8.0.9

set T2GClientDelivery_ZIPFilename=T2GClientDelivery1.0.12.0
set Ground_SourceCode_ZIPFilename=Ground_SourceCode%VERSION_NUMBER%
set Ground_Dependencies_ZIPFilename=PISGroundDependencies_V5_16_0
set GroundServer_Requisites_PrefixFileName=MT92-2PIS010010-PIS2G-Ground_Server-REQUISITES-
set GroundServer_Requisites_ZIPFileName=%GroundServer_Requisites_PrefixFileName%V%REQUESITE_VERSION%
set GroundServer_Requisites_x64_ZIPFileName=%GroundServer_Requisites_PrefixFileName%x64-V%REQUESITE_VERSION%
set GroundServer_Requisites_x86_ZIPFileName=%GroundServer_Requisites_PrefixFileName%x86-V%REQUESITE_VERSION%
set Groundserver_TestPackages_ZIPFileName=PISGroundIntegrationTestsPackages_V5.16.0
set PISEmbeddedSDK_ZIPFileName=PISEmbeddedSDK_V5.16.3.0

::=====================================================================================
:: Updatable section - END
::=====================================================================================

SET EXIT_CODE=0
SET CURRENT_PATH=%~dp0

set VOUTE_PATH=\\srvsiefnp01\sfsla1nas1\eng\Electrical\Release VERSION\Software\Libs

set ZIP_PATH=%ProgramFiles(x86)%\7-Zip\7z.exe
IF NOT EXIST "%ZIP_PATH%" set ZIP_PATH=C:\Program Files (x86)\7-Zip\7z.exe
IF NOT EXIST "%ZIP_PATH%" set ZIP_PATH=C:\Program Files\7-Zip\7z.exe
IF NOT EXIST "%ZIP_PATH%" set ZIP_PATH=%ProgramFiles%\7-Zip\7z.exe
if NOT EXIST "%ZIP_PATH%" (	
	echo 7-zip is not installed.
	SET EXIT_CODE=1
	goto :End
)

set WORKING_DIR=%~dp0

set BUILD_PATH=%WORKING_DIR%GROUND_DELIVERY

set CONF_BUILD=RELEASE

if not "%1"=="" goto end


SetLocal EnableDelayedExpansion		
echo Execution start at %date% !time:~0,2!:!time:~3,2!:!time:~6,2! > "%CURRENT_PATH%configLog.txt"
ENDLOCAL
echo execute script "%CURRENT_PATH%GROUND.PLATEFORM\util\copyVoute.bat"
call %CURRENT_PATH%GROUND.PLATEFORM\util\copyVoute.bat >> "%CURRENT_PATH%configLog.txt" 2>&1
IF ERRORLEVEL 1 (
	SET EXIT_CODE=2
	goto :EndScripts
)

:EndScripts
SetLocal EnableDelayedExpansion		
echo Execution Ends at %date% !time:~0,2!:!time:~3,2!:!time:~6,2! >> "%CURRENT_PATH%configLog.txt"
ENDLOCAL

if "%EXIT_CODE%"=="0" echo config Success >> "%CURRENT_PATH%configLog.txt"
if not "%EXIT_CODE%"=="0" echo config Failed with the EXIT_CODE : %EXIT_CODE% >> "%CURRENT_PATH%configLog.txt"
IF EXIST "%CURRENT_PATH%configLog.txt" type "%CURRENT_PATH%configLog.txt"

pause

:end

if "%1"=="" ENDLOCAL
exit /B %EXIT_CODE%

