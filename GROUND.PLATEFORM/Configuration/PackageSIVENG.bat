::=====================================================================================
:: File name      : 		PackageURBAN.bat
:: MakeFile name  : 
:: Description    : Create the PIS-Ground setup package for URBAN platform
::				  : 	
:: Update         :			   2016-02-26			
::=====================================================================================
@echo off
SETLOCAL

set ZIP_PATH=%ProgramFiles(x86)%\7-Zip\7z.exe
IF NOT EXIST "%ZIP_PATH%" set ZIP_PATH=C:\Program Files (x86)\7-Zip\7z.exe
IF NOT EXIST "%ZIP_PATH%" set ZIP_PATH=%ProgramFiles%\7-Zip\7z.exe
if NOT EXIST "%ZIP_PATH%" (	
	echo 7-zip is not installed.
	exit /B 20
)

if "%1"=="" goto error
if "%2"=="" goto error
if "%3"=="" goto error

if EXIST "%2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_siveng.zip" (
	DEL "%2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_siveng.zip"
	if ERRORLEVEL 1 (
		echo Cannot delete previous package: "%2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_siveng.zip"
		exit /B 3
	)
)

IF EXIST "%1\siveng" (
	rmdir /S /Q %1\siveng
	if ERRORLEVEL 1 (
		echo Cannot remove the directory: "%1\siveng"
		exit /B 2
	)
)

mkdir %1\siveng
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%1\siveng"
	exit /B 2
)

SET "MISSINGFILES="
xcopy /y %1\DatapackageSetup.msi %1\siveng\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%DatapackageSetup.msi "
xcopy /y %1\InfotainmentJournalingSetup.msi %1\siveng\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%InfotainmentJournalingSetup.msi "
xcopy /y %1\RemoteDataStoreSetup.msi %1\siveng\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%RemoteDataStoreSetup.msi "
xcopy /y %1\MissionSetup.msi %1\siveng\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%MissionSetup.msi "
xcopy /y %1\MaintenanceSetup.msi %1\siveng\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%MaintenanceSetup.msi "
xcopy /y %1\InstantMessageSetup.msi %1\siveng\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%InstantMessageSetup.msi "
xcopy /y %1\SessionSetup.msi %1\siveng\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%SessionSetup.msi "
xcopy /Y "%~dp0setup_SIVENG.bat" %1\siveng\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%setup_SIVENG.bat "

IF "%MISSINGFILES%" neq "" GOTO CopyError

"%ZIP_PATH%" a -r "%2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_siveng.zip" "%1\siveng\*"
IF ERRORLEVEL 1 GOTO ZipError

rmdir /s /q %1\siveng

goto end

:error
echo syntax should be : PackageSIVENG.bat "release directory path" "output directory path" "version" 
exit /B 1

:CopyError
echo ERROR: Some required files are missing or cannot be copied : %MISSINGFILES%
exit /B 2

:ZipError
echo Error while creating the archive file(Zip file)
exit /B 3

:end
exit /B 0