// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemWrapper
using UnityEngine;

public abstract class ItemWrapper : ScriptableObject
{
	public abstract Sprite GetSprite();

	public abstract string GetName();

	public virtual bool CanDrawObjectField()
	{
		return false;
	}

	public virtual void DrawObjectField()
	{
	}
}
