::=====================================================================================
:: File name      : copyVoute.bat
:: Description    : copy all files from the voute into the source code directory
:: Updated        :	2016-09-08
::=====================================================================================
@echo off

SETLOCAL


if "%VOUTE_PATH%"=="" call "%~dp0..\..\config.bat" version_number 

SET "ROOT_PATH=%WORKING_DIR%"
SET EXIT_CODE=0
													
SET "DEPENDENCIES_DESTINATION_PATH=%ROOT_PATH%GROUND.PLATEFORM\Dependencies"
SET "WSDL_DESTINATION_PATH=%ROOT_PATH%GROUND.PLATEFORM\WSDL"
SET "SVCUTIL_FILE_NAME=%ROOT_PATH%GROUND.PLATEFORM\Dependencies\SvcUtil\SvcUtil.exe"
SET "SQLITE_FILE_NAME=%ROOT_PATH%GROUND.PLATEFORM\Dependencies\SQLITE\System.Data.SQLite.dll"
SET "SQLITE_DESTINATION_PATH=%ROOT_PATH%GROUND.PLATEFORM\Common\GroundCore\SQLite"
SET "TEST_PACKAGE_DESTINATION_PATH=%ROOT_PATH%GROUND.PLATEFORM\IntegrationTests"
SET "T2G_DESTINATION_PATH=%ROOT_PATH%T2G_DELIVERY\T2G.CLIENT.DELIVERY"
SET "PISEmdededSDK_DESTINATION_PATH=%ROOT_PATH%PISEmbeddedSDK"
SET "SRC_DependenciesZip=%VOUTE_PATH%\PIS\GroundServer\%Ground_Dependencies_ZIPFilename%"

:: Check the presence of requisites files even if we copy them only in the deliver scripts. 
:: This permits to ensure that everything can be retrieved

SET "SRC_GroundServer_RequisitesZip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_ZIPFileName%"
SET "SRC_GroundServer_Requisites_x64Zip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_x64_ZIPFileName%"
SET "SRC_GroundServer_Requisites_x86Zip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_x86_ZIPFileName%"
SET "SRC_Groundserver_TestPackages_ZIP=%VOUTE_PATH%\PIS\GroundServer\%Groundserver_TestPackages_ZIPFileName%"
SET "SRC_T2GClientDelivery_ZIP=%VOUTE_PATH%\PIS\T2G\%T2GClientDelivery_ZIPFilename%"
SET "SRC_PISEmbeddedSDK_ZIP=%VOUTE_PATH%\PIS\Embedded\%PISEmbeddedSDK_ZIPFileName%"

::=====================================================================================
:: CHECK REQUIRED VARIABLE
::=====================================================================================
echo Verify that required variables are defined and the value is in the proper format.

if "%VOUTE_PATH%"=="" (
	echo Variable VOUTE_PATH not defined
	SET EXIT_CODE=1
	goto :End
)

if "%WORKING_DIR%"=="" (
	echo Variable WORKING_DIR not defined
	SET EXIT_CODE=2
	goto :End
)

if "%VERSION_NUMBER%" == "" (
	echo Variable VERSION_NUMBER not defined
	SET EXIT_CODE=3
	goto :End
)

if "%SETUP_VERSION%" == "" (
	echo Variable SETUP_VERSION not defined
	SET EXIT_CODE=4
	goto :End
)

if "%SETUP_PACKAGE_CODE_GUID%" == "" (
	echo Variable SETUP_PACKAGE_CODE_GUID not defined
	SET EXIT_CODE=5
	goto :End
)

echo %SETUP_PACKAGE_CODE_GUID% | findstr /R /C:{[A-F0-9]*-[A-F0-9]*-[A-F0-9]*-[A-F0-9]*-[A-F0-9]*} > NUL
if ERRORLEVEL 1 (
	echo Variable SETUP_PACKAGE_CODE_GUID is not a valid GUID with all upper case letters
	echo SETUP_PACKAGE_CODE_GUID=%SETUP_PACKAGE_CODE_GUID%
	SET EXIT_CODE=6
	goto :End
)

echo %VERSION_NUMBER% | findstr /R /C:[0-9]*.[0-9]*.[0-9]*.[0-9]* > NUL
if ERRORLEVEL 1 (
	echo Variable VERSION_NUMBER is not a valid version in format 1.2.3.4
	echo VERSION_NUMBER=%VERSION_NUMBER%
	SET EXIT_CODE=7
	goto :End
)

echo %SETUP_VERSION% | findstr /R /C:[0-9]*.[0-9]*.[0-9][0-9][0-9][0-9] >NUL
if ERRORLEVEL 1 (
	echo Variable SETUP_VERSION is not a valid version in format 1.2.0000
	echo SETUP_VERSION=%SETUP_VERSION%
	SET EXIT_CODE=8
	goto :End
)

::=====================================================================================
:: CHECK REQUIRED FILES
::=====================================================================================

echo Verify that required files exists

if not exist "%VOUTE_PATH%" (
	echo Voute path does not exist at this location "%VOUTE_PATH%"
	SET EXIT_CODE=9
	goto :End
)

if not exist "%WORKING_DIR%" (
	echo Path defined by variable WORKING_DIR does not exist at this location: "%WORKING_DIR%"
	SET EXIT_CODE=10
	goto :End
)

if not exist "%ROOT_PATH%" (
	echo Root path defined by variable ROOT_PATH does not exist at this location "%ROOT_PATH%"
	SET EXIT_CODE=11
	goto :End
)


if not exist "%DEPENDENCIES_DESTINATION_PATH%" (
	echo Dependencies destination path does not exist at this location "%DEPENDENCIES_DESTINATION_PATH%"
	SET EXIT_CODE=12
	goto :End
)

if not exist "%TEST_PACKAGE_DESTINATION_PATH%" (
	echo Test packages destination path does not exist at this location "%TEST_PACKAGE_DESTINATION_PATH%"
	SET EXIT_CODE=13
	goto :End
)


if not exist "%SRC_DependenciesZip%" (
	echo PIS-Ground dependencies zip file does not exist at this location "%SRC_DependenciesZip%"
	SET EXIT_CODE=14
	goto :End
)

if not exist "%ZIP_PATH%" (
	echo 7-Zip executable does not exist at this location "%ZIP_PATH%"
	SET EXIT_CODE=15
	goto :End
)

if not exist "%SRC_GroundServer_RequisitesZip%" (
	echo PIS-Ground server requisites zip file does not exist at this location "%SRC_GroundServer_RequisitesZip%"
	SET EXIT_CODE=16
	goto :End
)

if not exist "%SRC_GroundServer_Requisites_x64Zip%" (
	echo PIS-Ground server requisites for x64 zip file does not exist at this location "%SRC_GroundServer_Requisites_x64Zip%"
	SET EXIT_CODE=17
	goto :End
)

if not exist "%SRC_GroundServer_Requisites_x86Zip%" (
	echo PIS-Ground server requisites for x86 zip file does not exist at this location "%SRC_GroundServer_Requisites_x86Zip%"
	SET EXIT_CODE=18
	goto :End
)

if not exist "%SRC_Groundserver_TestPackages_ZIP%" (
	echo PIS-Ground server test packages zip file does not exist at this location "%SRC_Groundserver_TestPackages_ZIP%"
	SET EXIT_CODE=19
	goto :End
)

if not exist "%SRC_T2GClientDelivery_ZIP%" (
	echo T2G Client delivery zip file does not exist at this location "%SRC_T2GClientDelivery_ZIP%"
	SET EXIT_CODE=20
	goto :End
)

if not exist "%SRC_PISEmbeddedSDK_ZIP%" (
	echo PIS Emdedded SDK zip file does not exist at this location "%SRC_PISEmbeddedSDK_ZIP%"
	SET EXIT_CODE=21
	goto :End
)

::=====================================================================================
:: REMOVE PREVIOUS ZIP FILES
::=====================================================================================

echo Remove previous source code zip

call :DoDeleteFile "%ROOT_PATH%\Ground_SourceCode*.zip" || SET EXIT_CODE=22

echo Remove previous requisites files

call :DoDeleteFile "%ROOT_PATH%\%GroundServer_Requisites_PrefixFileName%*.zip" || SET EXIT_CODE=23

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: Remove previous T2G_DELIVERY 
::=====================================================================================
echo Remove previous T2G delivery files

call :DoDeleteDir "%T2G_DESTINATION_PATH%\.." || SET EXIT_CODE=24 && goto :End

::=====================================================================================
:: Remove previous PIS Embedded SDK
::=====================================================================================
echo Remove previous PIS Embedded SDK files

call :DoDeleteDir "%PISEmdededSDK_DESTINATION_PATH%" || SET EXIT_CODE=25 && goto :End

::=====================================================================================
:: COPY AND UNZIP PIS-GROUND DEPENDENCIES
::=====================================================================================
echo Gets the PIS-Ground dependencies file.							

cd "%DEPENDENCIES_DESTINATION_PATH%"
if ERRORLEVEL 1 (
	echo Cannot move to directory "%DEPENDENCIES_DESTINATION_PATH%"
	SET EXIT_CODE=26
	goto :End
)

echo Copy and Unzip "%Ground_Dependencies_ZIPFilename%" to "%DEPENDENCIES_DESTINATION_PATH%"
call :DoCopy "%SRC_DependenciesZip%" "%DEPENDENCIES_DESTINATION_PATH%" || SET EXIT_CODE=27 && goto :End

::If you got the error 28, verify the version of 7-zip. Version 15.14 and over is required.
"%ZIP_PATH%" x "%Ground_Dependencies_ZIPFilename%" -o. -aoa -bb3							
if ERRORLEVEL 1 (
	echo Failed to unzip "%Ground_Dependencies_ZIPFilename%"
	SET EXIT_CODE=28
)

call :DoDeleteFile "%Ground_Dependencies_ZIPFilename%"
if not "%EXIT_CODE%" == "0" goto :End

echo Copy to WSDL "%SVCUTIL_FILE_NAME%" to "%WSDL_DESTINATION_PATH%"
call :DoCopy "%SVCUTIL_FILE_NAME%" "%WSDL_DESTINATION_PATH%" || SET EXIT_CODE=29 && goto :End

echo Copy to SQLITE  "%SQLITE_FILE_NAME%" to "%SQLITE_DESTINATION_PATH%"
call :DoCopy "%SQLITE_FILE_NAME%" "%SQLITE_DESTINATION_PATH%" || SET EXIT_CODE=30 && goto :End


::=====================================================================================
:: COPY AND UNZIP PIS-GROUND TEST PACKAGES
::=====================================================================================
echo Gets the PIS-Ground test packages file.							

cd "%TEST_PACKAGE_DESTINATION_PATH%"
if ERRORLEVEL 1 (
	echo Cannot move to directory "%TEST_PACKAGE_DESTINATION_PATH%"
	SET EXIT_CODE=31
	goto :End
)

echo Copy and Unzip "%Groundserver_TestPackages_ZIPFileName%" to "%TEST_PACKAGE_DESTINATION_PATH%"
call :DoCopy "%SRC_Groundserver_TestPackages_ZIP%" "%TEST_PACKAGE_DESTINATION_PATH%" || SET EXIT_CODE=32 && goto :End

::If you got the error 33, verify the version of 7-zip. Version 15.14 and over is required.
"%ZIP_PATH%" x "%Groundserver_TestPackages_ZIPFileName%" -o. -aoa -bb3							
if ERRORLEVEL 1 (
	echo Failed to unzip "%Groundserver_TestPackages_ZIPFileName%"
	SET EXIT_CODE=33
)

call :DoDeleteFile "%Groundserver_TestPackages_ZIPFileName%"
if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: COPY AND UNZIP T2G CLIENT DELIVERY FILES
::=====================================================================================
echo Gets the T2G Client delivery files

call :DoDeleteDir "%T2G_DESTINATION_PATH%" || SET EXIT_CODE=34 && goto :End
call :DoCreateDir "%T2G_DESTINATION_PATH%" || SET EXIT_CODE=35 && goto :End

cd /D "%T2G_DESTINATION_PATH%"
if ERRORLEVEL 1 (
	echo Cannot move to directory "%T2G_DESTINATION_PATH%"
	SET EXIT_CODE=36
	goto :End
)

echo Copy and Unzip "%T2GClientDelivery_ZIPFilename%" to "%T2G_DESTINATION_PATH%"
call :DoCopy "%SRC_T2GClientDelivery_ZIP%%" "%T2G_DESTINATION_PATH%" || SET EXIT_CODE=37 && goto :End

::If you got the error 38, verify the version of 7-zip. Version 15.14 and over is required.
"%ZIP_PATH%" x "%T2GClientDelivery_ZIPFilename%" -o. -aoa -bb3							
if ERRORLEVEL 1 (
	echo Failed to unzip "%T2GClientDelivery_ZIPFilename%"
	SET EXIT_CODE=38
)

call :DoDeleteFile "%T2GClientDelivery_ZIPFilename%"

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: COPY AND UNZIP PIS EMBEDDED SDK FILES
::=====================================================================================
echo Gets the PIS Embedded SDK files

call :DoDeleteDir "%PISEmdededSDK_DESTINATION_PATH%" || SET EXIT_CODE=39 && goto :End
call :DoCreateDir "%PISEmdededSDK_DESTINATION_PATH%" || SET EXIT_CODE=40 && goto :End


cd /D "%PISEmdededSDK_DESTINATION_PATH%"
if ERRORLEVEL 1 (
	echo Failed to move to directory "%PISEmdededSDK_DESTINATION_PATH%"
	SET EXIT_CODE=41
	goto :End
)

echo Copy and Unzip "%PISEmbeddedSDK_ZIPFileName%" to "%PISEmdededSDK_DESTINATION_PATH%"
call :DoCopy "%SRC_PISEmbeddedSDK_ZIP%%" "%PISEmdededSDK_DESTINATION_PATH%" || SET EXIT_CODE=42 && goto :End

::If you got the error 43, verify the version of 7-zip. Version 15.14 and over is required.
"%ZIP_PATH%" x "%PISEmbeddedSDK_ZIPFileName%" -o. -aoa -bb3							
if ERRORLEVEL 1 (
	echo Failed to unzip "%PISEmbeddedSDK_ZIPFileName%"
	SET EXIT_CODE=43
)

call :DoDeleteFile "%PISEmbeddedSDK_ZIPFileName%"

if not "%EXIT_CODE%" == "0" goto :End

:End

if not "%EXIT_CODE%" == "0" echo %~nx0 failed with exit code %EXIT_CODE%
if "%EXIT_CODE%" == "0" echo %~nx0 succeeded
EXIT /B %EXIT_CODE%

::=====================================================================================
:: FUNCTION DoCreateDir
:: Create a directory if it does not exists. On failure, proper error message is generated
:: Parameter: - name of the directory to create (shall be quoted)
:: Return: 0 on success, 1 on failure
::=====================================================================================
:DoCreateDir
SETLOCAL
SET RETCODE=0

if not exist "%~1" (
	mkdir "%~1"
	if ERRORLEVEL 1 (	
		echo Cannot create directory "%~1"
		SET RETCODE=1
	)
)

ENDLOCAL && exit /B %RETCODE%

::=====================================================================================
:: FUNCTION DoCopy
:: Copy a file. On failure, proper error message is generated
:: Parameter2: 1. name of the file to copy
::             2. destination  
:: Return: 0 on success, 1 on failure
::=====================================================================================
:DoCopy
echo Copy "%~1" to "%~2"
copy /B /V /Y "%~1" "%~2" || echo Failed to copy "%~1" to "%~2"
exit /B %ERRORLEVEL%

::=====================================================================================
:: FUNCTION DoDeleteDir
:: Delete a directory and all if content if exist.
:: Parameter: - name of the directory to delete
:: Return: 0 on success, 1 on failure
::=====================================================================================
:DoDeleteDir
SETLOCAL
SET RETCODE=0

if exist "%~1" (
	echo Delete directory "%~1"
	DEL /F /S /Q "%~1" 1>NUL
	RMDIR /S /Q "%~1" 
	IF ERRORLEVEL 1 (
		echo Cannot remove directory "%~1"
		SET RETCODE=1
	)
)
ENDLOCAL && EXIT /B %RETCODE%

::=====================================================================================
:: FUNCTION DoDeleteFile
:: Delete a file if exist.
:: Parameter: - name of the file to delete
:: Return: 0 on success, 1 on failure
::=====================================================================================
:DoDeleteFile
SETLOCAL
SET RETCODE=0

if exist "%~1" (
	echo Delete file "%~1"
	DEL /F /Q "%~1"
	IF ERRORLEVEL 1 (
		echo Cannot delete file "%~1"
		SET RETCODE=1
	)
)
ENDLOCAL && EXIT /B %RETCODE%