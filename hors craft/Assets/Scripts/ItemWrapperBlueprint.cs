// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemWrapperBlueprint
using Uniblocks;
using UnityEngine;

public class ItemWrapperBlueprint : ItemWrapperGO
{
	public override Sprite GetSprite()
	{
		return prefab.GetComponent<BlueprintCraftableObject>().blueprintSprite;
	}

	public override string GetName()
	{
		return prefab.GetComponent<BlueprintCraftableObject>().blueprintResourceName;
	}
}
