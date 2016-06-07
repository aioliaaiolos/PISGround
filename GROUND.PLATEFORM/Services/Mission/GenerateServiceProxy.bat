@echo off

SETLOCAL
REM %1 - relative target directory path
REM %2 - namespace
REM %3 - target out filename
REM %4 - target config filename
REM Remaining parameters - wsdl/xsd sources

REM vcvars32.bat sets the appropriate environment variables to enable command line builds
SET VCVARSFILE=%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat

IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=%ProgramFiles%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" (
	echo "Visual Studio 2008 is required to build this application. vcvars32.bat file cannot be found"
	exit /B 10 
)

cd %~dp0

set CRCMAN="..\..\Dependencies\crcman\crcman.exe"

set pDir=%1
shift
set pNamespace=%1
shift
set pTargetName=%1
shift
set pTargetConfigName=%1
shift
set pOtherParams=%1
:loop
shift
if [%1]==[] goto afterloop
set pOtherParams=%pOtherParams% %1
goto loop
:afterloop

svcutil.exe /d:%pDir% /l:cs /n:*,%pNamespace% /tcv:Version35 /out:~%pTargetName%.cs /config:~%pTargetConfigName%.config %pOtherParams%

set doCopy=1
if not exist %pDir%\%pTargetName%.cs goto :skip1
%CRCMAN% %pDir%\%pTargetName%.cs %pDir%\~%pTargetName%.cs > ~.tmp
set /P cmdResult=<~.tmp
if "%cmdResult%"=="eq" set doCopy=0
del /Q ~.tmp
:skip1
if %doCopy%==1 (
echo -------------^> Updating [%pTargetName%.cs] && copy /Y %pDir%\~%pTargetName%.cs %pDir%\%pTargetName%.cs
) else (
echo -------------^> File     [%pTargetName%.cs] is up to date.
)
del /Q %pDir%\~%pTargetName%.cs

set doCopy=1
if not exist %pDir%\%pTargetConfigName%.config goto :skip2
%CRCMAN% %pDir%\%pTargetConfigName%.config %pDir%\~%pTargetConfigName%.config > ~.tmp
set /P cmdResult=<~.tmp
if "%cmdResult%"=="eq" set doCopy=0
del /Q ~.tmp
:skip2
if %doCopy%==1 (
echo -------------^> Updating [%pTargetConfigName%.config] && copy /Y %pDir%\~%pTargetConfigName%.config %pDir%\%pTargetConfigName%.config
) else (
echo -------------^> File     [%pTargetConfigName%.config] is up to date.
)
del /Q %pDir%\~%pTargetConfigName%.config

