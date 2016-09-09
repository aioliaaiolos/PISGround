::=====================================================================================
:: File name      : deliver.bat
:: Description    : Package the source code, PIS-Ground server setup(URBAN and SIVENG),
::                : and SDK (URBAN and SIVENG).
::                : Copy all packages into the GROUND_DELIVERY path.
:: Updated        :	2016-09-08
::=====================================================================================
@echo off
SETLOCAL

call %~dp0config.bat version_number 
SET EXIT_CODE=0

TYPE NUL > "%~dp0\deliverLog.txt"

set ROOT_PATH=%~dp0
SET SRC_GroundServer_RequisitesZip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_ZIPFileName%.zip
SET SRC_GroundServer_Requisites_x64Zip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_x64_ZIPFileName%.zip
SET SRC_GroundServer_Requisites_x86Zip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_x86_ZIPFileName%.zip
SET GROUND_TEST_RESULTS_PATH=%ROOT_PATH%GROUND_DELIVERY\UnitTests
SET GROUND_TEST_RESULTS_FOLDER=%ROOT_PATH%GROUND_TEST_RESULTS
SET T2GVEHICLEINFOSIVENG_AND_T2GVEHICLEINFOURBAN_PATH=%ROOT_PATH%GROUND_DELIVERY
SET T2GVEHICLEINFOSIVENG_FILE=%ROOT_PATH%GROUND.PLATEFORM\Setup\T2GConfiguration\T2GVehicleInfo_SIVENG.xml
SET T2GVEHICLEINFOURBAN_FILE=%ROOT_PATH%GROUND.PLATEFORM\Setup\T2GConfiguration\T2GVehicleInfo_URBAN.xml

::=====================================================================================
:: CHECK REQUIREMENTS
::=====================================================================================

echo Verify conditions to be able to perform deliver process
if "%VERSION_NUMBER%" == "" (
	echo Variable VERSION_NUMBER not set
	SET EXIT_CODE=1
	goto :End
)

if "%DELIVERY_PATH%" == "" (
	echo Variable DELIVERY_PATH not set
	SET EXIT_CODE=2
	goto :End
)

if "%ROOT_PATH%" == "" (
	echo Variable ROOT_PATH not set
	SET EXIT_CODE=3
	goto :End
)

if "%Ground_SourceCode_ZIPFilename%" == "" (
	echo Variable Ground_SourceCode_ZIPFilename not set
	SET EXIT_CODE=4
	goto :End
)

set TARGET_SOURCE_DIR=%DELIVERY_PATH%\GROUND_SOURCE_CODE

SET VCVARSFILE=%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat

IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=%ProgramFiles%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" (
	echo "Visual Studio 2008 is required to build this application. vcvars32.bat file cannot be found"
	SET EXIT_CODE=5
	goto :End
)

if not exist "%WORKING_DIR%GROUND.PLATEFORM\Configuration" (
	echo The required directory NOT FOUND: "%WORKING_DIR%GROUND.PLATEFORM\Configuration"
	SET EXIT_CODE=6
	goto :End
)

if not exist "%WORKING_DIR%GROUND.PLATEFORM\Setup\bin\release" (
	echo The required directory NOT FOUND: "%WORKING_DIR%GROUND.PLATEFORM\Setup\bin\release"
	SET EXIT_CODE=7
	goto :End
)

if not exist "%WORKING_DIR%GROUND.PLATEFORM\WSDL\" (
	echo The required directory NOT FOUND: "%WORKING_DIR%GROUND.PLATEFORM\WSDL\"
	SET EXIT_CODE=8
	goto :End
)

if not exist "%WORKING_DIR%\PISEmbeddedSDK\SIF\WSDL" (
	echo The required directory NOT FOUND: "%WORKING_DIR%\PISEmbeddedSDK\SIF\WSDL"
	SET EXIT_CODE=9
	goto :End
)

if not exist "%SRC_GroundServer_RequisitesZip%" (
	echo PIS-Ground server requisites zip file does not exist at this location "%SRC_GroundServer_RequisitesZip%"
	SET EXIT_CODE=10
	goto :End
)

if not exist "%SRC_GroundServer_Requisites_x64Zip%" (
	echo PIS-Ground server requisites for x64 zip file does not exist at this location "%SRC_GroundServer_Requisites_x64Zip%"
	SET EXIT_CODE=11
	goto :End
)

if not exist "%SRC_GroundServer_Requisites_x86Zip%" (
	echo PIS-Ground server requisites for x86 zip file does not exist at this location "%SRC_GroundServer_Requisites_x86Zip%"
	SET EXIT_CODE=12
	goto :End
)

::=====================================================================================
:: CREATE AND EMPTY OUTPUT DIRECTORIES
::=====================================================================================

echo Create and emtpy output directories
if exist "%DELIVERY_PATH%" (
	rmdir /S /Q "%DELIVERY_PATH%"
	if ERRORLEVEL 1 (
		echo Cannot empty the directory "%DELIVERY_PATH%"
		SET EXIT_CODE=13
		goto :End
	)
)

mkdir "%DELIVERY_PATH%"
if ERRORLEVEL 1 (
	echo Cannot create the directory "%DELIVERY_PATH%"
	SET EXIT_CODE=14
	goto :End
)

if not exist %DELIVERY_PATH% (
	mkdir %DELIVERY_PATH%
	if ERRORLEVEL 1 (
		echo Cannot create the directory "%DELIVERY_PATH%"
		SET EXIT_CODE=15
		goto :End
	)
)

mkdir %TARGET_SOURCE_DIR%
if ERRORLEVEL 1 (
	echo Cannot Create the directory "%TARGET_SOURCE_DIR%"
	SET EXIT_CODE=16
	goto :End
)

mkdir "%GROUND_TEST_RESULTS_PATH%"
if ERRORLEVEL 1 (
	echo Cannot Create the directory "%TARGET_SOURCE_DIR%"
	SET EXIT_CODE=17
	goto :End
)

::=====================================================================================
:: COPY REQUISITE FILES
::=====================================================================================
echo copy PIS-Ground requisite files

xcopy "%SRC_GroundServer_RequisitesZip%" "%DELIVERY_PATH%" /Y
if ERRORLEVEL 1 (
	SET EXIT_CODE=18
	goto :End
)
xcopy "%SRC_GroundServer_Requisites_x64Zip%" "%DELIVERY_PATH%" /Y
if ERRORLEVEL 1 ( 
	SET EXIT_CODE=19
	goto :End
)	
xcopy "%SRC_GroundServer_Requisites_x86Zip%" "%DELIVERY_PATH%" /Y
if ERRORLEVEL 1 ( 
	SET EXIT_CODE=20
	goto :End
)	
xcopy "%GROUND_TEST_RESULTS_FOLDER%" "%GROUND_TEST_RESULTS_PATH%" /Y
if ERRORLEVEL 1 ( 
	SET EXIT_CODE=21
	goto :End
)	
xcopy "%T2GVEHICLEINFOSIVENG_FILE%" "%T2GVEHICLEINFOSIVENG_AND_T2GVEHICLEINFOURBAN_PATH%" /Y
if ERRORLEVEL 1 ( 
	SET EXIT_CODE=22
	goto :End
)	
xcopy "%T2GVEHICLEINFOURBAN_FILE%" "%T2GVEHICLEINFOSIVENG_AND_T2GVEHICLEINFOURBAN_PATH%" /Y
if ERRORLEVEL 1 (
	SET EXIT_CODE=23 
	goto :End
)	

::=====================================================================================
:: CREATE THE PACKAGE FILES
::=====================================================================================

echo Package URBAN
echo Package URBAN >> "%~dp0\deliverLog.txt"
call "%WORKING_DIR%GROUND.PLATEFORM\Configuration\PackageURBAN.bat" "%WORKING_DIR%GROUND.PLATEFORM\Setup\bin\release" "%DELIVERY_PATH%\" %VERSION_NUMBER%  >> "%~dp0\deliverLog.txt" 2>&1
IF ERRORLEVEL 1 (
	echo PackageURBAN.bat Failed >> "%~dp0\deliverLog.txt"
	set EXIT_CODE=24
)

echo Package SIVENG
echo Package SIVENG >> "%~dp0\deliverLog.txt"
call "%WORKING_DIR%GROUND.PLATEFORM\Configuration\PackageSIVENG.bat" "%WORKING_DIR%GROUND.PLATEFORM\Setup\bin\release" "%DELIVERY_PATH%\" %VERSION_NUMBER% >> "%~dp0\deliverLog.txt" 2>&1
IF ERRORLEVEL 1 (
	echo PackageSIVENG.bat Failed >> "%~dp0\deliverLog.txt"
	set EXIT_CODE=11
)

echo Package URBAN SDK
echo Package URBAN SDK >> "%~dp0\deliverLog.txt"
call "%WORKING_DIR%GROUND.PLATEFORM\Configuration\PackageURBAN_SDK.bat" "%WORKING_DIR%GROUND.PLATEFORM"\WSDL\ "%DELIVERY_PATH%\" %VERSION_NUMBER% >> "%~dp0\deliverLog.txt" 2>&1
IF ERRORLEVEL 1 (
	echo PackageURBAN_SDK.bat Failed >> "%~dp0\deliverLog.txt"
	set EXIT_CODE=12
)

echo Package SIVENG SDK
echo Package SIVENG SDK >> "%~dp0\deliverLog.txt"
call "%WORKING_DIR%GROUND.PLATEFORM\Configuration\PackageSIVENG_SDK.bat" "%WORKING_DIR%GROUND.PLATEFORM"\WSDL\ "%DELIVERY_PATH%\" %VERSION_NUMBER% >> "%~dp0\deliverLog.txt" 2>&1
IF ERRORLEVEL 1 (
	echo PackageSIVENG_SDK.bat Failed >> "%~dp0\deliverLog.txt"
	set EXIT_CODE=13
)

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: SOURCE CODE PACKAGING
::=====================================================================================
echo Create the zip file that contains the source code			
			
echo Create Ground source 
"%ZIP_PATH%" a "%DELIVERY_PATH%\GROUND_SOURCE_CODE\%Ground_SourceCode_ZIPFilename%.zip" -xr@"%ROOT_PATH%GROUND.PLATEFORM\util\excludeFileList.txt" >> "%~dp0\deliverLog.txt" 2>&1
if ERRORLEVEL 1 SET EXIT_CODE=14

:: When 7Zip fails, a .zip.tmp file might remains
IF EXIST "%DELIVERY_PATH%\GROUND_SOURCE_CODE\%Ground_SourceCode_ZIPFilename%.zip.tmp" DEL "%DELIVERY_PATH%\GROUND_SOURCE_CODE\%Ground_SourceCode_ZIPFilename%.zip.tmp"

if not "%EXIT_CODE%" == "0" goto :End


:End

if "%EXIT_CODE%" == "0" echo Deliver succeeded >> "%~dp0\deliverLog.txt"
if not "%EXIT_CODE%" == "0" echo Deliver failed with error: %EXIT_CODE% >>"%~dp0\deliverLog.txt"
if EXIST "%~dp0\deliverLog.txt" type "%~dp0\deliverLog.txt"
pause
exit /B "%EXIT_CODE%"