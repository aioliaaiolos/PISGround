::=====================================================================================
:: File name      : 		UnitTest_GROUND.bat
:: Description    : Execute unit test for PIS-GROUND
::				  : 	
:: Update         :			   2016-12-15			
::=====================================================================================
@echo off
SETLOCAL

SET EXIT_CODE=0
if "%WORKING_DIR%" == "" SET "WORKING_DIR=%~dp0..\..\"
	
SetLocal EnableDelayedExpansion
set  nPassed=0
set  nFailed=0

REM Erase files in test results directory if it contains at least one file
if exist "%WORKING_DIR%GROUND_TEST_RESULTS" (
	for /F %%i in ('dir /b /a "%WORKING_DIR%GROUND_TEST_RESULTS\*"') do (
		del /F /S /Q "%WORKING_DIR%GROUND_TEST_RESULTS\*"
		if ERRORLEVEL 1 (
			echo "Failed to delete previous test results"
			SET EXIT_CODE=1
			goto :End
		)
		goto :ExecuteTests
	)
)

if not exist "%WORKING_DIR%GROUND_TEST_RESULTS" (
	mkdir "%WORKING_DIR%GROUND_TEST_RESULTS"
	if ERRORLEVEL 1 (
		echo "Cannot create test result directory"
		SET EXIT_CODE=2
		goto :End
	)
)

:ExecuteTests
 
@echo -------- TESTS SUMMARY [!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] ---------- > "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 

echo Execute Session Tests > CON
echo Execute Session Tests 
"%WORKING_DIR%\GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\SessionTests\bin\Release\SessionTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_Session_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] SessionTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] SessionTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute LiveVideoControl Tests > CON
echo Execute LiveVideoControl Tests 
"%WORKING_DIR%\GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\LiveVideoControlTests\bin\Release\LiveVideoControlTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_LiveVideoControl_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] LiveVideoControlTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] LiveVideoControlTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute GroundCore Tests > CON
echo Execute GroundCore Tests
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\GroundCoreTests\bin\Release\GroundCoreTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_GroundCore_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] GroundCoreTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] GroundCoreTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute DataPackage Tests > CON
echo Execute DataPackage Tests
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\DataPackageTests\bin\Release\DataPackageTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_DataPackage_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] DataPackageTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] DataPackageTests.dll >> %WORKING_DIR%\GROUND_TEST_RESULTS\summary_tests.log
	set /A nPassed=nPassed+1	
	)

echo Execute RealTime Tests > CON
echo Execute RealTime Tests
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\RealTimeTests\bin\Release\RealTimeTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_RealTime_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] RealTimeTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] RealTimeTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute Mission Tests > CON
echo Execute Mission Tests 
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\MissionTests\bin\Release\MissionTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_Mission_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] MissionTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] MissionTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute MissionControl Tests > CON
echo Execute MissionControl Tests
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\MissionControlTests\bin\Release\MissionControlTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_MissionControl_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] MissionControlTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] MissionControlTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo Execute InstantMessage Tests > CON
echo Execute InstantMessage Tests
"%WORKING_DIR%GROUND.PLATEFORM\Dependencies\NUnit-2.6.2\nunit-console-x86.exe" "%WORKING_DIR%GROUND.PLATEFORM\NUnittest\InstantMessageTests\bin\Release\InstantMessageTests.dll" /xml="%WORKING_DIR%GROUND_TEST_RESULTS\nunit_PISGround_InstantMessage_Result.xml"
IF ERRORLEVEL 1 ( echo [FAILED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] InstantMessageTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" 
	set /A nFailed=nFailed+1
	) else ( echo [PASSED][!date! !time:~0,2!:!time:~3,2!:!time:~6,2!] InstantMessageTests.dll >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	set /A nPassed=nPassed+1	
	)

echo -------------------------------------------- >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
echo %nFailed% test(s) FAILED >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
echo %nPassed% test(s) PASSED >> "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"


if not "%nFailed%" == "0" SET EXIT_CODE=1
goto End

:End
IF EXIST "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log" (
	type "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
	echo Tests summary saved in "%WORKING_DIR%GROUND_TEST_RESULTS\summary_tests.log"
)

if "%EXIT_CODE%" == "0" echo %~nx0 succeeded
if not "%EXIT_CODE%" == "0" echo %~nx0 failed with error: %EXIT_CODE%
exit /B %EXIT_CODE%
