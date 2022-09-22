// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingRequirementsFragment
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CraftingRequirementsFragment : Fragment
	{
		public GameObject questPrefab;

		public GameObject questParent;

		public Button returnButton;

		public Button unlockButton;

		public CraftItem gigantBlock;

		private CraftingFragment.CrafttStartParameter startParam;

		private List<GameObject> questList;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
			InitBlock();
			InitButtons();
			createNeededQuests(Manager.Get<CraftingManager>().GetCraftable(startParam.sourceBlockId));
			if (Manager.Get<ModelManager>().craftingSettings.AreCraftingFree() && Manager.Get<CraftingManager>().GetCraftable(startParam.sourceBlockId).recipeCategory != 0)
			{
				UnlockRecipePerAd();
			}
		}

		public void InitBlock()
		{
			int sourceBlockId = startParam.sourceBlockId;
			gigantBlock.Init(sourceBlockId, Singleton<PlayerData>.get.playerItems.GetCraftableCount(sourceBlockId), Manager.Get<CraftingManager>().GetCraftableStatus(sourceBlockId), Manager.Get<CraftingManager>().GetCraftable(sourceBlockId).GetGraphic());
		}

		public void InitButtons()
		{
			returnButton.onClick.RemoveAllListeners();
			returnButton.onClick.AddListener(delegate
			{
				startParam.parentFragment.SetState(CraftingFragment.State.Recipes, startParam);
			});
			if (startParam.parentFragment.enableResourcesForAd)
			{
				unlockButton.gameObject.SetActive(value: true);
				unlockButton.onClick.RemoveAllListeners();
				unlockButton.onClick.AddListener(delegate
				{
					UnlockRecipePerAd();
				});
			}
			else
			{
				unlockButton.gameObject.SetActive(value: false);
			}
		}

		private void createNeededQuests(Craftable craftable)
		{
			destroyCurrentQuests();
			questList = new List<GameObject>();
			TryToAddWorldQuestElement(craftable);
			for (int i = 0; i < craftable.unlockingQuestId.Count; i++)
			{
				Quest questId = Manager.Get<QuestManager>().GetQuestId(craftable.unlockingQuestId[i]);
				if (questId != null)
				{
					questList.Add(CreateElement(questId));
				}
			}
		}

		private void TryToAddWorldQuestElement(Craftable craftable)
		{
			if (!string.IsNullOrEmpty(craftable.connectedWorldID))
			{
				string text = Manager.Get<TranslationsManager>().GetText("quest.worldQuest", "Complete quests from this world");
				int worldQuestNeeded = craftable.worldQuestNeeded;
				int questCountPassedInWorld = Singleton<PlayerData>.get.playerQuests.GetQuestCountPassedInWorld(craftable.connectedWorldID);
				Sprite image = Manager.Get<QuestManager>().GetImage(1);
				questList.Add(CreateElement(text, questCountPassedInWorld, worldQuestNeeded, image));
			}
		}

		private GameObject CreateElement(Quest quest)
		{
			string questDescription = Manager.Get<QuestManager>().GetQuestDescription(quest.id);
			int questProgress = Singleton<PlayerData>.get.playerQuests.GetQuestProgress(quest.id);
			int stepsNeeded = quest.stepsNeeded;
			Sprite imageForQuest = Manager.Get<QuestManager>().GetImageForQuest(quest.id);
			return CreateElement(questDescription, questProgress, stepsNeeded, imageForQuest);
		}

		private GameObject CreateElement(string description, int currentProgress, int stepsNeeded, Sprite image)
		{
			GameObject gameObject = Object.Instantiate(questPrefab);
			gameObject.transform.SetParent(questParent.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.SetActive(value: true);
			gameObject.GetComponent<QuestItem>().Init(description, currentProgress, stepsNeeded, image);
			return gameObject;
		}

		private void destroyCurrentQuests()
		{
			if (questList != null)
			{
				foreach (GameObject quest in questList)
				{
					UnityEngine.Object.Destroy(quest);
				}
			}
		}

		private void UnlockRecipePerAd()
		{
			startParam.parentFragment.useCraftableId = true;
			ShowAd(startParam.sourceBlockId);
		}

		public static void ShowAd(int id)
		{
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				description = "Watch an ad to unlock this item",
				translationKey = "menu.watch.ad.to.unlock.craftable",
				type = AdsCounters.None,
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.defaultText = "Unlock now";
					componentInChildren.translationKey = "menu.unlock.blocks.now";
					componentInChildren.ForceRefresh();
				},
				onSuccess = delegate
				{
					Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(id);
					Singleton<PlayerData>.get.playerItems.OnCraftableUnlockByAd(craftable.id);
				},
				reason = StatsManager.AdReason.XCRAFT_UNLOCK_CRAFTABLE
			});
		}
	}
}
