// DecompilerFi decompiler from Assembly-CSharp.dll class: CurrencyShopConnector
using Common.Managers;
using Common.Managers.States.UI;
using GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyShopConnector : UIConnector
{
	public List<Sprite> packagesTextures;

	public GameObject packageHolder;

	public GameObject prefabPackage;

	public GameObject videoElement;

	public List<GameObject> createdPackages;

	public Text title;

	public Button returnButton;

	public void Init(List<PackageData> data, Action<string> onBuy, Action onVideo, Action onReturn)
	{
		data.ForEach(delegate(PackageData package)
		{
			CreatePackage(package, data.Count, onBuy, onVideo);
		});
		StartCoroutine(RevalidateAfterStart());
		SetReturnButton(onReturn);
	}

	private void SetReturnButton(Action onReturn)
	{
		returnButton.onClick.AddListener(delegate
		{
			if (onReturn == null)
			{
				Manager.Get<StateMachineManager>().PopState();
			}
			else
			{
				onReturn();
			}
		});
	}

	private IEnumerator RevalidateAfterStart()
	{
		yield return new WaitForEndOfFrame();
		RevalideSizeOfPackages();
		yield return new WaitForEndOfFrame();
		GameObject videoPackage = createdPackages.Find((GameObject package) => package.GetComponent<CurrencyPackage>().packageData.isCostVideo);
		if (videoPackage == null)
		{
			videoElement.SetActive(value: false);
			yield break;
		}
		videoElement.SetActive(value: true);
		videoElement.transform.SetPositionX(videoPackage.transform.GetPositionX());
	}

	private void CreatePackage(PackageData data, int count, Action<string> onBuy, Action onVideo)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefabPackage);
		gameObject.transform.localScale = Vector3.one;
		CurrencyPackage component = gameObject.GetComponent<CurrencyPackage>();
		component.transform.SetParent(packageHolder.transform);
		createdPackages.Add(gameObject);
		gameObject.transform.localScale = Vector3.one;
		SetSizeOfPackage(gameObject, count);
		component.Init(this, data, onBuy, onVideo);
	}

	private void Update()
	{
		SetVideoValues();
	}

	private void SetVideoValues()
	{
		Text[] componentsInChildren = videoElement.GetComponentsInChildren<Text>(includeInactive: true);
		PlayerPurchases.VideoTime videoInformation = Singleton<PlayerData>.get.playerPurchases.GetVideoInformation();
		TimeSpan timeSpan = TimeSpan.FromSeconds(videoInformation.seconds);
		componentsInChildren[0].text = string.Format(Manager.Get<TranslationsManager>().GetText("currency.videos.left", "VIDEOS LEFT: {0}"), videoInformation.numberOfAds);
		componentsInChildren[1].text = string.Format(Manager.Get<TranslationsManager>().GetText("currency.videos.time", "NEXT VIDEO IN: {0}"), $"{timeSpan:hh\\:mm\\:ss}");
		componentsInChildren[1].gameObject.SetActive(videoInformation.seconds > 0.0);
		Color colorForCategory = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.MAIN_COLOR);
		if (videoInformation.numberOfAds == 0)
		{
			colorForCategory = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.DISABLED_COLOR);
		}
		createdPackages[0].GetComponent<CurrencyPackage>().buyButton.GetComponent<Image>().color = colorForCategory;
	}

	private void SetSizeOfPackage(GameObject package, int totalCount)
	{
		float width = packageHolder.GetComponent<RectTransform>().rect.width;
		float height = packageHolder.GetComponent<RectTransform>().rect.height;
		float num = (width - width * 0.025f * (float)(totalCount - 1)) / (float)totalCount;
		float num2 = num * 1.33f;
		if (num2 > height)
		{
			num2 = height;
		}
		num = num2 * 0.75f;
		packageHolder.GetComponent<HorizontalLayoutGroup>().spacing = width * 0.025f;
		package.GetComponent<LayoutElement>().preferredWidth = num;
		package.GetComponent<LayoutElement>().preferredHeight = num2;
	}

	private void RevalideSizeOfPackages()
	{
		createdPackages.ForEach(delegate(GameObject package)
		{
			SetSizeOfPackage(package, createdPackages.Count);
			CurrencyPackage component = package.GetComponent<CurrencyPackage>();
			if (component.packageData.isCostVideo)
			{
				RectTransform component2 = videoElement.GetComponent<RectTransform>();
				Vector3 localPosition = videoElement.GetComponent<RectTransform>().localPosition;
				Vector3 localPosition2 = package.GetComponent<RectTransform>().localPosition;
				component2.localPosition = localPosition.WithX(localPosition2.x);
			}
		});
	}

	public Sprite GetSpriteOfName(string name)
	{
		return packagesTextures.Find((Sprite sprite) => sprite.name.ToUpper().Equals(name.ToUpper()));
	}
}
