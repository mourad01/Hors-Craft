// DecompilerFi decompiler from Assembly-CSharp.dll class: UsableIndicatorSingleConfig
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Indicators/UsableIndicatorSingleConfig")]
public class UsableIndicatorSingleConfig : ScriptableObject
{
	public bool spawnOnStart;

	public GameObject usableIndicatorPrefab
	{
		get;
		set;
	}
}
