// DecompilerFi decompiler from Assembly-CSharp.dll class: PetsList
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PetsList : MonoBehaviour
{
	[Serializable]
	public struct Pets
	{
		public GameObject prefab;

		public bool spawnOnWorld;

		public bool unlocked;

		public bool Equals(object obj)
		{
			if (!(obj is Pets))
			{
				return false;
			}
			Pets pets = (Pets)obj;
			return prefab == pets.prefab;
		}

		public int GetHashCode()
		{
			return ((ValueType)this).GetHashCode();
		}
	}

	public const string PREF_KEY = "petsList.";

	public Pets[] petsList;

	public static Dictionary<string, GameObject> petNameToGameObject;

	public string folderPath;

	private MobsManager mobsManager;

	private void Awake()
	{
		InitPetsList();
		petNameToGameObject = new Dictionary<string, GameObject>(petsList.Length);
		Pets[] array = petsList;
		for (int i = 0; i < array.Length; i++)
		{
			Pets pets = array[i];
			petNameToGameObject.Add(pets.prefab.name, pets.prefab);
		}
	}

	public void InitPetsList()
	{
		if (!Application.isPlaying)
		{
			mobsManager = UnityEngine.Object.FindObjectOfType<MobsManager>();
		}
		else
		{
			mobsManager = Manager.Get<MobsManager>();
		}
		for (int i = 0; i < petsList.Length; i++)
		{
			petsList[i].unlocked = (PlayerPrefs.GetInt("petsList." + i, 0) == 1 || i == 0);
		}
		MobsManager.MobSpawnConfig[] spawnConfigs = mobsManager.spawnConfigs;
		foreach (MobsManager.MobSpawnConfig mobSpawnConfig in spawnConfigs)
		{
			for (int k = 0; k < petsList.Length; k++)
			{
				if (mobSpawnConfig.prefab == petsList[k].prefab && mobSpawnConfig.weight > 0f && mobSpawnConfig.spawnCountFrom > 0)
				{
					petsList[k].spawnOnWorld = true;
				}
			}
		}
	}

	private void OnapplicationPause(bool paused)
	{
		Save();
	}

	private void OnApplicationQuit()
	{
		Save();
	}

	private void Save()
	{
		for (int i = 0; i < petsList.Length; i++)
		{
			if (petsList[i].unlocked)
			{
				PlayerPrefs.SetInt("petsList." + i, 1);
			}
			else
			{
				PlayerPrefs.SetInt("petsList." + i, 0);
			}
		}
	}
}
