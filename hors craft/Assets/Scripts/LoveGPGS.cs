// DecompilerFi decompiler from Assembly-CSharp.dll class: LoveGPGS
using Common.Managers;
using UnityEngine;

public class LoveGPGS : MonoBehaviour
{
	public enum RankingType
	{
		time,
		level
	}

	public RankingType rankingType;

	private const float UPDATE_LEADERBOARD_EVERY = 10f;

	private float timer;

	private void Update()
	{
		if (timer >= 10f)
		{
			UpdateLeaderboard();
			timer = 0f;
		}
		timer += Time.unscaledDeltaTime;
	}

	private void UpdateLeaderboard()
	{
		if (Manager.Get<LoveManager>().lovedOne != null)
		{
			Manager.Get<RankingManager>().SaveScore("leaderboardRelationshipScore", CalculateScore(Manager.Get<LoveManager>().lovedOne));
		}
	}

	private int CalculateScore(LovedOne lovedOne)
	{
		if (rankingType == RankingType.time)
		{
			float num = 0f;
			for (int i = 0; i < lovedOne.info.minutesInRelationshipPerStage.Count; i++)
			{
				num += lovedOne.info.minutesInRelationshipPerStage[i] * (float)(i + 1);
			}
			return (int)num;
		}
		if (rankingType == RankingType.level)
		{
			return lovedOne.info.relationshipStage;
		}
		return 0;
	}
}
