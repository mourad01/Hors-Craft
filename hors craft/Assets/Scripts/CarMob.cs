// DecompilerFi decompiler from Assembly-CSharp.dll class: CarMob
using Common.Managers;
using UnityEngine;

public class CarMob : Mob
{
	public bool hasToSpawnHumanWhenHijacked;

	public void HijackCar()
	{
		SaveTransform componentInChildren = GetComponentInChildren<SaveTransform>(includeInactive: true);
		if (componentInChildren != null)
		{
			componentInChildren.enabled = true;
		}
		DestroyCar();
		if (!hasToSpawnHumanWhenHijacked)
		{
			SummonHuman(new Vector3(2.3f, 0f));
		}
	}

	public void OnCarPickup(bool summonHuman)
	{
		DestroyCar();
		if (summonHuman)
		{
			SummonHuman(new Vector3(0f, 0f, 0f));
		}
	}

	private void DestroyCar()
	{
		UnityEngine.Object.Destroy(this);
		UnityEngine.Object.Destroy(GetComponent<CarNavigator>());
	}

	private void SummonHuman(Vector3 humanOffset)
	{
		MobsManager.MobSpawnConfig[] spawnConfigs = Manager.Get<MobsManager>().spawnConfigs;
		int num = 0;
		MobsManager.MobSpawnConfig mobSpawnConfig;
		while (true)
		{
			if (num < spawnConfigs.Length)
			{
				mobSpawnConfig = spawnConfigs[num];
				if (mobSpawnConfig.prefab.GetComponent<HumanMob>() != null)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		Vector3 pos = base.transform.position.Add(humanOffset);
		Manager.Get<MobsManager>().PlanSpawn(mobSpawnConfig, pos);
	}
}
