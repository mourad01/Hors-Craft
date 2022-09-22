// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingRecipesFragment
using Common.Managers;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CraftingRecipesFragment : Fragment
	{
		public enum IconSize
		{
			SMALL,
			MEDIUM,
			BIG
		}

		[Serializable]
		public class SizeSetup
		{
			public IconSize size;

			public GameObject prefab;

			public float width;
		}

		[Serializable]
		public class CraftingCategory
		{
			public Craftable.RecipeCategory category;

			public GameObject tabLine;

			public Button button;

			public IconSize size;
		}

		public GameObject craftableListParent;

		public GridLayoutGroup gridLayout;

		public CraftingCategory[] craftingTabs;

		public SizeSetup[] sizeSetups;

		public Sprite tabsBackground;

		public Sprite noTabsBackground;

		public GameObject tabsHolder;

		public Button resourcesForAdsButton;

		public Image background;

		[HideInInspector]
		public List<GameObject> recipies;

		[HideInInspector]
		public bool scrollPosDirty;

		[HideInInspector]
		public float nextScrollPosition;

		protected CraftingFragment.CrafttStartParameter startParam;

		private int currentTabIndex;

		private Dictionary<Craftable.RecipeCategory, List<CraftItem>> categoriesWithCraftables = new Dictionary<Craftable.RecipeCategory, List<CraftItem>>();

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
			InitCraftableList(Manager.Get<CraftingManager>().GetCraftableList());
			InitBlockCategories();
			EnableTabs();
		}

		protected void ShowResourceForAdButton(int index)
		{
			bool active = (!Manager.Get<ModelManager>().craftingSettings.AreCraftingFree() && index != 0) || (!Manager.Get<ModelManager>().craftingSettings.AreBlueprintsFree() && index == 0);
			if (startParam.parentFragment.enableResourcesForAd)
			{
				resourcesForAdsButton.gameObject.SetActive(active);
				resourcesForAdsButton.onClick.RemoveAllListeners();
				resourcesForAdsButton.onClick.AddListener(delegate
				{
					startParam.parentFragment.OnGetResourcesForAds("Watch an ad for extra resources", "menu.watch.ad.for.resources", _useCraftableId: false);
				});
			}
			else
			{
				resourcesForAdsButton.gameObject.SetActive(value: false);
			}
		}

		private void EnableTabs()
		{
			bool flag = Manager.Get<ModelManager>().craftingSettings.AreCategoriesEnabled();
			background.sprite = ((!flag) ? noTabsBackground : tabsBackground);
			tabsHolder.SetActive(flag);
		}

		private void InitCraftableList(List<Craftable> possibleCraftables)
		{
			if (recipies == null || recipies.Count == 0)
			{
				recipies = new List<GameObject>();
				foreach (Craftable possibleCraftable in possibleCraftables)
				{
					GameObject gameObject = InitCraftItem(possibleCraftable);
					recipies.Add(gameObject);
					if (possibleCraftable.ShouldBeActive())
					{
						AddToDictionary(possibleCraftable.recipeCategory, gameObject.GetComponent<CraftItem>());
					}
				}
			}
			else
			{
				recipies.ForEach(delegate(GameObject item)
				{
					item.GetComponent<CraftItem>().ReintializeCraftable();
				});
			}
		}

		public override void UpdateFragment()
		{
			while (craftableListParent.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(craftableListParent.transform.GetChild(0).gameObject);
			}
			recipies = null;
			categoriesWithCraftables = new Dictionary<Craftable.RecipeCategory, List<CraftItem>>();
			InitCraftableList(Manager.Get<CraftingManager>().GetCraftableList());
			InitBlockCategories();
			EnableTabs();
		}

		private GameObject InitCraftItem(Craftable craftable)
		{
			int craftableId = craftable.id;
			SizeSetup sizeSetup = GetSizeSetup(craftable.recipeCategory);
			GameObject prefab = sizeSetup.prefab;
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, craftableListParent.transform);
			InitUpgradeable(craftable);
			gameObject.SetActive(craftable.ShouldBeActive());
			Sprite graphic = craftable.GetGraphic();
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponent<CraftItem>().Init(craftableId, Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftableId), Manager.Get<CraftingManager>().GetCraftableStatus(craftableId), graphic);
			gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
			{
				onBlockClick(craftableId);
			});
			gameObject.transform.SetAsLastSibling();
			gameObject.GetComponent<CraftItem>().EnableNotification(Manager.Get<QuestManager>().IsThisCraftableNotified(craftableId));
			return gameObject;
		}

		private void InitUpgradeable(Craftable craftable)
		{
			int id = craftable.id;
			if (craftable.craftableType == Craftable.type.Upgradeable)
			{
				Craftable craftable2 = Manager.Get<CraftingManager>().GetCraftable(id + 1);
				if (craftable2.craftableType == Craftable.type.Upgradeable && Singleton<PlayerData>.get.playerItems.GetCraftableCount(id) > 0)
				{
					craftable.showInCraftableList = false;
				}
				else
				{
					craftable.showInCraftableList = true;
				}
			}
			if (craftable.status == CraftableStatus.FullyUpgraded)
			{
				craftable.showInCraftableList = true;
			}
		}

		private void InitBlockCategories()
		{
			for (int i = 0; i < craftingTabs.Length; i++)
			{
				if (!IsCategoryAllowed(craftingTabs[i].category))
				{
					HideTab(i);
					continue;
				}
				craftingTabs[i].tabLine.SetActive(value: true);
				InitCategoryButtonListener(i);
			}
			Craftable.RecipeCategory defaultValue = (!IsCategoryAllowed(Craftable.RecipeCategory.BLUEPRINT)) ? Craftable.RecipeCategory.OTHER : Craftable.RecipeCategory.BLUEPRINT;
			currentTabIndex = PlayerPrefs.GetInt("lastOpenedCraftableCategory", (int)defaultValue);
			DisableTabs();
			TurnOnTab(currentTabIndex);
			UpdateCraftableList();
		}

		private void AddToDictionary(Craftable.RecipeCategory category, CraftItem item)
		{
			if (!categoriesWithCraftables.ContainsKey(category))
			{
				categoriesWithCraftables.Add(category, new List<CraftItem>());
			}
			categoriesWithCraftables[category].Add(item);
		}

		private void UpdateCraftableList()
		{
			bool flag = Manager.Get<ModelManager>().craftingSettings.AreCategoriesEnabled();
			SizeSetup sizeSetup = GetSizeSetup((Craftable.RecipeCategory)currentTabIndex);
			gridLayout.cellSize = new Vector2(sizeSetup.width, sizeSetup.width);
			foreach (Craftable.RecipeCategory key in categoriesWithCraftables.Keys)
			{
				bool active = key == (Craftable.RecipeCategory)currentTabIndex || !flag;
				foreach (CraftItem item in categoriesWithCraftables[key])
				{
					Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(item.id);
					if (craftable.ShouldBeActive())
					{
						item.gameObject.SetActive(active);
						item.ReintializeCraftable();
					}
					else
					{
						item.gameObject.SetActive(value: false);
					}
				}
			}
		}

		private bool IsCategoryAllowed(Craftable.RecipeCategory category)
		{
			return startParam.pauseState.allowedCraftingCategories.Contains(category);
		}

		private void HideTab(int i)
		{
			if (craftingTabs[i].button != null)
			{
				craftingTabs[i].button.gameObject.SetActive(value: false);
			}
		}

		private void InitCategoryButtonListener(int i)
		{
			craftingTabs[i].button.onClick.RemoveAllListeners();
			craftingTabs[i].button.onClick.AddListener(delegate
			{
				if (currentTabIndex != i)
				{
					ChangeTab(currentTabIndex, i);
					currentTabIndex = i;
					UpdateCraftableList();
					PlayerPrefs.SetInt("lastOpenedCraftableCategory", i);
				}
			});
		}

		private void DisableTabs()
		{
			for (int i = 0; i < craftingTabs.Length; i++)
			{
				TurnOffTab(i);
			}
		}

		private void TurnOffTab(int i)
		{
			if (craftingTabs[i].tabLine != null)
			{
				craftingTabs[i].tabLine.SetActive(value: true);
			}
		}

		private void TurnOnTab(int i)
		{
			if (craftingTabs[i].tabLine != null)
			{
				craftingTabs[i].tabLine.SetActive(value: false);
			}
			startParam.parentFragment.UpdateResourceTab(i);
			ShowResourceForAdButton(i);
		}

		private void ChangeTab(int lastIndex, int index)
		{
			TurnOffTab(lastIndex);
			TurnOnTab(index);
		}

		private void onBlockClick(int itemId)
		{
			CraftableStatus craftableStatus = Manager.Get<CraftingManager>().GetCraftableStatus(itemId);
			startParam.sourceBlockId = itemId;
			List<Resource> requiredResourcesToCraft = Manager.Get<CraftingManager>().GetCraftable(itemId).requiredResourcesToCraft;
			Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(itemId);
			if (requiredResourcesToCraft == null || requiredResourcesToCraft.Count == 0)
			{
				if (Singleton<PlayerData>.get.playerItems.GetCraftableCount(itemId) <= 0)
				{
					Singleton<PlayerData>.get.playerItems.AddCraftable(itemId, 1);
				}
				if (craftable.recipeCategory == Craftable.RecipeCategory.BLUEPRINT || Singleton<PlayerData>.get.playerItems.IsCraftableUnlockedByAd(itemId))
				{
					startParam.parentFragment.OnBlockSelected(Manager.Get<CraftingManager>().GetCraftable(itemId));
					return;
				}
			}
			switch (craftableStatus)
			{
			case CraftableStatus.NoResources:
			case CraftableStatus.Craftable:
				startParam.parentFragment.SetState(CraftingFragment.State.Crafting, startParam);
				break;
			case CraftableStatus.FullyUpgraded:
				startParam.parentFragment.SetState(CraftingFragment.State.FullyUpgraded, startParam);
				break;
			default:
				startParam.parentFragment.SetState(CraftingFragment.State.Requirements, startParam);
				break;
			}
		}

		protected virtual SizeSetup GetSizeSetup(Craftable.RecipeCategory category)
		{
			IconSize size = craftingTabs.First((CraftingCategory ct) => ct.category == category).size;
			return sizeSetups.First((SizeSetup ss) => ss.size == size);
		}
	}
}
