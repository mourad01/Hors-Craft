// DecompilerFi decompiler from Assembly-CSharp.dll class: ResourceSpawner
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
	public string resourcesPath;

	private GameObject instance;

	private GameObject resource;

	public Transform parent;

	private void Awake()
	{
		resource = Resources.Load<GameObject>(resourcesPath);
		instance = UnityEngine.Object.Instantiate(resource);
		instance.transform.SetParent(parent);
	}

	public void UnloadSpawned()
	{
		UnityEngine.Object.Destroy(instance);
		Renderer[] componentsInChildren = resource.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			Resources.UnloadAsset(renderer.material.mainTexture);
		}
	}

	private void OnDestroy()
	{
		UnloadSpawned();
	}
}
