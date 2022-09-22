// DecompilerFi decompiler from Assembly-CSharp.dll class: RewardsList
using Common.Gameplay;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardsList", menuName = "ScriptableObjects/Rewards/RewardsList", order = 1)]
public class RewardsList : ScriptableObject
{
	public List<Reward> availableRewards;
}
