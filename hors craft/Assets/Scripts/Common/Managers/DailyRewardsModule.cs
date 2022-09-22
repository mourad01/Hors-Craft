// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.DailyRewardsModule
using Common.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Common.Managers
{
	public class DailyRewardsModule : ModelModule
	{
		public struct RewardConfig
		{
			public int id;

			public int count;

			public int day;

			public static RewardConfig Empty()
			{
				RewardConfig result = default(RewardConfig);
				result.id = 0;
				result.count = 0;
				result.day = 0;
				return result;
			}
		}

		public const int MAX_REWARDS_CONFIGS = 31;

		public Action onModelDownloaded;

		private string[] _keysRewardsConfigs;

		private bool downlodedModel;

		private string defaultRewardingDays;

		private DailyRewardsManager.WrappingMode defaultWrapping;

		private string keyRewardingDays => "daily.rewards.days";

		private string keyWrapping => "daily.rewards.wraping";

		private string keyEnable => "daily.rewards.enable";

		private string keyRewardsConfigs => "daily.rewards.rewards";

		private string[] keysRewardsConfigs
		{
			get
			{
				if (_keysRewardsConfigs == null)
				{
					_keysRewardsConfigs = new string[31];
					for (int i = 0; i < 31; i++)
					{
						_keysRewardsConfigs[i] = keyRewardsConfigs + "." + (i + 1).ToString();
					}
				}
				return _keysRewardsConfigs;
			}
		}

		private string keyDelayedWindow => "daily.rewards.delayed.window";

		public DailyRewardsModule(string rewardingDays = "1,3,4,5,6,7", DailyRewardsManager.WrappingMode wrappingMode = DailyRewardsManager.WrappingMode.Loop)
		{
			defaultRewardingDays = rewardingDays;
			defaultWrapping = wrappingMode;
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyRewardingDays, defaultRewardingDays);
			descriptions.AddDescription(keyWrapping, defaultWrapping.ToString());
			descriptions.AddDescription(keyEnable, defaultValue: false);
			descriptions.AddDescription(keyDelayedWindow, defaultValue: false);
			AddRewardsDescription(descriptions);
		}

		private void AddRewardsDescription(ModelDescription descriptions)
		{
			for (int i = 0; i < keysRewardsConfigs.Length; i++)
			{
				descriptions.AddDescription(keysRewardsConfigs[i], "0;0");
			}
		}

		public override void OnModelDownloaded()
		{
			if (Manager.Contains<DailyRewardsManager>())
			{
				Manager.Get<DailyRewardsManager>().Init(this);
			}
			downlodedModel = true;
			if (onModelDownloaded != null)
			{
				onModelDownloaded();
			}
		}

		public int[] RewardingDays()
		{
			string @string = base.settings.GetString(keyRewardingDays);
			string[] array = @string.Split(',');
			List<int> list = new List<int>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (int.TryParse(array[i], NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
				{
					list.Add(result);
				}
				else
				{
					UnityEngine.Debug.LogWarning($"Daily_Rewards_Module: wrong rewarding days format at split {i}: given string is {array[i]}; all string is {@string}");
				}
			}
			return list.ToArray();
		}

		public DailyRewardsManager.WrappingMode Wrapping()
		{
			DailyRewardsManager.WrappingMode result = DailyRewardsManager.WrappingMode.Loop;
			try
			{
				result = (DailyRewardsManager.WrappingMode)Enum.Parse(typeof(DailyRewardsManager.WrappingMode), base.settings.GetString(keyWrapping), ignoreCase: true);
				return result;
			}
			catch (ArgumentException ex)
			{
				UnityEngine.Debug.LogError("DailyRewardsModule: Wrapping(): wrong string to enum WrappingMode\n" + ex.Message);
				return result;
			}
		}

		public int MaxDaysInRow()
		{
			int[] array = RewardingDays();
			return array[array.Length - 1];
		}

		public bool Enable()
		{
			return base.settings.GetBool(keyEnable);
		}

		public bool IsDownloaded()
		{
			return downlodedModel;
		}

		public RewardConfig RewardConfigForDay(int day)
		{
			if (day > 31)
			{
				return RewardConfig.Empty();
			}
			string[] array = base.settings.GetString(keyRewardsConfigs + "." + day.ToString()).Split(';');
			if (array.Length < 2)
			{
				return RewardConfig.Empty();
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = RemoveWhitespace(array[i]);
			}
			RewardConfig result = default(RewardConfig);
			AssignRewardConfigValue(array[0], ref result.id);
			AssignRewardConfigValue(array[1], ref result.count);
			result.day = day;
			return result;
		}

		public RewardConfig[] RewardConfigs()
		{
			RewardConfig[] array = new RewardConfig[31];
			for (int i = 0; i < 31; i++)
			{
				array[i] = RewardConfigForDay(i + 1);
			}
			return array;
		}

		public bool IsDelayedWindow()
		{
			return base.settings.GetBool(keyDelayedWindow);
		}

		private void AssignRewardConfigValue(string stringValue, ref int toAssign)
		{
			int result = 0;
			if (int.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				toAssign = result;
			}
			else
			{
				toAssign = 0;
			}
		}

		public string RemoveWhitespace(string input)
		{
			return new string((from c in input.ToCharArray()
				where !char.IsWhiteSpace(c)
				select c).ToArray());
		}
	}
}
