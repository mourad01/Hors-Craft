// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ProgressCraftingRecipesFragment
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ProgressCraftingRecipesFragment : CraftingRecipesFragment
	{
		public GameObject craftablePrefab;

		[HideInInspector]
		public List<GameObject> recipiesList;

		private static Dictionary<int, int> items = new Dictionary<int, int>();

		public int blueprintsInitiallyUnlocked = 3;

		public static int BlueprintsInitiallyUnlocked;

		private ProgressCraftingFragment progressFragment => startParam.parentFragment as ProgressCraftingFragment;

		public override void Init(FragmentStartParameter parameter)
		{
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
			BlueprintsInitiallyUnlocked = blueprintsInitiallyUnlocked;
			UpdateFragment();
		}

		public override void UpdateFragment()
		{
			while (craftableListParent.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(craftableListParent.transform.GetChild(0).gameObject);
			}
			InitCraftableList(Manager.Get<CraftingManager>().GetCraftableList());
		}

		private void InitCraftableList(List<Craftable> possibleCraftables)
		{
			possibleCraftables = (from craft in possibleCraftables
				where craft.recipeCategory == Craftable.RecipeCategory.BLUEPRINT
				select craft).ToList();
			recipiesList = new List<GameObject>();
			items.Clear();
			for (int i = 0; i < possibleCraftables.Count; i++)
			{
				items.Add(possibleCraftables[i].id, i);
				GameObject item = InitCraftItem(possibleCraftables[i], i);
				recipiesList.Add(item);
			}
		}

		private GameObject InitCraftItem(Craftable craftable, int index)
		{
			int craftableId = craftable.id;
			SizeSetup sizeSetup = GetSizeSetup(craftable.recipeCategory);
			GameObject gameObject = Object.Instantiate(craftablePrefab, craftableListParent.transform);
			gameObject.SetActive(craftable.ShouldBeActive());
			Sprite graphic = craftable.GetGraphic();
			gameObject.transform.localScale = Vector3.one;
			CraftableStatus craftableStatus = LockStatus(craftableId, craftable);
			gameObject.GetComponent<CraftItem>().Init(craftableId, Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftableId), craftableStatus, graphic);
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

		private CraftableStatus LockStatus(int craftableId, Craftable craftable = null)
		{
			if (blueprintsInitiallyUnlocked == -1)
			{
				return CraftableStatus.Craftable;
			}
			int value = 0;
			items.TryGetValue(craftableId, out value);
			if (value < Manager.Get<ProgressManager>().level - 1 + blueprintsInitiallyUnlocked)
			{
				CraftableStatus? craftableStatus = craftable?.status;
				return (!craftableStatus.HasValue) ? CraftableStatus.Craftable : craftableStatus.Value;
			}
			return CraftableStatus.Locked;
		}

		public static int GetUnlockLevel(int craftableId)
		{
			int value = 0;
			items.TryGetValue(craftableId, out value);
			return value - ((BlueprintsInitiallyUnlocked >= 0) ? BlueprintsInitiallyUnlocked : 0) + 2;
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
