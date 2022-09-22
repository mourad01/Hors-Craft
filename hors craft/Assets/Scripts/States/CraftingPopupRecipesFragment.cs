// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingPopupRecipesFragment
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CraftingPopupRecipesFragment : CraftingRecipesFragment
	{
		public GameObject craftablePrefab;

		[HideInInspector]
		public Type listType;

		[HideInInspector]
		public List<GameObject> recipiesList;

		protected List<CraftItem> items = new List<CraftItem>();

		public override void Init(FragmentStartParameter parameter)
		{
			startParam = (parameter as CraftingFragment.CrafttStartParameter);
			UpdateFragment();
		}

		public override void UpdateFragment()
		{
			if (!(listType == null))
			{
				while (craftableListParent.transform.childCount > 0)
				{
					UnityEngine.Object.DestroyImmediate(craftableListParent.transform.GetChild(0).gameObject);
				}
				recipiesList = null;
				items = new List<CraftItem>();
				InitCraftableList(Manager.Get<CraftingManager>().GetCraftableList(), listType);
				startParam.parentFragment.UpdateResourceTab((int)Manager.Get<CraftingManager>().GetCraftable(recipiesList[0].GetComponent<CraftItem>().id).recipeCategory);
			}
		}

		protected void InitCraftableList<T>(List<Craftable> possibleCraftables, T listType)
		{
			possibleCraftables = (from craft in possibleCraftables
				where craft.customCraftableObject != null && craft.customCraftableObject.GetComponent(listType.ToString()) != null
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
			GameObject prefab = sizeSetup.prefab;
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, craftableListParent.transform);
			gameObject.SetActive(value: true);
			Sprite graphic = craftable.GetGraphic();
			gameObject.transform.localScale = Vector3.one;
			gameObject.GetComponent<CraftItem>().Init(craftableId, Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftableId), Manager.Get<CraftingManager>().GetCraftableStatus(craftableId), graphic);
			gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
			{
				onBlockClick(craftableId);
			});
			gameObject.transform.SetAsLastSibling();
			gameObject.GetComponent<CraftItem>().EnableNotification(Manager.Get<QuestManager>().IsThisCraftableNotified(craftableId));
			gridLayout.cellSize = new Vector2(sizeSetup.width, sizeSetup.width);
			return gameObject;
		}

		protected override SizeSetup GetSizeSetup(Craftable.RecipeCategory category)
		{
			UnityEngine.Debug.Log((int)category);
			if ((int)category >= sizeSetups.Length)
			{
				return sizeSetups[0];
			}
			return sizeSetups[(int)category];
		}

		private void onBlockClick(int itemId)
		{
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
			startParam.sourceBlockId = itemId;
			startParam.parentFragment.SetState(CraftingFragment.State.Crafting, startParam);
		}
	}
}
