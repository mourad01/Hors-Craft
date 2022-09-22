// DecompilerFi decompiler from Assembly-CSharp.dll class: AdventureQuestModule
using Common.Managers;
using Common.Model;
using QuestSystems.Adventure;
using System;
using System.Collections.Generic;

public class AdventureQuestModule : ModelModule
{
	protected List<QuestDataItem> downloadedQuests = new List<QuestDataItem>();

	public Action onQuestsDownloaded;

	public List<QuestDataItem> DownloadedQuests
	{
		get
		{
			if (downloadedQuests == null)
			{
				return new List<QuestDataItem>();
			}
			return downloadedQuests;
		}
	}

	private string keyQuestQuantity()
	{
		return "quest.adv.count";
	}

	private string keyQuestEnabled(int index)
	{
		return $"quest.adv.{index}.enabled";
	}

	private string keyQuestName(int index)
	{
		return $"quest.adv.{index}.name";
	}

	private string keyQuestId(int index)
	{
		return $"quest.adv.{index}.id";
	}

	private string keyQuestRequirementId(int index)
	{
		return $"quest.adv.{index}.requirementId";
	}

	private string keyQuestRequirementQty(int index)
	{
		return $"quest.adv.{index}.requirementQty";
	}

	private string keyQuestRewardType(int index)
	{
		return $"quest.adv.{index}.rewardType";
	}

	private string keyQuestRewardQty(int index)
	{
		return $"quest.adv.{index}.rewardQty";
	}

	private string keyQuestRewardItemId(int index)
	{
		return $"quest.adv.{index}.rewardItemId";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		for (int i = 0; i < 60; i++)
		{
			descriptions.AddDescription(keyQuestEnabled(i), defaultValue: false);
			descriptions.AddDescription(keyQuestName(i), string.Empty);
			descriptions.AddDescription(keyQuestId(i), -1);
			descriptions.AddDescription(keyQuestRequirementId(i), -1);
			descriptions.AddDescription(keyQuestRequirementQty(i), 0);
			descriptions.AddDescription(keyQuestRewardType(i), 0);
			descriptions.AddDescription(keyQuestRewardQty(i), 0);
			descriptions.AddDescription(keyQuestRewardItemId(i), 0);
		}
	}

	public override void OnModelDownloaded()
	{
		downloadedQuests = new List<QuestDataItem>();
		QuestDataItem questDataItem = null;
		for (int i = 0; i < 60; i++)
		{
			string @string = base.settings.GetString(keyQuestName(i), string.Empty);
			if (!string.IsNullOrEmpty(@string))
			{
				questDataItem = new QuestDataItem(i, base.settings.GetString(keyQuestName(i), string.Empty), 1, new RewardBase((ERewardType)base.settings.GetInt(keyQuestRewardType(i), 0), base.settings.GetInt(keyQuestRewardItemId(i), 0), base.settings.GetInt(keyQuestRewardQty(i), 0)), new RequirementBase(ERequirementType.gatherItems, base.settings.GetInt(keyQuestRequirementId(i), 0), base.settings.GetInt(keyQuestRequirementQty(i), 0)));
				downloadedQuests.Add(questDataItem);
			}
		}
		if (onQuestsDownloaded != null)
		{
			onQuestsDownloaded();
		}
	}

	public bool IsTrainPlacementEnabled()
	{
		return true;
	}
}
