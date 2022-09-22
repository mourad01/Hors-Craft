// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GrowthManager
using Common.Managers;
using Common.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class GrowthManager : Manager, IGameCallbacksListener
	{
		private const float CHECK_EVERY = 5f;

		private float nextCheck;

		private Dictionary<string, GrowableInfo> growablesDictionary = new Dictionary<string, GrowableInfo>();

		private Dictionary<string, GrowthData> growthData = new Dictionary<string, GrowthData>();

		private List<string> keys = new List<string>(100);

		private Dictionary<string, GrowthData> savedGrowthData
		{
			get
			{
				string text = string.Empty;
				if (WorldPlayerPrefs.get.HasString("grower.data"))
				{
					text = WorldPlayerPrefs.get.GetString("grower.data");
				}
				if (string.IsNullOrEmpty(text))
				{
					return new Dictionary<string, GrowthData>();
				}
				return JSONHelper.Deserialize<Dictionary<string, GrowthData>>(text);
			}
			set
			{
				string value2 = JSONHelper.ToJSON(value);
				WorldPlayerPrefs.get.SetString("grower.data", value2);
			}
		}

		public override void Init()
		{
			nextCheck = Time.time + 5f;
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
		}

		public void Register(GrowableInfo info, string id = "", int initialStage = 0)
		{
			string uniqueId = info.GetUniqueId();
			if (string.IsNullOrEmpty(id))
			{
				id = info.settingsKey;
			}
			growablesDictionary[uniqueId] = info;
			growthData[uniqueId] = new GrowthData
			{
				id = id,
				boosted = false,
				currentStage = initialStage
			};
			CheckGrowable(uniqueId);
		}

		public void Unregister(GrowableInfo info)
		{
			Unregister(info.GetUniqueId());
		}

		public void Unregister(string key)
		{
			if (growablesDictionary.ContainsKey(key))
			{
				GrowableInfo growableInfo = growablesDictionary[key];
				for (int i = 0; i < growableInfo.growStages.Length + 2; i++)
				{
					string key2 = $"{key}.{i}";
					AutoRefreshingStock.RemoveItem(key2);
				}
				growablesDictionary.Remove(key);
				RemoveFromData(key);
			}
		}

		public void Clear(string key)
		{
			for (int i = 0; i < 100; i++)
			{
				string key2 = $"{key}.{i}";
				AutoRefreshingStock.RemoveItem(key2);
			}
		}

		public int GetGrowthStage(string key)
		{
			if (growthData.ContainsKey(key))
			{
				return growthData[key].currentStage;
			}
			return -1;
		}

		public GrowableInfo GetGrowableInfo(string key)
		{
			if (growablesDictionary.ContainsKey(key))
			{
				return growablesDictionary[key];
			}
			return null;
		}

		public GrowthData GetGrowthData(string key)
		{
			if (growthData.ContainsKey(key))
			{
				return growthData[key];
			}
			return null;
		}

		public void Boost(string key, int timeSkip)
		{
			growthData[key].boosted = true;
			GrowableInfo growableInfo = growablesDictionary[key];
			if (growableInfo.CurrentStage() != null)
			{
				float b = growableInfo.CurrentStage().GrowDuration(growableInfo) - (float)timeSkip;
				b = Mathf.Max(0f, b);
				AutoRefreshingStock.UpdateStockItemWithValues(key, b, 1);
			}
			CheckGrowable(key);
		}

		public void FastForward(float time)
		{
			foreach (string key in growablesDictionary.Keys)
			{
				FastForward(key, time);
			}
		}

		public void FastForward(string key, float time)
		{
			CheckGrowable(key, time);
		}

		private void RemoveFromData(string key)
		{
			if (growthData.ContainsKey(key))
			{
				growthData.Remove(key);
			}
		}

		private void Update()
		{
			if (Time.time > nextCheck)
			{
				UpdateGrowth();
				nextCheck = Time.time + 5f;
			}
		}

		private void UpdateGrowth()
		{
			keys.Clear();
			foreach (string key in growablesDictionary.Keys)
			{
				keys.Add(key);
			}
			foreach (string key2 in keys)
			{
				CheckGrowable(key2);
			}
		}

		private void CheckGrowable(string key, float timeBonus = 0f)
		{
			GrowableInfo growableInfo = growablesDictionary[key];
			GrowthData growthData = this.growthData[key];
			float num = 0f;
			bool flag = timeBonus != 0f;
			for (int i = growthData.currentStage; i < growableInfo.growStages.Length; i++)
			{
				GrowthStage growthStage = growableInfo.growStages[i];
				num += growthStage.GrowDuration(growableInfo);
				string key2 = $"{key}.{i}";
				if (flag)
				{
					float num2 = Mathf.Max(0f, num - timeBonus);
					float num3 = num2 - num;
					num = num2;
					timeBonus += num3;
					AutoRefreshingStock.UpdateStockItemWithValues(key2, num, 1);
				}
				int stockCount = AutoRefreshingStock.GetStockCount(key2, num, 1, 0);
				if (stockCount == 1)
				{
					Grow(key);
				}
			}
		}

		public int TimeToFullyGrown(string key)
		{
			GrowableInfo growableInfo = growablesDictionary[key];
			string key2 = $"{key}.{growableInfo.growStages.Length - 1}";
			if (AutoRefreshingStock.HasItem(key2))
			{
				return (int)AutoRefreshingStock.GetNextItemCooldown(key2);
			}
			return 0;
		}

		public void GrowOnce(string key)
		{
			GrowableInfo growableInfo = growablesDictionary[key];
			int num = growableInfo.CurrentStageNumber();
			string key2 = $"{key}.{num}";
			float time = AutoRefreshingStock.GetNextItemCooldown(key2) + 0.1f;
			FastForward(key, time);
		}

		public void ForceGrow(string key)
		{
			Grow(key);
		}

		private void Grow(string key)
		{
			if (growablesDictionary.ContainsKey(key))
			{
				GrowableInfo growableInfo = growablesDictionary[key];
				growableInfo.Grow();
				GrowthStage growthStage = growableInfo.CurrentStage();
				if (growthStage != null)
				{
					growableInfo.CurrentStage().OnGrowth(growableInfo);
				}
				growthData[key].currentStage++;
			}
		}

		public void OnGameplayStarted()
		{
			growthData = savedGrowthData;
		}

		public void OnGameplayRestarted()
		{
			growthData = new Dictionary<string, GrowthData>();
			growablesDictionary = new Dictionary<string, GrowableInfo>();
		}

		public void OnGameSavedFrequent()
		{
		}

		public void OnGameSavedInfrequent()
		{
			savedGrowthData = growthData;
		}
	}
}
