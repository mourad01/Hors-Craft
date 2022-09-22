// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldShopElement
using Common.Managers;
using GameUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldShopElement : MonoBehaviour
{
	public Button mainButton;

	public Image mainScreen;

	public GameObject blocksArray;

	public GameObject blockPrefab;

	public GameObject costText;

	public GameObject playText;

	public GameObject coin;

	public Text titleText;

	public Text descriptionText;

	public VerticalLayoutGroup textLayout;

	public Text topLabelText;

	public Image topLabel;

	public Image frame;

	public Color baseFrameOffset;

	public string worldId;

	private SpawnerPool pool;

	private GameObject[] spawned;

	private void Awake()
	{
		pool = new SpawnerPool("blocks", blockPrefab);
	}

	public void Init(string uniqueId, Sprite mainImage, int[] craftableId, int cost, string title, string description, HashSet<string> tags, bool owned, Action buttonAction)
	{
		worldId = uniqueId;
		mainScreen.sprite = mainImage;
		titleText.text = title;
		descriptionText.text = description;
		costText.GetComponent<Text>().text = cost.ToString();
		PrepareLabel(CheckTag(tags));
		InitButton(owned, buttonAction);
		CreateBlocks(craftableId);
	}

	public string CheckTag(HashSet<string> tags)
	{
		if (tags == null)
		{
			return "none";
		}
		if (tags.Contains("new"))
		{
			return "new";
		}
		if (tags.Contains("popular"))
		{
			return "popular";
		}
		return "none";
	}

	public void SetBlocksSpacing()
	{
		if (blocksArray.transform.childCount != 0)
		{
			float width = blocksArray.GetComponent<RectTransform>().rect.width;
			float height = blocksArray.GetComponent<RectTransform>().rect.height;
			GameObject[] array = spawned;
			foreach (GameObject gameObject in array)
			{
				gameObject.GetComponent<LayoutElement>().minHeight = height;
				gameObject.GetComponent<LayoutElement>().minWidth = height;
			}
			blocksArray.GetComponent<HorizontalLayoutGroup>().spacing = (width - height * 5f) / 5f + 1f;
		}
	}

	private void CreateBlocks(int[] craftableId)
	{
		DespawnAllBlocks();
		if (craftableId != null)
		{
			spawned = new GameObject[craftableId.Length];
			for (int i = 0; i < craftableId.Length; i++)
			{
				spawned[i] = pool.Spawn(Vector3.zero, Quaternion.identity);
				Sprite graphic = Manager.Get<CraftingManager>().GetCraftable(craftableId[i]).GetGraphic();
				spawned[i].transform.SetParent(blocksArray.transform);
				spawned[i].transform.localScale = Vector3.one;
				spawned[i].transform.localPosition = Vector3.zero;
				spawned[i].GetComponent<Image>().sprite = graphic;
			}
		}
	}

	private void DespawnAllBlocks()
	{
		if (spawned != null)
		{
			for (int i = 0; i < spawned.Length; i++)
			{
				pool.Despawn(spawned[i]);
			}
		}
	}

	private void PrepareLabel(string type)
	{
		if (type.Equals("none"))
		{
			topLabel.gameObject.SetActive(value: false);
			Color colorForCategory = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.SECONDARY_COLOR);
			frame.color = new Color(colorForCategory.r * baseFrameOffset.r, colorForCategory.g * baseFrameOffset.g, colorForCategory.b * baseFrameOffset.b, 1f);
		}
		else
		{
			topLabel.gameObject.SetActive(Singleton<UltimateCraftModelDownloader>.get.GetTagVisiblity(type));
			frame.color = Singleton<UltimateCraftModelDownloader>.get.GetTagColor(type);
			topLabel.color = Singleton<UltimateCraftModelDownloader>.get.GetTagColor(type);
			topLabelText.text = Singleton<UltimateCraftModelDownloader>.get.GetTagText(type);
		}
	}

	public void InitButton(bool owned, Action buttonAction)
	{
		mainButton.onClick.RemoveAllListeners();
		mainButton.onClick.AddListener(delegate
		{
			if (buttonAction != null)
			{
				buttonAction();
			}
		});
		mainButton.image.color = Manager.Get<ColorManager>().GetColorForCategory(owned ? ColorManager.ColorCategory.HIGHLIGHT_COLOR : ColorManager.ColorCategory.MAIN_COLOR);
		costText.SetActive(!owned);
		coin.SetActive(!owned);
		playText.SetActive(owned);
	}
}
