// DecompilerFi decompiler from Assembly-CSharp.dll class: UsableIndicatorMultiConfig
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Indicators/UsableIndicatorMultiConfig")]
public class UsableIndicatorMultiConfig : ScriptableObject
{
	[Serializable]
	public struct PrefabToIndicator
	{
		public UsableIndicatorSingleConfig config;

		public GameObject usableIndicatorPrefab;
	}

	public PrefabToIndicator[] prefabs;
}
