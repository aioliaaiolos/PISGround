//================================================================================================
// File Name : MainDiffClass.cs
// Makefile Name : XmlDiffPatch.csproj
// Description : Main class to make it easier to use diff tools.
// Author : Sylvain Barbot
// Notes :
//================================================================================================
// Copyright by Alstom Transport Télécité Inc 2003. All rights reserved.
//
// The information contained herein is confidential property of Alstom
// Transport Télécité Inc. The use, copy, transfer of disclosure of such
// information is prohibited except by express written agreement with Alstom
// Transport Télécité Inc.
//
// $Log: $
//================================================================================================
//
//-----------------------------------------[ REFERENCES ]-----------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;
//---------------------------------------[ IMPLEMENTATION ]--------------------------------------- 
namespace Microsoft.XmlDiffPatch
{

    public class MainDiffClass
    {
        public Random aRandom { get; set; }

        public MainDiffClass()
        {
            aRandom = new Random();
        }

        /*================================================================================================
        Method            : mCreateXmlFromTxt
        Description       : create an Xml manifest file basing upon a text file representing a directory.
        Note              : lines should be like ((/Folder)+?|(/File)+?)
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        pOriginalFile   I   the original text file to convert
        pOutputFile     I   the resulting xml file
        Exit Conditions   :
        Returned Value(lEntryStream) :
        ================================================================================================*/
        public void mCreateXmlFromTxt(string pOriginalFile, string pOutputFile)
        {
            XmlDocument lDXmlDoc;
            XmlDeclaration lDeclaration;
            XmlElement lXmlRoot;
            System.IO.StreamReader lInputFile;
            string lLine;

            lDXmlDoc = new XmlDocument();
            lDeclaration = lDXmlDoc.CreateXmlDeclaration("1.0", null, null);
            lXmlRoot = lDXmlDoc.CreateElement("xmlfile");
            lInputFile = new System.IO.StreamReader(@pOriginalFile);

            lDXmlDoc.AppendChild(lDeclaration);
            lDXmlDoc.AppendChild(lXmlRoot);

            while ((lLine = lInputFile.ReadLine()) != null)
            {
                if (lLine.Trim().Length != 0)
                    mFindNodeToAdd(lDXmlDoc, lXmlRoot, lLine);
            }

            lInputFile.Close();
            lDXmlDoc.Save(pOutputFile);
        }

        /*================================================================================================
        Method            : mCreateManifestXmlFromDic
        Description       : create an Xml manifest file basing upon dictionnary representing a package.
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        Dictionary<string, long>   I pEntries list of entries names of a folder with crc  
        Exit Conditions   :
        Returned Value(lEntryStream) :
        ================================================================================================*/
        public void mCreateManifestXmlFromDic(Dictionary<string, long> pEntries, string pOutputFile)
        {
            XmlDocument lDXmlDoc;
            XmlDeclaration lDeclaration;
            XmlElement lXmlRoot;
            lDXmlDoc = new XmlDocument();
            lDeclaration = lDXmlDoc.CreateXmlDeclaration("1.0", null, null);
            lXmlRoot = lDXmlDoc.CreateElement("xmlfile");

            lDXmlDoc.AppendChild(lDeclaration);
            lDXmlDoc.AppendChild(lXmlRoot);

            foreach (string lLine in pEntries.Keys)
            {
                if (lLine.Trim().Length != 0)
                    mFindNodeToAdd(lDXmlDoc, lXmlRoot, lLine, pEntries);
            }

            lDXmlDoc.Save(pOutputFile);
        }

        /*================================================================================================
        Method            : mFindNodeToAdd
        Description       :
        Note              :
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        pCurrDoc     I
        pCurrNode    I
        pCurrString  I
        Exit Conditions   : 
        Returned Value(lEntryStream) : 
        ================================================================================================*/
        public void mFindNodeToAdd(XmlDocument pCurrDoc, XmlNode pCurrNode, string pCurrString)
        {
            bool lFind = false;

            if (pCurrString.IndexOf("\\") == 0)
                pCurrString = pCurrString.Substring(1);
            if (pCurrString.IndexOf("\\") != -1)
            {
                string lNodeName = pCurrString.Substring(0, pCurrString.IndexOf("\\"));
                string lSubstr = pCurrString.Substring(pCurrString.IndexOf("\\"));
                foreach (XmlNode lNode in pCurrNode.ChildNodes)
                {
                    foreach (XmlAttribute lAttr in lNode.Attributes)
                    {
                        if (lAttr.LocalName == "Name" && lAttr.Value == lNodeName)
                        {
                            lFind = true;
                            mFindNodeToAdd(pCurrDoc, lNode, lSubstr);
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode lNode in pCurrNode.ChildNodes)
                {
                    foreach (XmlAttribute lAttr in lNode.Attributes)
                    {
                        if (lAttr.LocalName == "Name" && lAttr.Value == pCurrString)
                        {
                            lFind = true;
                        }
                    }
                }
            }
            if (!lFind)
            {
                mAppendChildren(pCurrDoc, pCurrNode, pCurrString);
            }

        }

        public void mFindNodeToAdd(XmlDocument pCurrDoc, XmlNode pCurrNode, string pCurrString, Dictionary<string, long> pEntries)
        {
            bool lFind = false;

            if (pCurrString.IndexOf("/") == 0)
                pCurrString = pCurrString.Substring(1);
            if (pCurrString.IndexOf("/") != -1)
            {
                string lNodeName = pCurrString.Substring(0, pCurrString.IndexOf("/"));
                string lSubstr = pCurrString.Substring(pCurrString.IndexOf("/"));
                foreach (XmlNode lNode in pCurrNode.ChildNodes)
                {
                    foreach (XmlAttribute lAttr in lNode.Attributes)
                    {
                        if (lAttr.LocalName == "Name" && lAttr.Value == lNodeName)
                        {
                            lFind = true;
                            mFindNodeToAdd(pCurrDoc, lNode, lSubstr, pEntries);
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode lNode in pCurrNode.ChildNodes)
                {
                    foreach (XmlAttribute lAttr in lNode.Attributes)
                    {
                        if (lAttr.LocalName == "Name" && lAttr.Value == pCurrString)
                        {
                            lFind = true;
                        }
                    }
                }
            }
            if (!lFind)
            {
                mAppendChildren(pCurrDoc, pCurrNode, pCurrString, pEntries);
            }

        }

        /*================================================================================================
        Method            : mAppendChildren
        Description       :
        Note              :
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        pCurrDoc     I
        pCurrNode    I
        pCurrString  I
        Exit Conditions   :
        Returned Value(lEntryStream) :
        ================================================================================================*/
        public void mAppendChildren(XmlDocument pCurrDoc, XmlNode pCurrNode, string pCurrString)
        {
            XmlElement lNewNode;

            if (pCurrString.IndexOf("\\") == 0)
                pCurrString = pCurrString.Substring(1);
            if (pCurrString.IndexOf("\\") != -1)
            {
                int lFirstSlash = pCurrString.IndexOf("\\");
                string lNodeName = pCurrString.Substring(0, lFirstSlash);
                string lSubstr = pCurrString.Substring(lFirstSlash);

                lNewNode = pCurrDoc.CreateElement("Folder");
                lNewNode.SetAttribute("Name", lNodeName);
                mAppendChildren(pCurrDoc, lNewNode, lSubstr);
            }
            else
            {
                if (!pCurrString.Contains('.'))
                {
                    lNewNode = pCurrDoc.CreateElement("Folder");
                    lNewNode.SetAttribute("Name", pCurrString);
                }
                else
                {
                    lNewNode = pCurrDoc.CreateElement("File");
                    lNewNode.SetAttribute("Name", pCurrString);
                    lNewNode.SetAttribute("CRC", (aRandom.Next(0, 99999999)).ToString());
                }
            }
            pCurrNode.AppendChild(lNewNode);
        }

        public void mAppendChildren(XmlDocument pCurrDoc, XmlNode pCurrNode, string pCurrString, Dictionary<string, long> pEntries)
        {
            XmlElement lNewNode;

            if (pCurrString.IndexOf("/") == 0)
                pCurrString = pCurrString.Substring(1);
            if (pCurrString.IndexOf("/") != -1)
            {
                int lFirstSlash = pCurrString.IndexOf("/");
                string lNodeName = pCurrString.Substring(0, lFirstSlash);
                string lSubstr = pCurrString.Substring(lFirstSlash);
                
                lNewNode = pCurrDoc.CreateElement("Folder");
                lNewNode.SetAttribute("Name", lNodeName);
                mAppendChildren(pCurrDoc, lNewNode, lSubstr, pEntries);
            }
            else
            {
                if (!pCurrString.Contains('.'))
                {
                    lNewNode = pCurrDoc.CreateElement("Folder");
                    lNewNode.SetAttribute("Name", pCurrString);
                }
                else
                {
                    lNewNode = pCurrDoc.CreateElement("File");
                    lNewNode.SetAttribute("Name", pCurrString);
                    
                    string lCrcKey = pCurrString;
                    XmlNode lNode = pCurrNode;
                    while (lNode != pCurrDoc.DocumentElement)
                    {
                        foreach (XmlAttribute lAttr in lNode.Attributes)
                        {
                            if (lAttr.LocalName == "Name")
                            {
                                lCrcKey = lAttr.Value + "/" + lCrcKey;
                            }
                        }
                        lNode = lNode.ParentNode;
                    }

                    if (pEntries.ContainsKey("/" + lCrcKey))
                    {
                        lNewNode.SetAttribute("CRC", pEntries["/" + lCrcKey].ToString());
                    }
                    else
                    {
                        lNewNode.SetAttribute("CRC", (aRandom.Next(0, 99999999)).ToString());
                    }
                }
            }
            pCurrNode.AppendChild(lNewNode);
        }

        /*================================================================================================
        Method            : mCreateDiffCopy
        Description       :
        Note              :
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        pOriginalFile   I
        pOutputFile     I
        Exit Conditions   :
        Returned Value(lEntryStream) :
        ================================================================================================*/
        public void mCreateDiffCopy(string pOriginalFile, string pOutputFile)
        {
            XmlDocument lXmlDoc = new XmlDocument();
            lXmlDoc.Load(pOriginalFile);

            mEditRecursivelyRandomly(lXmlDoc, lXmlDoc.DocumentElement);

            lXmlDoc.Save(pOutputFile);
        }

        /*================================================================================================
        Method            : mEditRecursivelyRandomly
        Description       :
        Note              :
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        pXmlDoc      I
        pCurrNode    I
        pAlea        I
        Exit Conditions   :
        Returned Value(lEntryStream) :
        ================================================================================================*/
        public void mEditRecursivelyRandomly(XmlDocument pXmlDoc, XmlNode pCurrNode)
        {
            foreach (XmlNode lNode in pCurrNode.ChildNodes)
            {
                if (lNode != null && lNode != pCurrNode)
                {
                    mEditRecursivelyRandomly(pXmlDoc, lNode);
                    mEditRandomly(pXmlDoc, lNode);
                }
            }
        }

        /*================================================================================================
        Method            :
        Description       :
        Note              :
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        pXmlDoc      I
        pCurrNode    I
        pAlea        I
        Exit Conditions   :
        Returned Value(lEntryStream) :
        ================================================================================================*/
        public void mEditRandomly(XmlDocument pXmlDoc, XmlNode pCurrNode)
        {
            int i = aRandom.Next(0, 10);
            switch (i)
            {
                case 1:
                    {
                        foreach (XmlAttribute lAttr in pCurrNode.Attributes)
                            if (lAttr.Name == "Name")
                                lAttr.Value = lAttr.Value + (i).ToString();
                        break;
                    }
                case 2:
                    {
                        foreach (XmlAttribute lAttr in pCurrNode.Attributes)
                            if (lAttr.Name == "CRC")
                                lAttr.Value = lAttr.Value + (i).ToString();

                        break;
                    }
                case 3:
                    {
                        if (pCurrNode.ParentNode != pXmlDoc.DocumentElement)
                            pCurrNode.ParentNode.RemoveChild(pCurrNode);
                        break;
                    }
                case 4:
                case 6:
                    {
                        string lName = "";
                        foreach (XmlAttribute lAttr in pCurrNode.Attributes)
                            if (lAttr.Name == "Name")
                                lName = lAttr.Value + (i).ToString();
                        if (!lName.Contains('.'))
                        {
                            XmlElement lElt = pXmlDoc.CreateElement("Folder");
                            lElt.SetAttribute("Name", lName);
                            pCurrNode.ParentNode.AppendChild(lElt);
                        }
                        break;
                    }
                case 5:
                case 7:
                    {
                        string lName = "";
                        string lCRC = "";
                        foreach (XmlAttribute lAttr in pCurrNode.Attributes)
                        {
                            if (lAttr.Name == "Name")
                                lName = lAttr.Value + (i).ToString();
                            if (lAttr.Name == "CRC")
                                lCRC = lAttr.Value + (i).ToString();
                        }
                        if (lName.Contains('.'))
                        {
                            XmlElement lElt = pXmlDoc.CreateElement("File");
                            lElt.SetAttribute("Name", lName);
                            lElt.SetAttribute("CRC", lCRC);
                            pCurrNode.ParentNode.AppendChild(lElt);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        /*================================================================================================
        Method            : mGenerateDiffXml
        Description       :
        Note              :
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        pOriginalFile
        pNewFile
        pDiffXmlWriter
        pToRemove
        Exit Conditions   :
        Returned Value(lEntryStream) :
        ================================================================================================*/
        public void mGenerateDiffXml(string pOriginalFile, string pNewFile, XmlWriter pDiffXmlWriter, string[] pToRemove)
        {
            XmlDiff lXmlDiff = new XmlDiff(XmlDiffOptions.IgnoreComments | XmlDiffOptions.IgnoreNamespaces | XmlDiffOptions.IgnorePrefixes);

            lXmlDiff.Algorithm = XmlDiffAlgorithm.Fast;

            XmlReader lOriginalReader = mGetXmlReaderFromFile(pOriginalFile, pToRemove);
            XmlReader lNewReader = mGetXmlReaderFromFile(pNewFile, pToRemove);

            lXmlDiff.CompareXmlReadable(lOriginalReader, lNewReader, pDiffXmlWriter);
            pDiffXmlWriter.Close();
        }

        public void nGenerateReadableDiffXml(string pOriginalFile, string pNewFile, string pDiffManifestFile, string[] pToRemove, string pTmpPath, string pDPOnBoard, string pDPOnGround)
        {
           
                       

            string lTmpFile = Path.GetRandomFileName();
            lTmpFile = lTmpFile.Substring(0, lTmpFile.IndexOf('.')) + ".xml";
            lTmpFile = Path.Combine(pTmpPath, lTmpFile);
            
            XmlWriterSettings lSettings = new XmlWriterSettings();
            lSettings.NewLineHandling = NewLineHandling.Entitize;
            lSettings.NewLineChars = "\n";
            lSettings.Indent = true;
            lSettings.IndentChars = "\t";
            lSettings.Encoding = Encoding.Unicode;

            XmlWriter lXmlWriter = XmlWriter.Create(lTmpFile, lSettings);

            mGenerateDiffXml(pOriginalFile, pNewFile, lXmlWriter, pToRemove);
                      
            XmlDocument lXmlFirstDiffDoc = new XmlDocument();
            lXmlFirstDiffDoc.Load(lTmpFile);

            XmlNodeList lAddNodes = lXmlFirstDiffDoc.GetElementsByTagName("add");
            XmlNodeList lChangeNodes = lXmlFirstDiffDoc.GetElementsByTagName("change");
            XmlNodeList lRemoveNodes = lXmlFirstDiffDoc.GetElementsByTagName("remove");


            XmlDocument lXmlPackageDiffDoc = new XmlDocument();
            XmlDeclaration lDeclaration = lXmlPackageDiffDoc.CreateXmlDeclaration("1.0", null, null);
            XmlElement lXmlRoot= lXmlPackageDiffDoc.CreateElement("diff");
            lXmlRoot.SetAttribute("ReferenceDataPackage", pDPOnBoard);
            lXmlRoot.SetAttribute("NewDataPackage", pDPOnGround);

            lXmlPackageDiffDoc.AppendChild(lDeclaration);
            lXmlPackageDiffDoc.AppendChild(lXmlRoot);

            foreach (XmlNode lNode in lAddNodes)
            {
                foreach (XmlNode lChildNode in lNode.ChildNodes)
                {
                    lXmlRoot.AppendChild(generateXmlNodeAddRemove(lXmlPackageDiffDoc, "add", lChildNode));
                }
            }
            foreach (XmlNode lNode in lRemoveNodes)
            {
                foreach (XmlNode lChildNode in lNode.ChildNodes)
                {
                    lXmlRoot.AppendChild(generateXmlNodeAddRemove(lXmlPackageDiffDoc, "remove", lChildNode));
                }
            }
            foreach (XmlNode lNode in lChangeNodes)
            {
                if (lNode.ParentNode != lNode.OwnerDocument.DocumentElement.FirstChild.FirstChild)
                {
                    lXmlRoot.AppendChild(generateXmlNodeChange(lXmlPackageDiffDoc, "modify", lNode));
                }
            }

            File.Delete(lTmpFile);
            lXmlPackageDiffDoc.Save(pDiffManifestFile);
        }

        XmlNode generateXmlNodeAddRemove(XmlDocument pXmlDoc, string pAction, XmlNode pSrc)
        {
            XmlElement lNewNode = pXmlDoc.CreateElement(pAction);

            string lFullName = "";
            foreach (XmlAttribute lAttr in pSrc.Attributes)
            {
                if (lAttr.LocalName == "Name")
                {
                    lFullName = lAttr.Value + "/" + lFullName;
                }
            }
            pSrc = pSrc.ParentNode;
            while (pSrc != pSrc.OwnerDocument.DocumentElement.FirstChild)
            {
                foreach (XmlAttribute lAttr in pSrc.Attributes)
                {
                    if (lAttr.LocalName == "Name")
                    {
                        lFullName = lAttr.Value + "/" + lFullName;
                    }
                }
                pSrc = pSrc.ParentNode;
            }
            if (lFullName.IndexOf("/") == 0)
            {
                lFullName = lFullName.Substring(lFullName.IndexOf("/"));
            }
            if (lFullName.LastIndexOf("/") == lFullName.Length - 1)
            {
                lFullName = lFullName.Substring(0, lFullName.LastIndexOf("/"));              
            }
            lNewNode.SetAttribute("item", lFullName);

            return lNewNode;
        }


        XmlNode generateXmlNodeChange(XmlDocument pXmlDoc, string pAction, XmlNode pSrc)
        {
            XmlElement lNewNode = pXmlDoc.CreateElement(pAction);

            string lFullName = "";
            bool lIsNameChange = false;
            foreach (XmlAttribute lAttr in pSrc.Attributes)
            {
                if (lAttr.LocalName == "attr")
                {
                    if( lAttr.Value == "Name" )
                    {
                        lIsNameChange = true;
                        break;
                    }
                }
            }
            if (lIsNameChange)
            {
                foreach (XmlAttribute lAttr in pSrc.Attributes)
                {
                    if (lAttr.LocalName == "value")
                    {
                        lFullName = lAttr.Value;
                    }
                }
            }

            XmlNode lNode;
            if (lIsNameChange)
            {
                lNode = pSrc.ParentNode.ParentNode;
            }
            else
            {
                lNode = pSrc.ParentNode;
            }

            while (lNode != pSrc.OwnerDocument.DocumentElement.FirstChild)
            {
                foreach (XmlAttribute lAttr in lNode.Attributes)
                {
                    if (lAttr.LocalName == "Name")
                    {
                        lFullName = lAttr.Value + "/" + lFullName;
                    }
                }
                lNode = lNode.ParentNode;
            }
            if (lFullName.IndexOf("/") == 0)
            {
                lFullName = lFullName.Substring(lFullName.IndexOf("/"));
            }
            if (lFullName.LastIndexOf("/") == lFullName.Length - 1)
            {
                lFullName = lFullName.Substring(0, lFullName.LastIndexOf("/"));
            }

            lNewNode.SetAttribute("item", lFullName);

            return lNewNode;
        }


        /*================================================================================================
        Method            :
        Description       :
        Note              :
        --------------------------------------------------------------------------------------------------
        Parameters :
        Name        I/O     Descritpion
        Exit Conditions   :
        Returned Value(lEntryStream) :
        ================================================================================================*/        
        public XmlReader mGetXmlReaderFromFile(string pFile, string[] pToRemove)
        {
            XmlDocument lXmlDoc = new XmlDocument();
            lXmlDoc.Load(pFile);
            foreach (string lBalise in pToRemove)
            {
                foreach (XmlNode lNode in lXmlDoc.DocumentElement)
                {
                    if (lNode.LocalName == lBalise)
                    {
                        lXmlDoc.DocumentElement.RemoveChild(lNode);
                        continue;
                    }
                }
            }
            XmlReader lXmlReader = XmlReader.Create(new StringReader(lXmlDoc.OuterXml));
            return lXmlReader;
        }
    }
}
