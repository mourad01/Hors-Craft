// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftItem
using Common.Managers;
using Gameplay;
using GameUI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CraftItem : MonoBehaviour
{
	public GameObject lockWithAnimationGO;

	public Image statusVisual;

	public Image mainSprite;

	public Text text;

	public GameObject blockObject;

	public GameObject adsObject;

	public GameObject LockObject;

	public GameObject borderObject;

	public GameObject newNotificationObject;

	public GameObject questNotification;

	[HideInInspector]
	public int id;

	[HideInInspector]
	public int counter;

	[HideInInspector]
	public CraftableStatus status;

	private string textToFill;

	private bool isUpgradeable;

	protected virtual Action<int> additionalAction => null;

	public virtual void Init(int id, int count, CraftableStatus status, Sprite sprite, string textToFill = "{0}", bool resource = false)
	{
		this.id = id;
		counter = count;
		this.textToFill = textToFill;
		if (sprite != null)
		{
			mainSprite.sprite = sprite;
		}
		text.text = string.Format(textToFill, Mathf.Max(0, count));
		if (resource)
		{
			return;
		}
		Craftable craftable = Manager.Get<CraftingManager>().GetCraftable(id);
		if (craftable == null)
		{
			UnityEngine.Debug.LogWarning("Null craftable!");
			return;
		}
		isUpgradeable = (craftable.craftableType == Craftable.type.Upgradeable);
		if (statusVisual != null || lockWithAnimationGO != null)
		{
			SetActiveByState(status);
		}
		if (HasToHideText(craftable))
		{
			text.transform.parent.gameObject.SetActive(value: false);
		}
		if (additionalAction != null)
		{
			additionalAction(id);
		}
	}

	public void EnableNotification(bool enable)
	{
		if (questNotification != null)
		{
			questNotification.SetActive(enable);
		}
	}

	public bool Locked()
	{
		return adsObject.gameObject.IsActive();
	}

	public virtual void Lock(bool enable)
	{
		adsObject.gameObject.SetActive(enable);
	}

	public void SetTextOnLock(string value)
	{
		Text componentInChildren = adsObject.GetComponentInChildren<Text>();
		if (componentInChildren != null)
		{
			componentInChildren.text = value;
		}
	}

	public void Reinitialize(int count, CraftableStatus status, bool resource = false)
	{
		text.text = string.Format(textToFill, Mathf.Max(0, count));
		if (!resource)
		{
			isUpgradeable = (Manager.Get<CraftingManager>().GetCraftable(id).craftableType == Craftable.type.Upgradeable);
			if (statusVisual != null || lockWithAnimationGO != null)
			{
				SetActiveByState(status);
			}
			if (isUpgradeable)
			{
				text.transform.parent.gameObject.SetActive(value: false);
			}
		}
	}

	public void ReintializeCraftable()
	{
		int craftableCount = Singleton<PlayerData>.get.playerItems.GetCraftableCount(id);
		CraftableStatus craftableStatus = Manager.Get<CraftingManager>().GetCraftableStatus(id);
		Reinitialize(craftableCount, craftableStatus);
	}

	public ColorManager.ColorCategory GetTextColorCategory(CraftableStatus status)
	{
		ColorManager.ColorCategory result = ColorManager.ColorCategory.FONT_COLOR;
		switch (status)
		{
		case CraftableStatus.Locked:
			result = ColorManager.ColorCategory.FONT_COLOR_RED;
			break;
		case CraftableStatus.NoResources:
			result = ColorManager.ColorCategory.FONT_COLOR_RED;
			break;
		case CraftableStatus.Craftable:
			result = ColorManager.ColorCategory.FONT_COLOR_GREEN;
			break;
		case CraftableStatus.FullyUpgraded:
			result = ColorManager.ColorCategory.FONT_COLOR_GREEN;
			break;
		case CraftableStatus.Undefined:
			result = ColorManager.ColorCategory.FONT_COLOR_WHITE;
			break;
		}
		return result;
	}

	public Color GetColorForStatus(CraftableStatus status)
	{
		return Manager.Get<ColorManager>().GetColorForCategory(GetTextColorCategory(status));
	}

	public void ChangeTextColor(CraftableStatus status)
	{
		ColorManager.ColorCategory textColorCategory = GetTextColorCategory(status);
		text.GetComponent<ColorController>().category = textColorCategory;
		text.GetComponent<ColorController>().UpdateColor();
	}

	private void SetActiveByState(CraftableStatus status)
	{
		this.status = status;
		switch (status)
		{
		case CraftableStatus.Locked:
			SetLock(enable: true);
			text.transform.parent.gameObject.SetActive(value: false);
			break;
		case CraftableStatus.NoResources:
			SetLock(enable: false);
			text.transform.parent.gameObject.SetActive(value: true);
			break;
		case CraftableStatus.Craftable:
			SetLock(enable: false);
			text.transform.parent.gameObject.SetActive(value: true);
			break;
		case CraftableStatus.Undefined:
			SetLock(enable: true);
			text.transform.parent.gameObject.SetActive(value: true);
			break;
		case CraftableStatus.FullyUpgraded:
			SetLock(enable: false);
			text.transform.parent.gameObject.SetActive(value: false);
			break;
		}
		ChangeTextColor(status);
	}

	private void SetLock(bool enable)
	{
		if (lockWithAnimationGO != null)
		{
			if (enable)
			{
				lockWithAnimationGO.SetActive(value: true);
				return;
			}
			if (ExclamationMarkController.ExclamationMarkShown(id.ToString()))
			{
				lockWithAnimationGO.SetActive(value: false);
				return;
			}
			ExclamationMarkController.ShowExclamationMark(id.ToString());
			lockWithAnimationGO.SetActive(value: true);
			lockWithAnimationGO.GetComponentInChildren<Animator>().SetTrigger("play");
		}
		else if (enable)
		{
			statusVisual.gameObject.SetActive(value: true);
		}
		else
		{
			statusVisual.gameObject.SetActive(value: false);
		}
	}

	private Color getColorForState(CraftableStatus status)
	{
		switch (status)
		{
		case CraftableStatus.Locked:
			return Color.red;
		case CraftableStatus.NoResources:
			return Color.yellow;
		case CraftableStatus.Craftable:
			return Color.green;
		case CraftableStatus.FullyUpgraded:
			return Color.green;
		case CraftableStatus.Undefined:
			return Color.clear;
		default:
			return Color.clear;
		}
	}

	private bool HasToHideText(Craftable craftable)
	{
		return isUpgradeable || (Manager.Get<ModelManager>().craftingSettings.AreBlueprintsFree() && craftable.recipeCategory == Craftable.RecipeCategory.BLUEPRINT) || (Manager.Get<ModelManager>().craftingSettings.AreCraftingFree() && (craftable.recipeCategory == Craftable.RecipeCategory.FURNITURE || craftable.recipeCategory == Craftable.RecipeCategory.OTHER));
	}
}
