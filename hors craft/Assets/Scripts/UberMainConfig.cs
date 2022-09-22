// DecompilerFi decompiler from Assembly-CSharp.dll class: UberMainConfig
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UberConfigs/MainConfig")]
public class UberMainConfig : ScriptableObject
{
	public List<UberConfig> uberConfigs = new List<UberConfig>();
}
