// DecompilerFi decompiler from Assembly-CSharp.dll class: Fixable
using Gameplay.Audio;
using Uniblocks;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Fixable : InteractiveObject
{
	public AudioClip fixSound;

	public float hpPerFix;

	public int resourceId;

	public int resourceRequired;

	public Health health
	{
		get;
		private set;
	}

	protected override void Awake()
	{
		base.Awake();
		health = GetComponent<Health>();
		isUsable = true;
		isRotatable = false;
		isDestroyable = false;
	}

	[ContextMenu("Fix")]
	public void Fix(bool toMaxHp = false)
	{
		if (toMaxHp)
		{
			health.hp = health.maxHp;
		}
		else
		{
			health.hp += hpPerFix;
		}
		MixersManager.Play(fixSound);
	}

	public bool TryFix()
	{
		int resourcesCount = Singleton<PlayerData>.get.playerItems.GetResourcesCount(resourceId);
		if (resourcesCount >= resourceRequired)
		{
			Singleton<PlayerData>.get.playerItems.AddToResources(resourceId, -resourceRequired);
			health.hp += hpPerFix;
			MixersManager.Play(fixSound);
			return true;
		}
		return false;
	}
}
