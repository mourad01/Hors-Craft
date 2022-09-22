// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestSystems.Adventure.QuestWorldObjectBase
using Common.Managers;
using Uniblocks;
using UnityEngine;

namespace QuestSystems.Adventure
{
	public class QuestWorldObjectBase : InteractiveObject
	{
		public int questId;

		public Transform marker;

		public bool questGiverPopOnDone;

		public EQuestState popIndicatorAtState = EQuestState.done;

		public virtual void Start()
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
			QuestDataItem quest = adventureQuestManager.GetQuest(questId);
			if (quest != null)
			{
				if (quest.QuestState >= popIndicatorAtState)
				{
					OnQuestUpdate(quest.QuestState);
				}
				base.Init();
			}
		}

		[ContextMenu("Test player interaction")]
		public override void OnUse()
		{
			if (!Manager.Contains<AdventureQuestManager>())
			{
				UnityEngine.Debug.Log("[QuestWorldObjectBase]:No manager");
				return;
			}
			AdventureQuestManager adventureQuestManager = Manager.Get<AdventureQuestManager>();
			if (adventureQuestManager == null)
			{
				UnityEngine.Debug.Log("[QuestWorldObjectBase]:No manager");
			}
			else
			{
				adventureQuestManager.OnPlayerInteraction(questId, OnQuestUpdate);
			}
		}

		public virtual void OnQuestUpdate(EQuestState currentState)
		{
			if (currentState == EQuestState.disabled)
			{
				ChangeMarkerState(state: false);
			}
			else if (currentState >= popIndicatorAtState)
			{
				ChangeMarkerState(state: false);
				if (questGiverPopOnDone)
				{
					base.transform.gameObject.SetActive(value: false);
				}
			}
		}

		protected void ChangeMarkerState(bool state)
		{
			if (!(marker == null))
			{
				marker.gameObject.SetActive(state);
			}
		}
	}
}
