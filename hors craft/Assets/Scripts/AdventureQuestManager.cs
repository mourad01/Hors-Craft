// DecompilerFi decompiler from Assembly-CSharp.dll class: AdventureQuestManager
using Common.Managers;
using Gameplay;
using QuestSystems;
using QuestSystems.Adventure;
using States;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdventureQuestManager : Manager
{
	public QuestsData savedQuests;

	public QuestLoader questLoader = new QuestLoader();

	public QuestTranslationsParser questsTranslationsParser;

	public bool sequenceIsDone;

	public ImageGrabber imageGrabber;

	public PortalDataHolder portalDataHolder;

	protected AdventureQuestModule settings => Manager.Get<ModelManager>().adventureQuestSettings;

	public override void Init()
	{
		MonoBehaviourSingleton<GameplayFacts>.get.AddFactPersistent(Fact.ADVENTURE_MODE_ENABLED);
		imageGrabber = base.transform.GetComponent<ImageGrabber>();
		AdventureQuestModule settings = this.settings;
		settings.onQuestsDownloaded = (Action)Delegate.Remove(settings.onQuestsDownloaded, new Action(OnModelDownloadedQuests));
		AdventureQuestModule settings2 = this.settings;
		settings2.onQuestsDownloaded = (Action)Delegate.Combine(settings2.onQuestsDownloaded, new Action(OnModelDownloadedQuests));
		savedQuests = new QuestsData(questLoader);
		savedQuests.Merge(this.settings.DownloadedQuests);
		questsTranslationsParser = new QuestTranslationsParser();
		portalDataHolder = base.transform.GetComponent<PortalDataHolder>();
		if (portalDataHolder == null)
		{
			portalDataHolder = base.gameObject.AddComponent<PortalDataHolder>();
		}
	}

	public void OnModelDownloadedQuests()
	{
		savedQuests.Merge(this.settings.DownloadedQuests);
		AdventureQuestModule settings = this.settings;
		settings.onQuestsDownloaded = (Action)Delegate.Remove(settings.onQuestsDownloaded, new Action(OnModelDownloadedQuests));
	}

	public void OnPlayerInteraction(int questId, Action<EQuestState> onQuestSequenceEnd)
	{
		if (!savedQuests.HasQuest(questId))
		{
			UnityEngine.Debug.Log("[QuestWorldObjectBase]:No such quest!");
			ReturnToCaller(EQuestState.disabled, onQuestSequenceEnd);
			return;
		}
		QuestDataItem quest = savedQuests.GetQuest(questId);
		if (quest == null || quest.QuestState == EQuestState.disabled)
		{
			UnityEngine.Debug.Log("[QuestWorldObjectBase]:No quest or quest is disabled!");
			ReturnToCaller(EQuestState.disabled, onQuestSequenceEnd);
		}
		else
		{
			UpdateQuest(questId, onQuestSequenceEnd);
		}
	}

	private void UpdateQuest(int questId, Action<EQuestState> onQuestSequenceEnd, QuestDataItem quest = null)
	{
		if (quest == null)
		{
			quest = savedQuests.GetQuest(questId);
		}
		if (quest == null || quest.QuestState == EQuestState.disabled)
		{
			UnityEngine.Debug.Log("[QuestWorldObjectBase]:No quest or quest is disabled!(2)");
			ReturnToCaller(EQuestState.disabled, onQuestSequenceEnd);
			return;
		}
		if (sequenceIsDone)
		{
			UnityEngine.Debug.Log("[QuestWorldObjectBase]:Sequence ended");
			quest.UpdateState();
			EndSequenceAndClose(quest, onQuestSequenceEnd);
			return;
		}
		quest.CheckRequirements();
		if (!(Manager.Get<StateMachineManager>().currentState is AdventureQuestState))
		{
			UnityEngine.Debug.Log("[QuestWorldObjectBase]: State is NOT adventurestate. Will get texts...");
			AdventureScreenData textData = CheckForNextDialog(quest);
			ChangeGamePlayUiVisibility(newState: false);
			AdventureQuestStateParameter startParameter = new AdventureQuestStateParameter(delegate(int choosedOption)
			{
				OnPlayerClicked(quest, choosedOption, onQuestSequenceEnd);
			}, textData, onQuestSequenceEnd, GetSprite(quest));
			Manager.Get<StateMachineManager>().PushState<AdventureQuestState>(startParameter);
			return;
		}
		AdventureQuestState adventureQuestState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(AdventureQuestState)) as AdventureQuestState;
		if (adventureQuestState != null)
		{
			UnityEngine.Debug.Log("[QuestWorldObjectBase]: State is adventurestate. Will get texts...");
			AdventureScreenData newData = CheckForNextDialog(quest);
			adventureQuestState.UpdateContent(newData, GetSprite(quest));
		}
	}

	private void EndSequenceAndClose(QuestDataItem quest, Action<EQuestState> activeCallerAction)
	{
		sequenceIsDone = false;
		ReturnToCaller(quest.QuestState, activeCallerAction);
		Save();
	}

	private Sprite GetSprite(QuestDataItem quest)
	{
		Sprite newSprite = null;
		imageGrabber.GetSprite(quest.QuestId, out newSprite, quest.QuestState);
		return newSprite;
	}

	private void ReturnToCaller(EQuestState newState, Action<EQuestState> activeCallerAction)
	{
		if (Manager.Get<StateMachineManager>().currentState is AdventureQuestState)
		{
			Manager.Get<StateMachineManager>().PopState();
		}
		ChangeGamePlayUiVisibility(newState: true);
		if (activeCallerAction != null)
		{
			activeCallerAction(newState);
			activeCallerAction = null;
		}
	}

	private void OnPlayerClicked(QuestDataItem quest, int choosedOption, Action<EQuestState> activeCallerAction)
	{
		UnityEngine.Debug.Log("Player clicked option " + choosedOption);
		if (choosedOption >= 0)
		{
			quest.UpdateChoosedOption(choosedOption);
		}
		UpdateQuest(quest.QuestId, activeCallerAction, quest);
	}

	public void ClearQuests()
	{
		questLoader.DeleteAll();
		savedQuests = null;
	}

	public void AddQuest(QuestDataItem newQuest)
	{
		if (savedQuests != null && !savedQuests.HasQuest(newQuest.QuestId))
		{
			savedQuests.AddNewQuest(newQuest);
		}
	}

	public QuestDataItem GetQuest(int questId)
	{
		if (savedQuests == null)
		{
			return null;
		}
		return savedQuests.GetQuest(questId);
	}

	public List<QuestDataItem> GetActiveQuestsCopy()
	{
		if (savedQuests == null)
		{
			return null;
		}
		return savedQuests.GetActiveQuests();
	}

	public void DeleteQuest(int questId)
	{
		if (savedQuests != null)
		{
			savedQuests.DeleteQuest(questId);
		}
	}

	public void Save()
	{
		if (savedQuests != null)
		{
			savedQuests.Save();
		}
	}

	public AdventureScreenData CheckForNextDialog(QuestDataItem quest)
	{
		UnityEngine.Debug.Log("[QuestWorldObjectBase]: Checking for dialog lines.");
		AdventureScreenData dialog = new AdventureScreenData();
		QuestTranslationsParser.QuestDialogSequenceState dialogUntil = questsTranslationsParser.GetDialogUntil(quest, out dialog);
		CheckForMarkers(dialog, quest);
		switch (dialogUntil)
		{
		case QuestTranslationsParser.QuestDialogSequenceState.error:
			UnityEngine.Debug.Log("[QuestWorldObjectBase]: Checking for dialog lines : ERROR");
			dialog = new AdventureScreenData(string.Empty, new string[1]
			{
				"ok"
			});
			dialogUntil = QuestTranslationsParser.QuestDialogSequenceState.done;
			sequenceIsDone = true;
			quest.SetState(EQuestState.afterDone);
			break;
		case QuestTranslationsParser.QuestDialogSequenceState.done:
			UnityEngine.Debug.Log("[QuestWorldObjectBase]: Checking for dialog lines : DONE");
			sequenceIsDone = true;
			break;
		}
		return dialog;
	}

	private void ChangeGamePlayUiVisibility(bool newState)
	{
		GameplayState gameplayState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(GameplayState)) as GameplayState;
		if (gameplayState != null)
		{
			if (newState)
			{
				gameplayState.ShowUI();
			}
			else
			{
				gameplayState.HideUI(useAction: false);
			}
		}
	}

	public void CheckForMarkers(AdventureScreenData data, QuestDataItem quest)
	{
		List<string> markers = questsTranslationsParser.GetMarkers(data.MainText);
		foreach (string item in markers)
		{
			data.MainText = data.MainText.Replace(item, string.Empty);
		}
		if (questsTranslationsParser.CheckForMarker(markers, "{Q}"))
		{
			quest.ShowNewQuestToast();
		}
		int num = questsTranslationsParser.CheckForItemMarker(markers);
		if (num >= 0)
		{
			quest.GivePlayeritem(num);
		}
	}
}
