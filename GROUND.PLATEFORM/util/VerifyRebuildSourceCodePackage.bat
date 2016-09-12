::=====================================================================================
:: File name      : VerifyRebuildSourceCodePackage.bat
:: Description    : Script that verify the generated source code package can be rebuild.
:: Updated        :	2016-09-12
::=====================================================================================
@echo off
SETLOCAL
SETLOCAL EnableDelayedExpansion		

call "%~dp0..\..\config.bat" version_number 
IF ERRORLEVEL 1 (
	echo script config.bat failed with error code %ERRORLEVEL%
	SET EXIT_CODE=1
	goto :End
)

SET EXIT_CODE=0

SET "TEMP_DIR=%WORKING_DIR%REBUILD_TEST"

::=====================================================================================
:: CHECK REQUIREMENTS
::=====================================================================================
echo Verify conditions to be able to check that source code package can be rebuild

SET "SRC_ZIP_FILE=%TARGET_SOURCE_DIR%\%Ground_SourceCode_ZIPFilename%" 

IF NOT EXIST "%SRC_ZIP_FILE%" (
	echo Source code package file does not exist at this location: "%SRC_ZIP_FILE%"
	SET EXIT_CODE=2
	goto :End
)

echo **********************************************************************************
echo ** Source code package archive to test: "%SRC_ZIP_FILE%"
echo ** Temporary directory                : "%TEMP_DIR%"
echo **********************************************************************************

::=====================================================================================
:: EMPTY AND CREATE TEMP_DIRECTORY
::=====================================================================================
call :DoDeleteDir "%TEMP_DIR%" || SET EXIT_CODE=3 && goto :End
call :DoCreateDir "%TEMP_DIR%" || SET EXIT_CODE=4 && goto :End

::=====================================================================================
:: Extract the achive
::=====================================================================================
"%ZIP_PATH%" x "%SRC_ZIP_FILE%" -o"%TEMP_DIR%"
IF ERRORLEVEL 1 (
	echo Error while unzipping source code package zip file: "%SRC_ZIP_FILE%"
	SET EXIT_CODE=5 && goto :End
)


::=====================================================================================
:: Verify required files
::=====================================================================================
SET "REQUIRED_DIRS=GROUND.PLATEFORM GROUND.PLATEFORM\util GROUND.PLATEFORM\Common GROUND.PLATEFORM\WSDL PISEmbeddedSDK PISEmbeddedSDK\CONFIG PISEmbeddedSDK\URBAN PISEmbeddedSDK\SIF T2G_DELIVERY T2G_DELIVERY\T2G.CLIENT.DELIVERY\WSDL"
for %%D in (%REQUIRED_DIRS%) do (
	if not exist "%TEMP_DIR%\%%D" ( 
		echo Source code package is invalid: required directory "%%D" does not exist in archive file "%SRC_ZIP_FILE%".
		SET EXIT_CODE=6 
		goto :End
	)
)

SET "REQUIRED_FILES=build.bat config.bat deliver.bat execute_test.bat GROUND.PLATEFORM\PISGround.sln PISEmbeddedSDK\SIF\wsdl\Schema\Common.xsd "
for %%F in (%REQUIRED_FILES%) do (
	if not exist "%TEMP_DIR%\%%F" ( 
		echo Source code package is invlaid: required file "%%F does not exist in archive file "%SRC_ZIP_FILE%".
		SET EXIT_CODE=7
		goto :End
	)
)


::=====================================================================================
:: Call the build script
::=====================================================================================
cd /D "%TEMP_DIR%" || echo Cannot move to directory "%TEMP_DIR%" || SET EXIT_CODE=8 && goto :End

call .\Build.bat || echo Source code package is invalid: build failed. && SET EXIT_CODE=9 && goto :End

::=====================================================================================
:: Call the deliver script
::=====================================================================================
cd /D "%TEMP_DIR%" || echo Cannot move to directory "%TEMP_DIR%" || SET EXIT_CODE=10 && goto :End
call .\Deliver.bat || echo Source code package is invalid: deliver failed. && SET EXIT_CODE=11 && goto :End

:End

call :DoDeleteDir "%TEMP_DIR%"
if "%EXIT_CODE%" == "0" echo Source code package is valid. Build and deliver succeeded.

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

