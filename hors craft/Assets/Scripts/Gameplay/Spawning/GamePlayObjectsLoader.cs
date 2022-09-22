// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Spawning.GamePlayObjectsLoader
using Uniblocks;
using UnityEngine;

namespace Gameplay.Spawning
{
	public class GamePlayObjectsLoader
	{
		protected string subWorldKey = "world";

		protected string fileName = "gameplayObjects";

		protected string prefsKey = "gatheredObjects";

		public GamePlayObjectsLoader(string subWorldKey = "world", string fileName = "gameplayObjects")
		{
			this.subWorldKey = subWorldKey;
			this.fileName = fileName;
		}

		public WorldObjectsData LoadSceneObjects(int currentWorldIndex, bool forceLoadFromFile = false)
		{
			string empty = string.Empty;
			bool flag = false;
			string key = $"{prefsKey}.{currentWorldIndex}";
			if (forceLoadFromFile || !PlayerPrefs.HasKey(key))
			{
				string subPath = CreateFilePath(currentWorldIndex);
				empty = LoadStringFromFile(subPath);
				flag = true;
			}
			else
			{
				empty = LoadStringFromPrefs(key);
			}
			WorldObjectsData worldObjectsData = JsonUtility.FromJson<WorldObjectsData>(empty);
			if (flag)
			{
				SaveSceneObjects(worldObjectsData, currentWorldIndex);
			}
			return worldObjectsData;
		}

		public void SaveSceneObjects(WorldObjectsData data, int currentWorldIndex, bool forceSaveToFile = false)
		{
			string subPath = CreateFilePath(currentWorldIndex);
			string text = JsonUtility.ToJson(data);
			if (forceSaveToFile)
			{
				string key = $"{prefsKey}.{currentWorldIndex}";
				PlayerPrefs.DeleteKey(key);
				PlayerPrefs.Save();
				SaveStringToFile(text, subPath);
			}
			else
			{
				string key2 = $"{prefsKey}.{currentWorldIndex}";
				SaveStringToPrefs(key2, text);
			}
		}

		protected string CreateFilePath(int index)
		{
			return $"{subWorldKey}.{index}";
		}

		protected string LoadStringFromFile(string subPath = "")
		{
			FilesLoader filesLoader = new FilesLoader();
			return filesLoader.LoadFile(subPath, fileName, fromResources: true);
		}

		protected string LoadStringFromPrefs(string key)
		{
			return PlayerPrefs.GetString(key);
		}

		protected void SaveStringToPrefs(string key, string json)
		{
			PlayerPrefs.SetString(key, json);
			PlayerPrefs.Save();
		}

		protected void SaveStringToFile(string data, string subPath = "")
		{
			FilesLoader filesLoader = new FilesLoader();
			filesLoader.SaveFile(data, fileName, subPath, toResources: true);
		}
	}
}
