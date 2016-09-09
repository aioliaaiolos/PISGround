::=====================================================================================
:: File name      : build.bat
:: Description    : build GROUND project (PISGround.sln)
::				  : Before building the project, version number is updated into the script.
:: Updated        :	2016-09-09
::
:: Parameters     :   /quick  Do not force rebuild
::=====================================================================================
@echo off

SETLOCAL
SETLOCAL EnableDelayedExpansion		

call "%~dp0config.bat" version_number 

SET EXIT_CODE=0
SET QUICK_MODE=0
if /I "%1" == "/quick" SET QUICK_MODE=1

SET "PROJECT_FILE=%~dp0GROUND.PLATEFORM\PISGround.sln"


TYPE NUL >"%~dp0buildLog.txt" 

:: ===========================
:: Initialization
:: ===========================

SET BUILD_PARAM=/Build "Release"
if "%QUICK_MODE%"=="0" SET BUILD_PARAM=/Rebuild "Release"

>>"%~dp0buildLog.txt" 2>&1 (
	if "%VERSION_NUMBER%" == "" (
		echo VERSION_NUMBER not set.
		SET EXIT_CODE=1
		goto :End
	)

	SET "VCVARSFILE=%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
	IF NOT EXIST "!VCVARSFILE!" SET "VCVARSFILE=C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
	IF NOT EXIST "!VCVARSFILE!" SET "VCVARSFILE=%ProgramFiles%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat"
	IF NOT EXIST "!VCVARSFILE!" (
		echo "Visual Studio 2008 is required to build this application. vcvars32.bat file cannot be found"
		SET EXIT_CODE=2
		goto :End
	)

	if NOT EXIST "%PROJECT_FILE%" (
		echo The PIS-Ground visual studio solution file is missing at this location: "%PROJECT_FILE%"
		SET EXIT_CODE=3
		goto :End
	)


	echo Initialize Visual Studio environment by executing script "!VCVARSFILE!"
	call "!VCVARSFILE!"
	if ERRORLEVEL 1 (
		echo The script file "!VCVARSFILE!" terminated with error
		SET EXIT_CODE=4
		goto :End	
	)
)

:: ===========================
:: Update the version number
:: ===========================

>>"%~dp0buildLog.txt" 2>&1 (
	echo Updating version number...>CON
	echo Update version number
	echo execute script "%WORKING_DIR%GROUND.PLATEFORM\Dependencies\Util\changeVersion.bat"
	call "%WORKING_DIR%GROUND.PLATEFORM\Util\ChangeVersion\changeVersion.bat"
	if ERRORLEVEL 1 (
		echo Failed to update version number in files
		SET EXIT_CODE=5
		goto :End
	)
)

:: ===========================
:: Build PIS-Ground Solution
:: ===========================

>>"%~dp0buildLog.txt" 2>&1 (
	echo Building PIS-Ground project...>CON
	echo Build PIS-Ground project
	echo Command line: devenv "%PROJECT_FILE%" %BUILD_PARAM%
	devenv "%PROJECT_FILE%" %BUILD_PARAM%
	if ERRORLEVEL 1 (
		SET EXIT_CODE=6
		goto :End
	)
)

:End

if not "%EXIT_CODE%" == "0" echo Build Failed with exit code %EXIT_CODE% >> "%~dp0buildLog.txt"
if "%EXIT_CODE%" == "0" echo Build Succeeded >> "%~dp0buildLog.txt"
if EXIST "%~dp0buildLog.txt" type "%~dp0buildLog.txt"
pause
EXIT /B %EXIT_CODE%