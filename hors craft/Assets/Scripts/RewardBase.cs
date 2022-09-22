// DecompilerFi decompiler from Assembly-CSharp.dll class: RewardBase
using Common.Managers;
using System;
using System.Collections.Generic;

[Serializable]
public class RewardBase
{
	public static List<int> silentItems = new List<int>
	{
		500,
		501,
		502,
		503
	};

	public const string TRANSLATION_KEY = "item.quest.name";

	public ERewardType rewardType;

	public int rewardId;

	public int rewardCount;

	public bool playerInformedOfReward;

	public RewardBase()
	{
	}

	public RewardBase(ERewardType rewardType, int rewardId, int rewardCount)
	{
		this.rewardType = rewardType;
		this.rewardCount = rewardCount;
		this.rewardId = rewardId;
	}

	public override string ToString()
	{
		return $" [type:{rewardType}, id:{rewardId}, cty:{rewardCount} ]";
	}

	public virtual void Grant(RequirementBase requirement = null)
	{
		if (requirement != null)
		{
			Singleton<PlayerData>.get.playerItems.AddToResources(requirement.RequirementItemId, -requirement.RequirementQty);
		}
		Singleton<PlayerData>.get.playerItems.AddToResources(rewardId, rewardCount);
		InformPlayer();
	}

	public void InformPlayer(int itemOverride = -1)
	{
		if (!playerInformedOfReward && (rewardCount != 0 || itemOverride >= 0))
		{
			if (itemOverride < 0)
			{
				itemOverride = rewardId;
			}
			if (!silentItems.Contains(itemOverride))
			{
				playerInformedOfReward = true;
				string text = Manager.Get<TranslationsManager>().GetText(GenerateKey(itemOverride.ToString()), string.Empty);
				string text2 = Manager.Get<TranslationsManager>().GetText("item.received", string.Empty);
				Manager.Get<ToastManager>().ShowToast(string.Format("{2} {0} (x{1})", text, rewardCount, text2), 4f);
			}
		}
	}

	protected virtual string GenerateKey(string id)
	{
		return string.Format("{0}.{1}", "item.quest.name", id);
	}

	public virtual RewardBase DeepCopy()
	{
		RewardBase rewardBase = new RewardBase();
		rewardBase.rewardType = rewardType;
		rewardBase.rewardCount = rewardCount;
		rewardBase.rewardId = rewardId;
		return rewardBase;
	}
}
