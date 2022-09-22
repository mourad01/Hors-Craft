// DecompilerFi decompiler from Assembly-CSharp.dll class: MinigameLevel
using System;
using System.Collections.Generic;

[Serializable]
public class MinigameLevel
{
	public string name;

	public List<MinigameRequirement> requirements;

	public List<MinigameReward> rewards;

	public MinigameStartParameter parameter;
}
