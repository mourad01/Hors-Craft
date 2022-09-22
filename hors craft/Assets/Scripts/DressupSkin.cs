// DecompilerFi decompiler from Assembly-CSharp.dll class: DressupSkin
using System;
using UnityEngine;

[Serializable]
public class DressupSkin
{
	public enum Placement
	{
		Hat,
		Hair,
		Earrings,
		Necklace,
		Bracelets,
		Shoes,
		Bag
	}

	public int category;

	public Sprite sprite;

	public GameObject modelPrefab;

	public int id;

	public Placement placement;

	public Skin.Gender gender = Skin.Gender.FEMALE;
}
