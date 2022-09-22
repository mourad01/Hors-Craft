// DecompilerFi decompiler from Assembly-CSharp.dll class: WarplanesLeaderboardManager
using Common.Managers;
using UnityEngine;

public class WarplanesLeaderboardManager : Manager
{
	[SerializeField]
	private Leaderboard leaderboard;

	private string currentWorldId;

	public void InitForWorld(string id)
	{
		currentWorldId = id;
		Init();
	}

	public override void Init()
	{
	}

	public void ReInitForCurrentWorld()
	{
		string questsFinishedCurrentWorld = GetQuestsFinishedCurrentWorld();
		PlayerPrefs.SetInt(questsFinishedCurrentWorld, 0);
	}

	public void Increment()
	{
		string questsFinishedCurrentWorld = GetQuestsFinishedCurrentWorld();
		int num = PlayerPrefs.GetInt(questsFinishedCurrentWorld) + 1;
		PlayerPrefs.SetInt(questsFinishedCurrentWorld, num);
		Leaderboard.Rank[] ranks = leaderboard.ranks;
		foreach (Leaderboard.Rank rank in ranks)
		{
			if (num == rank.questRequired)
			{
				string text = "Your new rank: " + rank.defaultName;
				Manager.Get<ToastManager>().ShowToast(text);
				break;
			}
		}
		int questRequired = leaderboard.ranks[leaderboard.ranks.Length - 1].questRequired;
		if (questRequired < num && num % 100 == 0)
		{
			int num2 = (num - leaderboard.ranks[leaderboard.ranks.Length - 1].questRequired) / 100;
			string text2 = "Your new rank: " + leaderboard.ranks[leaderboard.ranks.Length - 1].defaultName + " " + num2 + 1;
			Manager.Get<ToastManager>().ShowToast(text2);
		}
	}

	public void SendToGoogleLeaderboard()
	{
	}

	private string keyQuestsFinished()
	{
		return "quests.finished";
	}

	private string GetQuestsFinishedCurrentWorld()
	{
		return "world." + currentWorldId + "." + keyQuestsFinished();
	}
}
