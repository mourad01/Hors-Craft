// DecompilerFi decompiler from Assembly-CSharp.dll class: StringExtensions
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

public static class StringExtensions
{
	public static string ReplaceFirst(this string text, string search, string replace)
	{
		int num = text.IndexOf(search);
		if (num < 0)
		{
			return text;
		}
		return text.Substring(0, num) + replace + text.Substring(num + search.Length);
	}

	public static bool IsDate(this string value)
	{
		try
		{
			DateTime result;
			return DateTime.TryParse(value, out result);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsInt(this string value)
	{
		try
		{
			int result;
			return int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static string Take(this string value, int count, bool ellipsis = false)
	{
		int num = Math.Min(count, value.Length);
		return (!ellipsis || num >= value.Length) ? value.Substring(0, num) : $"{value.Substring(0, num)}...";
	}

	public static string Skip(this string value, int count)
	{
		return value.Substring(Math.Min(count, value.Length) - 1);
	}

	public static string Reverse(this string input)
	{
		char[] array = input.ToCharArray();
		Array.Reverse((Array)array);
		return new string(array);
	}

	public static bool IsNullOrEmpty(this string value)
	{
		return string.IsNullOrEmpty(value);
	}

	public static bool IsNOTNullOrEmpty(this string value)
	{
		return !string.IsNullOrEmpty(value);
	}

	public static string Formatted(this string format, params object[] args)
	{
		return string.Format(format, args);
	}

	public static string StripHtml(this string html)
	{
		if (html.IsNullOrEmpty())
		{
			return string.Empty;
		}
		return Regex.Replace(html, "<[^>]*>", string.Empty);
	}

	public static bool Match(this string value, string pattern)
	{
		return Regex.IsMatch(value, pattern);
	}

	public static string RemoveSpaces(this string value)
	{
		return value.Replace(" ", string.Empty);
	}

	public static string ReplaceRNWithBr(this string value)
	{
		return value.Replace("\r\n", "<br />").Replace("\n", "<br />");
	}

	public static string ToEmptyString(string value)
	{
		return (!string.IsNullOrEmpty(value)) ? value : string.Empty;
	}

	public static string ToStringPretty<T>(this IEnumerable<T> source)
	{
		return (source != null) ? source.ToStringPretty(",") : string.Empty;
	}

	public static string ToStringPretty<T>(this IEnumerable<T> source, string delimiter)
	{
		return (source != null) ? source.ToStringPretty(string.Empty, delimiter, string.Empty) : string.Empty;
	}

	public static string ToStringPretty<T>(this IEnumerable<T> source, string before, string delimiter, string after)
	{
		if (source == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(before);
		bool flag = true;
		foreach (T item in source)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				stringBuilder.Append(delimiter);
			}
			stringBuilder.Append(item.ToString());
		}
		stringBuilder.Append(after);
		return stringBuilder.ToString();
	}

	public static string InvertCase(this string s)
	{
		return new string((from c in s
			select (!char.IsLetter(c)) ? c : ((!char.IsUpper(c)) ? char.ToUpper(c) : char.ToLower(c))).ToArray());
	}

	public static bool IsNullOrEmptyAfterTrimmed(this string s)
	{
		return s.IsNullOrEmpty() || s.Trim().IsNullOrEmpty();
	}

	public static List<char> ToCharList(this string s)
	{
		return (!s.IsNOTNullOrEmpty()) ? null : s.ToCharArray().ToList();
	}

	public static string SubstringFromXToY(this string s, int start, int end)
	{
		if (s.IsNullOrEmpty())
		{
			return string.Empty;
		}
		if (start >= s.Length)
		{
			return string.Empty;
		}
		if (end >= s.Length)
		{
			end = s.Length - 1;
		}
		return s.Substring(start, end - start);
	}

	public static string RemoveChar(this string s, char c)
	{
		return (!s.IsNOTNullOrEmpty()) ? string.Empty : s.Replace(c.ToString(), string.Empty);
	}

	public static int GetWordCount(this string s)
	{
		return new Regex("\\w+").Matches(s).Count;
	}

	public static bool IsPalindrome(this string s)
	{
		return s.Equals(s.Reverse());
	}

	public static bool IsNotPalindrome(this string s)
	{
		return s.IsPalindrome().Toggle();
	}

	public static bool IsValidEmail(this string email)
	{
		if (string.IsNullOrEmpty(email) || email.Length > 100)
		{
			return false;
		}
		return new EmailValidator().IsValidEmail(email);
	}

	public static bool IsValidIPAddress(this string s)
	{
		return Regex.IsMatch(s, "\\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b");
	}

	public static string AppendSep(this string s, string sep)
	{
		return s + sep;
	}

	public static string AppendComma(this string s)
	{
		return s.AppendSep(",");
	}

	public static string AppendNewLine(this string s)
	{
		return s.AppendSep("\r\n");
	}

	public static string AppendHtmlBr(this string s)
	{
		return s.AppendSep("<br />");
	}

	public static string AppendSpace(this string s)
	{
		return s.AppendSep(" ");
	}

	public static string AppendHyphen(this string s)
	{
		return s.AppendSep("-");
	}

	public static string AppendSep(this string s, char sep)
	{
		return s.AppendSep(sep.ToString());
	}

	public static string AppendWithSep(this string s, string newString, string sep)
	{
		return s.AppendSep(sep) + newString;
	}

	public static string AppendWithSep(this string s, string newString, char sep)
	{
		return s.AppendSep(sep) + newString;
	}

	public static string AppendWithComma(this string s, string newString)
	{
		return s.AppendWithSep(newString, ",");
	}

	public static string AppendWithNewLine(this string s, string newString)
	{
		return s.AppendWithSep(newString, "\r\n");
	}

	public static string AppendWithHtmlBr(this string s, string newString)
	{
		return s.AppendWithSep(newString, "<br />");
	}

	public static string AppendWithHyphen(this string s, string newString)
	{
		return s.AppendWithSep(newString, "-");
	}

	public static string AppendWithSpace(this string s, string newString)
	{
		return s.AppendWithSep(newString, " ");
	}

	public static string CreateRandomPassword(this string sVal, int PasswordLength)
	{
		return sVal.CreateRandomPassword(PasswordLength, allowMixedCase: true, allowNumbers: true, allowSpecialCharacters: true, mixedCaseRequired: true, numberRequired: true, specialRequired: true, ignoreProgrammingCharacters: false);
	}

	public static string CreateRandomPassword(this string sVal, int PasswordLength, bool allowMixedCase, bool allowNumbers, bool allowSpecialCharacters)
	{
		return sVal.CreateRandomPassword(PasswordLength, allowMixedCase: true, allowNumbers: true, allowSpecialCharacters: true, mixedCaseRequired: false, numberRequired: false, specialRequired: false, ignoreProgrammingCharacters: false);
	}

	public static string CreateRandomPassword(this string sVal, int PasswordLength, bool ignoreProgrammingCharacters)
	{
		return sVal.CreateRandomPassword(PasswordLength, allowMixedCase: true, allowNumbers: true, allowSpecialCharacters: true, mixedCaseRequired: true, numberRequired: true, specialRequired: true, ignoreProgrammingCharacters);
	}

	public static string CreateRandomPassword(this string sVal, int PasswordLength, bool allowMixedCase, bool allowNumbers, bool allowSpecialCharacters, bool ignoreProgrammingCharacters)
	{
		return sVal.CreateRandomPassword(PasswordLength, allowMixedCase: true, allowNumbers: true, allowSpecialCharacters: true, mixedCaseRequired: false, numberRequired: false, specialRequired: false, ignoreProgrammingCharacters);
	}

	public static string CreateRandomPassword(this string sVal, int PasswordLength, bool allowMixedCase, bool allowNumbers, bool allowSpecialCharacters, bool mixedCaseRequired, bool numberRequired, bool specialRequired, bool ignoreProgrammingCharacters)
	{
		if (!allowMixedCase && mixedCaseRequired)
		{
			throw new ArgumentException("mixedCaseRequired cannot be true if allowMixedCase is false", "allowMixedCase");
		}
		if (!allowNumbers && numberRequired)
		{
			throw new ArgumentException("numberRequired cannot be true if allowNumbers is false", "allowNumbers");
		}
		if (!allowSpecialCharacters && specialRequired)
		{
			throw new ArgumentException("specialRequired cannot be true if allowSpecialCharacters is false", "allowSpecialCharacters");
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0.RandomRangeInclusive(PasswordLength);
		int num2;
		for (num2 = 1.RandomRangeInclusive(PasswordLength); num2 == num; num2 = 1.RandomRangeInclusive(PasswordLength))
		{
		}
		int num3 = 1.RandomRangeInclusive(PasswordLength);
		while (num3 == num || num3 == num2)
		{
			num3 = 1.RandomRangeInclusive(PasswordLength);
		}
		for (int i = 0; i < PasswordLength; i++)
		{
			if (mixedCaseRequired && i == num)
			{
				stringBuilder.Append(65.RandomRangeInclusive(90).ToChar());
				continue;
			}
			if (i == 0)
			{
				stringBuilder.Append(97.RandomRangeInclusive(122).ToChar());
				continue;
			}
			if (numberRequired && i == num2)
			{
				stringBuilder.Append(48.RandomRangeInclusive(57).ToChar());
				continue;
			}
			if (specialRequired && i == num3)
			{
				stringBuilder.Append(GetSpecialCode(ignoreProgrammingCharacters));
				continue;
			}
			bool flag = false;
			while (!flag)
			{
				switch (1.RandomRangeInclusive(4))
				{
				case 1:
					stringBuilder.Append(97.RandomRangeInclusive(123).ToChar());
					flag = true;
					break;
				case 2:
					if (allowMixedCase)
					{
						stringBuilder.Append(65.RandomRangeInclusive(91).ToChar());
						flag = true;
					}
					break;
				case 3:
					if (allowNumbers)
					{
						stringBuilder.Append(48.RandomRangeInclusive(58).ToChar());
						flag = true;
					}
					break;
				case 4:
					if (allowSpecialCharacters)
					{
						stringBuilder.Append(GetSpecialCode(ignoreProgrammingCharacters));
						flag = true;
					}
					break;
				}
			}
		}
		return stringBuilder.ToString().Trim();
	}

	private static char GetSpecialCode(bool ignoreProgrammingCharacters)
	{
		int num;
		do
		{
			num = 32.RandomRangeInclusive(65);
		}
		while ((num > 47 && num < 58) || (ignoreProgrammingCharacters && num.IsNotIn(37, 42, 43, 45, 61)));
		return num.ToChar();
	}

	public static string ToTitleCase(this string mText)
	{
		if (mText.IsNullOrEmpty())
		{
			return mText;
		}
		CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
		TextInfo textInfo = currentCulture.TextInfo;
		return textInfo.ToTitleCase(mText);
	}

	public static string SubsetString(this string s, string startText, string endText, bool ignoreCase)
	{
		if (s.IsNullOrEmpty())
		{
			return string.Empty;
		}
		if (startText.IsNullOrEmpty() || endText.IsNullOrEmpty())
		{
			throw new ArgumentException("Start Text and End Text cannot be empty.");
		}
		string text = (!ignoreCase) ? s : s.ToUpperInvariant();
		int num = (!ignoreCase) ? text.IndexOf(startText) : text.IndexOf(startText.ToUpperInvariant());
		int end = (!ignoreCase) ? text.IndexOf(endText, num) : text.IndexOf(endText.ToUpperInvariant(), num);
		return text.SubstringFromXToY(num, end);
	}

	public static string PadRightEx(this string s, int length)
	{
		if (!s.IsNullOrEmpty() && s.Length >= length)
		{
			return s;
		}
		return (s == null) ? new string(' ', length) : $"{s}{new string(' ', length - s.Length)}";
	}

	public static string ReplaceInsensitive(this string str, string oldValue, string newValue)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		int num2;
		for (num2 = str.IndexOf(oldValue, StringComparison.InvariantCultureIgnoreCase); num2 != -1; num2 = str.IndexOf(oldValue, num2, StringComparison.InvariantCultureIgnoreCase))
		{
			stringBuilder.Append(str.Substring(num, num2 - num));
			stringBuilder.Append(newValue);
			num2 += oldValue.Length;
			num = num2;
		}
		stringBuilder.Append(str.Substring(num));
		return stringBuilder.ToString();
	}

	public static List<T> SplitToList<T>(this string str, char delimeter, Func<string, T> parse)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		List<T> list = new List<T>();
		string[] array = str.Split(delimeter);
		string[] array2 = array;
		foreach (string arg in array2)
		{
			list.Add(parse(arg));
		}
		return list;
	}
}
