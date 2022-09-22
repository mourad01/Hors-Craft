// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemWrapperGO
using UnityEngine;

public class ItemWrapperGO : ItemWrapper
{
	public GameObject prefab;

	public override Sprite GetSprite()
	{
		return null;
	}

	public override string GetName()
	{
		return prefab.name;
	}

	public override bool CanDrawObjectField()
	{
		return true;
	}

	public override void DrawObjectField()
	{
	}
}
