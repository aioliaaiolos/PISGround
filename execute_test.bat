::=====================================================================================
:: File name      : 		execute_test.bat
:: MakeFile name  : 
:: Description    : Unit test functions for GROUND project
::				  : 	compile all unit test files 
:: Date           :			   2015-09-30
:: Update         :			   2016-07-13			
::=====================================================================================
@echo off
SETLOCAL

SET EXIT_CODE=0

call %~dp0\config.bat WORKING_DIR 

TYPE NUL > "%~dp0execute_testLog.txt"
	
if "%WORKING_DIR%" == "" (
	echo WORKING_DIR variable not set.
	SET EXIT_CODE=1
	goto :End
)

:: Check if we have admin rights. Automated tests requires admin rights.
"%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system" >nul 2>&1

REM --> If error flag set, we do not have admin.
if "%errorlevel%" NEQ "0" (
	REM On Build machine, the check is deactivated
	if "%JENKINS_URL%" == "" (
		echo This script require Administrator rights to be executed.
		SET EXIT_CODE=2
		goto :End
	)
)

:: Cleanup

if EXIST "%WORKING_DIR%\GROUND_TEST_RESULTS" (
	RMDIR /S /Q "%WORKING_DIR%\GROUND_TEST_RESULTS"
	IF ERRORLEVEL 1 (
	SET EXIT_CODE=3
	goto :End
	)
)

echo Create test result directory at this location "%WORKING_DIR%GROUND_TEST_RESULTS\%"
mkdir %WORKING_DIR%GROUND_TEST_RESULTS\
IF ERRORLEVEL 1 (
	SET EXIT_CODE=4
	goto :End
)	

echo Execute unit tests
SetLocal EnableDelayedExpansion		
echo Execution start at %date% !time:~0,2!:!time:~3,2!:!time:~6,2! > "%~dp0execute_testLog.txt"
ENDLOCAL
call "%~dp0GROUND.PLATEFORM\util\UnitTest_GROUND.bat" >> "%~dp0execute_testLog.txt" 2>&1
IF ERRORLEVEL 1 (
	SET EXIT_CODE=5
	goto :End
)	
SetLocal EnableDelayedExpansion		
echo Execution Ends at %date% !time:~0,2!:!time:~3,2!:!time:~6,2! >> "%~dp0execute_testLog.txt"
ENDLOCAL
type "%~dp0execute_testLog.txt"
	
:End

if "%EXIT_CODE%" == "0" echo Success >> "%~dp0execute_testLog.txt"
if not "%EXIT_CODE%" == "0" echo Execute_test fail with error: %EXIT_CODE% >> "%~dp0execute_testLog.txt"
if EXIST "%~dp0execute_testLog.txt" type "%~dp0execute_testLog.txt"
pause
exit /B %EXIT_CODE%