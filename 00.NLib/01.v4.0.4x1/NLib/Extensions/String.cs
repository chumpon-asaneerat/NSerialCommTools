#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2014-10-15
=================
- NLib Extension Methods for String added.
  - Add new Extenstion methods for work with String.
	- Add methods: IsNumeric, IsInteger, IsBool.

======================================================================================================================
Update 2013-01-01
=================
- NLib Extension Methods for String added.
  - Add new Extenstion methods for work with String.
	- Add method: IsNullOrWhiteSpace for check instance.
	- Add method: args for formating output.
	- Add method: eq for compare 2 strings.

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

#endregion

namespace NLib
{
	#region StringExtensionMethods

	/// <summary>
	/// The Extension methods for String.
	/// </summary>
	public static class StringExtensionMethods
	{
		#region IsNullOrWhiteSpace

		/// <summary>
		/// Indicates whether a specificed string is null or empty or only contains of 
		/// white space characters.
		/// </summary>
		/// <param name="value">The target string.</param>
		/// <returns>
		/// Returns true if specificed string is null or empty or contains only white space characters.
		/// </returns>
		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		#endregion

		#region args

		/// <summary>
		/// Replace arguments in format string.
		/// </summary>
		/// <param name="format">The format string.</param>
		/// <param name="args">The argument list.</param>
		/// <returns>
		/// Returns formated string that replace with assigned arguments.
		/// </returns>
		public static string args(this string format, params object[] args)
		{
			return string.Format(format, args);
		}

		#endregion

		#region eq

		/// <summary>
		/// Checks is string is same.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="target">The target string.</param>
		/// <param name="ignoreCase">True for ignore case sensitive. Default is true.</param>
		/// <returns>Returns true if string is same.</returns>
		public static bool eq(this string source, string target, bool ignoreCase = true)
		{
			return (string.Compare(source, target, ignoreCase) == 0);
		}

		#endregion

		#region IsNumber

		/// <summary>
		/// Checks string is Number.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <returns>true if value can convert to number.</returns>
		public static bool IsNumber(string value)
		{
			return Utils.StringUtils.IsNumber(value);
		}
		/// <summary>
		/// Checks string is Number.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="style">specificed numeric style.</param>
		/// <returns>true if value can convert to number.</returns>
		public static bool IsNumber(this string value, NumberStyles style)
		{
			return Utils.StringUtils.IsNumber(value, style);
		}

		#endregion

		#region IsInteger

		/// <summary>
		/// Checks string is Integer.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <returns>true if value can convert to Integer.</returns>
		public static bool IsInteger(this string value)
		{
			return Utils.StringUtils.IsInteger(value);
		}
		/// <summary>
		/// Checks string is Integer.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="style">specificed numeric style.</param>
		/// <returns>true if value can convert to Integer.</returns>
		public static bool IsInteger(this string value, NumberStyles style)
		{
			return Utils.StringUtils.IsInteger(value, style);
		}

		#endregion

		#region IsBool

		/// <summary>
		/// Checks string is boolean.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <returns>true if value can convert to Boolean.</returns>
		public static bool IsBool(this string value)
		{
			return Utils.StringUtils.IsBool(value);
		}
		/// <summary>
		/// Checks string is boolean.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="trueValues">value array for true value checking.</param>
		/// <param name="falseValues">value array for false value checking.</param>
		/// <returns>true if value can convert to Boolean.</returns>
		public static bool IsBool(string value,
			string[] trueValues = null, string[] falseValues = null)
		{
			return Utils.StringUtils.IsBool(value, trueValues, falseValues);
		}

		#endregion
	}

	#endregion
}
