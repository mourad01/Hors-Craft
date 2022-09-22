// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.DailyRewardsManager
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Common.Managers
{
	public class DailyRewardsManager : Manager
	{
		public enum WrappingMode
		{
			Loop,
			Clamped,
			None
		}

		public const string DATE_SEPARATOR = "-";

		public const string KEY_LAST_LOG = "DRLastLog";

		public const string KEY_LAST_CLAIMED = "DRLastClaimed";

		public const string KEY_LOG_SPAN = "DRLogSpan";

		private DailyRewardsModule dailyRewardsModel;

		private DateTime _lastLogTime;

		private DateTime _lastClaimedTime;

		private int _daysInRow;

		[HideInInspector]
		private bool debugEnable = true;

		[HideInInspector]
		private int travelBackTime;

		[HideInInspector]
		private int travelForwardTime;

		private int todayChange;

		private bool customWrappingEnable;

		private WrappingMode customWrapping = WrappingMode.None;

		private string customReward = string.Empty;

		private bool useCustomRewards;

		[CompilerGenerated]
		private static Func<string, int> _003C_003Ef__mg_0024cache0;

		private DateTime today => DateTime.Today;

		public bool isEnable
		{
			get
			{
				if (dailyRewardsModel == null)
				{
					return false;
				}
				return dailyRewardsModel.Enable();
			}
		}

		public bool isDownloaded
		{
			get
			{
				if (dailyRewardsModel == null)
				{
					return false;
				}
				return dailyRewardsModel.IsDownloaded();
			}
		}

		private WrappingMode wrappingMode => dailyRewardsModel.Wrapping();

		private DateTime lastLogTime => _lastLogTime;

		private DateTime lastClaimedTime => _lastClaimedTime;

		public int daysInRow => _daysInRow;

		public int daysInRowWrapped
		{
			get
			{
				if (dailyRewardsModel == null)
				{
					return 0;
				}
				if (wrappingMode == WrappingMode.Clamped)
				{
					return Mathf.Clamp(_daysInRow, 1, dailyRewardsModel.MaxDaysInRow());
				}
				if (wrappingMode == WrappingMode.Loop)
				{
					return (_daysInRow - 1) % dailyRewardsModel.MaxDaysInRow() + 1;
				}
				if (wrappingMode == WrappingMode.None)
				{
					return _daysInRow;
				}
				return _daysInRow;
			}
		}

		public bool hasRewardToday
		{
			get
			{
				if (dailyRewardsModel == null || !isEnable || hasClaimedToday)
				{
					return false;
				}
				if (Array.IndexOf(dailyRewardsModel.RewardingDays(), daysInRowWrapped) > -1)
				{
					return true;
				}
				return false;
			}
		}

		public bool hasClaimedToday => GetDaysSpan(lastClaimedTime) == 0;

		public DailyRewardsModule.RewardConfig todayRewardConfig
		{
			get
			{
				if (debugEnable && useCustomRewards)
				{
					return CustomRewards()[0];
				}
				if (dailyRewardsModel == null)
				{
					return DailyRewardsModule.RewardConfig.Empty();
				}
				return dailyRewardsModel.RewardConfigForDay(daysInRowWrapped);
			}
		}

		public override void Init()
		{
		}

		public void Init(DailyRewardsModule dailyRewardsModule)
		{
			dailyRewardsModel = dailyRewardsModule;
			DailyRewardsModule dailyRewardsModule2 = dailyRewardsModel;
			dailyRewardsModule2.onModelDownloaded = (Action)Delegate.Combine(dailyRewardsModule2.onModelDownloaded, (Action)delegate
			{
				UnityEngine.Debug.Log(GetLog());
			});
			todayChange = 0;
			SetLastTimes();
			SetLogSpan();
			customWrapping = wrappingMode;
		}

		public void Claim()
		{
			_lastClaimedTime = today;
			PlayerPrefs.SetString("DRLastClaimed", GetTodayString());
			PlayerPrefs.Save();
		}

		public DailyRewardsModule.RewardConfig[] AllRewardConfigs()
		{
			if (dailyRewardsModel == null)
			{
				return null;
			}
			return dailyRewardsModel.RewardConfigs();
		}

		public DailyRewardsModule.RewardConfig[] FiltredRewardConfigs()
		{
			if (debugEnable && useCustomRewards)
			{
				return CustomRewards();
			}
			if (dailyRewardsModel == null)
			{
				return null;
			}
			return (from reward in dailyRewardsModel.RewardConfigs()
				where reward.id != 0 && reward.day <= dailyRewardsModel.MaxDaysInRow()
				select reward).ToArray();
		}

		private void SetLastTimes()
		{
			SetLastLogTime();
			SetLastClaimedTime();
		}

		private void SetLastLogTime()
		{
			SetLastGivenTime("DRLastLog", out _lastLogTime);
			PlayerPrefs.SetString("DRLastLog", GetTodayString());
			PlayerPrefs.Save();
		}

		private void SetLastClaimedTime()
		{
			SetLastGivenTime("DRLastClaimed", out _lastClaimedTime);
		}

		private void SetLastGivenTime(string key, out DateTime resultTime)
		{
			string @string = PlayerPrefs.GetString(key, GetYesterdayString());
			UnityEngine.Debug.Log($"key: {key}; value: {@string}");
			int result = 0;
			int result2 = 0;
			int result3 = 0;
			string[] array = @string.Split("-"[0]);
			if (!int.TryParse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture, out result) || !int.TryParse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture, out result2) || !int.TryParse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture, out result3))
			{
				resultTime = today - new TimeSpan(1, 0, 0, 0);
			}
			else
			{
				resultTime = new DateTime(result3, result2, result);
			}
		}

		private void SetLogSpan()
		{
			int @int = PlayerPrefs.GetInt("DRLogSpan", 0);
			if (GetDaysSpan(lastLogTime) == 1)
			{
				_daysInRow = @int + 1;
			}
			else if (GetDaysSpan(lastLogTime) == 0)
			{
				_daysInRow = @int;
			}
			else
			{
				_daysInRow = 1;
			}
			PlayerPrefs.SetInt("DRLogSpan", _daysInRow);
			PlayerPrefs.Save();
		}

		private int GetDaysSpan(DateTime value)
		{
			return (today - value).Days;
		}

		private string GetTodayString()
		{
			DateTime today = this.today;
			return today.Day + "-" + today.Month + "-" + today.Year;
		}

		private string GetYesterdayString()
		{
			DateTime dateTime = today - new TimeSpan(1, 0, 0, 0);
			return dateTime.Day + "-" + dateTime.Month + "-" + dateTime.Year;
		}

		private string GetLog()
		{
			string str = "Daily Rewards Manager Log: (drm)\n";
			str += $"Today: {today}\n";
			str += $"Is enable: {isEnable}\n";
			str += $"Days in row wrapped: {daysInRowWrapped}\n";
			str += $"Days in row: {daysInRow}\n";
			str += $"Has claimed today: {hasClaimedToday}\n";
			str += $"Has reward today: {hasRewardToday}\n";
			return str + $"Last login: {lastLogTime}\n";
		}

		private DailyRewardsModule.RewardConfig[] CustomRewards()
		{
			int[] config = customReward.Split(';').Select(int.Parse).ToArray();
			if (config.Length != 2)
			{
				return null;
			}
			return Enumerable.Range(0, 31).Select((Func<int, DailyRewardsModule.RewardConfig>)delegate
			{
				DailyRewardsModule.RewardConfig result = default(DailyRewardsModule.RewardConfig);
				result.id = config[0];
				result.count = config[1];
				return result;
			}).ToArray();
		}
	}
}
