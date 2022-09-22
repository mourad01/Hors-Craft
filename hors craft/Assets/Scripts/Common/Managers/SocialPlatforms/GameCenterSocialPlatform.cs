// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SocialPlatforms.GameCenterSocialPlatform
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Common.Managers.SocialPlatforms
{
	public class GameCenterSocialPlatform : CommonSocialPlatform
	{
		public override void LoadPlayerScore(string rankingId, OnScoreLoaded onPlayerScoreLoaded)
		{
			ILeaderboard ranking = Social.CreateLeaderboard();
			ranking.id = rankingId;
			ranking.userScope = UserScope.FriendsOnly;
			ranking.LoadScores(delegate(bool ok)
			{
				if (ok && ranking.localUserScore != null)
				{
					onPlayerScoreLoaded(ranking.localUserScore);
				}
			});
		}

		public override void LoadBestFriendScore(string rankingId, OnScoreLoaded onBestFriendScoreLoaded)
		{
			ILeaderboard ranking = Social.CreateLeaderboard();
			ranking.id = rankingId;
			ranking.userScope = UserScope.FriendsOnly;
			ranking.range = new Range(1, 1);
			ranking.LoadScores(delegate(bool ok)
			{
				if (ok && ranking.scores.Length > 0)
				{
					onBestFriendScoreLoaded(ranking.scores[0]);
				}
			});
		}

		public override void LoadBestScore(string rankingId, OnScoreLoaded onBestScoreLoaded)
		{
			ILeaderboard ranking = Social.CreateLeaderboard();
			ranking.id = rankingId;
			ranking.userScope = UserScope.Global;
			ranking.range = new Range(1, 1);
			ranking.LoadScores(delegate(bool ok)
			{
				if (ok && ranking.scores.Length > 0)
				{
					onBestScoreLoaded(ranking.scores[0]);
				}
			});
		}

		public override void ReportProgress(string achievementId, double progress)
		{
			GKAchievementReporter.ReportAchievement(achievementId, (float)progress, showsCompletionBanner: true);
		}

		public override void ShowRanking(string rankingId)
		{
			Social.ShowLeaderboardUI();
		}
	}
}
