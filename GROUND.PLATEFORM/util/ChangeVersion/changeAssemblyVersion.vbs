''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''  Update the version number of an Assembly.info
''  
''  Last update 2016.08.29
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
set a = wscript.arguments
set a = wscript.arguments
if a.count <> 2 then 
	wscript.StdErr.WriteLine "The script required 2 arguments and " & CStr(a.count) & " argument(s) where provided"
	wscript.StdErr.WriteLine "Argument 1: Name of the file to update"
	wscript.StdErr.WriteLine "Argument 2: Version number to set"
	wscript.quit 1
end if

fileName = a(0)
newVersion = a(1)

'read and backup project file
originalContent = ReadFileContent(fileName)

newContent = ReplaceVersion(originalContent, newVersion)

'write project file if updated
if newContent <> originalContent then
	wscript.StdOut.WriteLine "  ** Original file need to be updated"
	Call WriteFileContent(fileName, newContent)
else
	wscript.StdOut.WriteLine "  ** Version number was already set to proper value"
end if

Function ReplaceVersion(ByVal fileContent, ByVal newVersion)
	ReplaceVersion = fileContent
	newFileContent = fileContent
	
	On Error Resume Next

	set re = new regexp
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Cannot create regular expression object"
		wscript.quit 2
	end if 
	
	re.global = true
	
	' Validate the version
	re.pattern = "^\d+\.\d+\.\d+\.\d+$"
	set m = re.execute(newVersion)
	If m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The new version to set does not follow that format: 1.2.3.4")
		wscript.StdErr.WriteLine("-- New version value is '" & CStr(newVersion) & "'")
		wscript.quit 3
	End If
	
	
	'replace AssemblyFileVersion
	
	re.pattern = "(AssemblyVersion\("")(\d+(\.\d+)+)\""(\))"
	set m = re.execute(newFileContent)
	
	If m.Count = 0 then
		wscript.StdErr.WriteLine("-- The file does not contain AssemblyVersion to replace")
		wscript.quit 4
	ElseIf m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The file contains more than one AssemblyVersion to replace")
		wscript.quit 5
	End If
	
	
	newFileContent = re.replace(newFileContent, "$1" & newVersion & """)")

	'replace AssemblyFileVersion
	re.pattern = "(AssemblyFileVersion\("")(\d+(\.\d+)+)\""(\))"
	
	set m = re.Execute(newFileContent)
	If m.Count = 0 then
		wscript.StdErr.WriteLine("-- The file does not contain AssemblyFileVersion to replace")
		wscript.quit 6
	ElseIf m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The file contains more than one AssemblyFileVersion to replace")
		wscript.quit 7
	End If
	
	newFileContent = re.replace(newFileContent, "$1" & newVersion & """)")
	
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Unexpected error while replacing version number in the original file: " & err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 8
	end if
	
	ReplaceVersion = newFileContent
End Function


Function ReadFileContent(ByVal fileName)
	ReadFileContent = ""
	
	On Error Resume Next
	Set fso = CreateObject("Scripting.FileSystemObject")
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Cannot create file system object:" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 9
	end if
	
	Set f = fso.OpenTextFile(fileName)
	
	if Err.Number <> 0 then
		set fso = Nothing
		wscript.StdErr.WriteLine "Cannot open file '" & CStr(fileName) & "':" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 10
	end if
	
	fileContent = f.ReadAll

	f.close
	set f = Nothing
	set fso = Nothing
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Error while reading file '" & CStr(fileName) & "':" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 11
	end if
	
	if fileContent = "" then
		wscript.Echo "File '" & CStr(fileName) & "' is empty. Version number cannot be updated."
		wscript.quit 12
	end if
	
	ReadFileContent = fileContent
end Function

Sub WriteFileContent(ByVal fileName, ByVal newContent)
	On Error Resume Next
	Set fso = CreateObject("Scripting.FileSystemObject")

	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Cannot create file system object:" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 13
	end if
	
	set f = fso.CreateTextfile(fileName, true)
	if Err.Number <> 0 then
		set fso = Nothing
		wscript.StdErr.WriteLine "Cannot create file '" & CStr(fileName) & "':" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 14
	end if
	
	f.write(newContent)
	f.close
	set f = Nothing
	set fso = Nothing
	
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Error while writing file '" & CStr(fileName) & "':" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 15
	end if
End Sub
