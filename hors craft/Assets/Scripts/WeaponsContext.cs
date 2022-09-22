// DecompilerFi decompiler from Assembly-CSharp.dll class: WeaponsContext
using System.Linq;
using UnityEngine;
using UnityToolbag;

public class WeaponsContext : SurvivalContext
{
	[Reorderable]
	public WeaponConfig[] weaponsConfigs;

	public GameObject[] GetPrefabs()
	{
		return (from config in weaponsConfigs
			select config.prefab).ToArray();
	}
}
