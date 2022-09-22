// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestWorldObjectChain
using Common.Managers;
using QuestSystems.Adventure;
using System;
using UnityEngine;

public class QuestWorldObjectChain : QuestWorldObjectBase
{
	[Serializable]
	protected class ChainItem
	{
		public int questid;

		public EQuestState questSwitchState = EQuestState.afterDone;
	}

	[SerializeField]
	protected ChainItem[] questsChain = new ChainItem[0];

	public bool forceSwitchQuest;

	public EQuestState questSwitchState = EQuestState.started;

	public override void Start()
	{
		SetCurrentQuest();
	}

	private void SetCurrentQuest()
	{
		if (!Manager.Contains<AdventureQuestManager>())
		{
			return;
		}
		AdventureQuestManager adventureQuestManager = Manager.Get<AdventureQuestManager>();
		if (adventureQuestManager == null)
		{
			return;
		}
		QuestDataItem questDataItem = null;
		for (int i = 0; i < questsChain.Length; i++)
		{
			questDataItem = adventureQuestManager.GetQuest(questsChain[i].questid);
			if (questDataItem != null && questDataItem.QuestState != 0 && questDataItem.QuestState < questsChain[i].questSwitchState)
			{
				questId = questDataItem.QuestId;
				ChangeMarkerState(state: true);
				return;
			}
		}
		questId = -1;
	}

	public override void OnUse()
	{
		SetCurrentQuest();
		if (questId < 0)
		{
			ChangeMarkerState(state: false);
		}
		else
		{
			base.OnUse();
		}
	}

	public override void OnQuestUpdate(EQuestState currentState)
	{
		base.OnQuestUpdate(currentState);
		SetCurrentQuest();
		if (questId < 0)
		{
			ChangeMarkerState(state: false);
		}
	}
}
