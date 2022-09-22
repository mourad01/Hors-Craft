// DecompilerFi decompiler from Assembly-CSharp.dll class: AbstractSTModule
using Common.Model;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractSTModule : ScriptableObject
{
	[Serializable]
	public struct TransformData
	{
		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;
	}

	public const string BASE_PREFS_KEY = "STModule_";

	public const string INDEX_X = "indexX";

	public const string INDEX_Y = "indexY";

	public const string INDEX_Z = "indexZ";

	public const string NAME = "name";

	public const string POSITION_X = "positionX";

	public const string POSITION_Y = "positionY";

	public const string POSITION_Z = "positionZ";

	public const string ROTATION_X = "rotationX";

	public const string ROTATION_Y = "rotationY";

	public const string ROTATION_Z = "rotationZ";

	public const string ROTATION_W = "rotationW";

	public const string SCALE_X = "scaleX";

	public const string SCALE_Y = "scaleY";

	public const string SCALE_Z = "scaleZ";

	protected readonly List<AbstractSaveTransform> alive = new List<AbstractSaveTransform>();

	protected List<Settings> freezed = new List<Settings>();

	private Transform moduleSpawnedParent;

	protected AbstractSaveTransformManager stManager;

	public virtual string PrefsKey => "STModule_" + base.name;

	public Transform transformsParent
	{
		get
		{
			if (moduleSpawnedParent == null)
			{
				moduleSpawnedParent = new GameObject(base.name).transform;
				moduleSpawnedParent.position = Vector3.zero;
			}
			return moduleSpawnedParent.transform;
		}
	}

	protected virtual Func<Settings, bool> transferSelectionFunction => null;

	protected virtual Func<Settings, Settings> transferFunction => null;

	protected virtual string transferPrefsKey => null;

	public virtual void Init(AbstractSaveTransformManager manager)
	{
		stManager = manager;
		LoadSaved();
	}

	public virtual void Register(AbstractSaveTransform controller)
	{
		if (alive.Contains(controller))
		{
			UnityEngine.Debug.LogError("Allready in module");
			return;
		}
		if (stManager == null)
		{
			UnityEngine.Debug.LogErrorFormat("!!INIT MODULE FIRST {0} !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", base.name);
			return;
		}
		if (!ContainsPrefabFor(controller.PrefabName))
		{
			UnityEngine.Debug.LogError($"!!Add prefab: {controller.name} with name: {controller.PrefabName} to engine !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			return;
		}
		alive.Add(controller);
		if (!object.ReferenceEquals(controller.transform.parent, transformsParent))
		{
			controller.transform.SetParent(transformsParent, worldPositionStays: true);
		}
		stManager.AddDirty(this);
	}

	public virtual void Unregister(AbstractSaveTransform controller)
	{
		if (alive.Contains(controller))
		{
			alive.Remove(controller);
		}
		stManager.AddDirty(this);
	}

	public virtual GameObject Spawn(Settings settings)
	{
		if (!ContainsPrefabFor(GetName(settings)))
		{
			UnityEngine.Debug.LogError($"!!Add prefab: with name: {GetName(settings)} to engine !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			return null;
		}
		GameObject prefab = GetPrefab(GetName(settings));
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, transformsParent);
		TransformData transform = GetTransform(settings);
		gameObject.transform.localScale = transform.scale;
		gameObject.transform.rotation = transform.rotation;
		gameObject.transform.position = transform.position;
		stManager.EnqueueToActivate(gameObject);
		return gameObject;
	}

	public virtual GameObject GetPrefab(string name)
	{
		return stManager.GetPrefab(name);
	}

	public virtual bool ContainsPrefabFor(string name)
	{
		return stManager.ContainsPrefabFor(name);
	}

	public virtual Settings Save(AbstractSaveTransform controller)
	{
		controller.settings.ClearAll();
		Settings settings = controller.settings;
		SaveName(controller.PrefabName, settings);
		SaveChunkIndex(controller.chunkIndex, settings);
		SaveTransform(controller.transform, settings);
		return settings;
	}

	public virtual void ChunkSpawned(IIndexable chunk)
	{
		List<Settings> list = new List<Settings>();
		IVector3Index index = chunk.GetIndex();
		int num = 0;
		while (num < freezed.Count)
		{
			if (IsSameChunk(freezed[num], index))
			{
				Settings item = freezed[num];
				freezed.RemoveAt(num);
				list.Add(item);
			}
			else
			{
				num++;
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			Spawn(list[i]);
		}
	}

	public virtual void ChunkDespawned(IIndexable chunk)
	{
		IVector3Index index = chunk.GetIndex();
		int num = 0;
		while (num < alive.Count)
		{
			if (alive[num].chunkIndex.IsEqual(index))
			{
				freezed.Add(Save(alive[num]));
				GameObject gameObject = alive[num].gameObject;
				alive.RemoveAt(num);
				UnityEngine.Object.Destroy(gameObject);
			}
			else
			{
				num++;
			}
		}
	}

	public virtual void DeleteAll()
	{
		while (transformsParent.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(transformsParent.GetChild(0).gameObject);
		}
	}

	public virtual void SaveAll()
	{
		List<Settings> list = new List<Settings>(freezed.Count + alive.Count);
		list.AddRange(freezed);
		for (int i = 0; i < alive.Count; i++)
		{
			list.Add(Save(alive[i]));
		}
		SaveToPrefs(list);
	}

	public virtual void LoadSaved()
	{
		if (transferSelectionFunction != null && transferFunction != null && transferPrefsKey != null)
		{
			TransferSettingsFromParent();
		}
		LoadFreezed(PrefsKey);
	}

	public void RegisterToFreezed(AbstractSaveTransform saveTransform)
	{
		freezed.Add(Save(saveTransform));
	}

	protected virtual void LoadFreezed(string prefKey)
	{
		string jSON = GetJSON(prefKey);
		if (string.IsNullOrEmpty(jSON))
		{
			freezed = new List<Settings>();
			return;
		}
		List<Settings.SerializableSettings> list = JSONHelper.Deserialize<List<Settings.SerializableSettings>>(jSON);
		freezed = new List<Settings>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			freezed.Add(Settings.FromSerializableSettings(list[i]));
		}
	}

	protected virtual void TransferSettingsFromParent()
	{
		LoadFreezed(transferPrefsKey);
		if (freezed != null)
		{
			List<Settings> list = (from setting in freezed
				where transferSelectionFunction(setting)
				select transferFunction(setting)).ToList();
			SaveToPrefs((from setting in freezed
				where !transferSelectionFunction(setting)
				select setting).ToList(), transferPrefsKey);
			LoadFreezed(PrefsKey);
			list.AddRange(freezed);
			SaveToPrefs(list);
		}
	}

	protected abstract string GetJSON(string key);

	private void SaveToPrefs(IList<Settings> toSave, string prefsKey = null)
	{
		List<Settings.SerializableSettings> list = new List<Settings.SerializableSettings>(toSave.Count);
		for (int i = 0; i < toSave.Count; i++)
		{
			list.Add(toSave[i].ToSerializableSettings());
		}
		string json = JSONHelper.ToJSON(list);
		SaveJSON(prefsKey ?? PrefsKey, json);
	}

	protected abstract void SaveJSON(string key, string json);

	protected static void SaveChunkIndex(IVector3Index index, Settings settings)
	{
		settings.SetInt("indexX", index.GetX());
		settings.SetInt("indexY", index.GetY());
		settings.SetInt("indexZ", index.GetZ());
	}

	protected static void SaveName(string name, Settings settings)
	{
		settings.SetString("name", name);
	}

	protected static void SaveTransform(Transform transform, Settings settings)
	{
		TransformData transform2 = default(TransformData);
		transform2.position = transform.position;
		transform2.rotation = transform.rotation;
		transform2.scale = transform.lossyScale;
		SaveTransform(transform2, settings);
	}

	protected static void SaveTransform(TransformData transform, Settings settings)
	{
		settings.SetFloat("positionX", transform.position.x);
		settings.SetFloat("positionY", transform.position.y);
		settings.SetFloat("positionZ", transform.position.z);
		settings.SetFloat("rotationX", transform.rotation.x);
		settings.SetFloat("rotationY", transform.rotation.y);
		settings.SetFloat("rotationZ", transform.rotation.z);
		settings.SetFloat("rotationW", transform.rotation.w);
		settings.SetFloat("scaleX", transform.scale.x);
		settings.SetFloat("scaleY", transform.scale.y);
		settings.SetFloat("scaleZ", transform.scale.z);
	}

	protected string GetName(Settings settings)
	{
		return settings.GetString("name");
	}

	protected TransformData GetTransform(Settings settings)
	{
		Vector3 position = default(Vector3);
		Quaternion rotation = default(Quaternion);
		Vector3 scale = default(Vector3);
		position.x = settings.GetFloat("positionX", 0f);
		position.y = settings.GetFloat("positionY", 0f);
		position.z = settings.GetFloat("positionZ", 0f);
		rotation.x = settings.GetFloat("rotationX", 0f);
		rotation.y = settings.GetFloat("rotationY", 0f);
		rotation.z = settings.GetFloat("rotationZ", 0f);
		rotation.w = settings.GetFloat("rotationW", 1f);
		scale.x = settings.GetFloat("scaleX", 1f);
		scale.y = settings.GetFloat("scaleY", 1f);
		scale.z = settings.GetFloat("scaleZ", 1f);
		TransformData result = default(TransformData);
		result.position = position;
		result.rotation = rotation;
		result.scale = scale;
		return result;
	}

	protected abstract IVector3Index GetIndex(Settings settings);

	private bool IsSameChunk(Settings settings, IVector3Index index)
	{
		return index.IsEqual(GetIndex(settings));
	}

	public void LogJson()
	{
		UnityEngine.Debug.Log(GetJSON(PrefsKey));
	}

	public void Clear()
	{
	}
}
