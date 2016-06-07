::=====================================================================================
:: File name      : 		PackageURBAN_SDK.bat
:: MakeFile name  : 
:: Description    : Create the PIS-Ground SDK package for URBAN platform
::				  : 	
:: Update         :			   2016-02-29			
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

SET SRC=%1
IF "%SRC:~-1%"=="\" SET SRC=%SRC:~0,-1%

if EXIST "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_URBAN.zip" (
	DEL "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_URBAN.zip"
	if ERRORLEVEL 1 (
		echo Cannot delete previous sdk package: "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_URBAN.zip"
		exit /B 3
	)
)

IF EXIST "%SRC%\urban" (
	rmdir /S /Q %SRC%\urban
	if ERRORLEVEL 1 (
		echo Cannot remove the directory: "%SRC%\urban"
		exit /B 2
	)
)

mkdir %SRC%\urban
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%SRC%\urban"
	exit /B 2
)

mkdir %SRC%\urban\wsdl
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%SRC%\urban\wsdl"
	exit /B 2
)
mkdir %SRC%\urban\Schema
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%SRC%\urban\Schema"
	exit /B 2
)

SET "MISSINGFILES="
xcopy /y /e /r /i "%SRC%\Datapackage" "%SRC%\urban\Datapackage\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%Datapackage "
xcopy /y /e /r /i "%SRC%\InfotainmentJournaling" "%SRC%\urban\InfotainmentJournaling\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%InfotainmentJournaling "
xcopy /y /e /r /i "%SRC%\LiveVideoControl" "%SRC%\urban\LiveVideoControl\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%LiveVideoControl "
xcopy /y /e /r /i "%SRC%\MissionUrban" "%SRC%\urban\MissionUrban\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%MissionUrban "
rename "%SRC%\urban\MissionUrban" Mission
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%MissionUrban "
xcopy /y /e /r /i "%SRC%\Maintenance" "%SRC%\urban\Maintenance\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%Maintenance "
xcopy /y /e /r /i "%SRC%\InstantMessage" "%SRC%\urban\InstantMessage\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%InstantMessage "
xcopy /y /e /r /i "%SRC%\Session" "%SRC%\urban\Session\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%Session "
xcopy /y /e /r /i "%SRC%\RealTime" "%SRC%\urban\RealTime\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%RealTime "

xcopy /y /e /r /i "%SRC%\..\..\PISEmbeddedSDK\SIF\wsdl\AppGround" "%SRC%\urban\wsdl\AppGround\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\AppGround "
xcopy /y /i /r "%SRC%\..\..\PISEmbeddedSDK\SIF\wsdl\Schema\Common.xsd" "%SRC%\urban\wsdl\Schema\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\Schema\Common.xsd "
xcopy /y /i /r "%SRC%\..\..\PISEmbeddedSDK\SIF\wsdl\Schema\Notification.xsd" "%SRC%\urban\wsdl\Schema\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\Schema\Notification.xsd "

IF "%MISSINGFILES%" NEQ "" GOTO CopyError

"%ZIP_PATH%" a -r "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_URBAN.zip" "%SRC%\urban\*.wsdl" "%SRC%\urban\*.xsd"
IF ERRORLEVEL 1 GOTO ZipError
rmdir /s /q %SRC%\urban

goto end

:error
echo syntax should be : PackageURBAN_SDK.bat "release directory path" "output directory path" "version" 
exit /B 1

:CopyError
echo ERROR: Some required directories are missing or cannot be copied : %MISSINGFILES%
exit /B 2

:ZipError
echo Error while creating the archive file(Zip file)
if EXIST "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_URBAN.zip.tmp" "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_URBAN.zip.tmp"
exit /B 3

:end
exit /B 0