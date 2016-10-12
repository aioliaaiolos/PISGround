//---------------------------------------------------------------------------------------------------
// <copyright file="ExceptionMemoryCollection.cs" company="Alstom">
//		  (c) Copyright ALSTOM 2016.  All rights reserved.
//
//		  This computer program may not be used, copied, distributed, corrected, modified, translated,
//		  transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace PIS.Ground.Core.Common
{
    /// <summary>
    /// Class that memorize exceptions occurrences.
    /// </summary>
    public class ExceptionMemoryCollection
    {
        /// <summary>
        /// Mutex to allow usage in multiple threads.
        /// </summary>
        private object _lock = new object();

        /// <summary>
        /// Collection to keep previous exceptions. Key is the exception key. Value is the list of previous exceptions.
        /// </summary>
        /// <remark>List is used because insertion is faster and use less memory. It's not expected that these collection become use</remark>
        private Dictionary<string, List<string> > _previousExceptions = new Dictionary<string, List<string>>();


        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMemory"/> class.
        /// </summary>
        public ExceptionMemoryCollection()
        {
            // No logic body
        }

        /// <summary>
        /// Determines whether is the specified exception occurs for the first time for the given key.
        /// </summary>
        /// <param name="key">The key to associate the exception.</param>
        /// <param name="exception">The exception object to evaluate.</param>
        /// <returns>
        ///   <c>true</c> if it's the first occurrence, otherwise false.</c>.
        /// </returns>
        public bool IsFirstOccurrence(string key, Exception exception)
        {
            bool firstOccurrence;
            lock (_lock)
            {
                string exceptionMessage = (exception is TimeoutException) ? "TimeoutException" : exception.Message;
                List<string> exceptionList;
                if (_previousExceptions.TryGetValue(key, out exceptionList))
                {
                    firstOccurrence = !exceptionList.Contains(exceptionMessage);
                    if (firstOccurrence)
                    {
                        exceptionList.Add(exceptionMessage);
                    }
                }
                else
                {
                    exceptionList = new List<string>();
                    exceptionList.Add(exceptionMessage);
                    _previousExceptions.Add(key, exceptionList);
                    firstOccurrence = true;
                }
            }

            return firstOccurrence;
        }
    }
}
