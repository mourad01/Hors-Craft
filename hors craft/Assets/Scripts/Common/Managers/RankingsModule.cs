// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.RankingsModule
using Common.Model;
using System.Collections.Generic;

namespace Common.Managers
{
	public class RankingsModule : ModelModule
	{
		protected string keyRankingsEnabled()
		{
			return "rankings.enabled";
		}

		protected string keyTopScoresCount(string ranking)
		{
			return "ranking.top.scores." + ranking;
		}

		protected string keyAllowedRankings()
		{
			return "ranking.allowed.rankings";
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription(keyRankingsEnabled(), 1);
		}

		public override void OnModelDownloaded()
		{
			if (Manager.Contains<RankingManager>())
			{
				Manager.Get<RankingManager>().OnModelDownloaded();
			}
		}

		public bool RankingsEnabled()
		{
			return base.settings.GetInt(keyRankingsEnabled()) == 1;
		}

		public int GetTopScoresCount(string ranking)
		{
			return ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyTopScoresCount(ranking), 100);
		}

		public List<string> GetAllowedRankings()
		{
			return ModelSettingsHelper.GetStringListFromSettings(base.settings, keyAllowedRankings());
		}
	}
}
