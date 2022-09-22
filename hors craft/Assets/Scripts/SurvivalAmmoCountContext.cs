// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalAmmoCountContext
using System.Text;
using UnityEngine;

public class SurvivalAmmoCountContext : FactContext
{
	public int maxAmmo;

	public int currentAmmo;

	public bool isReloading;

	public Sprite customSprite;

	public override string GetContent()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Max ammo = {0}; Current Ammo = {1}; IsReloading = {2}; Sprite = {3}", maxAmmo, currentAmmo, isReloading, (!(customSprite != null)) ? "Null" : customSprite.name);
		return stringBuilder.ToString();
	}
}
