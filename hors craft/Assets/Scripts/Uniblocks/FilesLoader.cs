// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.FilesLoader
using Common.Managers;
using System.IO;
using System.Text;
using UnityEngine;

namespace Uniblocks
{
	public class FilesLoader
	{
		public enum Mode
		{
			RESOURCES,
			APP_DATA
		}

		public const int EMPTY_REGION_LENGTH = 999;

		private static string directory => Engine.WorldPath + "/";

		private static Mode mode
		{
			get
			{
				if (!Application.isPlaying || !Manager.Contains<SavedWorldManager>() || Manager.Get<SavedWorldManager>().IsWorldFromResources())
				{
					return Mode.RESOURCES;
				}
				return Mode.APP_DATA;
			}
		}

		public bool SeedExists()
		{
			if (mode == Mode.APP_DATA)
			{
				Directory.CreateDirectory(directory);
				return File.Exists(directory + "seed.txt");
			}
			int num = directory.IndexOf("Pregenerated");
			if (num < 0)
			{
				return false;
			}
			string str = directory.Substring(num);
			return Resources.Load<TextAsset>(str + "seed") != null;
		}

		public int LoadSeed()
		{
			if (mode == Mode.APP_DATA)
			{
				Directory.CreateDirectory(directory);
				int num = 0;
				StreamReader streamReader = new StreamReader(directory + "seed.txt");
				num = int.Parse(streamReader.ReadToEnd());
				streamReader.Close();
				streamReader.Dispose();
				return num;
			}
			string str = directory.Substring(directory.IndexOf("Pregenerated"));
			TextAsset textAsset = Resources.Load<TextAsset>(str + "seed");
			MemoryStream stream = new MemoryStream(textAsset.bytes, writable: false);
			StreamReader streamReader2 = new StreamReader(stream);
			int result = int.Parse(streamReader2.ReadToEnd());
			streamReader2.Close();
			streamReader2.Dispose();
			return result;
		}

		public void SaveSeed(int seed)
		{
			Directory.CreateDirectory(directory);
			StreamWriter streamWriter = new StreamWriter(directory + "seed.txt");
			streamWriter.Write(seed.ToString());
			streamWriter.Flush();
			streamWriter.Close();
			streamWriter.Dispose();
		}

		public void CreateRegionFile(string regionIndexString)
		{
			string path = directory + regionIndexString + ",.region.txt";
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 999; i++)
			{
				stringBuilder.Append('\uffff');
			}
			Directory.CreateDirectory(directory);
			StreamWriter streamWriter = new StreamWriter(path, append: false, Encoding.UTF8);
			streamWriter.Write(stringBuilder.ToString());
			streamWriter.Flush();
			streamWriter.Close();
			streamWriter.Dispose();
		}

		public static string[] LoadRegion(string regionIndexString, string directory)
		{
			string text = directory + regionIndexString + ",.region";
			if (mode == Mode.RESOURCES)
			{
				string path = text.Substring(text.IndexOf("Pregenerated"));
				TextAsset textAsset = Resources.Load<TextAsset>(path);
				if (textAsset != null)
				{
					MemoryStream stream = new MemoryStream(textAsset.bytes, writable: false);
					StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
					string[] result = streamReader.ReadToEnd().Split('\uffff');
					streamReader.Close();
					streamReader.Dispose();
					return result;
				}
				return null;
			}
			Directory.CreateDirectory(directory);
			text += ".txt";
			if (File.Exists(text))
			{
				StreamReader streamReader2 = new StreamReader(text, Encoding.UTF8);
				string[] result2 = streamReader2.ReadToEnd().Split('\uffff');
				streamReader2.Close();
				streamReader2.Dispose();
				return result2;
			}
			return null;
		}

		public static string[] LoadRegionFromFile(string path)
		{
			using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
			{
				return streamReader.ReadToEnd().Split('\uffff');
			}
		}

		public static string[] LoadRegion(string regionIndexString)
		{
			return LoadRegion(regionIndexString, directory);
		}

		public static string[] GetAllPregeneratedFilesPath()
		{
			if (mode == Mode.RESOURCES)
			{
				return null;
			}
			return Directory.GetFiles(directory, "*.region.txt");
		}

		public void SaveRegion(string regionIndexString, string[] regionData, string directory, bool refreshAssetsIfEditor = true)
		{
			Directory.CreateDirectory(directory);
			string path = directory + regionIndexString + ",.region.txt";
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (string value in regionData)
			{
				stringBuilder.Append(value);
				if (num != regionData.Length - 1)
				{
					stringBuilder.Append('\uffff');
				}
				num++;
			}
			StreamWriter streamWriter = new StreamWriter(path, append: false, Encoding.UTF8);
			streamWriter.Write(stringBuilder.ToString());
			streamWriter.Flush();
			streamWriter.Close();
			streamWriter.Dispose();
			if (!refreshAssetsIfEditor)
			{
			}
		}

		public void SaveRegion(string regionIndexString, string[] regionData)
		{
			SaveRegion(regionIndexString, regionData, directory);
		}

		public string LoadWorldPlayerPrefsJSONText()
		{
			string text = directory + "worldprefs";
			if (mode == Mode.RESOURCES)
			{
				string path = text.Substring(text.IndexOf("Pregenerated"));
				TextAsset textAsset = Resources.Load<TextAsset>(path);
				if (textAsset == null)
				{
					return null;
				}
				return textAsset.text;
			}
			text += ".txt";
			if (File.Exists(text))
			{
				StreamReader streamReader = new StreamReader(text, Encoding.UTF8);
				string result = streamReader.ReadToEnd();
				streamReader.Close();
				streamReader.Dispose();
				return result;
			}
			return null;
		}

		public void SaveWorldPlayerPrefsJSONText(string jsonText, bool refreshAssetsIfEditor = true)
		{
			Directory.CreateDirectory(directory);
			string path = directory + "worldprefs.txt";
			StreamWriter streamWriter = new StreamWriter(path, append: false, Encoding.UTF8);
			streamWriter.Write(jsonText);
			streamWriter.Flush();
			streamWriter.Close();
			streamWriter.Dispose();
			if (!refreshAssetsIfEditor)
			{
			}
		}

		public void SaveFile(string jsonText, string fileName, string subPath = "", bool toResources = false, bool refreshAssetsIfEditor = true)
		{
			string text = directory;
			if (toResources)
			{
				string gameName = Manager.Get<ConnectionInfoManager>().gameName;
				text = $"{Application.dataPath}/Resources/PregeneratedWorld_{gameName}/{subPath}/";
			}
			Directory.CreateDirectory(text);
			string path = $"{text}{fileName}.txt";
			File.WriteAllText(path, jsonText);
			if (!refreshAssetsIfEditor)
			{
			}
		}

		public string LoadFile(string subPath, string fileName, bool fromResources = false)
		{
			string directory = FilesLoader.directory;
			if (fromResources)
			{
				string gameName = Manager.Get<ConnectionInfoManager>().gameName;
				directory = $"PregeneratedWorld_{gameName}/{subPath}";
				directory = $"{directory}/{fileName}";
				TextAsset textAsset = Resources.Load<TextAsset>(directory);
				return (!(textAsset == null)) ? textAsset.text : null;
			}
			directory = $"{directory}.txt";
			if (!File.Exists(directory))
			{
				return null;
			}
			string empty = string.Empty;
			StreamReader streamReader = new StreamReader(directory, Encoding.UTF8);
			empty = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader.Dispose();
			return empty;
		}
	}
}
