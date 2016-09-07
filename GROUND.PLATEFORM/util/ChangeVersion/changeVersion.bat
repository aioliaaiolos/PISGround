::=====================================================================================
:: File name      : 					changeVersion.bat
:: Description    : 	This script update the version number in every setup project*Setup
::                :     and AssemblyInfo.cs file.
:: Updated        :						  2016-08-29
::=====================================================================================
@echo off
SETLOCAL
setlocal EnableDelayedExpansion

set "SCRIPT_PATH=%~dp0"

if "%VERSION_NUMBER%"=="" call "%SCRIPT_PATH%..\..\..\config.bat" version_number 

SET EXIT_CODE=0

if "%SETUP_VERSION%"=="" (
	echo Variable SETUP_VERSION not defined
	SET EXIT_CODE=1
	goto :End
)

if "%VERSION_NUMBER%"=="" (
	echo Variable VERSION_NUMBER not defined
	SET EXIT_CODE=2
	goto :End
)

if "%T2G_BUILD_NUMBER%" == "" (
	echo Variable T2G_BUILD_NUMBER not defined
	SET EXIT_CODE=3
	goto :End
)

if "%SETUP_PACKAGE_CODE_GUID%" == "" (
	echo Variable SETUP_PACKAGE_CODE_GUID not defined
	SET EXIT_CODE=4
	goto :End
)

set WORKING_DIR=%SCRIPT_PATH%..\..

for /f "delims=" %%f in ('dir "%WORKING_DIR%\*Setup.vdproj" /s/b') do (
	echo Update file "%%f"
	cscript /NoLogo /B "%SCRIPT_PATH%\changeSetupVersion.vbs" "%%f" %SETUP_VERSION% %SETUP_PACKAGE_CODE_GUID%
	if ERRORLEVEL 1 (
		echo Updating the version on file "%%f" FAILED with error !ERRORLEVEL!.
		SET EXIT_CODE=5
		goto :End
	)
)

for /f "delims=" %%f in ('dir "%WORKING_DIR%\AssemblyInfo.cs" /s/b') do (
	echo Update file "%%f"
	cscript /Nologo /B "%SCRIPT_PATH%\changeAssemblyVersion.vbs" "%%f" %VERSION_NUMBER%
	if ERRORLEVEL 1 ( 
		echo Updating the version on file "%%f" FAILED with error !ERRORLEVEL!.
		SET EXIT_CODE=6
		goto :End
	)
)
:End

if "%EXIT_CODE%" == "0" echo %~nx0 succeeded
if not "%EXIT_CODE%" == "0" echo %~nx0 failed with error: %EXIT_CODE%

exit /B %EXIT_CODE%
