// DecompilerFi decompiler from Assembly-CSharp.dll class: FishCollectionPrefab
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

public class FishCollectionPrefab : MonoBehaviour
{
	public GameObject fishStats;

	public GameObject fishUnknown;

	public Image backgroundImage;

	public Image rarityBackgroundImage;

	public Image raritySymbolBackgroundImage;

	public Image raritySymbolImage;

	public Image fishSpriteUnlocked;

	public Image fishSpriteLocked;

	public Text fishName;

	public Text fishWeight;

	public Text fishPoints;

	public Text fishRarity;

	public Color lockedColor;

	public Color commonColor;

	public Color rareColor;

	public Color epicColor;

	public Color legendaryColor;

	private FishingManager fishingManager;

	private Color currentColor;

	public void SetUp(Fish fish)
	{
		fishingManager = Manager.Get<FishingManager>();
		if (fish.rarity < 1)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		if (fish.catched)
		{
			SetFishUnlockedStatus(fish);
		}
		else
		{
			SetFishLockedStatus(fish);
		}
		currentColor = GetColorByRarity(fish.rarity, fish.catched);
		backgroundImage.color = currentColor;
		rarityBackgroundImage.color = currentColor;
		raritySymbolBackgroundImage.color = currentColor;
		raritySymbolImage.color = currentColor;
	}

	private void SetFishUnlockedStatus(Fish fish)
	{
		fishUnknown.SetActive(value: false);
		fishStats.SetActive(value: true);
		fishName.text = fish.name;
		fishWeight.text = ((float)fish.catchedWeight / 1000f).ToString("F2") + "kg";
		fishPoints.text = fish.catchedPoints.ToString();
		fishRarity.text = fishingManager.fishesRarity[fish.rarity - 1].symbolName;
		fishSpriteUnlocked.sprite = fish.icon;
		raritySymbolImage.sprite = fishingManager.fishesRarity[fish.rarity - 1].symbolIcon;
	}

	private void SetFishLockedStatus(Fish fish)
	{
		fishUnknown.SetActive(value: true);
		fishStats.SetActive(value: false);
		fishSpriteLocked.sprite = fish.icon;
	}

	private Color GetColorByRarity(int rarity, bool catched)
	{
		if (catched)
		{
			switch (rarity)
			{
			case 2:
				return commonColor;
			case 3:
				return rareColor;
			case 4:
				return epicColor;
			case 5:
				return legendaryColor;
			}
		}
		return lockedColor;
	}
}
