::=====================================================================================
:: File name      : 		PackageSIVENG_SDK.bat
:: MakeFile name  : 
:: Description    : Create the PIS-Ground SDK package for SIVENG platform
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

SET SRC=%1
IF "%SRC:~-1%"=="\" SET SRC=%SRC:~0,-1%

if EXIST "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_SIVENG.zip" (
	DEL "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_SIVENG.zip"
	if ERRORLEVEL 1 (
		echo Cannot delete previous sdk package: "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_SIVENG.zip"
		exit /B 3
	)
)

IF EXIST "%SRC%\siveng" (
	rmdir /S /Q %SRC%\siveng
	if ERRORLEVEL 1 (
		echo Cannot remove the directory: "%SRC%\siveng"
		exit /B 2
	)
)

mkdir %SRC%\siveng
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%SRC%\siveng"
	exit /B 2
)

mkdir %SRC%\siveng\wsdl
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%SRC%\siveng\wsdl"
	exit /B 2
)
mkdir %SRC%\siveng\Schema
if ERRORLEVEL 1 (
	echo Cannot create the directory: "%SRC%\siveng\Schema"
	exit /B 2
)

SET "MISSINGFILES="

xcopy /y /e /r /i "%SRC%\Datapackage" "%SRC%\siveng\Datapackage\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%Datapackage "
xcopy /y /e /r /i "%SRC%\InfotainmentJournaling" "%SRC%\siveng\InfotainmentJournaling\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%InfotainmentJournaling "
xcopy /y /e /r /i "%SRC%\Mission" "%SRC%\siveng\Mission\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%Mission "
xcopy /y /e /r /i "%SRC%\Maintenance" "%SRC%\siveng\Maintenance\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%Maintenance "
xcopy /y /e /r /i "%SRC%\InstantMessage" "%SRC%\siveng\InstantMessage\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%InstantMessage "
xcopy /y /e /r /i "%SRC%\Session" "%SRC%\siveng\Session\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%Session "

xcopy /y /e /r /i "%SRC%\..\..\PISEmbeddedSDK\SIF\wsdl\AppGround" "%SRC%\siveng\wsdl\AppGround\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\AppGround "
xcopy /y /i /r "%SRC%\..\..\PISEmbeddedSDK\SIF\wsdl\Schema\Common.xsd" "%SRC%\siveng\wsdl\Schema\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\Schema\Common.xsd "
xcopy /y /i /r "%SRC%\..\..\PISEmbeddedSDK\SIF\wsdl\Schema\Notification.xsd" "%SRC%\siveng\wsdl\Schema\"
IF ERRORLEVEL 1 SET "MISSINGFILES=%MISSINGFILES%SIF\wsdl\Schema\Notification.xsd "

IF "%MISSINGFILES%" NEQ "" GOTO CopyError

"%ZIP_PATH%" a -r "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_SIVENG.zip" "%SRC%\siveng\*.wsdl" "%SRC%\siveng\*.xsd"
IF ERRORLEVEL 1 GOTO ZipError

rmdir /s /q %SRC%\siveng

goto end

:error
echo syntax should be : PackageSIVENG_SDK.bat "release directory path" "output directory path" "version" 
exit /B 1

:CopyError
echo ERROR: Some required directories are missing or cannot be copied : %MISSINGFILES%
exit /B 2

:ZipError
echo Error while creating the archive file(Zip file)
if EXIST "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_SIVENG.zip.tmp" DEL "%2\MT92-2PIS010011-PIS2G-Ground_Server_SDK-V%3_SIVENG.zip.tmp"
exit /B 3

:end
exit /B 0