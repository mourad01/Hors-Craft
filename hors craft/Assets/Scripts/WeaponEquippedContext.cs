// DecompilerFi decompiler from Assembly-CSharp.dll class: WeaponEquippedContext
using UnityEngine;

public class WeaponEquippedContext : FactContext
{
	public Sprite shootSprite;

	public Sprite crosshairSprite;

	public override string GetContent()
	{
		if (crosshairSprite == null)
		{
			return string.Empty;
		}
		return crosshairSprite.name;
	}
}
