// DecompilerFi decompiler from Assembly-CSharp.dll class: SkinManager
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : Manager
{
	[Serializable]
	public struct GenderProbability
	{
		public Skin.Gender gender;

		public float probability;
	}

	public GenderProbability[] genderWeightMultipliers = new GenderProbability[3]
	{
		new GenderProbability
		{
			gender = Skin.Gender.MALE,
			probability = 1f
		},
		new GenderProbability
		{
			gender = Skin.Gender.FEMALE,
			probability = 1f
		},
		new GenderProbability
		{
			gender = Skin.Gender.TRANS,
			probability = 1f
		}
	};

	public GameObject skinListPrefab;

	public GameObject[] customPlayerSkinListsPrefab;

	public float hatSpawnChance = 1f;

	public GameObject[] hatsList;

	public float itemsToHoldSpawnChance = 1f;

	public GameObject[] itemsToHoldInHandList;

	private SkinList skinListInstance;

	public override void Init()
	{
		skinListInstance = UnityEngine.Object.Instantiate(skinListPrefab, base.transform).GetComponent<SkinList>();
		skinListInstance.Init();
		Dictionary<Skin.Gender, float> dictionary = new Dictionary<Skin.Gender, float>();
		GenderProbability[] array = genderWeightMultipliers;
		for (int i = 0; i < array.Length; i++)
		{
			GenderProbability genderProbability = array[i];
			dictionary.Add(genderProbability.gender, genderProbability.probability);
		}
		skinListInstance.SetGenderProbabilities(dictionary);
		if (customPlayerSkinListsPrefab != null && customPlayerSkinListsPrefab.Length > 0)
		{
			SkinList skinList = PrepareCustomList();
			skinList.Init();
			SkinList.customPlayerSkinList = skinList;
		}
	}

	private SkinList PrepareCustomList()
	{
		SkinList component = UnityEngine.Object.Instantiate(customPlayerSkinListsPrefab[0], base.transform).GetComponent<SkinList>();
		for (int i = 1; i < customPlayerSkinListsPrefab.Length; i++)
		{
			SkinList component2 = customPlayerSkinListsPrefab[i].GetComponent<SkinList>();
			if (!(component2 == null))
			{
				component.AddPossibleSkin(component2.possibleSkins);
			}
		}
		return component;
	}
}
