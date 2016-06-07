@echo off
SETLOCAL
REM %1 - namespace
REM %2 - wsdl source
REM %3 - filename target

REM vcvars32.bat sets the appropriate environment variables to enable command line builds

SET VCVARSFILE=%ProgramFiles(x86)%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat

IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=C:\Program Files (x86)\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" SET VCVARSFILE=%ProgramFiles%\Microsoft Visual Studio 9.0\VC\bin\vcvars32.bat
IF NOT EXIST "%VCVARSFILE%" (
	echo "Visual Studio 2008 is required to build this application. vcvars32.bat file cannot be found"
	exit /B 10 
)

CALL "%VCVARSFILE%"

set CRCMAN="..\..\..\Dependencies\crcman\crcman.exe"

svcutil.exe /d:.\generated /a /l:cs /n:*,%1 /tcv:Version35 %2 /out:~%3.cs /config:~%3.config

set doCopy=1

if not exist .\generated\%3.cs goto :skip1
%CRCMAN% %~dp0\generated\%3.cs %~dp0\generated\~%3.cs > ~.tmp
set /P cmdResult=<~.tmp
if "%cmdResult%"=="eq" set doCopy=0
del /Q ~.tmp
:skip1
if %doCopy%==1 (
echo -------------^> Updating...%3.cs && copy /Y .\generated\~%3.cs .\generated\%3.cs
) else (
echo -------------^> File %3.cs is up to date.
)
del /Q .\generated\~%3.cs

set doCopy=1
if not exist .\generated\%3.config goto :skip2
%CRCMAN% %~dp0\generated\%3.config %~dp0\generated\~%3.config > ~.tmp
set /P cmdResult=<~.tmp
if "%cmdResult%"=="eq" set doCopy=0
del /Q ~.tmp
:skip2
if %doCopy%==1 (
echo -------------^> Updating...%3.config && copy /Y .\generated\~%3.config .\generated\%3.config
) else (
echo -------------^> File %3.config is up to date.
)
del /Q .\generated\~%3.config
