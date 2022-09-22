// DecompilerFi decompiler from Assembly-CSharp.dll class: XMLExtensions
using System.Xml.Linq;

public static class XMLExtensions
{
	public static string GetValueOrNull(this XElement element)
	{
		return element?.Value;
	}

	public static string GetValueString(this XElement element)
	{
		return (element == null) ? string.Empty : element.Value;
	}

	public static decimal? ValueToDecimalNullable(this XElement element)
	{
		return element?.Value.ToDecimalNull();
	}

	public static int? ValueToIntNullable(this XElement element)
	{
		return element?.Value.ToIntNull();
	}

	public static long? ValueToLongNullable(this XElement element)
	{
		return element?.Value.ToLongNull();
	}

	public static float? ValueToFloatNullable(this XElement element)
	{
		return element?.Value.ToFloatNull();
	}
}
