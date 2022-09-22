// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldPlayerPrefs
using Common.Managers;
using Common.Model;
using Common.Utils;
using Uniblocks;
using UnityEngine;

public class WorldPlayerPrefs : Settings
{
	private static WorldPlayerPrefs instance;

	public static WorldPlayerPrefs get
	{
		get
		{
			if (instance == null)
			{
				instance = new WorldPlayerPrefs();
			}
			return instance;
		}
	}

	public void Save()
	{
		if (!Manager.Get<SavedWorldManager>().GetCurrentWorld().resources)
		{
			FilesLoader filesLoader = new FilesLoader();
			SerializableSettings ob = ToSerializableSettings();
			string jsonText = JSONHelper.ToJSON(ob);
			filesLoader.SaveWorldPlayerPrefsJSONText(jsonText, refreshAssetsIfEditor: false);
		}
	}

	public void Load()
	{
		ClearAll();
		FilesLoader filesLoader = new FilesLoader();
		string text = filesLoader.LoadWorldPlayerPrefsJSONText();
		if (!string.IsNullOrEmpty(text))
		{
			Deserialize(text);
		}
	}

	private void Deserialize(string jsonText)
	{
		SerializableSettings serializableSettings = JSONHelper.Deserialize<SerializableSettings>(jsonText);
		if (serializableSettings != null)
		{
			AppendSettingsFrom(Settings.FromSerializableSettings(serializableSettings));
			UnityEngine.Debug.Log("Loaded WorldPlayerPrefs from file.");
		}
		else
		{
			UnityEngine.Debug.LogError("Could not load WorldPlayerPrefs from file.");
		}
	}
}
