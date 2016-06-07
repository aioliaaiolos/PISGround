''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''  Update the version number of an MSI setup project
''  and update relevant GUIDs
''  
''  Hans-Jürgen Schmidt / 2007.12.19
''  Last Updated: 2016.03.01
''  
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
set a = wscript.arguments
if a.count <> 4 then 
	wscript.StdErr.WriteLine "The script required 4 arguments and " & CStr(a.count) & " argument(s) where provided"
	wscript.StdErr.WriteLine "Argument 1: Name of the file to update"
	wscript.StdErr.WriteLine "Argument 2: Version number to set"
	wscript.StdErr.WriteLine "Argument 3: Guid for product code"
	wscript.StdErr.WriteLine "Argument 4: Guid for package code"
	wscript.quit 1
end if

fileName = a(0)
newVersion = a(1)
newProductCodeGuid = a(2)
newPackageCodeGuid = a(3)

'read and backup project file
originalContent = ReadFileContent(fileName)

newContent = ReplaceVersion(originalContent, newVersion, newPackageCodeGuid, newProductCodeGuid)

'write project file if updated
if newContent <> originalContent then
	wscript.StdOut.WriteLine "  ** Original file need to be updated"
	Call WriteFileContent(fileName, newContent)
else
	wscript.StdOut.WriteLine "  ** Version number was already set to proper value"
end if


Function ReplaceVersion(ByVal fileContent, ByVal newVersion, ByVal newPackageCodeGuid, ByVal newProductCodeGuid)
	ReplaceVersion = fileContent
	newFileContent = fileContent
	
	On Error Resume Next
	'find, increment and replace version number
	set re = new regexp
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Cannot create regular expression object"
		wscript.quit 10
	end if 

	re.global = true
	
	' Validate the version
	re.pattern = "^\d+\.\d+\.\d\d\d\d$"
	set m = re.execute(newVersion)
	If m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The new version to set does not follow that format: 1.2.0000")
		wscript.StdErr.WriteLine("-- New version value is '" & CStr(newVersion) & "'")
		wscript.quit 11
	End If

	' Validate the package code guid
	re.pattern = "^\{[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}\}$"
	set m = re.execute(newPackageCodeGuid)
	If m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The new package code guid to set does not follow that format: {00000000-1111-2222-3333-444444444444}")
		wscript.StdErr.WriteLine("-- New package code guid value is '" & CStr(newPackageCodeGuid) & "'")
		wscript.quit 11
	End If

	' Validate the product code guid
	re.pattern = "^\{[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}\}$"
	set m = re.execute(newProductCodeGuid)
	If m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The new product code guid to set does not follow that format: {00000000-1111-2222-3333-444444444444}")
		wscript.StdErr.WriteLine("-- New product code guid value is '" & CStr(newProductCodeGuid) & "'")
		wscript.quit 11
	End If
	
	re.pattern = "(""ProductVersion"" = ""8:)(\d+(\.\d+)+)"""
	set m = re.execute(newFileContent)
	
	If m.Count = 0 then
		wscript.StdErr.WriteLine("-- The file does not contain ProductVersion to replace")
		wscript.quit 11
	ElseIf m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The file contains more than one ProductVersion to replace")
		wscript.quit 11
	End If
	
	
	newFileContent = re.replace(newFileContent, "$1" & newVersion & """")

	'replace ProductCode
	re.pattern = "(""ProductCode"" = ""8:)(\{.+\})"""
		
	set m = re.Execute(newFileContent)
	If m.Count = 0 then
		wscript.StdErr.WriteLine("-- The file does not contain ProductCode to replace")
		wscript.quit 11
	ElseIf m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The file contains more than one ProductCode to replace")
		wscript.quit 11
	End If
		
	newFileContent = re.replace(newFileContent, "$1" & newProductCodeGuid & """")

	'replace PackageCode
	re.pattern = "(""PackageCode"" = ""8:)(\{.+\})"""
	set m = re.Execute(newFileContent)
	If m.Count = 0 then
		wscript.StdErr.WriteLine("-- The file does not contain PackageCode to replace")
		wscript.quit 11
	ElseIf m.Count <> 1 then
		wscript.StdErr.WriteLine("-- The file contains more than one PackageCode to replace")
		wscript.quit 11
	End If

	newFileContent = re.replace(newFileContent, "$1" & newPackageCodeGuid & """")
	
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Unexpected error while replacing version number in the original file: " & err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 12
	end if
	
	ReplaceVersion = newFileContent
End Function

Function ReadFileContent(ByVal fileName)
	ReadFileContent = ""
	
	On Error Resume Next
	Set fso = CreateObject("Scripting.FileSystemObject")
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Cannot create file system object:" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 2
	end if
	
	Set f = fso.OpenTextFile(fileName)
	
	if Err.Number <> 0 then
		set fso = Nothing
		wscript.StdErr.WriteLine "Cannot open file '" & CStr(fileName) & "':" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 3
	end if
	
	fileContent = f.ReadAll

	f.close
	set f = Nothing
	set fso = Nothing
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Error while reading file '" & CStr(fileName) & "':" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 3
	end if
	
	if fileContent = "" then
		wscript.Echo "File '" & CStr(fileName) & "' is empty. Version number cannot be updated."
		wscript.quit 3
	end if
	
	ReadFileContent = fileContent
end Function

Sub WriteFileContent(ByVal fileName, ByVal newContent)
	On Error Resume Next
	Set fso = CreateObject("Scripting.FileSystemObject")

	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Cannot create file system object:" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 5
	end if
	
	set f = fso.CreateTextfile(fileName, true)
	if Err.Number <> 0 then
		set fso = Nothing
		wscript.StdErr.WriteLine "Cannot create file '" & CStr(fileName) & "':" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 6
	end if
	
	f.write(newContent)
	f.close
	set f = Nothing
	set fso = Nothing
	
	if Err.Number <> 0 then
		wscript.StdErr.WriteLine "Error while writing file '" & CStr(fileName) & "':" & Err.Description & "(" & CStr(Err.Number) & ")"
		wscript.quit 6
	end if
End Sub
