// DecompilerFi decompiler from Assembly-CSharp.dll class: LevelupRewardConfig
using System.Collections.Generic;
using UnityEngine;

public class LevelupRewardConfig : ScriptableObject
{
	public virtual List<LevelUpRewardItemData> GetRewards(int level)
	{
		return new List<LevelUpRewardItemData>();
	}
}
