// DecompilerFi decompiler from Assembly-CSharp.dll class: FishCatchedStats
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

public class FishCatchedStats : MonoBehaviour
{
	public Animator fishingButtonAnimator;

	public Image backgroundImage;

	public Image raritySymbolBackgroundImage;

	public Image raritySymbolImage;

	public Image fishSpriteUnlocked;

	public Text fishName;

	public Text fishWeight;

	public Text fishPoints;

	public Text fishRarity;

	public Color commonColor;

	public Color rareColor;

	public Color epicColor;

	public Color legendaryColor;

	private FishingManager fishingManager;

	private Color currentColor;

	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
		animator.updateMode = AnimatorUpdateMode.UnscaledTime;
		fishingButtonAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
	}

	public void SetUp(Fish fish)
	{
		fishingManager = Manager.Get<FishingManager>();
		SetFishUnlockedStatus(fish);
		currentColor = GetColorByRarity(fish.rarity, fish.catched);
		backgroundImage.color = currentColor;
		raritySymbolBackgroundImage.color = currentColor;
		raritySymbolImage.color = currentColor;
	}

	public void ShowFishStats()
	{
		animator.SetBool("Show", value: true);
		fishingButtonAnimator.SetBool("MoveLeft", value: true);
	}

	public void HideFishStats()
	{
		animator.SetBool("Show", value: false);
		fishingButtonAnimator.SetBool("MoveLeft", value: false);
	}

	private void SetFishUnlockedStatus(Fish fish)
	{
		fishName.text = fish.name;
		fishWeight.text = ((float)fish.catchedWeight / 1000f).ToString("F2") + "kg";
		fishPoints.text = fish.catchedPoints.ToString();
		fishRarity.text = fishingManager.fishesRarity[fish.rarity - 1].symbolName;
		fishSpriteUnlocked.sprite = fish.icon;
		raritySymbolImage.sprite = fishingManager.fishesRarity[fish.rarity - 1].symbolIcon;
	}

	private Color GetColorByRarity(int rarity, bool catched)
	{
		switch (rarity)
		{
		case 3:
			return rareColor;
		case 4:
			return epicColor;
		case 5:
			return legendaryColor;
		default:
			return commonColor;
		}
	}
}
