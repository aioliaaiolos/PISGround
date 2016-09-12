::=====================================================================================
:: File name      : deliver.bat
:: Description    : Package the source code, PIS-Ground server setup(URBAN and SIVENG),
::                : and SDK (URBAN and SIVENG).
::                : Copy all packages into the GROUND_DELIVERY path.
:: Updated        :	2016-09-12
::=====================================================================================
@echo off
SETLOCAL
SETLOCAL EnableDelayedExpansion		

call "%~dp0config.bat" version_number 
SET EXIT_CODE=0

TYPE NUL > "%~dp0deliverLog.txt"

set "ROOT_PATH=%~dp0"
SET "SRC_GroundServer_RequisitesZip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_ZIPFileName%"
SET "SRC_GroundServer_Requisites_x64Zip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_x64_ZIPFileName%"
SET "SRC_GroundServer_Requisites_x86Zip=%VOUTE_PATH%\PIS\GroundServer\%GroundServer_Requisites_x86_ZIPFileName%"
SET "GROUND_TEST_RESULTS_PATH=%ROOT_PATH%GROUND_DELIVERY\UnitTests"
SET "GROUND_TEST_RESULTS_FOLDER=%ROOT_PATH%GROUND_TEST_RESULTS"
SET "T2GVEHICLEINFOSIVENG_AND_T2GVEHICLEINFOURBAN_PATH=%ROOT_PATH%GROUND_DELIVERY"
SET "T2GVEHICLEINFOSIVENG_FILE=%ROOT_PATH%GROUND.PLATEFORM\Setup\T2GConfiguration\T2GVehicleInfo_SIVENG.xml"
SET "T2GVEHICLEINFOURBAN_FILE=%ROOT_PATH%GROUND.PLATEFORM\Setup\T2GConfiguration\T2GVehicleInfo_URBAN.xml"

::=====================================================================================
:: CHECK REQUIREMENTS
::=====================================================================================

>> "%~dp0deliverLog.txt" 2>&1 (

	echo Verifying conditions to be able to perform deliver process...>CON
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

	SET "VCVARSFILE=%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"

	IF NOT EXIST "!VCVARSFILE!" SET "VCVARSFILE=C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
	IF NOT EXIST "!VCVARSFILE!" SET "VCVARSFILE=%ProgramFiles%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
	IF NOT EXIST "!VCVARSFILE!" (
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

	if EXIST "%VOUTE_PATH%" (
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
	) else (
		if not "%JENKINS_URL%"=="" (
			echo The voute directory is not accessible at this location: "%VOUTE_PATH%".
			echo On build machine, this directory shall be accessible. Otherwise, it is optionnal
			SET EXIT_CODE=13
			goto :End
		)
	)
)	

::=====================================================================================
:: CREATE AND EMPTY OUTPUT DIRECTORIES
::=====================================================================================

>> "%~dp0deliverLog.txt" 2>&1 (
	echo Creating and emptying output directories...>CON
	echo Create and emtpy output directories
	call :DoDeleteDir "%DELIVERY_PATH%" || SET EXIT_CODE=14 && goto :End
	call :DoCreateDir "%DELIVERY_PATH%" || SET EXIT_CODE=15 && goto :End

	call :DoCreateDir "%TARGET_SOURCE_DIR%" || SET EXIT_CODE=16 && goto :End
	call :DoCreateDir "%GROUND_TEST_RESULTS_PATH%" || SET EXIT_CODE=17 && goto :End
)

::=====================================================================================
:: COPY REQUISITE FILES
::=====================================================================================
>> "%~dp0deliverLog.txt" 2>&1 (
	echo copying PIS-Ground requisite files...>CON
	echo copy PIS-Ground requisite files

	if EXIST "%VOUTE_PATH%" (

		call :DoCopy "%SRC_GroundServer_RequisitesZip%" "%DELIVERY_PATH%" || SET EXIT_CODE=18 && goto :End
		call :DoCopy "%SRC_GroundServer_Requisites_x64Zip%" "%DELIVERY_PATH%" || SET EXIT_CODE=19 && goto :End
		call :DoCopy "%SRC_GroundServer_Requisites_x86Zip%" "%DELIVERY_PATH%" || SET EXIT_CODE=20 && goto :End
	)

	REM Copy only test results folder if it exist. 
	if EXIST "%GROUND_TEST_RESULTS_FOLDER%" call :DoCopy "%GROUND_TEST_RESULTS_FOLDER%" "%GROUND_TEST_RESULTS_PATH%" || SET EXIT_CODE=21 && goto :End
	
	call :DoCopy "%T2GVEHICLEINFOSIVENG_FILE%" "%T2GVEHICLEINFOSIVENG_AND_T2GVEHICLEINFOURBAN_PATH%" || SET EXIT_CODE=21 && goto :End
	call :DoCopy "%T2GVEHICLEINFOURBAN_FILE%" "%T2GVEHICLEINFOSIVENG_AND_T2GVEHICLEINFOURBAN_PATH%" || SET EXIT_CODE=22 && goto :End
)

::=====================================================================================
:: CREATE THE PACKAGE FILES
::=====================================================================================
>> "%~dp0deliverLog.txt" 2>&1 (

	echo Packaging URBAN...>CON
	echo Package URBAN
	call "%WORKING_DIR%GROUND.PLATEFORM\Configuration\PackageURBAN.bat" "%WORKING_DIR%GROUND.PLATEFORM\Setup\bin\release" "%DELIVERY_PATH%" %VERSION_NUMBER%
	IF ERRORLEVEL 1 (
		echo PackageURBAN.bat Failed
		set EXIT_CODE=23
	)

	echo Packaging SIVENG...>CON
	echo Package SIVENG
	call "%WORKING_DIR%GROUND.PLATEFORM\Configuration\PackageSIVENG.bat" "%WORKING_DIR%GROUND.PLATEFORM\Setup\bin\release" "%DELIVERY_PATH%" %VERSION_NUMBER%
	IF ERRORLEVEL 1 (
		echo PackageSIVENG.bat Failed
		set EXIT_CODE=24
	)

	echo Packaging URBAN SDK...>CON
	echo Package URBAN SDK
	call "%WORKING_DIR%GROUND.PLATEFORM\Configuration\PackageURBAN_SDK.bat" "%WORKING_DIR%GROUND.PLATEFORM\WSDL" "%DELIVERY_PATH%" %VERSION_NUMBER%
	IF ERRORLEVEL 1 (
		echo PackageURBAN_SDK.bat Failed
		set EXIT_CODE=25
	)

	echo Packaging SIVENG SDK...>CON
	echo Package SIVENG SDK
	call "%WORKING_DIR%GROUND.PLATEFORM\Configuration\PackageSIVENG_SDK.bat" "%WORKING_DIR%GROUND.PLATEFORM\WSDL" "%DELIVERY_PATH%" %VERSION_NUMBER%
	IF ERRORLEVEL 1 (
		echo PackageSIVENG_SDK.bat Failed
		set EXIT_CODE=26
	)
)

if not "%EXIT_CODE%" == "0" goto :End

::=====================================================================================
:: SOURCE CODE PACKAGING
::=====================================================================================
>> "%~dp0deliverLog.txt" 2>&1 (

	echo Creating the zip file that contains the source code...>CON
	echo Create the zip file that contains the source code			
				
	cd /D "%ROOT_PATH%" || echo Cannot move to directory "%ROOT_PATH%" && SET EXIT_CODE=27 && goto :End
				
	"%ZIP_PATH%" a "%DELIVERY_PATH%\GROUND_SOURCE_CODE\%Ground_SourceCode_ZIPFilename%" -xr@"%ROOT_PATH%GROUND.PLATEFORM\util\excludeFileList.txt"
	if ERRORLEVEL 1 (
		echo Failed to create the zip file "%DELIVERY_PATH%\GROUND_SOURCE_CODE\%Ground_SourceCode_ZIPFilename%"
		SET EXIT_CODE=28
	)

	:: When 7Zip fails, a .zip.tmp file might remains
	IF EXIST "%DELIVERY_PATH%\GROUND_SOURCE_CODE\%Ground_SourceCode_ZIPFilename%.tmp" DEL "%DELIVERY_PATH%\GROUND_SOURCE_CODE\%Ground_SourceCode_ZIPFilename%.tmp"
)

if not "%EXIT_CODE%" == "0" goto :End


:End
IF "%EXIT_CODE%"=="0" (
	if NOT EXIST "%VOUTE_PATH%" (
		echo WARNING: Requisites files where not copied from the voute path because the folder "%VOUTE_PATH%" does not exist. >> "%~dp0deliverLog.txt"
		echo Ignore this problem if you perform a rebuild from a delivery source code package or you don't have access to the company network. >> "%~dp0deliverLog.txt"
	)
)
 
if "%EXIT_CODE%" == "0" echo %~nx0 succeeded >> "%~dp0deliverLog.txt"
if not "%EXIT_CODE%" == "0" echo %~nx0 failed with error: %EXIT_CODE% >> "%~dp0deliverLog.txt"
if EXIST "%~dp0deliverLog.txt" type "%~dp0deliverLog.txt"
pause
exit /B "%EXIT_CODE%"

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