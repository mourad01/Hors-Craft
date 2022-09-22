// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.TierEnhancementSpawning
using Common.Managers;
using Gameplay;
using System;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public class TierEnhancementSpawning : AbstractSpawnSpawningBehaviour
	{
		[Serializable]
		public class TierSettings
		{
			public int tierStartWave;

			public float tierMultiplier;

			public bool FallInTier(int wave, int iteration)
			{
				return tierStartWave <= wave + iteration;
			}
		}

		public TierSettings[] tiersSettings;

		public float[] baseHp;

		public float[] baseDmg;

		private void Start()
		{
			SurvivalTierSpawnModule survivalTierSpawnSettings = Manager.Get<ModelManager>().survivalTierSpawnSettings;
			survivalTierSpawnSettings.onDownload = (Action)Delegate.Combine(survivalTierSpawnSettings.onDownload, new Action(TrySetup));
		}

		public override void AfterSpawn(GameObject spawnedMonster, int wave, int iteration, int index)
		{
			int num = index / baseHp.Length;
			int num2 = index % baseHp.Length;
			IFighting componentInChildren = spawnedMonster.GetComponentInChildren<IFighting>();
			componentInChildren.HealtMultiplier(tiersSettings[num].tierMultiplier, baseHp[num2]);
			componentInChildren.DamageMultiplier(tiersSettings[num].tierMultiplier, baseDmg[num2]);
		}

		private void TrySetup()
		{
			SurvivalTierSpawnModule survivalTierSpawnSettings = Manager.Get<ModelManager>().survivalTierSpawnSettings;
			for (int i = 0; i < tiersSettings.Length; i++)
			{
				tiersSettings[i].tierStartWave = survivalTierSpawnSettings.GetStartWave(i);
				tiersSettings[i].tierMultiplier = survivalTierSpawnSettings.GetTierModifier(i);
			}
			for (int j = 0; j < baseHp.Length; j++)
			{
				baseHp[j] = survivalTierSpawnSettings.GetBaseHP(j, baseHp[j]);
				baseDmg[j] = survivalTierSpawnSettings.GetBaseDmg(j, baseDmg[j]);
			}
		}
	}
}
