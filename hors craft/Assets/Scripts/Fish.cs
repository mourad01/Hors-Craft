// DecompilerFi decompiler from Assembly-CSharp.dll class: Fish
using System;
using UnityEngine;

[Serializable]
public struct Fish
{
	[Range(1f, 5f)]
	public int rarity;

	public string codeName;

	public string name;

	public Sprite icon;

	public GameObject prefab;

	[Range(0f, 100f)]
	public float precentageToSuccess;

	public int weightMin;

	public int weightMax;

	public int pointsMin;

	public int pointsMax;

	[Range(1f, 50f)]
	public int bronzeScalesDrop;

	[Range(0f, 50f)]
	public int silverScalesDrop;

	[Range(0f, 50f)]
	public int goldenScalesDrop;

	public FishingManager.FishingMiniGameDifficulty difficulty;

	public FishingManager.FishingRodQuality minimalRodRequirements;

	public bool catched;

	[HideInInspector]
	public int catchedWeight;

	[HideInInspector]
	public int catchedPoints;
}
