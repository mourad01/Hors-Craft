// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.DefaultLootDropBehaviour
using Common.Audio;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using GameUI;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class DefaultLootDropBehaviour : AbstractLootDropBehaviour
	{
		[Serializable]
		public class RarityPrefab
		{
			public LootChestManager.Rarity rarity;

			public GameObject prefab;
		}

		public List<RarityPrefab> raritiesToPrefabs = new List<RarityPrefab>();

		private LootChest chest;

		private GameObject spawned;

		public override void Drop(LootChest chest)
		{
			this.chest = chest;
			chest.Claim();
			RarityPrefab rarityPrefab = raritiesToPrefabs.FirstOrDefault((RarityPrefab rtp) => rtp.rarity == chest.rarity);
			Spawn(rarityPrefab.prefab);
		}

		private void Spawn(GameObject prefab)
		{
			Manager.Get<StateMachineManager>().PushState<DummyState>(new DummyStateStartParameter
			{
				doOnStart = delegate
				{
					Manager.Get<CommonUIManager>().SetBackground(backgroundVisible: false, backgroundOverlayVisible: false);
				}
			});
			spawned = UnityEngine.Object.Instantiate(prefab);
			PlayAudio();
			StartCoroutine(GoBackFromDummy());
		}

		private IEnumerator GoBackFromDummy()
		{
			yield return new WaitForSecondsRealtime(1.5f);
			Manager.Get<StateMachineManager>().PopState();
			Manager.Get<StateMachineManager>().PushState<ClaimRewardsState>(new ClaimRewardsStateStartParameter
			{
				rewards = chest.rewards,
				spawnedChest = spawned
			});
		}

		private void PlayAudio()
		{
			Sound sound = new Sound();
			sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.DAILY_REWARD);
			sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}
	}
}
