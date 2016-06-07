::=====================================================================================
:: File name      : 					build.bat
:: MakeFile name  : 
:: Description    : 			    build GROUND project
::				  : 			execute PISGround.sln project					
:: Created        :						  2015-09-30
:: Updated        :						  2016-05-20
::
:: Parameters     :   /quick  Do not force rebuild
::=====================================================================================
@echo off

SETLOCAL
call %~dp0config.bat version_number 


SET EXIT_CODE=0
if "%VERSION_NUMBER%" == "" goto Skip


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
echo execute script "%CURRENT_PATH%GROUND.PLATEFORM\Dependencies\ChangeVersion\changeVersion.bat"
call "%CURRENT_PATH%GROUND.PLATEFORM\Dependencies\ChangeVersion\changeVersion.bat" > "%~dp0buildLog.txt" 2>&1
if ERRORLEVEL 1 (
	echo Failed to update version number in files
	SET EXIT_CODE=9
	goto :EndRunScripts
)

echo Build PIS-Ground project
echo Command line: devenv "%PROJECT_FILE%" %BUILD_PARAM%
devenv "%PROJECT_FILE%" %BUILD_PARAM% >> "%~dp0buildLog.txt" 2>&1
if ERRORLEVEL 1 SET EXIT_CODE=10

:EndRunScripts
type "%~dp0buildLog.txt"

goto End
:ProjectFileMissing
	echo The PIS-Ground visual studio solution file is missing at this location: "%PROJECT_FILE%"
	SET EXIT_CODE=6
	goto :End


:VCVARSFILESFAILED
	echo The script file "%VCVARSFILE%" terminated with error
	SET EXIT_CODE=3
	goto :End

:VSMissing
	echo "Visual Studio 2008 is required to build this application. vcvars32.bat file cannot be found"
	SET EXIT_CODE=2
	goto :End

:Skip
	echo VERSION_NUMBER not set.
	SET EXIT_CODE=5
	
:End

echo ""
if not "%EXIT_CODE%" == "0" echo Build Failed with exit code %EXIT_CODE%
if "%EXIT_CODE%" == "0" echo Build Succeeded
pause
EXIT /B %EXIT_CODE%