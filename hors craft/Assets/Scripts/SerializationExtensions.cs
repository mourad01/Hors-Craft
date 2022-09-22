// DecompilerFi decompiler from Assembly-CSharp.dll class: SerializationExtensions
using System;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class SerializationExtensions
{
	public static string ToStringSerial(this string str)
	{
		return string.IsNullOrEmpty(str) ? string.Empty.ToString(CultureInfo.InvariantCulture) : str.ToString(CultureInfo.InvariantCulture);
	}

	public static string ToStringSerial(this Guid? guid)
	{
		return (!guid.HasValue) ? new byte[0].ToBase64() : guid.Value.ToStringSerial();
	}

	public static string ToStringSerial(this Guid guid)
	{
		return guid.ToByteArray().ToBase64();
	}

	public static Guid? GuidNullableFromSerialString(this string str)
	{
		return (!string.IsNullOrEmpty(str)) ? new Guid?(new Guid(str.FromBase64())) : null;
	}

	public static Guid GuidFromSerialString(this string str)
	{
		return (!string.IsNullOrEmpty(str)) ? new Guid(str.FromBase64()) : default(Guid);
	}

	public static BinaryWriter WriteByteArray(this BinaryWriter bw, byte[] data)
	{
		if (data.IsNullOrEmpty())
		{
			bw.Write(0);
		}
		else
		{
			bw.Write(data.Length);
			bw.Write(data);
		}
		return bw;
	}

	public static byte[] ReadbByteArray(this BinaryReader bread)
	{
		int num = bread.ReadInt32();
		return (num != 0) ? bread.ReadBytes(num) : null;
	}

	public static BinaryWriter WriteByteArrayCompressed(this BinaryWriter bw, byte[] data)
	{
		return bw.WriteByteArray(data.CompressBytes());
	}

	public static byte[] ReadbByteArrayCompressed(this BinaryReader bread)
	{
		return bread.ReadbByteArray().DecompressBytes();
	}

	public static BinaryWriter WriteGuid(this BinaryWriter bw, Guid? guid)
	{
		bw.Write(guid.ToStringSerial());
		return bw;
	}

	public static BinaryWriter WriteGuid(this BinaryWriter bw, Guid guid)
	{
		bw.Write(guid.ToStringSerial());
		return bw;
	}

	public static Guid? ReadGuidNullable(this BinaryReader bread)
	{
		return bread.ReadString().GuidNullableFromSerialString();
	}

	public static Guid ReadGuid(this BinaryReader bread)
	{
		return bread.ReadString().GuidFromSerialString();
	}

	public static BinaryWriter WriteSerializedString(this BinaryWriter bw, string source)
	{
		bw.Write(source.ToStringSerial());
		return bw;
	}

	public static BinaryWriter WriteDateTime(this BinaryWriter bw, DateTime time)
	{
		bw.Write(time.ToUniversalTime().ToBinary());
		return bw;
	}

	public static BinaryWriter WriteDateTime(this BinaryWriter bw, DateTime? time)
	{
		if (!time.HasValue)
		{
			bw.WriteLong(0L);
		}
		else
		{
			bw.Write(time.Value.ToUniversalTime().ToBinary());
		}
		return bw;
	}

	public static DateTime ReadDateTimeAsUTC(this BinaryReader bread)
	{
		return DateTime.FromBinary(bread.ReadInt64());
	}

	public static DateTime? ReadDateTimeAsUTCNullable(this BinaryReader bread)
	{
		long num = bread.ReadInt64();
		return (num != 0) ? new DateTime?(DateTime.FromBinary(num)) : null;
	}

	public static DateTime ReadDateTimeAsLocalTime(this BinaryReader bread)
	{
		return DateTime.FromBinary(bread.ReadInt64()).ToLocalTime();
	}

	public static DateTime? ReadDateTimeAsLocalTimeNullable(this BinaryReader bread)
	{
		long num = bread.ReadInt64();
		return (num != 0) ? new DateTime?(DateTime.FromBinary(num).ToLocalTime()) : null;
	}

	public static BinaryWriter WriteInt(this BinaryWriter bw, int val)
	{
		bw.Write(val);
		return bw;
	}

	public static BinaryWriter WriteInt(this BinaryWriter bw, int? val, int defaultVal)
	{
		bw.Write((!val.HasValue) ? defaultVal : val.Value);
		return bw;
	}

	public static int ReadInt(this BinaryReader bread)
	{
		return bread.ReadInt32();
	}

	public static int? ReadIntNullable(this BinaryReader bread, int defaultOnNullValue)
	{
		int? result = bread.ReadInt32();
		if (result.Value == defaultOnNullValue)
		{
			result = null;
		}
		return result;
	}

	public static BinaryWriter WriteLong(this BinaryWriter bw, long val)
	{
		bw.Write(val);
		return bw;
	}

	public static BinaryWriter WriteFloat(this BinaryWriter bw, float val)
	{
		bw.Write(val);
		return bw;
	}

	public static BinaryWriter WriteVector3(this BinaryWriter bw, Vector3 v3)
	{
		bw.Write(v3.x);
		bw.Write(v3.y);
		bw.Write(v3.z);
		return bw;
	}

	public static BinaryWriter WriteVector2(this BinaryWriter bw, Vector2 v2)
	{
		bw.Write(v2.x);
		bw.Write(v2.y);
		return bw;
	}

	public static Vector3 ReadVector3(this BinaryReader bread)
	{
		return new Vector3(bread.ReadSingle(), bread.ReadSingle(), bread.ReadSingle());
	}

	public static Vector2 ReadVector2(this BinaryReader bread)
	{
		return new Vector2(bread.ReadSingle(), bread.ReadSingle());
	}

	public static BinaryWriter WriteBool(this BinaryWriter bw, bool val)
	{
		bw.Write(val);
		return bw;
	}

	public static BinaryWriter WriteBool(this BinaryWriter bw, bool? val)
	{
		int value = -1;
		if (val.HasValue)
		{
			value = (val.Value ? 1 : 0);
		}
		bw.Write(value);
		return bw;
	}

	public static bool? ReadBoolNullable(this BinaryReader bread)
	{
		int num = bread.ReadInt32();
		if (num == -1)
		{
			return null;
		}
		return num == 1;
	}
}
