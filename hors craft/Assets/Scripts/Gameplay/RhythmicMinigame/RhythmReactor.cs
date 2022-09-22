// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.RhythmicMinigame.RhythmReactor
using Common.Managers;
using UnityEngine;

namespace Gameplay.RhythmicMinigame
{
	public interface RhythmReactor
	{
		void OnSpawnBlob(int beatType);

		void OnCorrectHit(int beatType);

		void OnIncorrectHit(int beatType);

		void OnBlobFall(int beatType);

		void OnCorrectSequence();

		void OnBeat(int beatIndex);

		void SetTempo(float tempo);

		void SetPosition(Camera camera);

		void OnFinish();

		void OnSuccessScene();

		void OnFailureScene();

		void OnBlobInShootZone(int beatType);

		void Update();

		StatsManager.MinigameType GetGameType();
	}
}
