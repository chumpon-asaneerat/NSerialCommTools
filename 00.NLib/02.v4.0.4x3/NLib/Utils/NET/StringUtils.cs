#region History
#if HISTORY_COMMENT

// <[History]> 
Update 2015-06-17
=================
- String Utils class updated.
  - Change debug code back to using MethodBase instead of StackFrame.

======================================================================================================================
Update 2013-08-19
=================
- String Utils class updated.
  - Refactor code by change ArrayList ot List<T>.
  - Reformat code for more readable.
  - Change debug manager to new debug framework.

======================================================================================================================
Update 2013-08-19
=================
- String Utils class ported.
  - Required to change ArrayList to List<T>.
  - Required to re-designed and optimized.

======================================================================================================================
Update 2012-12-19
=================
- String Utils class changed.
  - Change call DebugManager call to Extension method call.

======================================================================================================================
Update 2011-11-07
=================
- String Utils ported from GFA40 to NLib.
  - Remove all ExceptionMode code.

======================================================================================================================
Update 2010-08-31
=================
- String Utils ported from GFA38v3 to GFA40 and GFA20.

======================================================================================================================
Update 2010-01-24
=================
- String Utilities ported from GFA Library GFA37 tor GFA38v3
  - Changed all exception handling code to used new ExceptionManager class instread of 
	LogManager.

======================================================================================================================
Update 2008-11-27
=================
- String Utilities move from GFA.Lib to GFA.Lib.Core.

======================================================================================================================
Update 2007-11-26
=================
- String Utilities provides support classes with some fixed for multithread environment and optimized
  for performance

======================================================================================================================
// </[History]>

#endif
#endregion

#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

#endregion

namespace NLib.Utils
{
	#region Radix

	/// <summary>
	/// Radix Class. This class used to change numeric base in String Utils class
	/// </summary>
	public class Radix
	{
		#region Change Base

		/// <summary>
		/// Change Base
		/// </summary>
		/// <param name="value">Value to change base</param>
		/// <param name="baseVal">target base value</param>
		/// <returns>array of value in target base</returns>
		public static uint[] ChangeBase(uint value, uint baseVal)
		{
			if (value == Convert.ToUInt32(0))
				return new uint[] { Convert.ToUInt32(0) };

			#region Get Number of Digit

			uint val = Convert.ToUInt32((value / baseVal));

			int digitSize = 0;
			if (value > 0)
				digitSize++; // check is single digit

			while (val > 0)
			{
				val = Convert.ToUInt32((val / baseVal));

				digitSize++;
			}

			#endregion

			#region Calc Value for each digit

			uint[] results = new uint[digitSize];

			val = value;
			for (int i = 0; i < digitSize - 1; i++)
			{
				int iDigit = digitSize - i - 1;
				uint digitBase = Convert.ToUInt32(Math.Pow(baseVal, iDigit));
				uint digitVal = Convert.ToUInt32((val / digitBase));
				results[i] = digitVal;
				val = val - (digitVal * digitBase);
			}

			results[digitSize - 1] = val; // rest value

			#endregion

			return results;
		}

		#endregion
	}

	#endregion

	#region Number To Thai

	/// <summary>
	/// Number To Thai
	/// </summary>
	internal class NumberToThai
	{
		private List<string> _unit = null;

		/// <summary>
		/// Constructor
		/// </summary>
		private NumberToThai()
		{
			InitUnit();
		}
		/// <summary>
		/// Destructor
		/// </summary>
		~NumberToThai()
		{
			// Free Memory
			NGC.FreeGC(this);
		}

		#region Init Unit / Digit

		private void InitUnit()
		{
			_unit = new List<string>();
			_unit.Add("");
			_unit.Add("สิบ");
			_unit.Add("ร้อย");
			_unit.Add("พัน");
			_unit.Add("หมื่น");
			_unit.Add("แสน");
		}

		#endregion

		#region GetStringValue

		private string GetStringValue(string digit, int index, bool SingleDigit)
		{
			string result = "";

			if (digit == "0")
			{
				if (SingleDigit)
				{
					result += "ศูนย์";
				}
			}
			else if (digit == "1")
			{
				if (index == 0)
				{
					if (SingleDigit)
					{
						result += "หนึ่ง";
					}
					else
					{
						result += "เอ็ด";
					}
				}
				else if (index == 1)
				{
					// สิบ
				}
				else
				{
					result += "หนึ่ง";
				}
				result += _unit[index];

			}
			else if (digit == "2")
			{
				if (index == 1)
				{
					result += "ยี่";
				}
				else
				{
					result += "สอง";
				}
				result += _unit[index];
			}
			else if (digit == "3")
			{
				result += "สาม" + _unit[index];
			}
			else if (digit == "4")
			{
				result += "สี่" + _unit[index];
			}
			else if (digit == "5")
			{
				result += "ห้า" + _unit[index];
			}
			else if (digit == "6")
			{
				result += "หก" + _unit[index];
			}
			else if (digit == "7")
			{
				result += "เจ็ด" + _unit[index];
			}
			else if (digit == "8")
			{
				result += "แปด" + _unit[index];
			}
			else if (digit == "9")
			{
				result += "เก้า" + _unit[index];
			}
			return result;
		}

		#endregion

		#region Parse

		/// <summary>
		/// Parse to string.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		/// <returns></returns>
		public static string Parse(decimal value)
		{
			return Parse(value.ToString());
		}
		/// <summary>
		/// Parse to string.
		/// </summary>
		/// <param name="Input">The value to parse.</param>
		/// <returns></returns>
		public static string Parse(string Input)
		{
			NumberToThai tcurr = new NumberToThai();
			string result = "";
			string txt = Input.Trim();

			// Remove Comma (,)
			txt = Input.Replace(",", "");

			// Remove Space
			txt = txt.Replace(" ", "");

			// Check Dot at starting and ending point
			if (txt.EndsWith("."))
			{
				txt = txt.Substring(0, txt.Length - 1);
			}
			if (txt.StartsWith("."))
			{
				txt = "0" + txt;
			}

			if (!tcurr.IsCorrectFormat(txt))
			{
				return "Incorrect Format";
			}

			string[] spByDot = tcurr.SplitByDot(txt);
			string leftDot = spByDot[0];

			result += tcurr.Convert(leftDot);
			result += "บาท";

			if (spByDot.Length > 1)
			{
				string rightDot = spByDot[1];
				if (rightDot == "0" || rightDot == "00")
				{
					result += "ถ้วน";
				}
				else
				{
					result += tcurr.Convert(rightDot) + "สตางค์";
				}
			}
			else result += "ถ้วน";

			return result;
		}
		#endregion

		#region Convert

		private string Convert(string Input)
		{
			string result = "";
			string temp = Reverse(Input);
			string[] arr = Split6(temp);
			for (int count = arr.Length - 1; count >= 0; count--)
			{
				bool singleDigit = arr[count].Length == 1;
				for (int c = arr[count].Length - 1; c >= 0; c--)
				{
					result += GetStringValue(arr[count][c].ToString(), c, singleDigit);
				}
				if (count > 0)
				{
					result += "ล้าน";
				}
			}
			return result;
		}

		#endregion

		#region IsCorrectFormat

		private bool IsCorrectFormat(string Input)
		{
			for (int c = 0; c <= Input.Length - 1; c++)
			{
				bool isDigit = char.IsDigit(Input[c]);
				bool isDot = Input[c] == '.';
				if (!(isDigit || isDot))
				{
					return false;
				}
			}
			// There is more than 1 dot
			if (Input.IndexOf(".") != Input.LastIndexOf("."))
			{
				return false;
			}
			// There are more than 2 digits after dot
			int i = Input.LastIndexOf(".");
			if (i != -1)
			{
				if (i + 3 < Input.Length)
				{
					return false;
				}
			}

			return true;
		}
		#endregion

		#region Reverse

		private string Reverse(string Input)
		{
			string result = "";
			for (int c = Input.Length - 1; c >= 0; c--)
			{
				result += Input[c];
			}
			return result;
		}

		#endregion

		#region SplitByDot

		private string[] SplitByDot(string Input)
		{
			List<string> result = new List<string>();
			int indexDot = Input.IndexOf(".");

			if (indexDot == -1)
				result.Add(Input);
			else
			{
				result.Add(Input.Substring(0, indexDot));
				result.Add(Input.Substring(indexDot + 1));
			}
			return result.ToArray();
		}

		#endregion

		#region Split6

		private string[] Split6(string Input)
		{
			List<string> result = new List<string>();
			int startIndex = 0;
			int lastIndex = Input.Length - 1;
			int len = 6;

			while (startIndex <= lastIndex)
			{
				if (lastIndex - startIndex >= len - 1)
				{
					result.Add(Input.Substring(startIndex, len));
				}
				else
				{
					result.Add(Input.Substring(startIndex));
				}
				startIndex += len;
			}

			return result.ToArray();
		}

		#endregion

	}

	#endregion

	#region String Utils

	/// <summary>
	/// String Utils
	/// </summary>
	public class StringUtils
	{
		#region Const Table

		private const string AsciiTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string HexTable = "0123456789ABCDEF";

		#endregion

		#region Is Number

		/// <summary>
		/// Checks string is Number.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <returns>true if value can convert to number.</returns>
		public static bool IsNumber(string value)
		{
			if (value.Trim().Length <= 0) return false;

			NumberStyles style = NumberStyles.Number | NumberStyles.AllowThousands;

			return IsNumber(value, style);
		}
		/// <summary>
		/// Checks string is Number.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="style">specificed numeric style.</param>
		/// <returns>true if value can convert to number.</returns>
		public static bool IsNumber(string value, NumberStyles style)
		{
			if (value.Trim().Length <= 0) return false;

			Double result = Double.MinValue;
			return Double.TryParse(value, style, 
				CultureInfo.InvariantCulture, out result);
		}

		#endregion

		#region Is Integer

		/// <summary>
		/// Checks string is Integer.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <returns>true if value can convert to Integer.</returns>
		public static bool IsInteger(string value)
		{
			if (value.Trim().Length <= 0) return false;

			NumberStyles style = NumberStyles.Integer | NumberStyles.AllowThousands;

			return IsInteger(value, style);
		}
		/// <summary>
		/// Checks string is Integer.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <param name="style">specificed numeric style.</param>
		/// <returns>true if value can convert to Integer.</returns>
		public static bool IsInteger(string value, System.Globalization.NumberStyles style)
		{
			if (value.Trim().Length <= 0) return false;

			Double result = Double.MinValue;
			return Double.TryParse(value, style, 
				CultureInfo.InvariantCulture, out result);
		}

		#endregion

		#region Is Boolean

		/// <summary>
		/// Checks string is boolean.
		/// </summary>
		/// <param name="value">Value to check.</param>
		/// <returns>true if value can convert to Boolean.</returns>
		public static bool IsBool(string value)
		{
			if (value.Trim().Length <= 0) return false;
			string[] trueVals = new string[]
				{
					"true",
					"t",
					"1",
					"-1",
					"y",
					"yes",
					"on",
					"p"
				};

			string[] falseVals = new string[]
				{
					"false",
					"f",
					"0",
					"n",
					"no",
					"off",
					"o"
				};
			return IsBool(value, trueVals, falseVals);
		}
		/// <summary>
		/// Checks string is boolean.
		/// </summary>
		/// <param name="value">Value to check</param>
		/// <param name="trueValues">value array for true value checking.</param>
		/// <param name="falseValues">value array for false value checking.</param>
		/// <returns>true if value can convert to Boolean.</returns>
		public static bool IsBool(string value, string[] trueValues, string[] falseValues)
		{
			string checkVal = value.Trim().ToLower();

			string[] chkTrueVals = null;
			string[] chkFalseVals = null;
			List<string> list = new List<string>();
			string val = "";

			if (trueValues != null && trueValues.Length > 0)
			{
				for (int i = 0; i < trueValues.Length; i++)
				{
					val = trueValues[i].Trim().ToLower();
					if (val.Length > 0)
					{
						list.Add(val);
					}
				}

				chkTrueVals = list.ToArray();
			}

			list.Clear();

			if (falseValues != null && falseValues.Length > 0)
			{
				for (int i = 0; i < falseValues.Length; i++)
				{
					val = falseValues[i].Trim().ToLower();
					if (val.Length > 0) list.Add(val);
				}

				chkFalseVals = list.ToArray();
			}

			list.Clear();
			list = null;

			if (chkTrueVals != null)
			{
				foreach (string trueVal in chkTrueVals)
				{
					if (trueVal == checkVal) return true;
				}
			}

			if (chkFalseVals != null)
			{
				foreach (string falseVal in chkFalseVals)
				{
					if (falseVal == checkVal) return true;
				}
			}

			return false; // not match
		}

		#endregion

		#region CanConvertTo

		/// <summary>
		/// Checks can convert to specificed type.
		/// </summary>
		/// <param name="value">value to convert</param>
		/// <param name="targetType">target type</param>
		/// <returns>true if value can convert to target type. otherwise return false</returns>
		public static bool CanConvertTo(object value, Type targetType)
		{
			if (value == null || value == DBNull.Value)
				return false;

			MethodBase med = MethodBase.GetCurrentMethod();
			object obj;

			if (targetType == typeof(int) ||
				targetType == typeof(UInt32) ||
				targetType == typeof(Int16) ||
				targetType == typeof(UInt16) ||
				targetType == typeof(UInt64) ||
				targetType == typeof(UInt64))
			{
				try
				{
					return IsInteger(value.ToString());
				}
				catch (Exception ex)
				{
					ex.Err(med);
					return false;
				}
			}
			else if (targetType == typeof(bool))
			{
				try
				{
					return IsBool(value.ToString());
				}
				catch (Exception ex)
				{
					ex.Err(med);
					return false;
				}
			}
			else if (targetType == typeof(Decimal) ||
				targetType == typeof(Double))
			{
				try
				{
					return IsNumber(value.ToString());
				}
				catch (Exception ex)
				{
					ex.Err(med);
					return false;
				}
			}
			else
			{
				try
				{
					obj = Convert.ChangeType(value, targetType);
				}
				catch (Exception ex)
				{
					ex.Err(med);
					obj = null;
				}
				return (obj != null);
			}
		}

		#endregion

		#region ToExcelName

		/// <summary>
		/// ToExcelName
		/// </summary>
		/// <param name="index">index to convert</param>
		/// <returns>Converted String</returns>
		public static string ToExcelName(int index)
		{
			string result = "";
			uint[] indexes = Radix.ChangeBase(Convert.ToUInt32(index), 26);

			for (int i = 0; i < indexes.Length; i++)
			{
				if (i == 0 && indexes.Length > 1)
					result += AsciiTable[(int)(indexes[i] - 1)].ToString();
				else
					result += AsciiTable[(int)indexes[i]].ToString();
			}

			return result;
		}

		#endregion

		#region ToHex

		/// <summary>
		/// ToHex
		/// </summary>
		/// <param name="index">index to convert</param>
		/// <returns>Converted String</returns>
		public static string ToHex(int index)
		{
			string result = "";
			uint[] indexes = Radix.ChangeBase(Convert.ToUInt32(index), 16);

			for (int i = 0; i < indexes.Length; i++)
			{
				result += HexTable[(int)indexes[i]].ToString();
			}

			return result;
		}

		#endregion

		#region Concat

		/// <summary>
		/// Concat object array to string
		/// </summary>
		/// <param name="delimeter">Delimeter that need to append between each object</param>
		/// <param name="values">object array</param>
		/// <returns>string that concat from all object in specificed parameters</returns>
		public static string Concat(string delimeter, params object[] values)
		{
			if (delimeter.Length <= 0) return string.Concat(values); // used default method
			else
			{
				if (values == null || values.Length <= 0) return ""; // no element
				else
				{
					int newSize = values.Length * 2;
					object[] results = new object[newSize];
					object[] targets = new object[newSize - 1];
					for (int iCnt = 0, iObj = 0; iCnt < newSize; iCnt += 2, iObj++)
					{
						results[iCnt] = values[iObj];
						results[iCnt + 1] = delimeter;
					}
					Array.Copy(results, targets, newSize - 1);
					return string.Concat(targets);
				}
			}
		}

		#endregion

		#region Parse

		#region ToThaiCurrency

		/// <summary>
		/// To Thai Currency.
		/// </summary>
		/// <param name="value">Numeric Value to parse</param>
		/// <returns>Thai Currency string.</returns>
		public static string ToThaiCurrency(string value)
		{
			return NumberToThai.Parse(value);
		}
		/// <summary>
		/// To Thai Currency.
		/// </summary>
		/// <param name="value">Decimal Value to parse</param>
		/// <returns>Thai Currency string.</returns>
		public static string ToThaiCurrency(int value)
		{
			return ToThaiCurrency(value.ToString());
		}
		/// <summary>
		/// To Thai Currency.
		/// </summary>
		/// <param name="value">Decimal Value to parse</param>
		/// <returns>Thai Currency string.</returns>
		public static string ToThaiCurrency(decimal value)
		{
			return ToThaiCurrency(value.ToString());
		}

		#endregion

		#endregion
	}

	#endregion
}
