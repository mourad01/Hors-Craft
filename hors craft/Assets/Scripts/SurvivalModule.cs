// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalModule
using Common.Managers;
using Common.Model;

public class SurvivalModule : ModelModule
{
	private string keyRankPointsRequirement(int index)
	{
		return "survival.rank.points.requirement." + index;
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		if (Manager.Contains<SurvivalRankManager>())
		{
			SurvivalRankManager survivalRankManager = Manager.Get<SurvivalRankManager>();
			for (int i = 0; i < survivalRankManager.ranks.Length; i++)
			{
				descriptions.AddDescription(keyRankPointsRequirement(i), survivalRankManager.ranks[i].pointsRequirements);
			}
		}
	}

	public override void OnModelDownloaded()
	{
		if (Manager.Contains<SurvivalRankManager>())
		{
			Manager.Get<SurvivalRankManager>().LoadFromModel();
		}
	}

	public float GetRankPointsRequirement(int index)
	{
		return base.settings.GetFloat(keyRankPointsRequirement(index));
	}
}
