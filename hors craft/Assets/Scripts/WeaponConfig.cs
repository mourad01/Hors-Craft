// DecompilerFi decompiler from Assembly-CSharp.dll class: WeaponConfig
using System;
using UnityEngine;

[Serializable]
public struct WeaponConfig
{
	public string name;

	public string nameTranslationKey;

	public Sprite icon;

	public AmmoType ammoType;

	public int maxMagAmmoAmount;

	public int unlockAtLevel;

	public bool claimed;

	public bool isFree;

	public GameObject prefab;
}
