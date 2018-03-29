::=====================================================================================
:: File name      : execute_test.bat
:: Description    : Execute automated tests of PIS-Ground projects
::				  : 	compile all unit test files 
:: Update         :	2016-09-12			
::=====================================================================================
@echo off
SETLOCAL
SETLOCAL EnableDelayedExpansion		

SET EXIT_CODE=0

call "%~dp0\config.bat" WORKING_DIR 

TYPE NUL > "%~dp0executeTestLog.txt"
	
::=====================================================================================
:: CHECK REQUIREMENTS
::=====================================================================================
	
>> "%~dp0executeTestLog.txt" 2>&1 (
	
	pushd "%~dp0"
	if ERRORLEVEL 1 (
		echo Cannot move to directory "%~dp0"
		SET EXIT_CODE=1
		goto :End
	)
			
	if "%WORKING_DIR%" == "" (
		echo WORKING_DIR variable not set.
		SET EXIT_CODE=2
		goto :End
	)
	
	REM On Build machine, the check is deactivated
	if not "%JENKINS_URL%" == "" goto :SkipRightsCheck

	:: Check if we have admin rights. Automated tests requires admin rights.
	fsutil dirty query %systemdrive% >nul 2>&1

	REM --> If error flag set, we do not have admin.
	if ERRORLEVEL 1 (
		echo This script require Administrator rights to be executed.
		SET EXIT_CODE=3
		goto :End
	)
)	
:SkipRightsCheck

::=====================================================================================
:: CREATE AND EMPTY OUTPUT DIRECTORIES
::=====================================================================================

>> "%~dp0executeTestLog.txt" 2>&1 (

	CALL :DoDeleteDir "%WORKING_DIR%\GROUND_TEST_RESULTS" || SET EXIT_CODE=4 && goto :End
	call :DoCreateDir "%WORKING_DIR%GROUND_TEST_RESULTS\" || SET EXIT_CODE=5 && goto :End
)

::=====================================================================================
:: Execute unit tests
::=====================================================================================

>> "%~dp0executeTestLog.txt" 2>&1 (

	echo Execute unit tests
	echo Execution start at !date! !time:~0,2!:!time:~3,2!:!time:~6,2!

	call "%~dp0GROUND.PLATEFORM\util\UnitTest_GROUND2.bat" || SET EXIT_CODE=6
	echo Execution Ends at !date! !time:~0,2!:!time:~3,2!:!time:~6,2! 
)
	
:End

if "%EXIT_CODE%" == "0" echo %~nx0 succeeded >> "%~dp0executeTestLog.txt"
if not "%EXIT_CODE%" == "0" echo %~nx0 failed with error: %EXIT_CODE% >> "%~dp0executeTestLog.txt"
if EXIST "%~dp0executeTestLog.txt" type "%~dp0executeTestLog.txt"

pause
exit /B %EXIT_CODE%

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
