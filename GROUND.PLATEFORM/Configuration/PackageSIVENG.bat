::=====================================================================================
:: File name      : 		PackageSIVENG.bat
:: Description    : Create the PIS-Ground setup package for SIVENG platform
:: Update         :			   2016-09-09		
::=====================================================================================
@echo off

SETLOCAL
SETLOCAL EnableDelayedExpansion

SET EXIT_CODE=0

if "%~1"=="" goto error
if "%~2"=="" goto error
if "%~3"=="" goto error

SET "DEST_PATH=%~1\siveng"
SET "SRC_PATH=%~1"
SET "ZIPFILENAME=%~2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_siveng.zip"
set "ZIP_PATH=%ProgramFiles(x86)%\7-Zip\7z.exe"
IF NOT EXIST "%ZIP_PATH%" set "ZIP_PATH=C:\Program Files (x86)\7-Zip\7z.exe"
IF NOT EXIST "%ZIP_PATH%" set "ZIP_PATH=%ProgramFiles%\7-Zip\7z.exe"
if NOT EXIST "%ZIP_PATH%" (	
	echo 7-zip is not installed.
	SET EXIT_CODE=1
	goto :End
)

call :DoDeleteFile "%ZIPFILENAME%" || SET EXIT_CODE=2 && goto :End

call :DoDeleteDir "%DEST_PATH%" || SET EXIT_CODE=3 && goto :End

mkdir "%DEST_PATH%"
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%DEST_PATH%"
	SET EXIT_CODE=4
	goto :End
)


SET "FILE_LIST=DatapackageSetup.msi InfotainmentJournalingSetup.msi RemoteDataStoreSetup.msi MissionSetup.msi"
SET "FILE_LIST=%FILE_LIST% MaintenanceSetup.msi InstantMessageSetup.msi SessionSetup.msi"

SET "MISSINGFILES="

for %%f in (%FILE_LIST%) do (
	call :DoCopy "%SRC_PATH%\%%f" "%DEST_PATH%" || SET "MISSINGFILES=!MISSINGFILES!%%f "
)

call :DoCopy "%~dp0setup_SIVENG.bat" "%DEST_PATH%" || SET "MISSINGFILES=%MISSINGFILES%setup_SIVENG.bat"

IF "%MISSINGFILES%" neq "" (
	echo ERROR: Some required files are missing or cannot be copied : %MISSINGFILES%
	SET EXIT_CODE=5
	goto :End
)

"%ZIP_PATH%" a -r "%ZIPFILENAME%" "%DEST_PATH%\*"
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