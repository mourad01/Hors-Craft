// DecompilerFi decompiler from Assembly-CSharp.dll class: CurrencyPackage
using Common.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyPackage : MonoBehaviour
{
	public Text titleText;

	public Text currencyCountText;

	public Text labelText;

	public Text priceText;

	public Button buyButton;

	public GameObject label;

	public Image mainImage;

	public PackageData packageData;

	public void Init(CurrencyShopConnector parent, PackageData data, Action<string> onButton, Action onVideo)
	{
		packageData = data;
		titleText.text = Manager.Get<TranslationsManager>().GetText(data.packageTitle, "Pack");
		currencyCountText.text = data.currencyCount.ToString();
		if (!string.IsNullOrEmpty(data.labelText) && !data.labelText.Equals("none"))
		{
			label.SetActive(value: true);
			labelText.text = Manager.Get<TranslationsManager>().GetText(data.labelText, string.Empty);
		}
		else
		{
			label.SetActive(value: false);
		}
		buyButton.onClick.RemoveAllListeners();
		buyButton.onClick.AddListener(delegate
		{
			if (data.isCostVideo)
			{
				onVideo();
			}
			else
			{
				onButton(data.iapIdentifier);
			}
		});
		mainImage.sprite = parent.GetSpriteOfName(data.imageName);
		if (!data.isCostVideo)
		{
			SetRealCost(data.iapIdentifier);
		}
		else
		{
			SetButtonVideo();
		}
	}

	private void SetButtonVideo()
	{
		priceText.text = string.Empty;
		buyButton.GetComponentInChildren<HorizontalLayoutGroup>(includeInactive: true).gameObject.SetActive(value: true);
	}

	private void SetRealCost(string iap)
	{
		buyButton.GetComponentInChildren<HorizontalLayoutGroup>(includeInactive: true).gameObject.SetActive(value: false);
		priceText.gameObject.SetActive(value: true);
		if (string.IsNullOrEmpty(priceText.text))
		{
			priceText.text = Singleton<UltimateCraftModelDownloader>.get.GetPriceString(iap);
		}
	}
}
