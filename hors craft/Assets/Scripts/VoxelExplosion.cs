// DecompilerFi decompiler from Assembly-CSharp.dll class: VoxelExplosion
using Common.Audio;
using Common.Behaviours;
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class VoxelExplosion : MonoBehaviour
{
	public ParticleSystem explosionSystem;

	public float radius = 5f;

	public float maxDamage = 3f;

	[HideInInspector]
	public float baseDamage;

	private TriggerTargetAcquisition _targetAcquisition;

	private TriggerTargetAcquisition targetAcquisition
	{
		get
		{
			if (_targetAcquisition == null)
			{
				InitTargetAcquisition();
			}
			return _targetAcquisition;
		}
	}

	private void Awake()
	{
		InitTargetAcquisition();
	}

	protected float Damage(float r)
	{
		float num = maxDamage;
		float num2 = 0f - maxDamage / Mathf.Pow(radius, 2f);
		float b = num2 * r * r + num;
		return baseDamage + Mathf.Max(0f, b);
	}

	protected void InitTargetAcquisition()
	{
		_targetAcquisition = GetComponent<TriggerTargetAcquisition>();
		if (_targetAcquisition == null)
		{
			_targetAcquisition = base.gameObject.AddComponent<TriggerTargetAcquisition>();
			_targetAcquisition.setRange = radius;
		}
	}

	public static void ExplodeTerrain(ParticleSystem particles, Transform transform, float range, bool digDown = true, bool canDestroyAllBlocks = false)
	{
		ExplodeTerrain(particles, transform, range, digDown, out bool _, canDestroyAllBlocks);
	}

	public static void ExplodeTerrain(ParticleSystem particles, Transform transform, float range, bool digDown, out bool somethingDestroyed, bool canDestroyAllBlocks = false)
	{
		List<VoxelInfo> list = CreateDestroyableVoxelInfo(transform, range, digDown, canDestroyAllBlocks);
		somethingDestroyed = (list != null && list.Count > 0);
		DestroyVoxels(list);
		ExplodeEffects(particles);
	}

	public void ExplodeAtCollider(Collider collider)
	{
		ExplodeTerrain(explosionSystem, base.transform, radius);
		DealDamage(collider.gameObject);
	}

	public void ExplodeArea(bool digDown = true, bool canDestroyAllBlocks = false)
	{
		ExplodeArea(IsEnemyDefault, digDown, canDestroyAllBlocks);
	}

	public void ExplodeArea(Func<GameObject, bool> IsEnemyFunction, bool digDown = true, bool canDestroyAllBlocks = false)
	{
		List<GameObject> targets = targetAcquisition.GetTargets(IsEnemyFunction);
		targets.ForEach(delegate(GameObject t)
		{
			DealDamage(t);
		});
		List<VoxelInfo> voxelsToDestroy = CreateDestroyableVoxelInfo(base.transform, 0.5f * radius, digDown, canDestroyAllBlocks);
		DestroyVoxels(voxelsToDestroy);
		ExplodeEffects(explosionSystem);
	}

	private bool IsEnemyDefault(GameObject go)
	{
		return (go.GetComponentInParent<Mob>() as IFighting)?.IsEnemy() ?? false;
	}

	private void DealDamage(GameObject targetObject)
	{
		Health componentInParent = targetObject.GetComponentInParent<Health>();
		if (!(componentInParent != null))
		{
			return;
		}
		float r = Vector3.Distance(targetObject.transform.position, base.transform.position);
		float dmg = Damage(r);
		bool flag = componentInParent.hp > 0f;
		componentInParent.OnHit(dmg, (targetObject.transform.position - base.transform.position).normalized, 2f);
		if (flag && componentInParent.hp <= 0f)
		{
			IFighting componentInParent2 = targetObject.GetComponentInParent<IFighting>();
			if (componentInParent2 != null && componentInParent2.IsEnemy())
			{
				Manager.Get<QuestManager>().OnEnemyKilled();
			}
		}
	}

	private static List<VoxelInfo> CreateDestroyableVoxelInfo(Transform transform, float radius, bool digDown = true, bool canDestroyAllBlocks = false)
	{
		List<VoxelInfo> list = new List<VoxelInfo>();
		VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(transform.position);
		if (!digDown)
		{
			voxelInfo = Engine.PositionToVoxelInfo(transform.position + Vector3.up * 0.1f);
		}
		if (voxelInfo == null)
		{
			return null;
		}
		int num = Mathf.RoundToInt(radius);
		int num2 = digDown ? 1 : 0;
		for (int i = voxelInfo.index.y - num2; i < voxelInfo.index.y + num; i++)
		{
			for (int j = voxelInfo.index.x - num; j < voxelInfo.index.x + num; j++)
			{
				for (int k = voxelInfo.index.z - num; k < voxelInfo.index.z + num; k++)
				{
					if (canDestroyAllBlocks || (voxelInfo.chunk.GetVoxelInfo(j, i, k) != null && CanDestroyVoxel(voxelInfo.chunk.GetVoxelInfo(j, i, k).GetVoxel())))
					{
						list.Add(voxelInfo.chunk.GetVoxelInfo(j, i, k));
					}
				}
			}
		}
		return list;
	}

	protected static bool CanDestroyVoxel(ushort id)
	{
		return id != Engine.usefulIDs.stoneBlockID && id != Engine.usefulIDs.waterBlockID;
	}

	private static void DestroyVoxels(List<VoxelInfo> voxelsToDestroy)
	{
		if (voxelsToDestroy != null)
		{
			foreach (VoxelInfo item in voxelsToDestroy)
			{
				if (item != null)
				{
					Voxel.DestroyBlock(item);
				}
			}
		}
	}

	private static void ExplodeEffects(ParticleSystem explosionSystem)
	{
		Sound sound = new Sound();
		sound.clip = Manager.Get<ClipsManager>().GetClipFor(GameSound.CREEPER_EXPLODE);
		sound.mixerGroup = Manager.Get<Gameplay.Audio.MixersManager>().uiMixerGroup;
		Sound sound2 = sound;
		sound2.Play();
		if (explosionSystem != null)
		{
			explosionSystem.transform.SetParent(null, worldPositionStays: true);
			explosionSystem.Play();
			DestroyAfter destroyAfter = explosionSystem.gameObject.AddComponent<DestroyAfter>();
			destroyAfter.delay = explosionSystem.main.duration;
		}
	}
}
