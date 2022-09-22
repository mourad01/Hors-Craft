// DecompilerFi decompiler from Assembly-CSharp.dll class: ModuleLoader
using States;
using System;
using UnityEngine;

public class ModuleLoader : MonoBehaviour
{
	public const string FOLDER_PATH = "UI/Modules/";

	public string path;

	public bool spawnAsChild = true;

	private GameObject prefab;

	private GameObject instance;

	public void Spawn(bool init = true, Action<ModuleLoader, GameObject> OnSpawnAction = null)
	{
		Despawn();
		if (prefab == null)
		{
			prefab = Resources.Load<GameObject>("UI/Modules/" + path);
		}
		instance = UnityEngine.Object.Instantiate(prefab);
		if (spawnAsChild)
		{
			instance.transform.SetParent(base.transform, worldPositionStays: false);
		}
		else
		{
			int siblingIndex = base.transform.GetSiblingIndex();
			instance.transform.SetParent(base.transform.parent, worldPositionStays: false);
			instance.transform.SetSiblingIndex(siblingIndex);
		}
		instance.transform.localScale = Vector3.one;
		if (init)
		{
			instance.GetComponent<GameplayModule>().Init();
		}
		OnSpawnAction?.Invoke(this, instance);
	}

	public void Despawn()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(instance);
		}
	}
}
