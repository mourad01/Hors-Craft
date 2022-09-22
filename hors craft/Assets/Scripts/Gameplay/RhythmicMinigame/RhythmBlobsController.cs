// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RhythmicMinigame.RhythmBlobsController
using Common.Behaviours;
using States;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.RhythmicMinigame
{
	public class RhythmBlobsController
	{
		private class Blob
		{
			public int beatType;

			public RectTransform rectTransform;

			public RhythmController.OnBeatsPassed onBeatsPassed;
		}

		public enum TapEffect
		{
			TAP_CORRECT,
			TAP_INCORRECT,
			TAP_NO_EFFECT
		}

		private const float WRONG_TAP_TOLERANCE = 0.5f;

		private const float CORRECT_TAP_TOLERANCE = 0.04f;

		private const float PERFECT_TAP_ANCHOR = 0.14f;

		private const float DISTANCE_FROM_PERFECT_TO_SCALE = 0.3f;

		private const float PERCENT_SOUND_LAG = 0.1f;

		private RhythmController rhythmController;

		private GameObject blobPrefab;

		private RhythmicMinigameState state;

		private RhythmicMinigameConfigBehaviour.Sequence currentSequence;

		private int nextBeatIndex;

		private List<Blob> spawnedBlobs;

		public RhythmBlobsController(RhythmicMinigameState state, RhythmController rhythmController, GameObject blobPrefab)
		{
			this.state = state;
			this.rhythmController = rhythmController;
			this.blobPrefab = blobPrefab;
			spawnedBlobs = new List<Blob>();
		}

		public void AssignNewSequence(RhythmicMinigameConfigBehaviour.Sequence sequence)
		{
			currentSequence = sequence;
			nextBeatIndex = 0;
		}

		public void OnBeat()
		{
			if (nextBeatIndex < currentSequence.beats.Length)
			{
				if (currentSequence.beats[nextBeatIndex] > 0)
				{
					SpawnBlob(currentSequence.beats[nextBeatIndex]);
				}
				nextBeatIndex++;
			}
			if (spawnedBlobs.Count > 0)
			{
				Vector2 anchorMin = spawnedBlobs[0].rectTransform.anchorMin;
				if ((double)anchorMin.x <= 0.1)
				{
					state.OnBlobFall(spawnedBlobs[0].beatType);
					RemoveFirstBlob(correct: false);
				}
			}
		}

		private void SpawnBlob(int beatType)
		{
			GameObject gameObject = Object.Instantiate(blobPrefab);
			gameObject.SetActive(value: true);
			gameObject.transform.SetParent(blobPrefab.transform.parent, worldPositionStays: false);
			gameObject.transform.SetSiblingIndex(2);
			RectTransform rectTransform = gameObject.transform as RectTransform;
			RectTransform rectTransform2 = rectTransform;
			Vector2 anchorMin = rectTransform.anchorMin;
			rectTransform2.anchorMin = new Vector2(1f, anchorMin.y);
			RectTransform rectTransform3 = rectTransform;
			Vector2 anchorMax = rectTransform.anchorMax;
			rectTransform3.anchorMax = new Vector2(1f, anchorMax.y);
			gameObject.GetComponentInChildren<Animator>().SetTrigger("Bump");
			RectTransform bitTransform = gameObject.transform as RectTransform;
			spawnedBlobs.Add(new Blob
			{
				rectTransform = bitTransform,
				beatType = beatType,
				onBeatsPassed = delegate(float beats)
				{
					UpdateBit(bitTransform, beats, beatType);
				}
			});
			rhythmController.AddBeatsPassedListener(spawnedBlobs[spawnedBlobs.Count - 1].onBeatsPassed);
			state.OnSpawnBlob(beatType);
		}

		public bool IsFirstBlobInPerfectPlace()
		{
			if (spawnedBlobs.Count > 0)
			{
				Vector2 anchorMin = spawnedBlobs[0].rectTransform.anchorMin;
				if (anchorMin.x < 0.5f)
				{
					RectTransform rectTransform = spawnedBlobs[0].rectTransform;
					Vector2 anchorMin2 = rectTransform.anchorMin;
					if (Mathf.Abs(0.14f - anchorMin2.x) < 0.04f)
					{
						Vector2 anchorMin3 = rectTransform.anchorMin;
						float correctPercent = GetCorrectPercent(anchorMin3.x);
						return correctPercent > 0.75f;
					}
				}
			}
			return false;
		}

		public void UpdateBit(RectTransform bit, float beats, int beatType)
		{
			float num = 1f - 0.86f * (beats / ((float)currentSequence.beats.Length + 0.1f));
			if (num <= 0.22f)
			{
				Vector2 anchorMin = bit.anchorMin;
				if (anchorMin.x > 0.22f)
				{
					state.OnBlobInShootZone(beatType);
				}
			}
			float x = num;
			Vector2 anchorMin2 = bit.anchorMin;
			bit.anchorMin = new Vector2(x, anchorMin2.y);
			float x2 = num;
			Vector2 anchorMax = bit.anchorMax;
			bit.anchorMax = new Vector2(x2, anchorMax.y);
			bit.localScale = Vector3.one * (0.95f + GetScaleAmount(num) * 0.4f);
		}

		private float GetScaleAmount(float position)
		{
			return Mathf.Max(0f, 1f - Mathf.Abs(0.14f - position) / 0.3f);
		}

		private void RemoveFirstBlob(bool correct)
		{
			rhythmController.RemoveBeatsPassedListener(spawnedBlobs[0].onBeatsPassed);
			spawnedBlobs[0].rectTransform.gameObject.GetComponentInChildren<Animator>().SetTrigger((!correct) ? "wrong" : "correct");
			DestroyAfter destroyAfter = spawnedBlobs[0].rectTransform.gameObject.AddComponent<DestroyAfter>();
			destroyAfter.realtime = true;
			destroyAfter.delay = 0.4f;
			destroyAfter.CalculateDestroyTime();
			spawnedBlobs.RemoveAt(0);
		}

		public TapEffect TryToTap()
		{
			if (spawnedBlobs.Count > 0)
			{
				Vector2 anchorMin = spawnedBlobs[0].rectTransform.anchorMin;
				if (anchorMin.x < 0.5f)
				{
					RectTransform rectTransform = spawnedBlobs[0].rectTransform;
					Vector2 anchorMin2 = rectTransform.anchorMin;
					if (Mathf.Abs(0.14f - anchorMin2.x) < 0.04f)
					{
						int beatType = spawnedBlobs[0].beatType;
						RemoveFirstBlob(correct: true);
						state.CorrectHitEffect(beatType);
						return TapEffect.TAP_CORRECT;
					}
					state.IncorrectHitEffect(spawnedBlobs[0].beatType);
					RemoveFirstBlob(correct: false);
					return TapEffect.TAP_INCORRECT;
				}
			}
			return TapEffect.TAP_NO_EFFECT;
		}

		private float GetCorrectPercent(float position)
		{
			return 1f - Mathf.Abs(0.14f - position) / 0.14f;
		}
	}
}
