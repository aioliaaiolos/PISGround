//---------------------------------------------------------------------------------------------------
// <copyright file="IRemoteFileClass.cs" company="Alstom">
//          (c) Copyright ALSTOM 2013.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Configuration;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// This class manage remote file for read access. It simplified acces methods to don't care
    /// about ftp, http or local file when dealing with files.
    /// </summary>
    public interface IRemoteFileClass : IComparable<IRemoteFileClass>
    {

        #region properties

        /// <summary>Return the file name. FileName is set from filePath.</summary>
        /// <value>The name of the file.</value>
        string FileName
        {
            get;
        }

        /// <summary>Return the file path.</summary>
        /// <value>The full pathname of the file.</value>
        string FilePath
        {
            get;
        }

        /// <summary>
        /// return the file size using filetype and http/ftp request to resolve it over network (FileInfo
        /// otherwise).
        /// </summary>
        /// <value>The size.</value>
        long Size
        {
            get;
        }

        /// <summary>
        /// Open the file for reading only. Return a stream on the file so you can read it.
        /// </summary>
        /// <param name="pStream">The ouput stream.</param>
        void OpenStream(out System.IO.Stream pStream);

        /// <summary>
        /// Return wheter or not the file realy exists (and is a ccessible). True if the file is.
        /// </summary>
        /// <value>true if exists, false if not.</value>
        bool Exists
        {
            get;
        }

        FileTypeEnum FileType
        {
            get;
        }

        /// <summary>Return the CRC of the file.</summary>
        /// <value>The CRC.</value>
        string CRC
        {
            get;
        }

        /// <summary>Return or set FtpStatus</summary>
        /// <value>The CRC.</value>
        FtpStatus FtpStatus
        {
            get;
            set;
        }

        /// <summary>Return or set FtpStatus</summary>
        /// <value>The CRC.</value>
        bool HasError
        {
            get;
            set;
        }


        /// <summary>Return if the file has been initialized.</summary>
        /// <value>The CRC.</value>
        bool IsInitialized
        {
            get;
        }

        #endregion

        #region methods

        /// <summary>
        /// Initialization for file (i.e UNC or local path). 
        /// </summary>
        void mInitFile();

        #endregion
    }
}
