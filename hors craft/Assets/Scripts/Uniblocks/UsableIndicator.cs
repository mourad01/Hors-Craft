// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.UsableIndicator
using Common.Managers;
using Gameplay;
using System;
using UnityEngine;

namespace Uniblocks
{
	public class UsableIndicator : MonoBehaviour
	{
		public UsableIndicatorSingleConfig usableIndicatorConfig;

		public Transform indicatorHolder;

		private GameObject usableIndicatorInstance;

		public static Action checkAfterInteraction;

		private void Awake()
		{
			if (usableIndicatorConfig == null || !Manager.Contains<ModelManager>() || !Manager.Get<ModelManager>().configSettings.IsUsableIndicatorsEnabled())
			{
				UnityEngine.Object.Destroy(this);
				return;
			}
			checkAfterInteraction = (Action)Delegate.Combine(checkAfterInteraction, new Action(CheckIfEnabled));
			if (usableIndicatorConfig.spawnOnStart)
			{
				SpawnIndicator();
			}
		}

		private void OnDestroy()
		{
			checkAfterInteraction = (Action)Delegate.Remove(checkAfterInteraction, new Action(CheckIfEnabled));
		}

		public void SpawnIndicator()
		{
			if (usableIndicatorInstance == null && !HaveIteracted() && usableIndicatorConfig.usableIndicatorPrefab != null)
			{
				usableIndicatorInstance = UnityEngine.Object.Instantiate(usableIndicatorConfig.usableIndicatorPrefab, indicatorHolder, worldPositionStays: false);
			}
		}

		public void DestroyIndicator()
		{
			if (usableIndicatorInstance != null)
			{
				UnityEngine.Object.Destroy(usableIndicatorInstance);
			}
		}

		public bool HaveIteracted()
		{
			return PlayerPrefs.GetInt("interacted.with." + base.gameObject.name, 0) == 1;
		}

		public void Interact()
		{
			PlayerPrefs.SetInt("interacted.with." + base.gameObject.name, 1);
			checkAfterInteraction();
		}

		private void CheckIfEnabled()
		{
			if (HaveIteracted())
			{
				DestroyIndicator();
			}
		}
	}
}
