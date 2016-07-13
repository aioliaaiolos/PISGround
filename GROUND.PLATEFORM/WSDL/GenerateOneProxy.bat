::=====================================================================================
:: File name      : 					GenerateOneProxy.bat
:: MakeFile name  : 
:: Description    : 	File specialized to generate WCF proxies files and ensure that file has been modified
::                       before replacing the previous files.			
:: Updated        :						  2016-07-13
::=====================================================================================

@echo off

REM This batch file generate one proxy client and the config file and ensure that they have changed before replacing the destination one
REM Parameters: directory namespace csfile configfile wsdlfiles
REM  directory is the location where output file shall be 

SET EXIT_CODE=0

SET /A ARGS_COUNT=0    
FOR %%A in (%*) DO SET /A ARGS_COUNT+=1    

SET SVCUTIL=%~dp0svcutil.exe
if NOT EXIST "%SVCUTIL%" (
	echo SVCUTIL executable does not exist at this location "%SVCUTIL%"
	SET EXIT_CODE=1
	goto :End
)

if "%ARGS_COUNT%" GEQ 10 (
	echo This scripts does not support %ARGS_COUNT% arguments and more. The maximum supported arguments is 9
	SET EXIT_CODE=2
	goto :End
)

if %ARGS_COUNT% GEQ 5 goto :Ok

echo USAGE: %0 directory namespace csfile configfile wsdlfiles [keepConfig]
echo directory The path where output files shall be saved
echo namespace The wsdl namespace to pass as argument to svcutil.exe method
echo csfile The name of the destination source file (c# file)
echo configfile The name of the destination config file (.config)
echo wsdlfiles One or more filename that describe the wsdl.
echo keepConfig When keepConfig is specified at the end, the configuration file is keep after generation. By defaut, configuration file is not kept.
SET EXIT_CODE=3
goto :End

:Ok

SET DIRECTORY=%1
SET NAMESPACE=%2
SET CSFILE=%3
SET CONFIGFILE=%4
SHIFT
SHIFT
SHIFT
SHIFT

SET KEEPCONFIG=N

IF /I "%1" == "keepConfig" (
	SET KEEPCONFIG=Y
	SHIFT
)

@echo on
"%SVCUTIL%" /directory:%DIRECTORY% /language:cs /namespace:*,%NAMESPACE% /targetClientVersion:Version35 /out:%CSFILE%.temp /config:%CONFIGFILE%.temp %1 %2 %3 %4 %5 %6 %7 %8 %9
IF NOT ERRORLEVEL 1  goto CopyStep
@echo off
echo Error while generating proxy
SET EXIT_CODE=4
goto :End

:CopyStep
@echo off
IF EXIST %DIRECTORY%\%CSFILE% (

	REM ignoring line that contains CODEGEN allow to make comparison agnostic to the os language. In French machine, CODEGEN comment are in French,
	more +9 %DIRECTORY%\%CSFILE%.temp.cs | find /v "// CODEGEN" > %DIRECTORY%\%CSFILE%.temp.cs.2
	more +9 %DIRECTORY%\%CSFILE% | find /v "// CODEGEN" > %DIRECTORY%\%CSFILE%.2
	FC /B %DIRECTORY%\%CSFILE%.2 %DIRECTORY%\%CSFILE%.temp.cs.2 >NUL
	IF NOT ERRORLEVEL 1 GOTO :Config
)

echo File "%DIRECTORY%\%CSFILE%" replaced
ATTRIB -R %DIRECTORY%\%CSFILE%
COPY /V /Y %DIRECTORY%\%CSFILE%.temp.cs %DIRECTORY%\%CSFILE%
IF NOT ERRORLEVEL 1 goto :Config

echo Cannot copy %DIRECTORY%\%CSFILE%.temp.cs to %DIRECTORY%\%CSFILE%
SET EXIT_CODE=5

goto :End

:Config

if "%KEEPCONFIG%" NEQ "Y" goto :End

IF EXIST %DIRECTORY%\%CONFIGFILE% (
	FC /B %DIRECTORY%\%CONFIGFILE% %DIRECTORY%\%CONFIGFILE%.temp.config >NUL
	IF NOT ERRORLEVEL 1 goto :End
	)

	echo File "%DIRECTORY%\%CONFIGFILE%" replaced
ATTRIB -R %DIRECTORY%\%CONFIGFILE%
COPY /V /Y %DIRECTORY%\%CONFIGFILE%.temp.config %DIRECTORY%\%CONFIGFILE%
IF NOT ERRORLEVEL 1 goto :End

echo Cannot copy %DIRECTORY%\%CONFIGFILE%.temp.config to %DIRECTORY%\%CONFIGFILE%
SET EXIT_CODE=6
goto :End

:End

IF EXIST %DIRECTORY%\%CONFIGFILE%.temp.config DEL /F %DIRECTORY%\%CONFIGFILE%.temp.config
IF EXIST %DIRECTORY%\%CSFILE%.temp.cs DEL /F %DIRECTORY%\%CSFILE%.temp.cs
IF EXIST %DIRECTORY%\%CSFILE%.temp.cs.2 DEL /F %DIRECTORY%\%CSFILE%.temp.cs.2
IF EXIST %DIRECTORY%\%CSFILE%.2 DEL /F %DIRECTORY%\%CSFILE%.2

if "%EXIT_CODE%" == "0" echo Success
if not "%EXIT_CODE%" == "0" echo GenerateOneProxy fail with error: %EXIT_CODE%

EXIT /B %EXIT_CODE%
