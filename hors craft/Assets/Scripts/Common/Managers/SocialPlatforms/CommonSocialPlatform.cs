// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SocialPlatforms.CommonSocialPlatform
using System;
using UnityEngine;

namespace Common.Managers.SocialPlatforms
{
	public abstract class CommonSocialPlatform : ISocialPlatform
	{
		public virtual bool isLoggedIn => Social.localUser != null && Social.localUser.authenticated;

		public virtual void Init()
		{
			Social.localUser.Authenticate(delegate(bool success)
			{
				UnityEngine.Debug.Log("Finished authenticating Social User with result: " + success);
			});
		}

		public abstract void LoadPlayerScore(string rankingId, OnScoreLoaded onPlayerScoreLoaded);

		public abstract void LoadBestFriendScore(string rankingId, OnScoreLoaded onBestFriendScoreLoaded);

		public abstract void LoadBestScore(string rankingId, OnScoreLoaded onBestScoreLoaded);

		public virtual void SaveScore(string rankingId, long score)
		{
			Social.ReportScore(score, rankingId, delegate(bool ok)
			{
				UnityEngine.Debug.Log("Score " + score.ToString() + " finished saving. Result: " + ok);
			});
		}

		public abstract void ReportProgress(string achievementId, double progress);

		public virtual void ShowRankings()
		{
			Social.ShowLeaderboardUI();
		}

		public abstract void ShowRanking(string rankingId);

		public virtual void ShowRanking(string rankingId, Action<RankingModel> onSuccess, Action onError)
		{
			ShowRanking(rankingId);
		}

		public void ShowAchievements()
		{
			Social.ShowAchievementsUI();
		}
	}
}
