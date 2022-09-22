// DecompilerFi decompiler from Assembly-CSharp.dll class: States.QuestDrawerModule
using Common.Managers;
using Gameplay;
using System.Collections;
using UnityEngine;

namespace States
{
	public class QuestDrawerModule : GameplayModule
	{
		public GameObject questDrawer;

		protected override Fact[] listenedFacts => new Fact[0];

		public override void Init()
		{
			base.Init();
			StartCoroutine(ShowInitialGameplayThings());
		}

		private IEnumerator ShowInitialGameplayThings()
		{
			if (Manager.Get<ModelManager>().worldsSettings.AreWorldQuestEnabled())
			{
				questDrawer.SetActive(value: true);
				yield return new WaitForSeconds(1.5f);
				if (PlayerQuests.HaveToShowOnStart())
				{
					questDrawer.GetComponentInChildren<QuestsDrawer>().ShowIfHidden();
				}
			}
			else
			{
				questDrawer.SetActive(value: false);
			}
			CheckDailyChest();
		}

		private void CheckDailyChest()
		{
			if (Manager.Contains<DailyChestManager>() && Manager.Get<DailyChestManager>().IsChestReadyToOpen())
			{
				Manager.Get<DailyChestManager>().SpawnIfNeeded(new Vector2(0.5f, 0.8f), hideAfterOpen: true);
			}
		}
	}
}
