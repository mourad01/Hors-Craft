// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SocialPlatforms.ISocialPlatform
using System;

namespace Common.Managers.SocialPlatforms
{
	public interface ISocialPlatform
	{
		bool isLoggedIn
		{
			get;
		}

		void Init();

		void LoadPlayerScore(string rankingId, OnScoreLoaded onPlayerScoreLoaded);

		void LoadBestFriendScore(string rankingId, OnScoreLoaded onBestFriendScoreLoaded);

		void LoadBestScore(string rankingId, OnScoreLoaded onBestScoreLoaded);

		void SaveScore(string rankingId, long score);

		void ReportProgress(string achievementId, double progress);

		void ShowRankings();

		void ShowRanking(string rankingId);

		void ShowRanking(string rankingId, Action<RankingModel> onSuccess, Action onError);
	}
}
