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


if EXIST "%2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_urban.zip" (
	DEL "%2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_urban.zip"
	if ERRORLEVEL 1 (
		echo Cannot delete previous package: "%2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_urban.zip"
		exit /B 3
	)
)

IF EXIST "%1\urban" (
	rmdir /S /Q %1\urban
	if ERRORLEVEL 1 (
		echo Cannot remove the directory: "%1\urban"
		exit /B 2
	)
)

mkdir %1\urban
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%1\urban"
	exit /B 2
)

SET "MISSINGFILES="
xcopy /y %1\DatapackageSetup.msi %1\urban\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%DatapackageSetup.msi "
xcopy /y %1\InfotainmentJournalingSetup.msi %1\urban\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%InfotainmentJournalingSetup.msi "
xcopy /y %1\LiveVideoControlSetup.msi %1\urban\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%LiveVideoControlSetup.msi "
xcopy /y %1\RemoteDataStoreSetup.msi %1\urban\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%RemoteDataStoreSetup.msi "
echo f |xcopy /y %1\MissionUrbanSetup.msi %1\urban\MissionSetup.msi
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%MissionUrbanSetup.msi "
xcopy /y %1\MaintenanceSetup.msi %1\urban\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%MaintenanceSetup.msi "
xcopy /y %1\InstantMessageSetup.msi %1\urban\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%InstantMessageSetup.msi "
echo f | xcopy /y %1\SessionUrbanSetup.msi %1\urban\SessionSetup.msi
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%SessionUrbanSetup.msi "
xcopy /y %1\RealTimeSetup.msi %1\urban\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%RealTimeSetup.msi "
xcopy /Y "%~dp0setup_URBAN.bat" %1\urban\
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%setup_URBAN.bat "

IF "%MISSINGFILES%" neq "" GOTO CopyError

"%ZIP_PATH%" a -r "%2\MT92-2PIS010010-PIS2G-Ground_Server-V%3_urban.zip" "%1\urban\*"
IF ERRORLEVEL 1 GOTO ZipError

rmdir /s /q %1\urban

goto end

:error
echo syntax should be : PackageURBAN.bat "release directory path" "output directory path" "version" 
exit /B 1

:CopyError
echo ERROR: Some required files are missing or cannot be copied : %MISSINGFILES%
exit /B 2

:ZipError
echo Error while creating the archive file(Zip file)
exit /B 3

:end
exit /B 0