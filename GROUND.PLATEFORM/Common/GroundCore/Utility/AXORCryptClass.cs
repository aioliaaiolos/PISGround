using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PIS.Ground.Core.Utility
{
    /// <summary>
    /// Class for encrypting/decrypting a file with a 256 bits key.
    /// </summary>
    public class AXORCryptClass
    {
        //Key length
        const byte sCypherKeyLength = 32;
        const byte sFileHeaderLength = 16;

        //The GUID that is used as the header of a crypted file - {DD35461F-AD64-481c-9D7E-4BFED688F3A9}
        public static readonly Guid sFileHeader = new Guid(0xdd35461F, 0xad64, 0x481c, 0x9d, 0x7e, 0x4b, 0xfe, 0xd6, 0x88, 0xf3, 0xa9);

        /// <summary>Encrypt/Decrypt a file.</summary>
        /// <param name="pSrcFilePath">Full pathname of the source file.</param>
        /// <param name="pDstFilePath">Full pathname of the destination file.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool ConvertFile(String pSrcFilePath, String pDstFilePath)
        {
            bool lResult = false;

            FileInfo lFileInfo = new FileInfo(pSrcFilePath);

            //Check if file exists
            if (lFileInfo.Exists)
            {
                bool lEncrypted = IsFileEncrypted(pSrcFilePath);

                FileStream lInputStream = lFileInfo.OpenRead();
                long lSrcFileSize = lInputStream.Length;

                try
                {
                    FileStream lOutputStream = new FileStream(pDstFilePath, FileMode.CreateNew);

                    byte lCypherKeyHalfLength = sCypherKeyLength >> 1;
                    byte[] lCypherKey = new byte[sCypherKeyLength];
                    byte[] lCypherKeyTop = new byte[lCypherKeyHalfLength];
                    byte[] lCypherKeyBottom = new byte[lCypherKeyHalfLength];

                    //If file is encrypted - decrypt it. Otherwise - encrypt it
                    if (lEncrypted)
                    {
                        // Get top part key
                        lInputStream.Seek(sFileHeaderLength, SeekOrigin.Begin);
                        lInputStream.Read(lCypherKeyTop, 0, lCypherKeyHalfLength);

                        // Get bottom part key
                        lInputStream.Seek(-lCypherKeyHalfLength, SeekOrigin.End);
                        lInputStream.Read(lCypherKeyBottom, 0, lCypherKeyHalfLength);

                        // Put both ends together and put seeker at start
                        Array.Copy(lCypherKeyTop, 0, lCypherKey, lCypherKeyHalfLength, lCypherKeyHalfLength);
                        Array.Copy(lCypherKeyBottom, lCypherKey, lCypherKeyHalfLength);
                        lInputStream.Seek(sFileHeaderLength + lCypherKeyHalfLength, SeekOrigin.Begin);

                        // Need to change filesize
                        // because we are skipping header
                        lSrcFileSize -= sFileHeaderLength + lCypherKeyHalfLength;
                    }
                    else
                    {
                        // Create a new cypher key
                        AXORCryptClass.createCypherKey(lCypherKey);

                        // Split the cypher key in 2 so we can hide it in
                        // the target file
                        Array.Copy(lCypherKey, lCypherKeyHalfLength, lCypherKeyTop, 0, lCypherKeyHalfLength);
                        Array.Copy(lCypherKey, lCypherKeyBottom, lCypherKeyHalfLength);

                        // Write the right header
                        lOutputStream.Seek(0, SeekOrigin.Begin);
                        lOutputStream.Write(sFileHeader.ToByteArray(), 0, sFileHeaderLength);

                        // Hide part of key in top                            
                        lOutputStream.Seek(sFileHeaderLength, SeekOrigin.Begin);
                        lOutputStream.Write(lCypherKeyTop, 0, lCypherKeyHalfLength);
                    }

                    // Init vars for reading files
                    const int lBufferSize = 512;
                    byte[] lBuffer = new byte[lBufferSize];
                    int lRecordCount;
                    int lRecordCountTotal = 0;

                    // Read maxBufferSize (512) chars of file
                    // recordCount = number of chars read
                    lRecordCount = lInputStream.Read(lBuffer, 0, lBufferSize);

                    while (lRecordCount > 0)
                    {
                        // Keep track of how many bytes we read
                        // So we can know if we are reading the last line
                        lRecordCountTotal += lRecordCount;

                        // Encrypt or decrypt
                        // See upper init of cypherKey
                        for (int i = 0; i < lRecordCount; i++)
                        {
                            lBuffer[i] ^= lCypherKey[i % sCypherKeyLength];
                        }

                        // If it's our last line writing
                        if (lRecordCountTotal == lSrcFileSize)
                        {
                            // And we are decrypting a file
                            if (lEncrypted)
                            {
                                // Remove the cypherKeyBottom from the decrypted file
                                lRecordCount -= lCypherKeyHalfLength / 2;
                            }
                        }

                        // Write the encrypted/decrypted line to the destination file
                        lOutputStream.Write(lBuffer, 0, lRecordCount);

                        // Read another maxBufferSize (512) number of nytes
                        lRecordCount = lInputStream.Read(lBuffer, 0, lBufferSize);
                    }

                    // If we are encrypting
                    if (!lEncrypted)
                    {
                        // Hide rest of key at end				                                
                        lOutputStream.Write(lCypherKeyBottom, 0, lCypherKeyHalfLength);
                    }

                    lInputStream.Close();
                    lOutputStream.Close();

                    lResult = true;
                }
                catch (System.IO.IOException)
                {

                }
            }

            return lResult;
        }

        /// <summary>Check whether file is encrypted.</summary>
        /// <param name="pFilePath">Full pathname of the file.</param>
        /// <returns>"true" if file encrypted, "false" - otherwise.</returns>
        public static bool IsFileEncrypted(String pFilePath)
        {
            bool lResult = false;

            FileInfo lFileInfo = new FileInfo(pFilePath);

            //Check if file exists
            if (lFileInfo.Exists)
            {
                byte[] lCypherFileHeader = new byte[sFileHeaderLength];

                FileStream lFStream = lFileInfo.OpenRead();

                //Read first 16 bytes from the file
                lFStream.Read(lCypherFileHeader, 0, sFileHeaderLength);

                Guid lReadGuid = new Guid(lCypherFileHeader);

                //Check whether file is encrypted
                if (lReadGuid == sFileHeader)
                {
                    lResult = true;
                }

                lFStream.Close();
            }

            return lResult;
        }

        /// <summary>Creates a cypher key.</summary>
        /// <param name="p256BitsCypherKey">The 32 bytes array to be filled with cypher key.</param>
        private static void createCypherKey(byte[] p256BitsCypherKey)
        {
            //Generate a GUID
            Guid lGuid = Guid.NewGuid();

            //Convert the GUID to a string
            String lGuidString = lGuid.ToString("N");

            //Create an istance of Random class
            Random lRandom = new Random();

            //Generate a chypher key
            for (int i = 0; i < sCypherKeyLength; i++)
            {
                byte lByte1 = Convert.ToByte(lGuidString[i]);
                byte lByte2 = Convert.ToByte(lRandom.Next(0, 255));
                p256BitsCypherKey[i] = Convert.ToByte(lByte1 ^ lByte2);
            }
        }
    }
}
