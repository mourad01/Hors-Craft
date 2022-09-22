// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftWindow
using Common.Managers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindow : MonoBehaviour
{
	public delegate void OnClick();

	public GameObject neededResourcePrefab;

	public GameObject neededResourcesParent;

	public Image previewImage;

	public Text numberOfItems;

	public OnClick onReturnButton;

	public Button returnButton;

	public Button craftButton;

	private int itemIdToCraft;

	public void Init(int itemId)
	{
		itemIdToCraft = itemId;
		Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(itemId);
		previewImage.sprite = ((!(craftable.sprite == null)) ? craftable.sprite : VoxelSprite.GetVoxelSprite((ushort)craftable.blockId));
		createNeededResources(craftable);
		craftButton.gameObject.SetActive(craftable.status == CraftableStatus.Craftable);
		SetNnumberOfItems(itemId);
	}

	public void SetCraftButton(Action<int> onCraftButton)
	{
		craftButton.onClick.AddListener(delegate
		{
			if (onCraftButton != null)
			{
				onCraftButton(itemIdToCraft);
			}
		});
	}

	public void SetNnumberOfItems(int itemId)
	{
		numberOfItems.text = Mathf.Max(0, Singleton<PlayerData>.get.playerItems.GetCraftableCount(itemId), 0).ToString();
	}

	private void createNeededResources(Craftable craftable)
	{
		destroyCurrentReources();
		for (int i = 0; i < craftable.requiredResourcesToCraft.Count; i++)
		{
			CreateElement(craftable.requiredResourcesToCraft[i].count).GetComponent<Image>().sprite = Manager.Get<CraftingManager>().GetResourceImage(craftable.requiredResourcesToCraft[i].id);
		}
		for (int j = 0; j < craftable.requiredCraftableToCraft.Count; j++)
		{
			CreateElement(craftable.requiredResourcesToCraft[j].count).GetComponent<Image>().sprite = Manager.Get<CraftingManager>().GetCraftable(craftable.requiredCraftableToCraft[j].id).GetGraphic();
		}
	}

	private GameObject CreateElement(int counter)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(neededResourcePrefab);
		gameObject.transform.parent = neededResourcesParent.transform;
		gameObject.GetComponentInChildren<Text>().text = $"x{counter}";
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private void destroyCurrentReources()
	{
		IEnumerator enumerator = neededResourcesParent.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				UnityEngine.Object.Destroy(transform.gameObject);
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
}
