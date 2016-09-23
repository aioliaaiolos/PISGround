//---------------------------------------------------------------------------------------------------
// <copyright file="NotificationInfo.cs" company="Alstom">
//          (c) Copyright ALSTOM 2016.  All rights reserved.
//
//          This computer program may not be used, copied, distributed, corrected, modified, translated,
//          transmitted or assigned without the prior written authorization of ALSTOM.
// </copyright>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PIS.Ground.GroundCore.AppGround;
using System.Globalization;

namespace DataPackageTests.Stubs
{
    /// <summary>
    /// Describe a notification.
    /// </summary>
    /// <remarks>This object is immutable</remarks>
    class NotificationInfo
    {
        // String that allow cleaning parameter string.
        private const string BEGIN_INSTR = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<ArrayOfString xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <string>";
        private const string END_INSTR = "</string>\r\n</ArrayOfString>";
        private const string COMMA_INSTR = "</string>\r\n  <string>";
        private const string EMPTY_VALUE = "<string />";
        private const string NEW_EMPTY_VALUE = "<string></string>";

        /// <summary>
        /// Gets the notification identifier.
        /// </summary>
        public NotificationIdEnum Id { get; private set; }

        /// <summary>
        /// Gets the notification request identifier.
        /// </summary>
        public Guid RequestId { get; private set; }

        /// <summary>
        /// Gets the parameter associated to the notification.
        /// </summary>
        public string Parameter { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
        /// </summary>
        /// <param name="id">The notification identifier.</param>
        public NotificationInfo(NotificationIdEnum id)
            : this(id, Guid.Empty, null)
        {
            /* No logic body */
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
        /// </summary>
        /// <param name="id">The notification identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        public NotificationInfo(NotificationIdEnum id, Guid requestId)
            : this(id, requestId, null)
        {
            /* No logic body */
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationInfo"/> class.
        /// </summary>
        /// <param name="id">The notification identifier.</param>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="parameter">Additional parameter</param>
        public NotificationInfo(NotificationIdEnum id, Guid requestId, string parameter)
        {
            Id = id;
            RequestId = requestId;
            Parameter = parameter;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return (string.IsNullOrEmpty(Parameter)) ?
                string.Format(CultureInfo.InvariantCulture, "{0,-50} @ {1}", Id, RequestId) :
                string.Format(CultureInfo.InvariantCulture, "{0,-50} @ {1} [{2}]", Id, RequestId, GetParameterAsCommaDelimitedString());
        }

        /// <summary>
        /// Clear the parameter string to display the content as a comma delimited string.
        /// </summary>
        /// <returns>Parameter values separated with comma character.</returns>
        public string GetParameterAsCommaDelimitedString()
        {
            return Parameter.Replace(EMPTY_VALUE, NEW_EMPTY_VALUE).Replace(BEGIN_INSTR, string.Empty).Replace(END_INSTR, string.Empty).Replace(COMMA_INSTR, ", ");
        }
    }
}
