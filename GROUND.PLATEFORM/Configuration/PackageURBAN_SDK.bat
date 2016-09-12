::=====================================================================================
:: File name      : PackageURBAN_SDK.bat
:: Description    : Create the PIS-Ground SDK package for URBAN platform
::				  : 	
:: Update         :	2016-09-09			
::=====================================================================================
@echo off

SETLOCAL
SETLOCAL EnableDelayedExpansion

SET EXIT_CODE=0

if "%~1"=="" goto error
if "%~2"=="" goto error
if "%~3"=="" goto error

SET "DEST_PATH=%~1\urban"
SET "SRC_PATH=%~1"
SET "ZIPFILENAME=%~2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_URBAN.zip"

set "ZIP_PATH=%ProgramFiles(x86)%\7-Zip\7z.exe"
IF NOT EXIST "%ZIP_PATH%" set "ZIP_PATH=C:\Program Files (x86)\7-Zip\7z.exe"
IF NOT EXIST "%ZIP_PATH%" set "ZIP_PATH=%ProgramFiles%\7-Zip\7z.exe"
if NOT EXIST "%ZIP_PATH%" (	
	echo 7-zip is not installed.
	SET EXIT_CODE=1
	goto :End
)

if "%~1"=="" goto error
if "%~2"=="" goto error
if "%~3"=="" goto error

call :DoDeleteFile "%ZIPFILENAME%" || SET EXIT_CODE=2 && goto :End
call :DoDeleteDir "%DEST_PATH%" || SET EXIT_CODE=3 && goto :End

(call :DoCreateDir "%DEST_PATH%" && call :DoCreateDir "%DEST_PATH%\wsdl" && call :DoCreateDir "%DEST_PATH%\wsdl\Schema") || SET EXIT_CODE=4 && goto :End

SET "DIR_LIST=Datapackage InfotainmentJournaling LiveVideoControl Maintenance InstantMessage Session RealTime"

SET "MISSINGFILES="

for %%d in (%DIR_LIST%) do (
	call :DoCopyDirectory "%SRC_PATH%\%%d" "%DEST_PATH%\%%d\" || SET "MISSINGFILES=!MISSINGFILES!%%d "
)

call :DoCopyDirectory "%SRC_PATH%\MissionUrban" "%DEST_PATH%\Mission" || SET "MISSINGFILES=%MISSINGFILES%MissionUrban "
call :DoCopyDirectory "%SRC_PATH%\..\..\PISEmbeddedSDK\SIF\wsdl\AppGround" "%DEST_PATH%\wsdl\AppGround\" || SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\AppGround "
call :DoCopy "%SRC_PATH%\..\..\PISEmbeddedSDK\SIF\wsdl\Schema\Common.xsd" "%DEST_PATH%\wsdl\Schema\" || SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\Schema\Common.xsd "
call :DoCopy "%SRC_PATH%\..\..\PISEmbeddedSDK\SIF\wsdl\Schema\Notification.xsd" "%DEST_PATH%\wsdl\Schema\" || SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\Schema\Notification.xsd "


IF "%MISSINGFILES%" NEQ "" (
	echo ERROR: Some required directories or files are missing or cannot be copied : %MISSINGFILES%
	SET EXIT_CODE=5
	goto :End
)

"%ZIP_PATH%" a -r "%ZIPFILENAME%" "%DEST_PATH%\*.wsdl" "%DEST_PATH%\*.xsd"
IF ERRORLEVEL 1 (
	echo Error while creating the archive file^("%ZIPFILENAME%"^)
	SET EXIT_CODE=6
	goto :End
)

call :DoDeleteDir "%DEST_PATH%"

goto end

:error
echo syntax should be : %~nx0 "release directory path" "output directory path" "version" 
SET EXIT_CODE=7
goto :End

:end
if "%EXIT_CODE%"=="0" echo %~nx0 succeeded
if not "%EXIT_CODE%"=="0" echo %~nx0 failed with the EXIT_CODE : %EXIT_CODE%

exit /B %EXIT_CODE%

::=====================================================================================
:: FUNCTION DoDeleteFile
:: Delete a file
:: Parameter: - name of the file to delete
:: Return: 0 on success, 1 on failure
::=====================================================================================
:DoDeleteFile
SETLOCAL
SET RETCODE=0

if exist "%~1" (
	echo Delete file "%~1"
	DEL /F /S /Q "%~1" 1>NUL
	RMDIR /S /Q "%~1" 
	IF ERRORLEVEL 1 (
		echo Cannot remove file "%~1"
		SET RETCODE=1
	)
)
ENDLOCAL && EXIT /B %RETCODE%

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
:: FUNCTION DoCreateDir
:: Create a directory. On failure, proper error message is generated
:: Parameter2: 1. name of the directory to create
:: Return: 0 on success, 1 on failure
::=====================================================================================
:DoCreateDir
SETLOCAL
SET RETCODE=0
if not exist "%~1" (
	echo Create directory "%~1"
	mkdir "%~1"
	IF ERRORLEVEL 1 (
		echo Cannot create directory "%~1"
		SET RETCODE=1
	)
)
ENDLOCAL && EXIT /B %RETCODE%

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
:: FUNCTION DoCopyDirectory
:: Copy a directory and sub-directory to another directory. Empty directories included.
:: File .gitignore is not copied
:: Parameter2: 1. source directory to copy
::             2. destination directory
:: Return: 0 on success, 1 on failure
::=====================================================================================
:DoCopyDirectory
SETLOCAL

echo Copy directory "%~1" to "%~2"
echo -----------------------------
@echo on
xcopy /E /V /I /Y /EXCLUDE:.gitignore "%~1" "%~2" || echo Failed to copy directory "%~1" to "%~2"
@echo off
SET RETCODE=%ERRORLEVEL%
echo -----------------------------
ENDLOCAL & exit /B %RETCODE%

