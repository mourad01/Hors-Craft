// DecompilerFi decompiler from Assembly-CSharp.dll class: STModule
using Common.Model;
using Common.Utils;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

[CreateAssetMenu(fileName = "STModule", menuName = "SaveTransformsModule/STModule")]
public class STModule : AbstractSTModule
{
	private class TransformsData
	{
		public int lastId;

		public Dictionary<string, Dictionary<string, TransformIndividualData>> transforms;
	}

	private class TransformIndividualData
	{
		public int prefabId;

		public string prefabName;

		public Vector3 prefabPosition;

		public Quaternion prefabRotation;

		public float prefabControllerRadius;
	}

	private const string OLD_SYSTEM_TRANSFORMS_KEY = "transforms";

	public override void LoadSaved()
	{
		if (GetType() == typeof(STModule))
		{
			ValidateWorldPrefs(WorldPlayerPrefs.get, PrefsKey, stManager);
		}
		base.LoadSaved();
	}

	protected override IVector3Index GetIndex(Settings settings)
	{
		return new Index(settings.GetInt("indexX"), settings.GetInt("indexY"), settings.GetInt("indexZ"));
	}

	protected override string GetJSON(string key)
	{
		return WorldPlayerPrefs.get.GetString(key, string.Empty);
	}

	protected override void SaveJSON(string key, string json)
	{
		WorldPlayerPrefs.get.SetString(key, json);
	}

	public static void ValidateWorldPrefs(WorldPlayerPrefs prefs, string key, AbstractSaveTransformManager manager = null)
	{
		if (prefs.HasKey("transforms"))
		{
			string @string = prefs.GetString("transforms");
			TransformsData transformsData = JSONHelper.Deserialize<TransformsData>(@string);
			prefs.DeleteKey("transforms");
			SaveToPrefs(TransformOldData(transformsData.transforms, manager), prefs, key);
		}
	}

	private static List<Settings> TransformOldData(Dictionary<string, Dictionary<string, TransformIndividualData>> transforms, AbstractSaveTransformManager manager)
	{
		List<Settings> list = new List<Settings>();
		foreach (Dictionary<string, TransformIndividualData> value in transforms.Values)
		{
			foreach (TransformIndividualData value2 in value.Values)
			{
				Settings settings = new Settings();
				Index index = GetIndex(value2.prefabPosition);
				string prefabName = value2.prefabName;
				TransformData transformData = default(TransformData);
				transformData.position = value2.prefabPosition;
				transformData.rotation = value2.prefabRotation;
				transformData.scale = GetScale(prefabName, manager);
				TransformData transform = transformData;
				SavedToSettings(transform, index, prefabName, settings);
				list.Add(settings);
			}
		}
		return list;
	}

	private static Vector3 GetScale(string prefabName, AbstractSaveTransformManager manager)
	{
		GameObject prefab = manager.GetPrefab(prefabName);
		if (prefab == null)
		{
			UnityEngine.Debug.LogError($"ADD {prefabName} PREFAB TO SAVE_TRANSFORM_MANAGER");
			return Vector3.one;
		}
		return prefab.transform.lossyScale;
	}

	private static Index GetIndex(Vector3 position)
	{
		if (Engine.EngineInstance == null)
		{
			return new Index(Mathf.FloorToInt((float)Mathf.RoundToInt(position.x) / 16f), Mathf.FloorToInt((float)Mathf.RoundToInt(position.y) / 16f), Mathf.FloorToInt((float)Mathf.RoundToInt(position.z) / 16f));
		}
		return Engine.PositionToIndex(position);
	}

	private static void SavedToSettings(TransformData transform, Index index, string prefabName, Settings settings)
	{
		AbstractSTModule.SaveChunkIndex(index, settings);
		AbstractSTModule.SaveName(prefabName, settings);
		AbstractSTModule.SaveTransform(transform, settings);
	}

	private static void SaveToPrefs(IList<Settings> toSave, WorldPlayerPrefs prefs, string key)
	{
		List<Settings.SerializableSettings> list = new List<Settings.SerializableSettings>(toSave.Count);
		for (int i = 0; i < toSave.Count; i++)
		{
			list.Add(toSave[i].ToSerializableSettings());
		}
		string value = JSONHelper.ToJSON(list);
		prefs.SetString(key, value);
	}
}
