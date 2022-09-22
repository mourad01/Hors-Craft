// DecompilerFi decompiler from Assembly-CSharp.dll class: PettableAdventure
using Common.Managers;
using QuestSystems.Adventure;
using System.Collections.Generic;

public class PettableAdventure : Pettable, IPettable
{
	public int itemidToGive;

	public bool canBeTamed = true;

	public void Start()
	{
		if (canBeTamed)
		{
			AddToWhiteList();
		}
		ShowHideMob();
	}

	public virtual void ShowHideMob()
	{
		List<Resource> resourcesList = Singleton<PlayerData>.get.playerItems.GetResourcesList();
		foreach (Resource item in resourcesList)
		{
			if (item.id == itemidToGive)
			{
				QuestWorldObjectBase component = GetComponent<QuestWorldObjectBase>();
				if (!(component == null))
				{
					component.OnQuestUpdate(EQuestState.done);
				}
				break;
			}
		}
	}

	protected override void OnTamed()
	{
		GiveItem();
	}

	protected virtual void GiveItem()
	{
		if (itemidToGive >= 0)
		{
			RewardBase rewardBase = new RewardBase(ERewardType.ItemReward, itemidToGive, 1);
			rewardBase.Grant();
		}
		QuestWorldObjectBase component = GetComponent<QuestWorldObjectBase>();
		if (!(component == null))
		{
			component.OnQuestUpdate(EQuestState.done);
		}
	}

	protected override void UpdateTameProgress()
	{
	}

	public void AddToWhiteList()
	{
		PetManager petManager = Manager.Get<PetManager>();
		if (!(petManager == null))
		{
			petManager.AddToAllowedPets(this);
		}
	}
}
