// DecompilerFi decompiler from Assembly-CSharp.dll class: Device
using Common.Managers;
using Common.Utils;
using Cooking;
using Gameplay;
using System;
using System.Linq;
using UnityEngine;

public abstract class Device : CookingBaseObject, IUsable
{
	public enum UpgradeEffect
	{
		SPEED,
		CAPACITY,
		COINS,
		BURN_TIME,
		PATIENCE,
		WALKING_SPEED,
		TIPS
	}

	public enum Category
	{
		FOOD = 0,
		DEVICE = 1,
		FURNITURE = 2,
		NONE = 999
	}

	[Serializable]
	public class UpgradeConfig
	{
		public GameObject prefab;

		public int minLevel;
	}

	public Category category;

	public UpgradeConfig[] upgradePrefabs;

	public UpgradeEffect[] upgradeEffects;

	public Transform visualizationParent;

	public bool unlockedFromBeginning;

	public override int Price
	{
		get
		{
			if (upgradeEffects.Contains(UpgradeEffect.COINS))
			{
				return Mathf.RoundToInt(Manager.Get<ModelManager>().cookingSettings.GetUpgradeValue(base.Key, Misc.CustomNameToKey(UpgradeEffect.COINS.ToString()), GetUpgradeLevel()));
			}
			return 0;
		}
	}

	public int MaxLevel => Manager.Get<ModelManager>().cookingSettings.GetDeviceMaxUpgradeLevel(base.Key);

	public int GetPrice()
	{
		return Price;
	}

	public string GetKey()
	{
		return base.Key;
	}

	public Sprite GetSprite()
	{
		return sprite;
	}

	public virtual Product SpawnNewProduct(Product baseProduct, IUsable usable = null)
	{
		if (usable == null)
		{
			usable = this;
		}
		GameObject gameObject = base.workController.recipesList.GetResult(baseProduct, usable);
		if (gameObject == null)
		{
			gameObject = base.workController.recipesList.burntDish;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, base.transform, worldPositionStays: false);
		Product component = gameObject2.GetComponent<Product>();
		component.Init(GetUpgradeLevel());
		return gameObject2.GetComponent<Product>();
	}

	public virtual bool Unlocked()
	{
		if (base.workController.workData.IsDeviceUnlocked(base.Key))
		{
			return true;
		}
		if (unlockedFromBeginning)
		{
			Unlock();
			return true;
		}
		if (GetPrestigeRequired() <= base.workController.workData.prestigeLevel && GetUnlockPrice() == 0)
		{
			Unlock();
			return true;
		}
		return false;
	}

	public int GetUpgradePrice()
	{
		return Manager.Get<ModelManager>().cookingSettings.GetUpgradePrice(base.Key, GetUpgradeLevel() + 1);
	}

	public int GetUnlockPrice()
	{
		return Manager.Get<ModelManager>().cookingSettings.GetDeviceUnlockPrice(base.Key);
	}

	public int GetUpgradeLevel()
	{
		return base.workController.workData.GetUpgradeLevel(base.Key);
	}

	public void IncrementUpgradeLevel()
	{
		base.workController.workData.SetUpgradeLevel(base.Key, GetUpgradeLevel() + 1);
		Init();
	}

	public void Unlock()
	{
		base.workController.workData.UnlockDevice(base.Key);
		Init();
	}

	public int GetPrestigeRequired()
	{
		return Manager.Get<ModelManager>().cookingSettings.GetDevicePrestigeRequirement(base.Key);
	}

	protected abstract void SetUpgradeValues(UpgradeEffect effect, float value);

	protected virtual void Start()
	{
		Init();
	}

	public void Init()
	{
		int upgradeLevel = GetUpgradeLevel();
		SetVisualization(upgradeLevel);
		UpgradeEffect[] array = upgradeEffects;
		for (int i = 0; i < array.Length; i++)
		{
			UpgradeEffect effect = array[i];
			float upgradeValue = Manager.Get<ModelManager>().cookingSettings.GetUpgradeValue(base.Key, Misc.CustomNameToKey(effect.ToString()), base.workController.workData.GetUpgradeLevel(base.Key));
			SetUpgradeValues(effect, upgradeValue);
		}
	}

	protected virtual void SetVisualization(int upgrade)
	{
		if (!(visualizationParent == null) && upgradePrefabs != null && upgradePrefabs.Length != 0)
		{
			while (visualizationParent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(visualizationParent.GetChild(0).gameObject);
			}
			GameObject prefab = (from up in upgradePrefabs
				where upgrade >= up.minLevel
				orderby up.minLevel descending
				select up).First().prefab;
			if (prefab != null)
			{
				UnityEngine.Object.Instantiate(prefab, visualizationParent, worldPositionStays: false);
			}
		}
	}
}
