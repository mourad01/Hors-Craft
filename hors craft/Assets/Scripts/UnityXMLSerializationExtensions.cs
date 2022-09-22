// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityXMLSerializationExtensions
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public static class UnityXMLSerializationExtensions
{
	public static byte[] XMLSerialize_ToArray<T>(this T objToSerialize) where T : class
	{
		if (objToSerialize.IsTNull())
		{
			return null;
		}
		return objToSerialize.XMLSerialize_ToString().UnityStringToBytes();
	}

	public static string XMLSerialize_ToString<T>(this T objToSerialize) where T : class
	{
		if (objToSerialize.IsTNull())
		{
			return null;
		}
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.Encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: false);
		xmlWriterSettings.Indent = false;
		xmlWriterSettings.OmitXmlDeclaration = false;
		using (StringWriter stringWriter = new StringWriter())
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, xmlWriterSettings))
			{
				xmlSerializer.Serialize(xmlWriter, objToSerialize);
			}
			return stringWriter.ToString();
		}
	}

	public static T XMLDeserialize_ToObject<T>(this string strSerial) where T : class
	{
		if (string.IsNullOrEmpty(strSerial))
		{
			return (T)null;
		}
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		XmlReaderSettings settings = new XmlReaderSettings();
		using (StringReader input = new StringReader(strSerial))
		{
			using (XmlReader xmlReader = XmlReader.Create(input, settings))
			{
				return (T)xmlSerializer.Deserialize(xmlReader);
			}
		}
	}

	public static T XMLDeserialize_ToObject<T>(byte[] objSerial) where T : class
	{
		if (objSerial.IsNullOrEmpty())
		{
			return (T)null;
		}
		return objSerial.UnityBytesToString().XMLDeserialize_ToObject<T>();
	}

	public static void XMLSerialize_AndSaveTo<T>(this T objToSerialize, string path) where T : class
	{
		if (!objToSerialize.IsTNull() && !path.IsNullOrEmpty())
		{
			path.CreateDirectoryIfNotExists();
			XmlSerializer xmlSerializer = new XmlSerializer(objToSerialize.GetType());
			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				xmlSerializer.Serialize(stream, objToSerialize);
			}
		}
	}

	public static void XMLSerialize_AndSaveToPersistentDataPath<T>(this T objToSerialize, string folderName, string filename) where T : class
	{
		if (!objToSerialize.IsTNull() && !filename.IsNullOrEmpty())
		{
			string text = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename) : Path.Combine(Application.persistentDataPath, filename);
			text.CreateDirectoryIfNotExists();
			XmlSerializer xmlSerializer = new XmlSerializer(objToSerialize.GetType());
			using (FileStream stream = new FileStream(text, FileMode.Create))
			{
				xmlSerializer.Serialize(stream, objToSerialize);
			}
		}
	}

	public static void XMLSerialize_AndSaveToDataPath<T>(this T objToSerialize, string folderName, string filename) where T : class
	{
		if (!objToSerialize.IsTNull() && !filename.IsNullOrEmpty())
		{
			string text = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename) : Path.Combine(Application.persistentDataPath, filename);
			text.CreateDirectoryIfNotExists();
			XmlSerializer xmlSerializer = new XmlSerializer(objToSerialize.GetType());
			using (FileStream stream = new FileStream(text, FileMode.Create))
			{
				xmlSerializer.Serialize(stream, objToSerialize);
			}
		}
	}

	public static T XMLDeserialize_AndLoadFrom<T>(this string path) where T : class
	{
		if (path.IsNullOrEmpty())
		{
			return (T)null;
		}
		if (!File.Exists(path))
		{
			return (T)null;
		}
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		using (FileStream stream = new FileStream(path, FileMode.Open))
		{
			return xmlSerializer.Deserialize(stream) as T;
		}
	}

	public static T XMLDeserialize_AndLoadFromPersistentDataPath<T>(this string filename, string folderName) where T : class
	{
		if (filename.IsNullOrEmpty())
		{
			return (T)null;
		}
		string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename) : Path.Combine(Application.persistentDataPath, filename);
		return path.XMLDeserialize_AndLoadFrom<T>();
	}

	public static T XMLDeserialize_AndLoadFromDataPath<T>(this string filename, string folderName) where T : class
	{
		if (filename.IsNullOrEmpty())
		{
			return (T)null;
		}
		string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.dataPath, folderName), filename) : Path.Combine(Application.dataPath, filename);
		return path.XMLDeserialize_AndLoadFrom<T>();
	}
}
