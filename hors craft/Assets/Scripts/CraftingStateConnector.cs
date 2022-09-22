// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftingStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingStateConnector : UIConnector
{
	public enum State
	{
		List,
		Crafting,
		Quest
	}

	public delegate void OnClick();

	private State currentState;

	public Button returnButton;

	public GameObject craftableListParent;

	public GameObject resourcesListParent;

	public GameObject resourcesPrefab;

	public GameObject craftablePrefab;

	public GameObject questWindow;

	public GameObject craftWindow;

	public GameObject allBlocksWindow;

	public OnClick onReturnButton;

	public OnClick onLesserReturnButton;

	public OnClick onCraftableClick;

	public Action<int> onBlockClick;

	private void Awake()
	{
		returnButton.onClick.AddListener(delegate
		{
			if (onReturnButton != null)
			{
				onReturnButton();
			}
		});
		craftWindow.GetComponent<CraftWindow>().returnButton.onClick.AddListener(delegate
		{
			if (onLesserReturnButton != null)
			{
				onLesserReturnButton();
			}
		});
		questWindow.GetComponent<QuestWindow>().returnButton.onClick.AddListener(delegate
		{
			if (onLesserReturnButton != null)
			{
				onLesserReturnButton();
			}
		});
	}

	public void Init(List<Resource> currentResources, List<Craftable> possibleCraftables, Action<int> onCraft)
	{
		InitCraftable(possibleCraftables);
		InitResources(currentResources);
		SetListState();
		craftWindow.GetComponent<CraftWindow>().SetCraftButton(onCraft);
	}

	private void InitCraftable(List<Craftable> possibleCraftables)
	{
		foreach (Craftable possibleCraftable in possibleCraftables)
		{
			int craftableId = possibleCraftable.id;
			GameObject gameObject = UnityEngine.Object.Instantiate(craftablePrefab, craftableListParent.transform);
			gameObject.SetActive(value: true);
			Sprite graphic = possibleCraftable.GetGraphic();
			gameObject.GetComponent<CraftItem>().Init(craftableId, Singleton<PlayerData>.get.playerItems.GetCraftableCount(craftableId), Manager.Get<CraftingManager>().GetCraftableStatus(craftableId), graphic);
			gameObject.GetComponent<Button>().onClick.AddListener(delegate
			{
				onBlockClick(craftableId);
			});
			gameObject.transform.SetAsLastSibling();
		}
	}

	private void InitResources(List<Resource> currentResources)
	{
		foreach (Resource currentResource in currentResources)
		{
			if (currentResource.id >= 0)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(resourcesPrefab, resourcesListParent.transform);
				gameObject.SetActive(value: true);
				Sprite image = Manager.Get<CraftingManager>().GetResourceDefinition(currentResource.id).GetImage();
				gameObject.GetComponent<CraftItem>().Init(currentResource.id, Singleton<PlayerData>.get.playerItems.GetResourcesCount(currentResource.id), CraftableStatus.Undefined, image, "x{0}");
				gameObject.transform.SetAsLastSibling();
			}
		}
	}

	public void UpdateAfterCrafting(int itemCrafted)
	{
		UpdateResourcesList();
		UpdateCraftableList();
		craftWindow.GetComponent<CraftWindow>().SetNnumberOfItems(itemCrafted);
	}

	public void UpdateResourcesList()
	{
		IEnumerator enumerator = resourcesListParent.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform trans = (Transform)enumerator.Current;
				CraftItem componentOnObject = trans.GetComponentOnObject<CraftItem>(showErrorInConsole: false);
				if (!(componentOnObject == null))
				{
					int resourcesCount = Singleton<PlayerData>.get.playerItems.GetResourcesCount(componentOnObject.id);
					componentOnObject.Reinitialize(resourcesCount, CraftableStatus.Undefined);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void EnableCraftButton(bool enabled)
	{
		craftWindow.GetComponent<CraftWindow>().craftButton.gameObject.SetActive(enabled);
	}

	public void UpdateCraftableList()
	{
		IEnumerator enumerator = craftableListParent.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform trans = (Transform)enumerator.Current;
				CraftItem componentOnObject = trans.GetComponentOnObject<CraftItem>(showErrorInConsole: false);
				if (!(componentOnObject == null))
				{
					int craftableCount = Singleton<PlayerData>.get.playerItems.GetCraftableCount(componentOnObject.id);
					componentOnObject.Reinitialize(craftableCount, Manager.Get<CraftingManager>().GetCraftable(componentOnObject.id).status);
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private void DeactivateAll()
	{
		questWindow.gameObject.SetActive(value: false);
		craftWindow.gameObject.SetActive(value: false);
		allBlocksWindow.gameObject.SetActive(value: false);
	}

	public void SetListState()
	{
		DeactivateAll();
		allBlocksWindow.gameObject.SetActive(value: true);
	}

	public void SetCraftingState(int blockId)
	{
		DeactivateAll();
		EnableCraftButton(enabled: true);
		craftWindow.gameObject.SetActive(value: true);
		craftWindow.GetComponent<CraftWindow>().Init(blockId);
	}

	public void SetQuestState(int itemId)
	{
		DeactivateAll();
		questWindow.gameObject.SetActive(value: true);
		questWindow.GetComponent<QuestWindow>().Init(itemId);
	}
}
