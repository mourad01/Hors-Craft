// DecompilerFi decompiler from Assembly-CSharp.dll class: MinigamesManager
using Common.Managers;
using Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MinigamesManager : Manager
{
	public List<MinigameLevel> levels;

	private MinigameLevel activelevel;

	public override void Init()
	{
	}

	public void InitMinigame(MinigameStartParameter parameter)
	{
		AbstractMinigameManager abstractMinigameManager = Object.FindObjectOfType<AbstractMinigameManager>();
		if (abstractMinigameManager == null)
		{
			UnityEngine.Debug.LogError("Ther is no minigame manager on scene");
			return;
		}
		abstractMinigameManager.Init(parameter);
		TimeScaleHelper.value = 1f;
	}

	public void PlayLevel(string name)
	{
		MinigameLevel minigameLevel = levels.FirstOrDefault((MinigameLevel l) => l.name == name);
		if (minigameLevel != null && CheckIfMetRequirements(minigameLevel))
		{
			activelevel = minigameLevel;
			minigameLevel.parameter.minigame.Play(minigameLevel.parameter);
		}
	}

	public bool CheckIfMetRequirements(MinigameLevel level)
	{
		return level.requirements.All((MinigameRequirement r) => r.requirement.CheckIfMet(r.value));
	}

	public void ClaimRewards()
	{
		for (int i = 0; i < activelevel.rewards.Count; i++)
		{
			MinigameReward minigameReward = activelevel.rewards[i];
			minigameReward.reward.amount = minigameReward.amount;
			minigameReward.reward.ClaimReward();
		}
	}
}
