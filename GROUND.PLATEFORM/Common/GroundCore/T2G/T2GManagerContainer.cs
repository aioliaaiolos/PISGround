//---------------------------------------------------------------------------------------------------
// <copyright file="T2GManagerContainer.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
namespace PIS.Ground.Core.T2G
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>T2G manager container.</summary>
    public class T2GManagerContainer
    {
        #region Private static fields

        /// <summary>T2G manager singleton instance.</summary>
        private static IT2GManager _t2gManager = null;
        
        /// <summary>The initialized.</summary>
        private static volatile bool _initialized = false;

        private static object _initializationLock = new object();
        
        #endregion

        /// <summary>Gets or sets (only for tests) a T2G manager instance.</summary>
        /// <value>The T2G manager.</value>
        public static IT2GManager T2GManager
        {
            get
            {
                // This implementation caches a singleton instance
                // instead of creating a new object, for performance
                // reasons

                if (!_initialized)
                {
                    lock (_initializationLock)
                    {
                        if (!_initialized)
                        {
                            _t2gManager = new T2GManager();
                            _initialized = true;
                        }
                    }
                }

                return _t2gManager;
            }

            set
            {
                lock (_initializationLock)
                {
                    _t2gManager = value;
                    _initialized = value != null;
                }
            }
        }        
    }
}
