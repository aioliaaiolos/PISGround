using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PIS.Ground.Core.Data
{
    public class ServiceInfoList : IEnumerable<ServiceInfo>
    {
        private readonly ServiceInfo[] _serviceInfoArray;

        /// <summary>
        /// Retrieves the immutable count of the list.
        /// </summary>
        public int Count
        {
            get { return _serviceInfoArray.Length; }
        }

        /// <summary>Initializes a new instance of the ServiceInfoList class.</summary>
        public ServiceInfoList()
        {
            _serviceInfoArray = new ServiceInfo[0];            
        }

        /// <summary>
        /// Create a new list, copying elements from the specified array.
        /// </summary>
        /// <param name=”arrayToCopy”>An array whose contents will be copied.</param>
        public ServiceInfoList(ServiceInfo[] arrayToCopy)
        {
            _serviceInfoArray = new ServiceInfo[arrayToCopy.Length];
            Array.Copy(arrayToCopy, _serviceInfoArray, arrayToCopy.Length);
        }

        /// <summary>
        /// Create a new list, copying elements from the specified enumerable.
        /// </summary>
        /// <param name=”enumerableToCopy”>An enumerable whose contents will
        /// be copied.</param>
        public ServiceInfoList(IEnumerable<ServiceInfo> enumerableToCopy)
        {
            _serviceInfoArray = new List<ServiceInfo>(enumerableToCopy).ToArray();
        }

        /// <summary>
        /// Retrieves an enumerator for the list’s collections.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<ServiceInfo> GetEnumerator()
        {
            for (int i = 0; i < _serviceInfoArray.Length; i++)
            {
                yield return _serviceInfoArray[i];
            }
        }
        /// <summary>
        /// Retrieves an enumerator for the list’s collections.
        /// </summary>
        /// <returns>An enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ServiceInfo>)this).GetEnumerator();
        }
    }
}
