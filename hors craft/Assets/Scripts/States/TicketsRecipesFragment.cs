// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TicketsRecipesFragment
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class TicketsRecipesFragment : CraftingRecipesFragment
	{
		public GameObject craftablePrefab;

		[HideInInspector]
		public List<GameObject> recipiesList;

		private List<CraftItem> items = new List<CraftItem>();

		public int blueprintsInitiallyUnlocked = 3;

		private static int unlockedBlueprintsCounter;

		private TicketsFragment ticketsFragment => startParam.parentFragment as TicketsFragment;

		public int ticketsForAd => Manager.Get<ModelManager>().ticketsSettings.GetTicketsForAd();

		public override void Init(FragmentStartParameter parameter)
		{
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
			UpdateFragment();
		}

		public override void UpdateFragment()
		{
			while (craftableListParent.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(craftableListParent.transform.GetChild(0).gameObject);
			}
			recipiesList = null;
			items = new List<CraftItem>();
			InitCraftableList(Manager.Get<CraftingManager>().GetCraftableList());
		}

		private void InitCraftableList(List<Craftable> possibleCraftables)
		{
			possibleCraftables = (from craft in possibleCraftables
				where craft.recipeCategory == Craftable.RecipeCategory.BLUEPRINT
				select craft).ToList();
			if (recipiesList == null || recipiesList.Count == 0)
			{
				recipiesList = new List<GameObject>();
				foreach (Craftable possibleCraftable in possibleCraftables)
				{
					GameObject gameObject = InitCraftItem(possibleCraftable);
					recipiesList.Add(gameObject);
					if (possibleCraftable.ShouldBeActive())
					{
						items.Add(gameObject.GetComponent<CraftItem>());
					}
				}
			}
			else
			{
				recipiesList.ForEach(delegate(GameObject item)
				{
					item.GetComponent<CraftItem>().ReintializeCraftable();
				});
			}
		}

		private GameObject InitCraftItem(Craftable craftable)
		{
			int craftableId = craftable.id;
			SizeSetup sizeSetup = GetSizeSetup(craftable.recipeCategory);
			GameObject gameObject = Object.Instantiate(craftablePrefab, craftableListParent.transform);
			gameObject.SetActive(craftable.ShouldBeActive());
			Sprite graphic = craftable.GetGraphic();
			gameObject.transform.localScale = Vector3.one;
			CraftableStatus craftableStatus = LockStatus(craftableId, craftable);
			gameObject.GetComponent<CraftItem>().Init(craftableId, Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftableId), craftableStatus, graphic);
			gameObject.GetComponent<TicketCraftItem>().onAddTickets = OnAddTickets;
			gameObject.GetComponentInChildren<Button>().interactable = (craftableStatus == CraftableStatus.Craftable || craftableStatus == CraftableStatus.NoResources);
			gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
			{
				onBlockClick(craftableId);
			});
			gameObject.transform.SetAsLastSibling();
			gameObject.GetComponent<CraftItem>().EnableNotification(Manager.Get<QuestManager>().IsThisCraftableNotified(craftableId));
			gridLayout.cellSize = new Vector2(sizeSetup.width, sizeSetup.width);
			return gameObject;
		}

		public void OnAddTickets()
		{
			string text = Manager.Get<TranslationsManager>().GetText("watch.ad.get.tickets", "Watch ad to get {0} tickets");
			text = text.Replace("{0}", ticketsForAd.ToString());
			Manager.Get<StateMachineManager>().PushState<WatchXAdsPopUpState>(new WatchXAdsPopUpStateStartParameter
			{
				onSuccess = delegate(bool s)
				{
					if (s)
					{
						Manager.Get<TicketsManager>().ownedTickets += ticketsForAd;
					}
					if (Manager.Get<StateMachineManager>().IsStateInStack(typeof(CraftingPopupState)))
					{
						Manager.Get<StateMachineManager>().PopStatesUntil<CraftingPopupState>();
					}
					else
					{
						Manager.Get<StateMachineManager>().PopStatesUntil<PauseState>();
					}
				},
				reason = StatsManager.AdReason.XCRAFT_CURRENCY,
				translationKey = "invalid.key.on.purpose.such.programming.much.sense",
				description = text,
				allowRemoveAdsButton = false,
				configWatchButton = delegate(GameObject go)
				{
					TranslateText componentInChildren = go.GetComponentInChildren<TranslateText>();
					componentInChildren.translationKey = "menu.watch";
					componentInChildren.defaultText = "watch";
					componentInChildren.ForceRefresh();
				}
			});
		}

		private CraftableStatus LockStatus(int craftableId, Craftable craftable = null)
		{
			if (blueprintsInitiallyUnlocked == -1)
			{
				return CraftableStatus.Craftable;
			}
			List<TicketsFragment.BlueprintUnlockStats> blueprintsUnlockList = TicketsFragment.GetBlueprintsUnlockList();
			if (!blueprintsUnlockList.Any((TicketsFragment.BlueprintUnlockStats bl) => bl.id == craftableId))
			{
				bool flag = unlockedBlueprintsCounter < blueprintsInitiallyUnlocked;
				blueprintsUnlockList.Add(new TicketsFragment.BlueprintUnlockStats
				{
					id = craftableId,
					unlocked = flag
				});
				TicketsFragment.SaveList(blueprintsUnlockList);
				if (flag)
				{
					unlockedBlueprintsCounter++;
				}
			}
			if (blueprintsUnlockList.FirstOrDefault((TicketsFragment.BlueprintUnlockStats bl) => bl.id == craftableId).unlocked)
			{
				return craftable?.status ?? CraftableStatus.Craftable;
			}
			return CraftableStatus.Locked;
		}

		private void onBlockClick(int itemId)
		{
			CraftableStatus craftableStatus = LockStatus(itemId);
			startParam.sourceBlockId = itemId;
			if (craftableStatus != CraftableStatus.Craftable && craftableStatus != 0)
			{
				return;
			}
			if (Manager.Get<ModelManager>().craftingSettings.AreBlueprintsFree())
			{
				if (Singleton<PlayerData>.get.playerItems.GetCraftableCount(itemId) <= 0)
				{
					Singleton<PlayerData>.get.playerItems.AddCraftable(itemId, 1);
				}
				startParam.parentFragment.OnBlockSelected(Manager.Get<CraftingManager>().GetCraftable(itemId));
			}
			else
			{
				startParam.parentFragment.SetState(CraftingFragment.State.Crafting, startParam);
			}
		}
	}
}
