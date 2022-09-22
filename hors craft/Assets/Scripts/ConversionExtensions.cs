// DecompilerFi decompiler from Assembly-CSharp.dll class: ConversionExtensions
using System;
using System.Globalization;

public static class ConversionExtensions
{
	public static float ToFloat(this int value)
	{
		return value;
	}

	public static decimal ToDecimal(this int value)
	{
		return value;
	}

	public static char ToChar(this int value)
	{
		return Convert.ToChar(value);
	}

	public static int ToInt(this string value, int defaultValue)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		int result;
		return (!int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? defaultValue : result;
	}

	public static int? ToIntNull(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		int result;
		return (!int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? null : new int?(result);
	}

	public static long ToLong(this string value, long defaultValue)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		long result;
		return (!long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? defaultValue : result;
	}

	public static long? ToLongNull(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		long result;
		return (!long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? null : new long?(result);
	}

	public static decimal ToDecimal(this string value, decimal defaultValue)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		decimal result;
		return (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? defaultValue : result;
	}

	public static double ToDouble(this string value, double defaultValue)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		double result;
		return (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? defaultValue : result;
	}

	public static decimal? ToDecimalNull(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		decimal result;
		return (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? null : new decimal?(result);
	}

	public static float ToFloat(this string value, float defaultValue)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		float result;
		return (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? defaultValue : result;
	}

	public static float? ToFloatNull(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		float result;
		return (!float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? null : new float?(result);
	}

	public static bool ToBool(this string value, bool defaultValue)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		bool result;
		return (!bool.TryParse(value, out result)) ? defaultValue : result;
	}

	public static bool? ToBoolNull(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		bool result;
		return (!bool.TryParse(value, out result)) ? null : new bool?(result);
	}

	public static DateTime ToDateTime(this string value, DateTime defaultValue)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		DateTime result;
		return (!DateTime.TryParse(value, out result)) ? defaultValue : result;
	}

	public static DateTime? ToDateTimeNull(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		DateTime result;
		return (!DateTime.TryParse(value, out result)) ? null : new DateTime?(result);
	}

	public static Guid? ToGuid(this string gString)
	{
		try
		{
			return new Guid(gString);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static double? ToDoubleNullable(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		double result;
		return (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? null : new double?(result);
	}

	public static decimal? ToDecimalNullable(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		decimal result;
		return (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? null : new decimal?(result);
	}

	public static int? ToIntNullable(this string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		int result;
		return (!int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) ? null : new int?(result);
	}

	public static int ToInt(this byte value)
	{
		return Convert.ToInt32(value);
	}

	public static long ToLong(this byte value)
	{
		return Convert.ToInt64(value);
	}

	public static double ToDouble(this byte value)
	{
		return Convert.ToDouble(value);
	}

	public static string ToBase64(this byte[] data)
	{
		return Convert.ToBase64String(data);
	}

	public static byte[] FromBase64(this string data)
	{
		return Convert.FromBase64String(data);
	}

	public static T To<T>(this IConvertible obj)
	{
		return (T)Convert.ChangeType(obj, typeof(T));
	}

	public static T ToOrDefault<T>(this IConvertible obj)
	{
		try
		{
			return obj.To<T>();
		}
		catch (Exception ex)
		{
			if (!(ex is InvalidCastException) && !(ex is ArgumentNullException))
			{
				throw;
			}
			return default(T);
		}
	}

	public static bool ToOrDefault<T>(this IConvertible obj, out T newObj)
	{
		try
		{
			newObj = obj.To<T>();
			return true;
		}
		catch (Exception ex)
		{
			if (!(ex is InvalidCastException) && !(ex is ArgumentNullException))
			{
				throw;
			}
			newObj = default(T);
			return false;
		}
	}

	public static T ToOrOther<T>(this IConvertible obj, T other)
	{
		try
		{
			return obj.To<T>();
		}
		catch (Exception ex)
		{
			if (!(ex is InvalidCastException) && !(ex is ArgumentNullException))
			{
				throw;
			}
			return other;
		}
	}

	public static bool ToOrOther<T>(this IConvertible obj, out T newObj, T other)
	{
		try
		{
			newObj = obj.To<T>();
			return true;
		}
		catch (Exception ex)
		{
			if (!(ex is InvalidCastException) && !(ex is ArgumentNullException))
			{
				throw;
			}
			newObj = other;
			return false;
		}
	}

	public static T ToOrNull<T>(this IConvertible obj) where T : class
	{
		try
		{
			return obj.To<T>();
		}
		catch (Exception ex)
		{
			if (!(ex is InvalidCastException) && !(ex is ArgumentNullException))
			{
				throw;
			}
			return (T)null;
		}
	}

	public static bool ToOrNull<T>(this IConvertible obj, out T newObj) where T : class
	{
		try
		{
			newObj = obj.To<T>();
			return true;
		}
		catch (Exception ex)
		{
			if (!(ex is InvalidCastException) && !(ex is ArgumentNullException))
			{
				throw;
			}
			newObj = (T)null;
			return false;
		}
	}
}
