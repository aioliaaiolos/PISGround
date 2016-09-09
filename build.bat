::=====================================================================================
:: File name      : build.bat
:: Description    : build GROUND project (PISGround.sln)
::				  : Before building the project, version number is updated into the script.
:: Updated        :	2016-09-08
::
:: Parameters     :   /quick  Do not force rebuild
::=====================================================================================
@echo off

SETLOCAL
call %~dp0config.bat version_number 


SET EXIT_CODE=0
if "%VERSION_NUMBER%" == "" goto Skip

TYPE NUL >"%~dp0buildLog.txt" 

SET VCVARSFILE=%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat

IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=C:\Program Files(x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=%ProgramFiles%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" goto :VSMissing

SET PROJECT_FILE=%~dp0GROUND.PLATEFORM\PISGround.sln

if NOT EXIST "%PROJECT_FILE%" goto :ProjectFileMissing

echo Initialize Visual Studio environment by executing script "%VCVARSFILE%"
call "%VCVARSFILE%"
if ERRORLEVEL 1 goto :VCVARSFILESFAILED

SET BUILD_PARAM=/Build "Release"
if /I not "%1" == "/quick" SET BUILD_PARAM=/Rebuild "Release"

echo Update version number
echo execute script "%WORKING_DIR%GROUND.PLATEFORM\Dependencies\Util\changeVersion.bat"
call "%WORKING_DIR%GROUND.PLATEFORM\Util\ChangeVersion\changeVersion.bat" > "%~dp0buildLog.txt" 2>&1
if ERRORLEVEL 1 (
	echo Failed to update version number in files
	SET EXIT_CODE=1
	goto :EndRunScripts
)

echo Build PIS-Ground project
echo Command line: devenv "%PROJECT_FILE%" %BUILD_PARAM%
devenv "%PROJECT_FILE%" %BUILD_PARAM% >> "%~dp0buildLog.txt" 2>&1
if ERRORLEVEL 1 (
	SET EXIT_CODE=2 
	goto :End
)

:EndRunScripts

goto End
:ProjectFileMissing
	echo The PIS-Ground visual studio solution file is missing at this location: "%PROJECT_FILE%"
	SET EXIT_CODE=3
	goto :End


:VCVARSFILESFAILED
	echo The script file "%VCVARSFILE%" terminated with error
	SET EXIT_CODE=4
	goto :End

:VSMissing
	echo "Visual Studio 2008 is required to build this application. vcvars32.bat file cannot be found"
	SET EXIT_CODE=5
	goto :End

:Skip
	echo VERSION_NUMBER not set.
	SET EXIT_CODE=6
	
:End

if not "%EXIT_CODE%" == "0" echo Build Failed with exit code %EXIT_CODE% >> "%~dp0buildLog.txt"
if "%EXIT_CODE%" == "0" echo Build Succeeded >> "%~dp0buildLog.txt"
if EXIST "%~dp0buildLog.txt" type "%~dp0buildLog.txt"
pause
EXIT /B %EXIT_CODE%