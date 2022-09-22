// DecompilerFi decompiler from Assembly-CSharp.dll class: GameplaySubstateAddGameobject
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameplaySubstateAddGameobject : MonoBehaviour, IGameplaySubstateAction
{
	[Serializable]
	public struct AddGameobjectData
	{
		public string path;

		public string parentName;

		public GameObject gameobject;

		public void Add(ModuleLoader module, GameObject other)
		{
			Transform parent = GetParent(other.transform);
			if (!(parent == null))
			{
				UnityEngine.Object.Instantiate(gameobject, parent.transform, worldPositionStays: false);
			}
		}

		private Transform GetParent(Transform parentTransform)
		{
			if (parentTransform.name == parentName)
			{
				return parentTransform;
			}
			if (parentTransform.name + "(Clone)" == parentName)
			{
				return parentTransform;
			}
			Transform transform = parentTransform.Find(parentName);
			if (transform == null)
			{
				transform = parentTransform.FindChildRecursively(parentName);
			}
			if (transform == null)
			{
				transform = parentTransform.Find(parentName + "(Clone)");
			}
			if (transform == null)
			{
				transform = parentTransform.FindChildRecursively(parentName + "(Clone)");
			}
			if (transform == null)
			{
				return null;
			}
			return transform;
		}
	}

	public List<AddGameobjectData> additionalGameobjects;

	protected Dictionary<string, List<AddGameobjectData>> additionalGameobjectsDictionary;

	private void Awake()
	{
		if (additionalGameobjects == null)
		{
			return;
		}
		additionalGameobjectsDictionary = new Dictionary<string, List<AddGameobjectData>>(additionalGameobjects.Count);
		for (int i = 0; i < additionalGameobjects.Count; i++)
		{
			Dictionary<string, List<AddGameobjectData>> dictionary = additionalGameobjectsDictionary;
			AddGameobjectData addGameobjectData = additionalGameobjects[i];
			if (!dictionary.ContainsKey(addGameobjectData.path))
			{
				Dictionary<string, List<AddGameobjectData>> dictionary2 = additionalGameobjectsDictionary;
				AddGameobjectData addGameobjectData2 = additionalGameobjects[i];
				dictionary2.Add(addGameobjectData2.path, new List<AddGameobjectData>());
			}
			Dictionary<string, List<AddGameobjectData>> dictionary3 = additionalGameobjectsDictionary;
			AddGameobjectData addGameobjectData3 = additionalGameobjects[i];
			dictionary3[addGameobjectData3.path].Add(additionalGameobjects[i]);
		}
	}

	public Action<ModuleLoader, GameObject> GetAction()
	{
		return AddGameobject;
	}

	public void AddGameobject(ModuleLoader moduleLoader, GameObject other)
	{
		if (additionalGameobjectsDictionary != null && additionalGameobjectsDictionary.ContainsKey(moduleLoader.path))
		{
			List<AddGameobjectData> list = additionalGameobjectsDictionary[moduleLoader.path];
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Add(moduleLoader, other);
			}
		}
	}
}
