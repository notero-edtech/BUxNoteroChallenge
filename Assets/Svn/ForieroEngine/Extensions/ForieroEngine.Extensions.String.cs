using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{

		static char apostrophe = '\u0027';
		static List<char> apostrophes = new List<char>() { '\u0027', '\u2019', '\u2018', '\u201b' };

		public static string UnifyApostrophes(this string s)
        {
			var r = s;

            foreach(var ch in apostrophes)
            {
				r = r.Replace(ch, apostrophe);
            }

			return r;
        }

		public static string UnifyCarriageReturnCR(this string s) => s.Replace("\r\n", "\r").Replace("\n", "\r");
		public static string UnifyLineFeedLF(this string s) => s.Replace("\r\n", "\n").Replace("\r", "\n");
		public static string UnifyCarriageReturnCRLineFeedLF(this string s) => s.UnifyLineFeedLF().Replace("\n", "\r\n");

        public static char UnifyApostrophe(this char ch)
        {
			return apostrophes.Contains(ch) ? apostrophe : ch;
        }

        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length) return source;
            return source.Substring(source.Length - tail_length);
        }

        public static bool IsNullOrEmpty(this string s)
		{
			return string.IsNullOrEmpty(s);
		}

		public static string DumpCharType(this char aChar)
		{
			char c = aChar;
			if (char.IsDigit(c))
			{
				return ("Char " + c + " is digit");
			}
			else if (char.IsNumber(c))
			{
				return ("Char " + c + " is number");
			}
			else if (char.IsSeparator(c))
			{
				return ("Char " + c + " is separator");
			}
			else if (char.IsSymbol(c))
			{
				return ("Char " + c + " is symbol");
			}
			else if (char.IsControl(c))
			{
				return ("Char " + c + " is control");
			}
			else if (char.IsLetter(c))
			{
				return ("Char " + c + " is letter");
			}
			else if (char.IsPunctuation(c))
			{
				return ("Char " + c + " is punctuation");
			}
			else if (char.IsSurrogate(c))
			{
				return ("Char " + c + " is surrogate");
			}
			else if (char.IsWhiteSpace(c))
			{
				return ("Char " + c + " is whitespace");
			}

			return "Char Type Not Found";
		}

#if !UNITY_WSA

		public static string EncodeTo64(this string toEncode)
		{
			byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(toEncode);
			string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
			return returnValue;
		}

		public static string DecodeFrom64(this string encodedData)
		{
			byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
			string returnValue = Encoding.UTF8.GetString(encodedDataAsBytes);
			return returnValue;
		}

		public static MemoryStream ToASCIIStream(this string s)
		{
			byte[] byteArray = Encoding.ASCII.GetBytes(s);
			return new MemoryStream(byteArray);
		}

		public static MemoryStream ToUTF8Stream(this string s)
		{
			byte[] byteArray = Encoding.UTF8.GetBytes(s);
			return new MemoryStream(byteArray);
		}

#endif

#if !UNITY_WSA
		static public void SaveToTxt(this string aString, string aFilePath, System.Text.Encoding anEncoding)
		{
			if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(aFilePath)))
			{
				System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(aFilePath));
			}
			using (TextWriter tw = new StreamWriter(aFilePath, false, anEncoding))
			{
				;
				tw.Write(aString);
				tw.Close();
			}
		}

		static public string LoadFromTxt(this string astring, string aFilePath, System.Text.Encoding anEncoding)
		{
			string result = "";
			if (System.IO.File.Exists(aFilePath))
			{
				using (TextReader tr = new StreamReader(aFilePath, anEncoding))
				{
					;
					result = tr.ReadToEnd();
				}
			}
			else
			{
				Debug.LogError("LoadFromTxt file not found : " + aFilePath);
			}
			return result;
		}
#endif

		public static string CamelToWords(this string value)
		{
			if (value != null && value.Length > 0)
			{
				var result = new List<char>();
				char[] array = value.ToCharArray();
				//bool lastWasNumber = false;
				foreach (var item in array)
				{
					if (char.IsUpper(item))
					{
						//lastWasNumber = false;
						result.Add(' ');
						result.Add(item);
					}
					else if (char.IsNumber(item))
					{
						//lastWasNumber = true;
						result.Add(' ');
						result.Add(item);
					}
					else
					{
						//lastWasNumber = false;
						result.Add(item);
					}
				}

				return new string(result.ToArray());
			}
			return value;
		}

		public static string RemoveDiacritics(this string stIn)
		{
			string stFormD = stIn.Normalize(NormalizationForm.FormD);
			StringBuilder sb = new StringBuilder();

			for (int ich = 0; ich < stFormD.Length; ich++)
			{
				switch (CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]))
				{
					case UnicodeCategory.NonSpacingMark:
					case UnicodeCategory.SpacingCombiningMark:
					case UnicodeCategory.EnclosingMark:
						break;
					default:
						sb.Append(stFormD[ich]);
						break;
				}
			}

			return (sb.ToString().Normalize(NormalizationForm.FormC));
		}

		public static char GetAccent(this string stIn)
		{
			string stFormD = stIn.Normalize(NormalizationForm.FormD);

			StringBuilder sb = new StringBuilder();

			for (int ich = 0; ich < stFormD.Length; ich++)
			{
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
				if (uc == UnicodeCategory.NonSpacingMark)
				{
					sb.Append(stFormD[ich]);
				}
			}

			if (sb.Length > 0)
			{
				return sb.ToString().Normalize(NormalizationForm.FormC)[0];
			}
			else
			{
				return default(char);
			}
		}

		public static bool IsDiacriticsed(this string stIn)
		{
			string stFormD = stIn.Normalize(NormalizationForm.FormD);

			for (int ich = 0; ich < stFormD.Length; ich++)
			{
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
				if (uc == UnicodeCategory.NonSpacingMark)
				{
					return true;
				}
			}

			return false;
		}

		public static string FixNewLine(this string s) => s.Replace("\r", "\n").Replace(((char)3).ToString(), "\n");
		public static string RemoveNewLine(this string s) => s.Replace("\r", "").Replace("\n", "").Replace(((char) 3).ToString(), "");
		

		/// <summary>
		/// Remove HTML from string with Regex.
		/// </summary>
		public static string StripTagsRegex(this string source)
		{
			return Regex.Replace(source, "<.*?>", string.Empty);
		}

		/// <summary>
		/// Compiled regular expression for performance.
		/// </summary>
		//static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

		/// <summary>
		/// Remove HTML from string with compiled Regex.
		/// </summary>
		//public static string StripTagsRegexCompiled(this string source)
		//{
		//	return _htmlRegex.Replace(source, string.Empty);
		//}

		/// <summary>
		/// Remove HTML tags from string using char array.
		/// </summary>
		public static string StripTagsCharArray(this string source)
		{
			char[] array = new char[source.Length];
			int arrayIndex = 0;
			bool inside = false;

			for (int i = 0; i < source.Length; i++)
			{
				char let = source[i];
				if (let == '<')
				{
					inside = true;
					continue;
				}
				if (let == '>')
				{
					inside = false;
					continue;
				}
				if (!inside)
				{
					array[arrayIndex] = let;
					arrayIndex++;
				}
			}
			return new string(array, 0, arrayIndex);
		}

		public static string[] Split(this string s, string separator)
		{
			return s.Split(new string[] { separator }, System.StringSplitOptions.None);
		}

		public static int OccurenceCount(this string str, string val)
		{
			int occurrences = 0;
			int startingIndex = 0;

			while ((startingIndex = str.IndexOf(val, startingIndex)) >= 0)
			{
				++occurrences;
				++startingIndex;
			}

			return occurrences;
		}

		public static int NthIndexOf(this string target, string value, int n)
		{

			string[] result = target.Split(value);
			n--;
			if (n >= 0 && n < result.Length)
			{
				int index = 0;
				for (int i = 0; i <= n; i++)
				{
					index += result[i].Length + value.Length;
				}
				return index - value.Length;
			}
			else
			{
				return -1;
			}
		}

		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			return source.IndexOf(toCheck, comp) >= 0;
		}
	}
}
