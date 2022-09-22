// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerPurchases
using Common.Managers;
using Common.Utils;
using Gameplay;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPurchases
{
	public struct VideoTime
	{
		public double seconds;

		public int numberOfAds;

		public VideoTime(double seconds, int numberOfAds)
		{
			this.seconds = seconds;
			this.numberOfAds = numberOfAds;
		}
	}

	private const string keyVideoCurrentKey = "video.current.videos";

	private const string keyTimeVideoReset = "video.time.to.reset";

	private const string keyRestoreTimes = "video.times.to.restore";

	private List<double> _timesOfRestores;

	private List<double> timesOfRestores
	{
		get
		{
			if (_timesOfRestores != null)
			{
				return _timesOfRestores;
			}
			string @string = PlayerPrefs.GetString("video.times.to.restore", string.Empty);
			if (string.IsNullOrEmpty(@string))
			{
				_timesOfRestores = new List<double>();
			}
			else
			{
				_timesOfRestores = JSONHelper.Deserialize<List<double>>(@string);
			}
			return _timesOfRestores;
		}
	}

	private int maxNumberOfAds => Manager.Get<ModelManager>().videoPackage.GetMaxNumberOfAds();

	private int secondsToResetAd => Manager.Get<ModelManager>().videoPackage.TimeToResetCounter();

	private int diffrenceToMax => maxNumberOfAds - currentStroredAds;

	private double secondsForNearestRestore
	{
		get
		{
			if (timesOfRestores.Count > 0)
			{
				return timesOfRestores[0] - Misc.GetTimeStampDouble();
			}
			return -1.0;
		}
	}

	private int currentStroredAds
	{
		get
		{
			return PlayerPrefs.GetInt("video.current.videos", maxNumberOfAds);
		}
		set
		{
			PlayerPrefs.SetInt("video.current.videos", Mathf.Min(value, maxNumberOfAds));
		}
	}

	public VideoTime GetVideoInformation()
	{
		return new VideoTime(GetTimeForNextVideo(), GetNumberOfPossibleVideos());
	}

	private double GetTimeForNextVideo()
	{
		if (secondsForNearestRestore <= 0.0)
		{
			RestoreVideo();
		}
		return secondsForNearestRestore;
	}

	private void RestoreVideo()
	{
		int num = 0;
		foreach (double timesOfRestore in timesOfRestores)
		{
			int num2 = (int)timesOfRestore;
			if ((double)num2 - Misc.GetTimeStampDouble() <= 0.0)
			{
				num++;
			}
		}
		for (int i = 0; i < num; i++)
		{
			if (timesOfRestores.Count <= 0)
			{
				break;
			}
			timesOfRestores.RemoveAt(0);
		}
		currentStroredAds += maxNumberOfAds;
		if (currentStroredAds == maxNumberOfAds)
		{
			timesOfRestores.Clear();
		}
		if (timesOfRestores.Count == 0 && diffrenceToMax > 0)
		{
			timesOfRestores.Add(Misc.GetTimeStampDouble() + (double)secondsToResetAd);
		}
		SaveTimes();
	}

	private int GetNumberOfPossibleVideos()
	{
		return currentStroredAds;
	}

	private void SaveTimes()
	{
		string value = JSONHelper.ToJSON(timesOfRestores);
		PlayerPrefs.SetString("video.times.to.restore", value);
	}

	public bool CanWatchAdd()
	{
		return currentStroredAds > 0;
	}

	public bool ViewVideoForCurrency(int currencyValue)
	{
		if (currentStroredAds <= 0)
		{
			return false;
		}
		currentStroredAds--;
		SetResetTimer();
		Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(currencyValue);
		return true;
	}

	private void SetResetTimer()
	{
		if (timesOfRestores.Count > 0)
		{
			double lastItem = timesOfRestores.GetLastItem();
			timesOfRestores.Add(lastItem + (double)secondsToResetAd);
		}
		else
		{
			timesOfRestores.Add(Misc.GetTimeStampDouble() + (double)secondsToResetAd);
		}
		SaveTimes();
	}
}
