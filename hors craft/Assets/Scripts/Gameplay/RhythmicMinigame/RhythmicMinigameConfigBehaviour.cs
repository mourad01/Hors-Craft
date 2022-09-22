// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RhythmicMinigame.RhythmicMinigameConfigBehaviour
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.RhythmicMinigame
{
	public class RhythmicMinigameConfigBehaviour : MonoBehaviour
	{
		[Serializable]
		public class Sequence
		{
			public string beatsSchema = "00000000";

			[HideInInspector]
			public int correctHits;

			[HideInInspector]
			public int[] beats;

			public void PrepareBeats()
			{
				beats = new int[beatsSchema.Length];
				int num = 0;
				string text = beatsSchema;
				for (int i = 0; i < text.Length; i++)
				{
					char c = text[i];
					beats[num++] = int.Parse(c.ToString());
				}
			}
		}

		[Serializable]
		public class SequencesList
		{
			public int progressToFinish = 1;

			public float tempo = 1f;

			public AudioClip musicClip;

			public List<Sequence> list = new List<Sequence>();
		}

		public Sequence tutorialSequence = new Sequence();

		public List<SequencesList> sequencesPerDifficulty = new List<SequencesList>();
	}
}
