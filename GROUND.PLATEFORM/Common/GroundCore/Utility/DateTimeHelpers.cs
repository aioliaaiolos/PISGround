// <copyright file="DateTimeHelpers.cs" company="Alstom">
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

namespace PIS.Ground.Core.Utility
{
    /// <summary>
    /// Utility class that permits to add extensions to DateTime object like safe method to compare date-time
    /// </summary>
    public static class DateTimeHelpers
    {
        /// <summary>
        /// Compare to DateTime object by ensuring that comparison is performed on same time zone. If time zone differ, the comparison is performed on the UTC value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>A signed number indicating the relative values of left and right.
        /// <list type="table">
        /// <listheader><term>Value</term><term>Description</term></listheader>
        /// <item><term>Less than zero</term><term>Left is earlier than right.</term></item>
        /// <item><term>Zero</term><term>Left is the same as right.</term></item>
        /// <item><term>Greater than zero</term><term>Left is later than right.</term></item>
        /// </list></returns>
        public static int SafeCompareTo(this DateTime left, DateTime right)
        {
            return (left.Kind == right.Kind) ? left.CompareTo(right) : left.ToUniversalTime().CompareTo(right.ToUniversalTime());
        }

        /// <summary>
        /// Compare to DateTime object by ensuring that comparison is performed on same time zone. If time zone differ, the comparison is performed on the UTC value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>A signed number indicating the relative values of left and right.
        /// <list type="table">
        /// <listheader><term>Value</term><term>Description</term></listheader>
        /// <item><term>Less than zero</term><term>Left is earlier than right.</term></item>
        /// <item><term>Zero</term><term>Left is the same as right.</term></item>
        /// <item><term>Greater than zero</term><term>Left is later than right.</term></item>
        /// </list></returns>
        public static int SafeCompare(DateTime left, DateTime right)
        {
            return left.SafeCompareTo(right);
        }
    }
}