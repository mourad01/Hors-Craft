// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.RankingManager
using Common.Managers.SocialPlatforms;
using Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Common.Managers
{
	public class RankingManager : Manager, ISocialPlatform
	{
		public List<string> allowedRankings = new List<string>();

		private const string LEADERBOARD_URL = "Ranking";

		private const string LAST_SCORE_PREFS = "ranking.last.score";

		private RankingDataInterpreter[] interpreters;

		private string homeUrl;

		private string gamename;

		private ISocialPlatform fallbackSocial;

		public bool isLoggedIn => Time.realtimeSinceStartup > 2f;

		public override void Init()
		{
			ConnectionInfoManager connectionInfoManager = Manager.Get<ConnectionInfoManager>();
			homeUrl = connectionInfoManager.homeURL;
			gamename = connectionInfoManager.gameName;
			interpreters = GetComponentsInChildren<RankingDataInterpreter>();
			InitFallbackSocial();
		}

		public void OnModelDownloaded()
		{
			List<string> data = Manager.Get<AbstractModelManager>().rankingsSettings.GetAllowedRankings();
			if (!data.IsNullOrEmpty())
			{
				allowedRankings = data;
			}
		}

		public RankingModel.ScoreData GetLastScoreFromPrefs()
		{
			string @string = PlayerPrefs.GetString("ranking.last.score", string.Empty);
			return (!@string.IsNullOrEmpty()) ? JSONHelper.Deserialize<RankingModel.ScoreData>(@string) : null;
		}

		public void SaveScore(string rankingId, long score)
		{
			if (!Manager.Get<AbstractModelManager>().rankingsSettings.RankingsEnabled())
			{
				fallbackSocial.SaveScore(Singleton<GooglePlayConstants>.get.GetIDFor(rankingId, force: true), score);
			}
			Dictionary<string, object> baseDictionary = GetBaseDictionary();
			baseDictionary.Add("score", score);
			baseDictionary.Add("rankingId", rankingId);
			string text = ConstructPlayerData();
			RankingModel.ScoreData scoreData = new RankingModel.ScoreData();
			scoreData.score = (int)score;
			scoreData.playerDataJson = text;
			scoreData.isItMe = true;
			RankingModel.ScoreData ob = scoreData;
			PlayerPrefs.SetString("ranking.last.score", JSONHelper.ToJSON(ob));
			MakePost("saveScore", baseDictionary, text, null, null);
		}

		private string ConstructPlayerData()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			RankingDataInterpreter[] array = interpreters;
			foreach (RankingDataInterpreter rankingDataInterpreter in array)
			{
				KeyValuePair<string, string>? keyValuePair = rankingDataInterpreter.PrepareForSerialization();
				if (keyValuePair.HasValue)
				{
					dictionary[keyValuePair.Value.Key] = keyValuePair.Value.Value;
				}
			}
			return JSONHelper.ToJSON(dictionary);
		}

		public void ShowRanking(string rankingId, Action<RankingModel> onSuccess, Action onError)
		{
			if (!Manager.Get<AbstractModelManager>().rankingsSettings.RankingsEnabled())
			{
				fallbackSocial.ShowRanking(Singleton<GooglePlayConstants>.get.GetIDFor(rankingId, force: true), onSuccess, onError);
			}
			Dictionary<string, object> baseDictionary = GetBaseDictionary();
			baseDictionary.Add("rankingId", rankingId);
			baseDictionary.Add("fromRank", 0);
			baseDictionary.Add("toRank", Manager.Get<AbstractModelManager>().rankingsSettings.GetTopScoresCount(rankingId));
			MakeRequest("getRanking", baseDictionary, delegate(WWW www)
			{
				OnRankingDownloaded(onSuccess, www.text);
			}, onError);
		}

		private void OnRankingDownloaded(Action<RankingModel> onSuccess, string json)
		{
			RankingModel obj = ParseRanking(json);
			onSuccess?.Invoke(obj);
		}

		private RankingModel ParseRanking(string json)
		{
			Dictionary<string, List<RankingModel.ScoreData>> scores = JSONHelper.Deserialize<Dictionary<string, List<RankingModel.ScoreData>>>(json);
			return new RankingModel(scores);
		}

		public void SetElementWithPlayerData(RankingScoreElement element, RankingModel.ScoreData score)
		{
			Dictionary<string, string> data = JSONHelper.Deserialize<Dictionary<string, string>>(score.playerDataJson);
			RankingDataInterpreter[] array = interpreters;
			foreach (RankingDataInterpreter rankingDataInterpreter in array)
			{
				rankingDataInterpreter.SetElement(element, data, score);
			}
		}

		private void MakeRequest(string url, Dictionary<string, object> dict, Action<WWW> onSuccess, Action onError)
		{
			if (!Manager.Get<AbstractModelManager>().rankingsSettings.RankingsEnabled())
			{
				if (onError != null)
				{
					onError();
				}
			}
			else
			{
				SimpleRequestMaker.MakeRequest(homeUrl, "Ranking/" + url, dict, delegate(WWW www)
				{
					if (onSuccess != null)
					{
						onSuccess(www);
					}
				}, delegate(string err)
				{
					UnityEngine.Debug.LogError("Connection error: " + err);
					if (onError != null)
					{
						onError();
					}
				});
			}
		}

		private void MakePost(string url, Dictionary<string, object> dict, string data, Action<UnityWebRequest> onSuccess, Action onError)
		{
			if (!Manager.Get<AbstractModelManager>().rankingsSettings.RankingsEnabled())
			{
				if (onError != null)
				{
					onError();
				}
			}
			else
			{
				SimpleRequestMaker.MakePost(homeUrl, "Ranking/" + url, dict, data, delegate(UnityWebRequest www)
				{
					if (onSuccess != null)
					{
						onSuccess(www);
					}
				}, delegate(string err)
				{
					UnityEngine.Debug.LogError("Connection error: " + err);
					if (onError != null)
					{
						onError();
					}
				});
			}
		}

		private Dictionary<string, object> GetBaseDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("platform", GetPlatform());
			dictionary.Add("gamename", gamename);
			dictionary.Add("playerId", PlayerId.GetId());
			return dictionary;
		}

		private string GetPlatform()
		{
			return "android";
		}

		private void InitFallbackSocial()
		{
			if (fallbackSocial == null)
			{
				fallbackSocial = new DummySocialPlatform();
				fallbackSocial.Init();
			}
		}

		public void ReportProgress(string achievementId, double progress)
		{
			fallbackSocial.ReportProgress(Singleton<GooglePlayConstants>.get.GetIDFor(achievementId, force: true), progress);
		}

		public void ShowRankings()
		{
			if (!Manager.Get<AbstractModelManager>().rankingsSettings.RankingsEnabled())
			{
				fallbackSocial.ShowRankings();
			}
		}

		public void ShowRanking(string rankingId)
		{
			if (!Manager.Get<AbstractModelManager>().rankingsSettings.RankingsEnabled())
			{
				fallbackSocial.ShowRanking(Singleton<GooglePlayConstants>.get.GetIDFor(rankingId, force: true));
			}
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
	}
}
