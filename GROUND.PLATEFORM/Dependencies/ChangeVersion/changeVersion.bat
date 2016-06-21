::=====================================================================================
:: File name      : 					changeVersion.bat
:: MakeFile name  : 
:: Description    : 	copy all files from the voute into the source code
::				  : 				   copyVoute.bat				
:: Created        :						  ????
:: Updated        :						  2016-06-21
::=====================================================================================
@echo off
SETLOCAL
set SCRIPT_PATH=%~dp0

if "%VERSION_NUMBER%"=="" call %SCRIPT_PATH%..\..\..\config.bat version_number 

SET ERROR_CODE=0

if "%SETUP_VERSION%"=="" (
	echo Variable SETUP_VERSION not defined
	SET ERROR_CODE=1
	goto :End
)

if "%VERSION_NUMBER%"=="" (
	echo Variable VERSION_NUMBER not defined
	SET ERROR_CODE=2
	goto :End
)

if "%SETUP_PRODUCT_CODE_GUID%" == "" (
	echo Variable SETUP_PRODUCT_CODE_GUID not defined
	SET ERROR_CODE=3
	goto :End
)

if "%SETUP_PACKAGE_CODE_GUID%" == "" (
	echo Variable SETUP_PACKAGE_CODE_GUID not defined
	SET ERROR_CODE=4
	goto :End
)


set WORKING_DIR=%SCRIPT_PATH%..\..

for /f %%f in ('dir "%WORKING_DIR%\*Setup.vdproj" /s/b') do (
	echo Update file "%%f"
	cscript /NoLogo  "%SCRIPT_PATH%\changeSetupVersion.vbs" "%%f" %SETUP_VERSION% %SETUP_PRODUCT_CODE_GUID% %SETUP_PACKAGE_CODE_GUID%
	if ERRORLEVEL 1 (
		echo Updating the version on file "%%f" FAILED.
		SET ERROR_CODE=5
		goto :End
	)
)

for /f %%f in ('dir "%WORKING_DIR%\AssemblyInfo.cs" /s/b') do (
	echo Update file "%%f"
	cscript /NoLogo /B "%SCRIPT_PATH%\changeAssemblyVersion.vbs" "%%f" %VERSION_NUMBER%
	if ERRORLEVEL 1 (
		echo Updating the version on file "%%f" FAILED.
		SET ERROR_CODE=6
		goto :End
	)
)
:End

if "%EXIT_CODE%" == "0" echo Success
if not "%EXIT_CODE%" == "0" echo changeVersion fail with error: %EXIT_CODE% > "%~dp0changeversionLog.txt"

exit /B %ERROR_CODE%
