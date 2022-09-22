// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.GameObjectsSpawner
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Behaviours
{
	public class GameObjectsSpawner : MonoBehaviour
	{
		public string prefabsDirectory = string.Empty;

		private Dictionary<int, GameObject> enumValuesToPrefabs = new Dictionary<int, GameObject>();

		private void OnValidate()
		{
			while (prefabsDirectory.EndsWith("/"))
			{
				prefabsDirectory = prefabsDirectory.SubstringFromXToY(0, prefabsDirectory.Length - 1);
			}
		}

		protected void RegisterPrefab(int enumValue, GameObject prefab)
		{
			enumValuesToPrefabs.Add(enumValue, prefab);
		}

		protected void RegisterPrefab(int enumValue, string filename)
		{
			GameObject gameObject = TryToLoadPrefab(filename);
			if (gameObject == null)
			{
				UnityEngine.Debug.LogError("No prafab like: " + filename);
			}
			enumValuesToPrefabs.Add(enumValue, gameObject);
		}

		public GameObject Spawn(int enumValue, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
		{
			GameObject prefab = GetPrefab(enumValue);
			return prefab.Spawn(position, rotation);
		}

		private GameObject TryToLoadPrefab(string filename)
		{
			string text = prefabsDirectory + "/" + filename;
			GameObject gameObject = Resources.Load<GameObject>(text);
			if (gameObject == null)
			{
				throw new Exception("Could not load prefab, path: " + text);
			}
			return gameObject;
		}

		private GameObject GetPrefab(int enumValue)
		{
			if (!enumValuesToPrefabs.TryGetValue(enumValue, out GameObject value))
			{
				UnityEngine.Debug.LogError("There's no prefab registered to enumValue " + enumValue + "!");
			}
			return value;
		}
	}
}
