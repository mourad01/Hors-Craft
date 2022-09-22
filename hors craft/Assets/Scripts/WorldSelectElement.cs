// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldSelectElement
using Common.Managers;
using Gameplay;
using GameUI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelectElement : MonoBehaviour
{
	public Image mainImage;

	public Image plusImage;

	public Image checkmark;

	public Texture emptyImage;

	public Image frame;

	public GameObject lastPlayed;

	public Text nameText;

	public Button button;

	public Image dowloadProgress;

	public WorldDownloader downloader;

	private WorldData data;

	public GameObject priceTag;

	public void Init(WorldData data, Action onAdd, Action<string> onClick)
	{
		this.data = data;
		EnableCheckmark(enable: false);
		if (data == null)
		{
			CreateEmpty(onAdd);
		}
		else
		{
			CreateFull(onClick);
		}
	}

	public WorldData GetData()
	{
		return data;
	}

	private void Update()
	{
		if (downloader != null)
		{
			dowloadProgress.fillAmount = downloader.GetProgress();
		}
	}

	public void SetPriceOnTag(int price)
	{
		if (!(priceTag == null))
		{
			priceTag.GetComponentInChildren<Text>().text = price.ToString();
			priceTag.SetActive(price > 0);
			priceTag.SetActive(value: false);
		}
	}

	public void CreateEmpty(Action onClick)
	{
		nameText.text = string.Empty;
		mainImage.GetComponent<Image>().material = new Material(mainImage.GetComponent<Image>().material);
		plusImage.gameObject.SetActive(value: true);
		mainImage.GetComponent<Image>().sprite = null;
		mainImage.GetComponent<Image>().color = Manager.Get<ColorManager>().colors[1].color;
		mainImage.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.None;
		mainImage.GetComponent<RectTransform>().anchorMin = new Vector2(0.05f, 0.05f);
		mainImage.GetComponent<RectTransform>().anchorMax = new Vector2(0.95f, 0.95f);
		mainImage.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		lastPlayed.gameObject.SetActive(value: false);
		button.onClick.AddListener(delegate
		{
			if (onClick != null)
			{
				onClick();
			}
		});
		if (Manager.Get<ModelManager>().worldsSettings.GetUnlockType() == ItemsUnlockModel.SoftCurrency)
		{
			SetPriceOnTag(Manager.Get<ModelManager>().worldsSettings.GetNewWorldCostCurrency());
		}
		else
		{
			SetPriceOnTag(0);
		}
	}

	public void EnableCheckmark(bool enable)
	{
		if (checkmark != null)
		{
			checkmark.gameObject.SetActive(enable);
		}
	}

	public void SetProgress(float progress)
	{
		dowloadProgress.fillAmount = progress;
	}

	public void Select(bool highlight)
	{
		int num = (!highlight) ? 1 : 0;
		frame.color = Manager.Get<ColorManager>().colors[num].color;
		if (data != null)
		{
			data.selected = highlight;
		}
	}

	private void CreateFull(Action<string> onClick)
	{
		int num = (!data.selected) ? 1 : 0;
		frame.color = Manager.Get<ColorManager>().colors[num].color;
		if (nameText != null)
		{
			nameText.text = data.name;
			if (data.name.Equals(Manager.Get<ConnectionInfoManager>().gameName))
			{
				nameText.text = "The World";
			}
		}
		data.TryToGetThumbnail(delegate(Sprite sprite)
		{
			if (!(mainImage == null))
			{
				mainImage.GetComponent<Image>().sprite = sprite;
				if (mainImage.GetComponent<Image>().sprite == null)
				{
					mainImage.GetComponent<Image>().color = Manager.Get<ColorManager>().colors[1].color;
				}
			}
		}, delegate(bool succes)
		{
			EnableCheckmark(succes);
		});
		if (plusImage != null)
		{
			plusImage.gameObject.SetActive(value: false);
		}
		if (lastPlayed != null)
		{
			lastPlayed.gameObject.SetActive(data.lastUsed);
		}
		mainImage.GetComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
		button.onClick.AddListener(delegate
		{
			if (onClick != null)
			{
				onClick(data.uniqueId);
			}
		});
		SetPriceOnTag(data.cost);
	}
}
