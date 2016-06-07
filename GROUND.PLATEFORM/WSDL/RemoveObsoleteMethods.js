// <copyright file="RemoveObsoleteMethods.js" company="Adetel SIE.">
// Copyright by Adetel SIE 2015.  All rights reserved.
// 
// The information contained herein is confidential property of Adetel SIE.
// The use, copy, transfer or disclosure of such
// information is prohibited except by express written agreement with Adetel SIE.
// </copyright>

 
// <summary>
// Removes a method definition for a specified web service interface from the wsdl file.
// It removes the "wsdl:operation" node from the "/wsdl:definitions/wsdl:portType" node for which the name attribute corresponds to the methodToRemove parameter.
// It removes the "wsdl:operation" node from the "/wsdl:definitions/wsdl:binding" node for which the name attribute corresponds to the methodToRemove parameter.
// </summary>
// <param name="wsdlFileName">The wsdl file to work on.</param>
// <param name="interfaceName">The interface name from which we want to remove the method definition.</param>
// <param name="methodToRemove">The method name to remove.</param>
// <returns>Nothing is returned from this function.</returns>
function RemoveMethodFromWsdlFile(wsdlFileName, interfaceName, methodToRemove)
{
	// Remove the read only attribute to the wsdl file.
	RemoveReadOnlyAttribute(wsdlFileName);
	
	// Now open the wsdl file.
	var xmlDoc = new ActiveXObject("Msxml2.DOMDocument");
	xmlDoc.async = false;
	xmlDoc.load(wsdlFileName);

	// Select the parent "wsdl:portType" node from which we want to remove the child node.
	var node;
	var parent = xmlDoc.selectNodes("/wsdl:definitions/wsdl:portType[@name='" + interfaceName + "']")[0];
	//WScript.echo("Parent item selected: " + parent.nodeName);
	
	var nodes = parent.selectNodes("wsdl:operation[@name='" + methodToRemove + "']");
	for (i = 0; i < nodes.length; i++)
	{
		node = nodes[i];
		//WScript.echo("Removing: " + node.xml);
		parent.removeChild(node);
	}

	var parent = xmlDoc.selectNodes("/wsdl:definitions/wsdl:binding[@name='BasicHttpBinding_" + interfaceName + "']")[0];
	//WScript.echo("Parent item selected: " + parent.nodeName);
	
	var nodes = parent.selectNodes("wsdl:operation[@name='" + methodToRemove + "']");
	for (i = 0; i < nodes.length; i++)
	{
		node = nodes[i];
		//WScript.echo("Removing: " + node.xml);
		parent.removeChild(node);
	}
	
	// Now save the wsdl file.
	xmlDoc.save(wsdlFileName);
}

// <summary>
// Removes an element definition for a specified namespace from the xsd file.
// It removes the "xs:element" node from the "/xs:schema" node of the targetNamespace for which the name attribute corresponds to the methodToRemove parameter.
// </summary>
// <param name="xsdFileName">The xsd file to work on.</param>
// <param name="targetNamespace">The target namespace parent node from which we want to remove the method definition.</param>
// <param name="methodToRemove">The method name to remove.</param>
// <returns>Nothing is returned from this function.</returns>
function RemoveElementFromXsdFile(xsdFileName, targetNamespace, methodToRemove)
{
	// Remove the read only attribute to the xsd file.
	RemoveReadOnlyAttribute(xsdFileName);
	
	// Now open the wsdl file.
	var xmlDoc = new ActiveXObject("Msxml2.DOMDocument");
	xmlDoc.async = false;
	xmlDoc.load(xsdFileName);

	// Select the parent "xs:schema" node from which we want to remove the child node.
	var node;
	var parent = xmlDoc.selectNodes("/xs:schema[@targetNamespace='" + targetNamespace + "']")[0];
	//WScript.echo("Parent item selected: " + parent.nodeName);
	
	var nodes = parent.selectNodes("xs:element[@name='" + methodToRemove + "']");
	for (i = 0; i < nodes.length; i++)
	{
		node = nodes[i];
		//WScript.echo("Removing: " + node.xml);
		parent.removeChild(node);
	}

	// Now save the xsd file.
	xmlDoc.save(xsdFileName);
}

// <summary>
// Removes the read only attribute of a file.
// </summary>
// <param name="fileName">The file name for which we want to remove the read-only attribute.</param>
// <returns>Nothing is returned from this function.</returns>
function RemoveReadOnlyAttribute(fileName)
{
	var ReadOnly = 1;
	var fso = new ActiveXObject("Scripting.FileSystemObject");
	var file = fso.GetFile(fileName);
	if (file.Attributes & ReadOnly)
	{
		file.Attributes = file.Attributes - ReadOnly;
	}
}