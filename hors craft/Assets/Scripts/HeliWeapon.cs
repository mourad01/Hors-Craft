// DecompilerFi decompiler from Assembly-CSharp.dll class: HeliWeapon
using System;
using UnityEngine;

public class HeliWeapon : WeaponHandler
{
	[Serializable]
	private struct Parameters
	{
		public float cooldown;

		public AudioClip audioClip;
	}

	[SerializeField]
	private Parameters parameters;

	private float currentCooldown;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = parameters.audioClip;
		currentCooldown = parameters.cooldown;
	}

	protected void Fire()
	{
		UnityEngine.Debug.LogError("FIRE");
		PlaySound(parameters.audioClip);
	}

	public override void OnPress()
	{
		if (currentCooldown >= parameters.cooldown)
		{
			Fire();
			currentCooldown = 0f;
		}
	}

	public override void OnRelease()
	{
	}
}
