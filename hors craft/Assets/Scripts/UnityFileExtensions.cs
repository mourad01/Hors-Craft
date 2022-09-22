// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityFileExtensions
using System.IO;
using UnityEngine;

public static class UnityFileExtensions
{
	public static void SaveTo(this string data, string path)
	{
		if (!data.IsNullOrEmpty() && !path.IsNullOrEmpty())
		{
			path.CreateDirectoryIfNotExists();
			File.WriteAllText(data, path);
		}
	}

	public static void SaveTo(this byte[] data, string path)
	{
		if (!data.IsNullOrEmpty() && !path.IsNullOrEmpty())
		{
			path.CreateDirectoryIfNotExists();
			File.WriteAllBytes(path, data);
		}
	}

	public static void SaveToPersistentDataPath(this string data, string folderName, string filename)
	{
		if (!data.IsNullOrEmpty() && !filename.IsNullOrEmpty())
		{
			string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename) : Path.Combine(Application.persistentDataPath, filename);
			data.SaveTo(path);
		}
	}

	public static void SaveToPersistentDataPath(this byte[] data, string folderName, string filename)
	{
		if (!data.IsNullOrEmpty() && !filename.IsNullOrEmpty())
		{
			string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename) : Path.Combine(Application.persistentDataPath, filename);
			data.SaveTo(path);
		}
	}

	public static void SaveToDataPath(this string data, string folderName, string filename)
	{
		if (!data.IsNullOrEmpty() && !filename.IsNullOrEmpty())
		{
			string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.dataPath, folderName), filename) : Path.Combine(Application.dataPath, filename);
			data.SaveTo(path);
		}
	}

	public static void SaveToDataPath(this byte[] data, string folderName, string filename)
	{
		if (!data.IsNullOrEmpty() && !filename.IsNullOrEmpty())
		{
			string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.dataPath, folderName), filename) : Path.Combine(Application.dataPath, filename);
			data.SaveTo(path);
		}
	}

	public static string LoadFrom_AsString(this string path)
	{
		if (path.IsNullOrEmpty())
		{
			return null;
		}
		if (!File.Exists(path))
		{
			return null;
		}
		return File.ReadAllText(path);
	}

	public static byte[] LoadFrom_AsBytes(this string path)
	{
		if (path.IsNullOrEmpty())
		{
			return null;
		}
		if (!File.Exists(path))
		{
			return null;
		}
		return File.ReadAllBytes(path);
	}

	public static string LoadFromPeristantDataPath_AsString(this string filename, string folderName)
	{
		if (filename.IsNullOrEmpty())
		{
			return null;
		}
		string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename) : Path.Combine(Application.persistentDataPath, filename);
		return path.LoadFrom_AsString();
	}

	public static byte[] LoadFromPeristantDataPath_AsBytes(this string filename, string folderName)
	{
		if (filename.IsNullOrEmpty())
		{
			return null;
		}
		string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.persistentDataPath, folderName), filename) : Path.Combine(Application.persistentDataPath, filename);
		return path.LoadFrom_AsBytes();
	}

	public static string LoadFromDataPath_AsString(this string filename, string folderName)
	{
		if (filename.IsNullOrEmpty())
		{
			return null;
		}
		string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.dataPath, folderName), filename) : Path.Combine(Application.dataPath, filename);
		return path.LoadFrom_AsString();
	}

	public static byte[] LoadFromDataPath_AsBytes(this string filename, string folderName)
	{
		if (filename.IsNullOrEmpty())
		{
			return null;
		}
		string path = (!folderName.IsNullOrEmpty()) ? Path.Combine(Path.Combine(Application.dataPath, folderName), filename) : Path.Combine(Application.dataPath, filename);
		return path.LoadFrom_AsBytes();
	}
}
