// DecompilerFi decompiler from Assembly-CSharp.dll class: FurnitureItemCompleteWorker
using Common.Managers;
using GameUI;
using States;
using Uniblocks;
using UnityEngine;

public class FurnitureItemCompleteWorker : ScrollableItemWorker
{
	private VoxelDataElement furniture;

	private FurnitureItemConnector connector;

	public override void Work(ScrollableListElement element)
	{
		furniture = (element as VoxelDataElement);
		connector = (element.connector as FurnitureItemConnector);
		connector.furnitureIcon.sprite = furniture.icon;
		bool flag = false;
		VoxelDataElement voxelDataElement = furniture;
		FurnitureItemConnector furnitureItemConnector = connector;
		connector.bottomButton.onClick.RemoveAllListeners();
		if (IsLocked())
		{
			SetLockedButtonLayout(furniture, connector);
			flag = false;
		}
		else
		{
			SetBoughtButtonLayout(furniture, connector);
			flag = true;
		}
		if (!flag)
		{
			connector.disabledGO.SetActive(value: true);
			return;
		}
		if (ExclamationMarkController.ExclamationMarkShown(furniture.name))
		{
			connector.disabledGO.SetActive(value: false);
			return;
		}
		ExclamationMarkController.ShowExclamationMark(furniture.name);
		connector.disabledGO.SetActive(value: true);
		connector.disabledGO.GetComponentInChildren<Animator>().SetTrigger("play");
	}

	private void SetBoughtButtonLayout(VoxelDataElement furniture, FurnitureItemConnector connector)
	{
		connector.buildButtonGO.SetActive(value: true);
		connector.unlockButtonGO.SetActive(value: false);
		ushort voxel = furniture.voxel.GetUniqueID();
		connector.bottomButton.onClick.AddListener(delegate
		{
			ExampleInventory.HeldBlock = voxel;
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		});
		connector.buttonColorController.category = ColorManager.ColorCategory.HIGHLIGHT_COLOR;
		connector.buttonColorController.UpdateColor();
	}

	private void SetLockedButtonLayout(VoxelDataElement furniture, FurnitureItemConnector connector)
	{
		connector.buildButtonGO.SetActive(value: false);
		connector.unlockButtonGO.SetActive(value: true);
		connector.bottomButton.onClick.AddListener(delegate
		{
			Manager.Get<RewardedAdsManager>().ShowRewardedAd(StatsManager.AdReason.XCRAFT_UNLOCK_CRAFTABLE, delegate(bool b)
			{
				if (b)
				{
					string key = $"{furniture.name}.ads";
					if (!AutoRefreshingStock.HasItem(key))
					{
						AutoRefreshingStock.InitStockItem(key, float.NaN, int.MaxValue, 1);
					}
					else
					{
						AutoRefreshingStock.IncrementStockCount(key);
					}
					furniture.isDirty = true;
					connector.bottomButton.onClick.RemoveAllListeners();
				}
			});
		});
		connector.buttonColorController.category = ColorManager.ColorCategory.MAIN_COLOR;
		connector.buttonColorController.UpdateColor();
	}

	private bool IsLocked()
	{
		return AutoRefreshingStock.GetStockCount($"{furniture.name}.ads", float.NaN, int.MaxValue, 0) <= 0;
	}
}
