// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.RankingModel
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	[Serializable]
	public class RankingModel
	{
		[Serializable]
		public class ScoreData
		{
			public int position;

			public int score;

			public bool isItMe;

			public string playerDataJson;

			public void LogContent()
			{
				UnityEngine.Debug.Log("POSITION: " + position + " SCORE: " + score);
				UnityEngine.Debug.Log("DATA: " + playerDataJson);
			}
		}

		public Dictionary<string, List<ScoreData>> dict;

		public List<ScoreData> top => dict["top"];

		public List<ScoreData> neighbours => dict["neighbours"];

		public RankingModel(Dictionary<string, List<ScoreData>> scores)
		{
			dict = scores;
		}

		public void LogContent()
		{
			UnityEngine.Debug.Log("---Top: ");
			for (int i = 0; i < top.Count; i++)
			{
				top[i].LogContent();
			}
			UnityEngine.Debug.Log("---Neighbours: ");
			for (int j = 0; j < neighbours.Count; j++)
			{
				neighbours[j].LogContent();
			}
		}
	}
}
