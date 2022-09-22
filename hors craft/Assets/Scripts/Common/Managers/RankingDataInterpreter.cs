// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.RankingDataInterpreter
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public abstract class RankingDataInterpreter : MonoBehaviour
	{
		public abstract KeyValuePair<string, string>? PrepareForSerialization();

		public abstract void SetElement(RankingScoreElement element, Dictionary<string, string> data, RankingModel.ScoreData score);
	}
}
