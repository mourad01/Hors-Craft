// DecompilerFi decompiler from Assembly-CSharp.dll class: AdScoringModule
using Common.Managers;
using Common.Model;
using Common.Utils;
using Common.Waterfall;
using System;
using System.Collections.Generic;
using UnityEngine;

/*public class AdScoringModule : ModelModule
{
	public class AdEntry
	{
		public bool rewarded;

		public double timeStamp;
	}

	private List<AdEntry> entries;

	private const string ENTRIES_KEY = "ad.score.entries";

	public AdScoringModule()
	{
		AdWaterfall get = AdWaterfall.get;
		get.onInterstitialShown = (Action)Delegate.Combine(get.onInterstitialShown, (Action)delegate
		{
			ShownAd();
		});
		AdWaterfall get2 = AdWaterfall.get;
		get2.onRewardedShown = (Action)Delegate.Combine(get2.onRewardedShown, (Action)delegate
		{
			ShownAd(rewarded: true);
		});
	}

	protected string keyUseAdScoring()
	{
		return "ad.score.enabled";
	}

	protected string keyScoreEasing()
	{
		return "ad.score.easing";
	}

	protected string keyScoreMinScore()
	{
		return "ad.score.min";
	}

	protected string keyScoreMaxScore()
	{
		return "ad.score.max";
	}

	protected string keyScoreInterval()
	{
		return "ad.score.interval";
	}

	protected string keyScoreMultiplierForRewarded()
	{
		return "ad.score.multi.rewarded";
	}

	protected string keyAdProbabilityEasing()
	{
		return "ad.probability.easing";
	}

	protected string keyAdProbabilityMaxScore()
	{
		return "ad.probability.max.score";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyUseAdScoring(), defaultValue: false);
		descriptions.AddDescription(keyScoreEasing(), "OutCubic");
		descriptions.AddDescription(keyScoreMinScore(), 5);
		descriptions.AddDescription(keyScoreMaxScore(), 100);
		descriptions.AddDescription(keyScoreInterval(), 300f);
		descriptions.AddDescription(keyScoreMultiplierForRewarded(), 1f);
		descriptions.AddDescription(keyAdProbabilityEasing(), "Lerp");
		descriptions.AddDescription(keyAdProbabilityMaxScore(), 150);
	}

	public override void OnModelDownloaded()
	{
	}

	public void ShownAd(bool rewarded = false)
	{
		if (entries == null)
		{
			Load();
		}
		entries.Add(new AdEntry
		{
			rewarded = rewarded,
			timeStamp = Misc.GetTimeStampDouble()
		});
		Save();
	}

	private void Load()
	{
		if (PlayerPrefs.HasKey("ad.score.entries"))
		{
			entries = JSONHelper.Deserialize<List<AdEntry>>(PlayerPrefs.GetString("ad.score.entries"));
		}
		else
		{
			entries = new List<AdEntry>();
		}
	}

	private void Save()
	{
		PlayerPrefs.SetString("ad.score.entries", JSONHelper.ToJSON(entries));
	}

	public bool CanShowAd()
	{
		if (!UseAdScoring())
		{
			return true;
		}
		if (entries == null)
		{
			Load();
		}
		float score = Score();
		float num = Probability(score);
		return UnityEngine.Random.value <= num;
	}

	private bool UseAdScoring()
	{
		return base.settings.GetBool(keyUseAdScoring());
	}

	private float Score()
	{
		int @int = base.settings.GetInt(keyScoreMinScore());
		int int2 = base.settings.GetInt(keyScoreMaxScore());
		float @float = base.settings.GetFloat(keyScoreInterval());
		float float2 = base.settings.GetFloat(keyScoreMultiplierForRewarded());
		EaseType easeType = (EaseType)Enum.Parse(typeof(EaseType), base.settings.GetString(keyScoreEasing()));
		float num = 0f;
		foreach (AdEntry entry in entries)
		{
			double num2 = Misc.GetTimeStampDouble() - entry.timeStamp;
			float value = (float)(num2 / (double)@float);
			float num3 = Easing.Ease(easeType, int2, @int, value);
			if (entry.rewarded)
			{
				num3 *= float2;
			}
			num += num3;
		}
		return num;
	}

	private float Probability(float score)
	{
		EaseType easeType = (EaseType)Enum.Parse(typeof(EaseType), base.settings.GetString(keyAdProbabilityEasing()));
		int @int = base.settings.GetInt(keyAdProbabilityMaxScore());
		if (@int == 0)
		{
			return 1f;
		}
		return Easing.Ease(easeType, 1f, 0f, score / (float)@int);
	}
}*/
