// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SocialPlatforms.DummySocialPlatform
using System;
using UnityEngine;

namespace Common.Managers.SocialPlatforms
{
	public class DummySocialPlatform : ISocialPlatform
	{
		public bool isLoggedIn => Time.realtimeSinceStartup > 2f;

		public void Init()
		{
		}

		public void LoadPlayerScore(string rankingId, OnScoreLoaded onPlayerScoreLoaded)
		{
		}

		public void LoadBestFriendScore(string rankingId, OnScoreLoaded onBestFriendScoreLoaded)
		{
		}

		public void LoadBestScore(string rankingId, OnScoreLoaded onBestScoreLoaded)
		{
		}

		public void SaveScore(string rankingId, long score)
		{
		}

		public void ReportProgress(string achievementId, double progress)
		{
		}

		public void ShowRankings()
		{
		}

		public void ShowRanking(string rankingId)
		{
		}

		public void ShowRanking(string rankingId, Action<RankingModel> onSuccess, Action onError)
		{
			onError();
		}
	}
}
