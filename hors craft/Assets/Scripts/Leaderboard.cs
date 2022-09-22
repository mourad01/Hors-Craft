// DecompilerFi decompiler from Assembly-CSharp.dll class: Leaderboard
using System;
using UnityEngine;

public class Leaderboard : ScriptableObject
{
	[Serializable]
	public class Rank
	{
		public int questRequired;

		public string defaultName;

		public string translationKey;
	}

	[SerializeField]
	public Rank[] ranks;
}
