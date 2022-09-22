// DecompilerFi decompiler from Assembly-CSharp.dll class: CompressionExtensions
using ICSharpCode.SharpZipLib.BZip2;
using System.IO;

public static class CompressionExtensions
{
	public static byte[] CompressBytes(this byte[] data)
	{
		if (data.IsNullOrEmpty())
		{
			return null;
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			int num = data.Length;
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(num);
				using (BZip2OutputStream bZip2OutputStream = new BZip2OutputStream(memoryStream))
				{
					bZip2OutputStream.Write(data, 0, num);
				}
			}
			return memoryStream.ToArray();
		}
	}

	public static string CompressBytesToString(this byte[] data)
	{
		return data.CompressBytes().UnityBytesToString();
	}

	public static string CompressString(this string sBuffer)
	{
		return sBuffer.UnityStringToBytes().CompressBytesToString();
	}

	public static byte[] CompressStringToBytes(this string sBuffer)
	{
		return sBuffer.UnityStringToBytes().CompressBytes();
	}

	public static byte[] DecompressBytes(this byte[] data)
	{
		if (data.IsNullOrEmpty())
		{
			return null;
		}
		using (MemoryStream memoryStream = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				int num = binaryReader.ReadInt32();
				using (BZip2InputStream bZip2InputStream = new BZip2InputStream(memoryStream))
				{
					byte[] array = new byte[num];
					bZip2InputStream.Read(array, 0, num);
					return array;
				}
			}
		}
	}

	public static byte[] DecompressBytesFromString(this string data)
	{
		return data.UnityStringToBytes().DecompressBytes();
	}

	public static string DecompressStringFromBytes(this byte[] sBuffer)
	{
		return sBuffer.DecompressBytes().UnityBytesToString();
	}

	public static string DecompressString(this string sBuffer)
	{
		return sBuffer.DecompressBytesFromString().UnityBytesToString();
	}
}
