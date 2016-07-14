::=====================================================================================
:: File name      : 					copyVoute.bat
:: Description    : 	copy all files from the voute into the source code
::				  : 				   copyVoute.bat				
:: Created        :						  2015-10-07
:: Updated        :						  2016-07-13
::=====================================================================================
@echo off

SETLOCAL
set ROOT_PATH=%~dp0..\..\

if "%VOUTE_PATH%"=="" call "%~dp0..\..\config.bat" version_number 
							
if "%VOUTE_PATH%"=="" (
	echo Variable VOUTE_PATH not defined
	SET EXIT_CODE=1
	goto :End
)
							
SET EXIT_CODE=0
SET DEPENDENCIES_DESTINATION_PATH=%ROOT_PATH%GROUND.PLATEFORM\Dependencies
SET WSDL_DESTINATION_PATH=%ROOT_PATH%GROUND.PLATEFORM\WSDL
SET SVCUTIL_FILE_NAME=%ROOT_PATH%GROUND.PLATEFORM\Dependencies\SvcUtil\SvcUtil.exe
SET SQLITE_FILE_NAME=%ROOT_PATH%GROUND.PLATEFORM\Dependencies\SQLITE\System.Data.SQLite.dll
SET SQLITE_DESTINATION_PATH=%ROOT_PATH%GROUND.PLATEFORM\Common\GroundCore\SQLite
SET TEST_PACKAGE_DESTINATION_PATH=%ROOT_PATH%GROUND.PLATEFORM\IntegrationTests
SET T2G_DESTINATION_PATH=%ROOT_PATH%T2G_DELIVERY\T2G.CLIENT.DELIVERY
SET PISEmdededSDK_DESTINATION_PATH=%ROOT_PATH%PISEmbeddedSDK
SET SRC_DependenciesZip=%VOUTE_PATH%\PIS\GroundServer\%Ground_Dependencies_ZIPFilename%.zip

:: Check the presence of requisites files even if we copy them only in the deliver scripts. 
:: This permits to ensure that everything can be retrieved

SET SRC_GroundServer_RequisitesZip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_ZIPFileName%.zip
SET SRC_GroundServer_Requisites_x64Zip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_x64_ZIPFileName%.zip
SET SRC_GroundServer_Requisites_x86Zip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_x86_ZIPFileName%.zip
SET SRC_Groundserver_TestPackages_ZIP=%VOUTE_PATH%\PIS\GroundServer\%Groundserver_TestPackages_ZIPFileName%.zip
SET SRC_T2GClientDelivery_ZIP=%VOUTE_PATH%\PIS\T2G\%T2GClientDelivery_ZIPFilename%.zip
SET SRC_PISEmbeddedSDK_ZIP=%VOUTE_PATH%\PIS\Embedded\%PISEmbeddedSDK_ZIPFileName%.zip

::=====================================================================================
:: CHECK REQUIRED VARIABLE
::=====================================================================================
echo Verify that required variables are defined and the value is in the proper format.

if "%VERSION_NUMBER%" == "" (
	echo Variable VERSION_NUMBER not defined
	SET EXIT_CODE=2
	goto :End
)

if "%SETUP_VERSION%" == "" (
	echo Variable SETUP_VERSION not defined
	SET EXIT_CODE=3
	goto :End
)

if "%SETUP_PACKAGE_CODE_GUID%" == "" (
	echo Variable SETUP_PACKAGE_CODE_GUID not defined
	SET EXIT_CODE=4
	goto :End
)

echo %SETUP_PACKAGE_CODE_GUID% | findstr /R /C:{[A-F0-9]*-[A-F0-9]*-[A-F0-9]*-[A-F0-9]*-[A-F0-9]*} > NUL
if ERRORLEVEL 1 (
	echo Variable SETUP_PACKAGE_CODE_GUID is not a valid GUID with all upper case letters
	echo SETUP_PACKAGE_CODE_GUID=%SETUP_PACKAGE_CODE_GUID%
	SET EXIT_CODE=5
	goto :End
)

echo %VERSION_NUMBER% | findstr /R /C:[0-9]*.[0-9]*.[0-9]*.[0-9]* > NUL
if ERRORLEVEL 1 (
	echo Variable VERSION_NUMBER is not a valid version in format 1.2.3.4
	echo VERSION_NUMBER=%VERSION_NUMBER%
	SET EXIT_CODE=6
	goto :End
)

echo %SETUP_VERSION% | findstr /R /C:[0-9]*.[0-9]*.[0-9][0-9][0-9][0-9] >NUL
if ERRORLEVEL 1 (
	echo Variable SETUP_VERSION is not a valid version in format 1.2.0000
	echo SETUP_VERSION=%SETUP_VERSION%
	SET EXIT_CODE=7
	goto :End
)

::=====================================================================================
:: CHECK REQUIRED FILES
::=====================================================================================

echo Verify that required files exists

if not exist "%VOUTE_PATH%" (
	echo Voute path does not exist at this location "%VOUTE_PATH%"
	SET EXIT_CODE=8
	goto :End
)

if not exist "%ROOT_PATH%" (
	echo Voute path does not exist at this location "%ROOT_PATH%"
	SET EXIT_CODE=9
	goto :End
)


if not exist "%DEPENDENCIES_DESTINATION_PATH%" (
	echo Dependencies destination path does not exist at this location "%DEPENDENCIES_DESTINATION_PATH%"
	SET EXIT_CODE=10
	goto :End
)

if not exist "%TEST_PACKAGE_DESTINATION_PATH%" (
	echo Test packages destination path does not exist at this location "%TEST_PACKAGE_DESTINATION_PATH%"
	SET EXIT_CODE=11
	goto :End
)


if not exist "%SRC_DependenciesZip%" (
	echo PIS-Ground dependencies zip file does not exist at this location "%SRC_DependenciesZip%"
	SET EXIT_CODE=12
	goto :End
)

if not exist "%ZIP_PATH%" (
	echo 7-Zip executable does not exist at this location "%ZIP_PATH%"
	SET EXIT_CODE=13
	goto :End
)

if not exist "%SRC_GroundServer_RequisitesZip%" (
	echo PIS-Ground server requisites zip file does not exist at this location "%SRC_GroundServer_RequisitesZip%"
	SET EXIT_CODE=14
	goto :End
)

if not exist "%SRC_GroundServer_Requisites_x64Zip%" (
	echo PIS-Ground server requisites for x64 zip file does not exist at this location "%SRC_GroundServer_Requisites_x64Zip%"
	SET EXIT_CODE=15
	goto :End
)

if not exist "%SRC_GroundServer_Requisites_x86Zip%" (
	echo PIS-Ground server requisites for x86 zip file does not exist at this location "%SRC_GroundServer_Requisites_x86Zip%"
	SET EXIT_CODE=16
	goto :End
)

if not exist "%SRC_Groundserver_TestPackages_ZIP%" (
	echo PIS-Ground server test packages zip file does not exist at this location "%SRC_Groundserver_TestPackages_ZIP%"
	SET EXIT_CODE=17
	goto :End
)

if not exist "%SRC_T2GClientDelivery_ZIP%" (
	echo T2G Client delivery zip file does not exist at this location "%SRC_T2GClientDelivery_ZIP%"
	SET EXIT_CODE=18
	goto :End
)

if not exist "%SRC_PISEmbeddedSDK_ZIP%" (
	echo PIS Emdedded SDK zip file does not exist at this location "%SRC_PISEmbeddedSDK_ZIP%"
	SET EXIT_CODE=19
	goto :End
)

::=====================================================================================
:: REMOVE PREVIOUS ZIP FILES
::=====================================================================================

echo Remove previous source code zip
if exist "%ROOT_PATH%\Ground_SourceCode*.zip" (
	DEL /F /Q "%ROOT_PATH%\Ground_SourceCode*.zip"
	if ERRORLEVEL 1 SET EXIT_CODE=20
)

echo Remove previous requisites files
if exist "%ROOT_PATH%\GroundServer_Requisites_PrefixFileName*.zip" (
	DEL /F /Q "%ROOT_PATH%\GroundServer_Requisites_PrefixFileName*.zip"
	if ERRORLEVEL 1 SET EXIT_CODE=21
)

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: Remove previous T2G_DELIVERY 
::=====================================================================================
echo Remove previous T2G delivery files

if exist "%T2G_DESTINATION_PATH%\.." (
	RMDIR /S /Q "%T2G_DESTINATION_PATH%\.."
	if ERRORLEVEL 1 SET EXIT_CODE=22
)

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: Remove previous PIS Embedded SDK
::=====================================================================================
echo Remove previous PIS Embedded SDK files

if exist "%PISEmdededSDK_DESTINATION_PATH%" (
	RMDIR /S /Q "%PISEmdededSDK_DESTINATION_PATH%"
	if ERRORLEVEL 1 SET EXIT_CODE=23
)

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: COPY AND UNZIP PIS-GROUND DEPENDENCIES
::=====================================================================================
echo Gets the PIS-Ground dependencies file.							

cd "%DEPENDENCIES_DESTINATION_PATH%"
if ERRORLEVEL 1 (
	SET EXIT_CODE=24
	goto :End
)

echo Copy and Unzip "%Ground_Dependencies_ZIPFilename%.zip" to "%DEPENDENCIES_DESTINATION_PATH%"
xcopy "%SRC_DependenciesZip%" .\ /Y
if ERRORLEVEL 1 (
	SET EXIT_CODE=25
	goto :End	
)

::Si vous avez l'erreur 27, vérifier que votre 7-zip est bien à la version 15.14 pi plus
"%ZIP_PATH%" x "%Ground_Dependencies_ZIPFilename%.zip" -o. -aoa -bb3							
if ERRORLEVEL 1 SET EXIT_CODE=26

DEL /F /Q "%Ground_Dependencies_ZIPFilename%.zip"

echo Copy to WSDL "%SVCUTIL_FILE_NAME%" to "%WSDL_DESTINATION_PATH%"
xcopy "%SVCUTIL_FILE_NAME%" "%WSDL_DESTINATION_PATH%" /Y
if ERRORLEVEL 1 (
	SET EXIT_CODE=27
	goto :End	
)

echo Copy to SQLITE  "%SQLITE_FILE_NAME%" to "%SQLITE_DESTINATION_PATH%"
xcopy "%SQLITE_FILE_NAME%" "%SQLITE_DESTINATION_PATH%" /Y
if ERRORLEVEL 1 (
	SET EXIT_CODE=28
	goto :End	
)
if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: COPY AND UNZIP PIS-GROUND TEST PACKAGES
::=====================================================================================
echo Gets the PIS-Ground test packages file.							

cd "%TEST_PACKAGE_DESTINATION_PATH%"
if ERRORLEVEL 1 (
	SET EXIT_CODE=29
	goto :End
)

echo Copy and Unzip "%Groundserver_TestPackages_ZIPFileName%.zip" to "%TEST_PACKAGE_DESTINATION_PATH%"
xcopy "%SRC_Groundserver_TestPackages_ZIP%" .\ /Y
if ERRORLEVEL 1 (
	SET EXIT_CODE=30
	goto :End	
)

::Si vous avez l'erreur 32, vérifier que votre 7-zip est bien à la version 15.14 pi plus
"%ZIP_PATH%" x "%Groundserver_TestPackages_ZIPFileName%.zip" -o. -aoa -bb3							
if ERRORLEVEL 1 SET EXIT_CODE=31

DEL /F /Q "%Groundserver_TestPackages_ZIPFileName%.zip"

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: COPY AND UNZIP T2G CLIENT DELIVERY FILES
::=====================================================================================
echo Gets the T2G Client delivery files

IF NOT EXIST "%T2G_DESTINATION_PATH%" (
	mkdir "%T2G_DESTINATION_PATH%"
	if ERRORLEVEL 1 SET EXIT_CODE=32
)

if not "%EXIT_CODE%" == "0" goto :End

cd "%T2G_DESTINATION_PATH%"
if ERRORLEVEL 1 (
	SET EXIT_CODE=33
	goto :End
)

echo Copy and Unzip "%T2GClientDelivery_ZIPFilename%.zip" to "%T2G_DESTINATION_PATH%"
xcopy "%SRC_T2GClientDelivery_ZIP%%" .\ /Y
if ERRORLEVEL 1 (
	SET EXIT_CODE=34
	goto :End	
)

::Si vous avez l'erreur 26, vérifier que votre 7-zip est bien à la version 15.14 pi plus
"%ZIP_PATH%" x "%T2GClientDelivery_ZIPFilename%.zip" -o. -aoa -bb3							
if ERRORLEVEL 1 SET EXIT_CODE=35

DEL /F /Q "%T2GClientDelivery_ZIPFilename%.zip"

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: COPY AND UNZIP PIS EMBEDDED SDK FILES
::=====================================================================================
echo Gets the PIS Embedded SDK files

IF NOT EXIST "%PISEmdededSDK_DESTINATION_PATH%%" (
	mkdir "%PISEmdededSDK_DESTINATION_PATH%"
	if ERRORLEVEL 1 SET EXIT_CODE=36
)

if not "%EXIT_CODE%" == "0" goto :End

cd "%PISEmdededSDK_DESTINATION_PATH%"
if ERRORLEVEL 1 (
	SET EXIT_CODE=37
	goto :End
)

echo Copy and Unzip "%PISEmbeddedSDK_ZIPFileName%.zip" to "%PISEmdededSDK_DESTINATION_PATH%"
xcopy "%SRC_PISEmbeddedSDK_ZIP%%" .\ /Y
if ERRORLEVEL 1 (
	SET EXIT_CODE=38
	goto :End	
)

::Si vous avez l'erreur 40, vérifier que votre 7-zip est bien à la version 15.14 pi plus
"%ZIP_PATH%" x "%PISEmbeddedSDK_ZIPFileName%.zip" -o. -aoa -bb3							
if ERRORLEVEL 1 SET EXIT_CODE=39

DEL /F /Q "%PISEmbeddedSDK_ZIPFileName%.zip"

if not "%EXIT_CODE%" == "0" goto :End

:End

if not "%EXIT_CODE%" == "0" echo Script CopyVoute terminated with error: %EXIT_CODE%
exit /B %EXIT_CODE%
ENDLOCAL
