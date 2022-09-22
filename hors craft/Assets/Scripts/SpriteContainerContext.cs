// DecompilerFi decompiler from Assembly-CSharp.dll class: SpriteContainerContext
using UnityEngine;

public class SpriteContainerContext : FactContext
{
	public Sprite sprite;

	public override string GetContent()
	{
		return base.GetContent() + ((!(sprite != null)) ? string.Empty : sprite.name);
	}
}
