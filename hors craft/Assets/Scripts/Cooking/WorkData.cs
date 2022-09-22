// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.WorkData
using Common.Managers;
using Gameplay;
using System;
using UnityEngine;

namespace Cooking
{
	public class WorkData : MonoBehaviour
	{
		private int lastWave;

		private float lastPrestige;

		private int lastPrestigeLevel = 1;

		private int lastMoney;

		private string key => "kitchen." + Manager.Get<CookingManager>().GetKitchenKey();

		public int wave
		{
			get
			{
				return lastWave;
			}
			set
			{
				lastWave = value;
				PlayerPrefs.SetInt(key + ".wave", value);
			}
		}

		public float prestige
		{
			get
			{
				return lastPrestige;
			}
			set
			{
				lastPrestige = value;
				while (lastPrestige > (float)prestigeToLevelUp)
				{
					lastPrestige -= prestigeToLevelUp;
					LevelUp();
				}
				PlayerPrefs.SetFloat(key + ".prestige", lastPrestige);
				Manager.Get<CookingManager>().IsProgressDirty = true;
			}
		}

		public int prestigeToLevelUp
		{
			get
			{
				float baseExpFactor = Manager.Get<ModelManager>().cookingSettings.GetBaseExpFactor();
				return Mathf.RoundToInt((float)Math.Sinh((float)prestigeLevel * baseExpFactor + 1f) * 10f);
			}
		}

		public int prestigeLevel
		{
			get
			{
				return lastPrestigeLevel;
			}
			set
			{
				lastPrestigeLevel = value;
				PlayerPrefs.SetInt(key + ".prestige.level", value);
			}
		}

		public int money
		{
			get
			{
				return lastMoney;
			}
			set
			{
				lastMoney = value;
				PlayerPrefs.SetInt(key + ".money", value);
				Manager.Get<CookingManager>().IsProgressDirty = true;
			}
		}

		private void Awake()
		{
			lastWave = PlayerPrefs.GetInt(key + ".wave", 0);
			lastPrestige = PlayerPrefs.GetFloat(key + ".prestige", 0f);
			lastPrestigeLevel = PlayerPrefs.GetInt(key + ".prestige.level", 1);
			lastMoney = PlayerPrefs.GetInt(key + ".money", 0);
			UnlockLevel(1);
		}

		public int PortionsLeft(BaseIngredientDevice device)
		{
			return PlayerPrefs.GetInt(key + ".portions." + device.GetKey(), device.portionsMax);
		}

		public void SetPortionsLeft(Device device, int number)
		{
			PlayerPrefs.SetInt(key + ".portions." + device.GetKey(), number);
		}

		public bool IsDeviceUnlocked(string device)
		{
			return PlayerPrefs.GetInt(key + "." + device + ".unlocked", 0) == 1;
		}

		public void UnlockDevice(string device)
		{
			PlayerPrefs.SetInt(key + "." + device + ".unlocked", 1);
		}

		public bool WasProductTutorialShown(string product)
		{
			return PlayerPrefs.GetInt(key + "." + product + ".tutorial", 0) == 1;
		}

		public void ProductTutorialShown(string product)
		{
			PlayerPrefs.SetInt(key + "." + product + ".tutorial", 1);
		}

		public int GetUpgradeLevel(string device)
		{
			return PlayerPrefs.GetInt(key + "." + device, 0);
		}

		public void SetUpgradeLevel(string device, int value)
		{
			PlayerPrefs.SetInt(key + "." + device, value);
		}

		public bool IsLevelUnlocked(int id)
		{
			return PlayerPrefs.GetInt(key + ".unlocked.level." + id, 0) == 1;
		}

		public void UnlockLevel(int id)
		{
			PlayerPrefs.SetInt(key + ".unlocked.level." + id, 1);
		}

		public int GetLevelStars(int id)
		{
			return PlayerPrefs.GetInt(key + ".stars.level." + id, 0);
		}

		public void SetLevelStars(int id, int numOfStars)
		{
			PlayerPrefs.SetInt(key + ".stars.level." + id, numOfStars);
		}

		private void LevelUp()
		{
			prestigeLevel++;
		}
	}
}
