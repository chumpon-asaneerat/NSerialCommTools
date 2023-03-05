#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2013-08-19
=================
- DecimalUtils class ported from NLib40x3 to NLib40x5.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace NLib.Utils
{
    /// <summary>
    /// Decimal Utils
    /// </summary>
    public class DecimalUtils
    {
        /// <summary>
        /// ConvertToString
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToString(decimal value)
        {
            string result = string.Empty;

            string str = value.ToString();
            if (str.Contains("."))
            {
                string output = string.Empty;
                int iPos = str.Length - 1;
                while (iPos > 0)
                {
                    if (str[iPos] == '0')
                    {
                        --iPos;
                    }
                    else if (str[iPos] == '.')
                    {
                        --iPos;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                output = str.Substring(0, iPos + 1);
                result = output;
            }
            else
            {
                result = str;
            }

            return result;
        }
        /// <summary>
        /// GetProperValue
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal GetProperValue(decimal value)
        {
            string dStr = ConvertToString(value);
            return decimal.Parse(dStr);
        }
    }
}
