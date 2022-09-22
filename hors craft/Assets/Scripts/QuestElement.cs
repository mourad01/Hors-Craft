// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestElement
using Common.Managers;
using Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;

public class QuestElement : MonoBehaviour
{
	public Text titleText;

	public Text progressText;

	public Text buttonCoinText;

	public Button claimButton;

	public Image progressQuestProgress;

	public GameObject buttonHolder;

	public GameObject progressBarHolder;

	public Image questTarget;

	public bool isReadyForClaim;

	public void InitializeWithData(Quest data, int progress, RectTransform buttonParent, Action<Quest> onClaimClick)
	{
		titleText.text = Manager.Get<QuestManager>().GetQuestDescription(data.type, data.stepsNeeded);
		if (Manager.Get<ModelManager>().chestSettings.GetChestSpawnsResources() == 1f)
		{
			int chestCoins = Manager.Get<ModelManager>().chestSettings.GetChestCoins();
			buttonCoinText.text = chestCoins.ToString();
		}
		else
		{
			buttonCoinText.text = data.prize.ToString();
		}
		int num = Mathf.RoundToInt((float)progress / (float)data.stepsNeeded * 100f);
		progressText.text = $"{progress}/{data.stepsNeeded} ({num}%)";
		progressQuestProgress.fillAmount = (float)progress / (float)data.stepsNeeded;
		buttonHolder.SetActive(progress >= data.stepsNeeded);
		progressBarHolder.SetActive(progress < data.stepsNeeded);
		isReadyForClaim = (progress >= data.stepsNeeded);
		if (isReadyForClaim)
		{
			GetComponent<Animator>().SetTrigger("Claim");
			GetComponent<Animator>().SetTrigger("Normal");
		}
		Sprite spriteForQuest = GetSpriteForQuest(data);
		if (spriteForQuest != null)
		{
			questTarget.gameObject.SetActive(value: true);
			questTarget.sprite = spriteForQuest;
		}
		else
		{
			questTarget.gameObject.SetActive(value: false);
		}
		questTarget.color = Color.white;
		claimButton.onClick.RemoveAllListeners();
		claimButton.onClick.AddListener(delegate
		{
			onClaimClick(data);
		});
	}

	private Sprite GetSpriteForQuest(Quest data)
	{
		switch (QuestManager.GetNeededParameter(data.type))
		{
		case ParameterType.Block:
			if (!data.paramatersInt.IsNullOrEmpty())
			{
				return VoxelSprite.GetVoxelSprite((ushort)data.paramatersInt[0]);
			}
			break;
		case ParameterType.Animal:
			if (!data.paramatersString.IsNullOrEmpty())
			{
				return Manager.Get<MobsManager>().GetMobSpriteFromContainer(data.paramatersString[0]);
			}
			break;
		case ParameterType.Craftable:
			if (!data.paramatersInt.IsNullOrEmpty())
			{
				return Manager.Get<CraftingManager>().GetCraftableGraphic(data.paramatersInt[0]);
			}
			break;
		case ParameterType.Resource:
			if (!data.paramatersInt.IsNullOrEmpty())
			{
				return Manager.Get<CraftingManager>().GetResourceDefinition(data.paramatersInt[0]).GetImage();
			}
			break;
		}
		return null;
	}
}
