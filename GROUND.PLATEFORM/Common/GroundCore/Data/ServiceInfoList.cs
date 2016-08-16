using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace PIS.Ground.Core.Data
{
    /// <summary>
    /// Manage a readonly collection of ServiceInfo object.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{PIS.Ground.Core.Data.ServiceInfo}" />
    public class ServiceInfoList : IEnumerable<ServiceInfo>
    {
        private static readonly ServiceInfoList _empty = new ServiceInfoList();

        private readonly ServiceInfo[] _serviceInfoArray;

        /// <summary>
        /// Retrieves the immutable count of the list.
        /// </summary>
        public int Count
        {
            get { return _serviceInfoArray.Length; }
        }

        /// <summary>
        /// Gets the read-only empty service info list.
        /// </summary>
        public static ServiceInfoList Empty
        {
            get
            {
                return _empty;
            }
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

        /// <summary>
        /// Gets the <see cref="ServiceInfo"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public ServiceInfo this[int index]
        {
            get
            {
                return _serviceInfoArray[index];
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool isEqual = !object.ReferenceEquals(null, obj);
            if (isEqual)
            {
                ServiceInfoList other = obj as ServiceInfoList;
                if (!object.ReferenceEquals(null, other))
                {
                    if (Count == other.Count)
                    {
                        for (int i = 0; i < Count && isEqual; ++i)
                        {
                            isEqual = this[i].Equals(other[i]);
                        }
                    }
                    else
                    {
                        isEqual = false;
                    }
                }
                else
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _serviceInfoArray.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder(200);
            Dump(string.Empty, output);
            return output.ToString();
        }

        /// <summary>
        /// Dumps the current collection into an output string.
        /// </summary>
        /// <param name="prefix">The prefix to add when writing each item.</param>
        /// <param name="output">The output string.</param>
        public void Dump(string prefix, StringBuilder output)
        {
            output.Append("{");

            string itemPrefix = prefix + "\t";
            for (int i = 0; i < Count; ++i)
            {
                if (i != 0)
                {
                    output.AppendLine(",");
                }
                else
                {
                    output.AppendLine();
                }


                output.AppendFormat("{0}[{1}] = ", itemPrefix, i);
                this[i].Dump(itemPrefix, output);
            }
            output.Append("}");
        }
    }
}
