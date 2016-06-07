::=====================================================================================
:: File name      : 		UnitTest_GROUND.bat
:: MakeFile name  : 
:: Description    : Execute unit test for PIS-GROUND
::				  : 	
:: Date           :			   2015-09-30
:: Update         :			   2016-05-20			
::=====================================================================================
@echo off
SETLOCAL

if "%WORKING_DIR%" == "" SET WORKING_DIR=%~dp0..\..\
	
SetLocal EnableDelayedExpansion
set  nPassed=0
set  nFailed=0

REM Erase files in test results directory if it contains at least one file
if exist "%WORKING_DIR%GROUND_TEST_RESULTS" (
	for /F %%i in ('dir /b /a "%WORKING_DIR%GROUND_TEST_RESULTS\*"') do (
		del /F /S /Q "%WORKING_DIR%GROUND_TEST_RESULTS\*"
		if ERRORLEVEL 1 (
			echo "Failed to delete previous test results"
			exit /B 3
		)
		goto :ExecuteTests
	)
)

if not exist "%WORKING_DIR%GROUND_TEST_RESULTS" (
	mkdir "%WORKING_DIR%GROUND_TEST_RESULTS"
	if ERRORLEVEL 1 (
		echo "Cannot create test result directory"
		exit /B 2
	)
)

:ExecuteTests
 
@echo -------- TESTS SUMMARY [%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] ---------- > "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 

echo Execute LiveVideoControl Tests > CON
"%WORKING_DIR%\GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\LiveVideoControlTests\bin\Release\LiveVideoControlTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_LiveVideoControl_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] LiveVideoControlTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] LiveVideoControlTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute GroundCore Tests > CON
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\GroundCoreTests\bin\Release\GroundCoreTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_GroundCore_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] GroundCoreTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] GroundCoreTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute DataPackage Tests > CON
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\DataPackageTests\bin\Release\DataPackageTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_DataPackage_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] DataPackageTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] DataPackageTests.dll >> %WORKING_DIR%\GROUND_TEST_RESULTS\summary_tests.log
	set /A nPassed=nPassed+1	
	)

echo Execute RealTime Tests > CON
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\RealTimeTests\bin\Release\RealTimeTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_RealTime_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] RealTimeTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] RealTimeTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute Mission Tests > CON
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\MissionTests\bin\Release\MissionTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_Mission_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] MissionTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] MissionTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute MissionControl Tests > CON
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\MissionControlTests\bin\Release\MissionControlTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_MissionControl_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] MissionControlTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] MissionControlTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute InstantMessage Tests > CON
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\InstantMessageTests\bin\Release\InstantMessageTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_InstantMessage_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] InstantMessageTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][%date% !time:~0,2!:!time:~3,2!:!time:~6,2!] InstantMessageTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo -------------------------------------------- >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
echo %nFailed% test(s) FAILED >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
echo %nPassed% test(s) PASSED >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"

SET EXIT_CODE=0
if not "%nFailed%" == "0" SET EXIT_CODE=2
goto End

:End

exit /B %EXIT_CODE%