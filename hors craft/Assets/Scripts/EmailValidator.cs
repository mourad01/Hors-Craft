// DecompilerFi decompiler from Assembly-CSharp.dll class: EmailValidator
using System;
using System.Globalization;
using System.Text.RegularExpressions;

internal class EmailValidator
{
	private bool invalid;

	public bool IsValidEmail(string strIn)
	{
		invalid = false;
		if (string.IsNullOrEmpty(strIn))
		{
			return false;
		}
		strIn = Regex.Replace(strIn, "(@)(.+)$", DomainMapper, RegexOptions.None);
		if (invalid)
		{
			return false;
		}
		return Regex.IsMatch(strIn, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);
	}

	private string DomainMapper(Match match)
	{
		IdnMapping idnMapping = new IdnMapping();
		string text = match.Groups[2].Value;
		try
		{
			text = idnMapping.GetAscii(text);
		}
		catch (ArgumentException)
		{
			invalid = true;
		}
		return match.Groups[1].Value + text;
	}
}
