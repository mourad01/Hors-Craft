// DecompilerFi decompiler from Assembly-CSharp.dll class: DressupSkinList
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DressupSkinList : MonoBehaviourSingleton<DressupSkinList>
{
	[Serializable]
	public class HeadInfo
	{
		public enum HeadType
		{
			DEFAULT,
			DRESSUP
		}

		public Texture texture;

		public Skin.Gender gender = Skin.Gender.FEMALE;

		public HeadType headType;
	}

	public List<DressupSkin> possibleSkins = new List<DressupSkin>();

	public List<HeadInfo> headsList;

	public SkinList oldSkinList;

	public bool ChangeOrderAfterModelDownloaded;

	private static DressupSkinList _instance;

	private Dictionary<int, Material> headsMaterials = new Dictionary<int, Material>();

	public SkinList AlternativeSkinList;

	public static DressupSkinList instance => _instance;

	private void Awake()
	{
		int num = 0;
		foreach (Skin possibleSkin in oldSkinList.possibleSkins)
		{
			possibleSkin.id = num;
			num++;
		}
		foreach (DressupSkin possibleSkin2 in possibleSkins)
		{
			possibleSkin2.id = num;
			num++;
		}
		if (AlternativeSkinList != null)
		{
			AlternativeSkinList.Init();
		}
		_instance = this;
	}

	public void ChangeOrder(DressupModule module)
	{
		if (ChangeOrderAfterModelDownloaded && module != null)
		{
			possibleSkins = (from skin in possibleSkins
				orderby module.GetClothesItemBasePriceValue(skin.id)
				select skin).ToList();
		}
	}

	public Material GetHeadMaterial(Skin.Gender gender, HeadInfo.HeadType headType = HeadInfo.HeadType.DEFAULT)
	{
		HeadInfo item = (from h in headsList
			where h.gender == gender && h.headType == headType
			select h).Random();
		int num = headsList.IndexOf(item);
		if (!headsMaterials.ContainsKey(num))
		{
			CreateNewMaterial(num, headsList[num].texture);
		}
		return headsMaterials[num];
	}

	public Material GetHeadMaterial(int index)
	{
		if (!headsMaterials.ContainsKey(index))
		{
			CreateNewMaterial(index, headsList[index].texture);
		}
		return headsMaterials[index];
	}

	private void CreateNewMaterial(int index, Texture texture)
	{
		headsMaterials[index] = new Material(Shader.Find("_X/Diffuse"));
		headsMaterials[index].SetTexture("_MainTex", texture);
	}
}
