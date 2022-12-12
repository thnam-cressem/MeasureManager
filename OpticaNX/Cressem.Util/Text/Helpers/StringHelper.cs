using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cressem.Util.Text
{
	public static class StringHelper
	{
		#region Validation

		/// <summary>
		/// Checks if a character is in Hangul
		/// </summary>
		/// <param name="c">Character to check</param>
		/// <returns>True if Hangul, otherwise false</returns>
		private static bool IsHangul(char c)
		{
			if ((c >= '\xAC00' && c <= '\xD7AF') || (c >= '\x3130' && c <= '\x318F'))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks if a string ID contains only alphanumeric, underscore, and dash characters.
		/// </summary>
		/// <param name="id">ID to check</param>
		/// <returns>True if valid, otherwise false</returns>
		public static bool ValidateStringID(string id)
		{
			Regex regex = new Regex("^[a-zA-Z0-9_-]*$");

			if (regex.IsMatch(id))
				return true;

			return false;
		}

		/// <summary>
		/// Checks if a string contains only alphanumeric charachers.
		/// </summary>
		/// <param name="txt">String to check</param>
		/// <returns><c>true</c> if valid, otherwise <c>false</c></returns>
		public static bool IsAlphanumeric(string txt)
		{
			Regex regex = new Regex("^[a-zA-Z0-9]*$");

			if (regex.IsMatch(txt))
				return true;

			return false;
		}

		/// <summary>
		/// Checks if a string contains only alphabet charachers.
		/// </summary>
		/// <param name="txt">String to check</param>
		/// <returns><c>true</c> if valid, otherwise <c>false</c></returns>
		public static bool IsAlphabet(string txt)
		{
			Regex regex = new Regex("^[a-zA-Z]*$");

			if (regex.IsMatch(txt))
				return true;

			return false;
		}

		/// <summary>
		/// Indicates whether the input string can convert to the given type or not
		/// </summary>
		/// <param name="input">Target input string, culture dependent</param>
		/// <param name="type">Type to convert</param>
		/// <returns>True if possible, otherwise false</returns>
		public static bool Is(string input, Type type)
		{
			return Is(input, type, false, true);
		}

		/// <summary>
		/// Indicates whether the input string can convert to the given type or not
		/// </summary>
		/// <param name="input">Target input string</param>
		/// <param name="type">Type to convert</param>
		/// <param name="isInvariantString">True if input is invariant string in culture</param>
		/// <returns>True if possible, otherwise false</returns>
		public static bool Is(string input, Type type, bool isInvariantString, bool isNullable)
		{
			try
			{
				// not null필드이고, input값이 없으면 false를 반환한다.
				if (isNullable == false && String.IsNullOrEmpty(input))
					return false;

				// Identify a nullable data
				if (String.IsNullOrEmpty(input))
				{
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
						return true;
				}

				// Identify boolean data
				if (type == typeof(bool) || type == typeof(Nullable<bool>))
				{
					if (String.IsNullOrEmpty(input))
						return false;
					else if (input == "0" || input == "1" || input.ToLower() == "true" || input.ToLower() == "false")
						return true;
				}

				if (isInvariantString)
					TypeDescriptor.GetConverter(type).ConvertFromInvariantString(input);
				else
					TypeDescriptor.GetConverter(type).ConvertFromString(input);

				return true;
			}
			catch
			{
				return false;
			}
		}

		#endregion

		/// <summary>
		/// Removes white spaces in the specified text
		/// </summary>
		/// <param name="text">String to remove white space</param>
		/// <returns>String without any white space</returns>
		public static string RemoveSpace(string text)
		{
			if (text == null)
				return text;

			return text.Replace(" ", String.Empty);
		}

		/// <summary>
		/// Removes alphabet in the specified text
		/// </summary>
		/// <param name="text">String to remove alphabet characters</param>
		/// <returns>String without any alphabet character</returns>
		public static string RemoveAlphabet(string text)
		{
			if (String.IsNullOrEmpty(text))
				return text;

			string newTxt = String.Empty;

			for (int i = 0; i < text.Length; i++)
			{
				if (IsAlphabet(text[i].ToString()) == false)
					newTxt += text[i];
			}

			return newTxt;
		}

		/// <summary>
		/// Extension method to truncate a string value
		/// </summary>
		/// <param name="value">String value</param>
		/// <param name="maxLength">Max length to trucate</param>
		/// <returns>Truncated string</returns>
		public static string Truncate(this string value, int maxLength)
		{
			if (String.IsNullOrEmpty(value) || maxLength <= 0)
				return value;

			return value.Length <= maxLength ? value : value.Substring(0, maxLength);
		}
	}
}
