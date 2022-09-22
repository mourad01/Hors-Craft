// DecompilerFi decompiler from Assembly-CSharp.dll class: AnimationHPController
using System;
using UnityEngine;

public class AnimationHPController : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string[] parameters;

	[SerializeField]
	private bool areBooleans;

	[SerializeField]
	private Health healtController;

	private float[] damageLevelsTresholds;

	private void Start()
	{
		float num = healtController.maxHp / (float)parameters.Length;
		damageLevelsTresholds = new float[parameters.Length - 1];
		float num2 = healtController.maxHp;
		for (int i = 0; i < damageLevelsTresholds.Length; i++)
		{
			num2 -= num;
			damageLevelsTresholds[i] = num2;
		}
		Health health = healtController;
		health.onHpChangeAction = (Health.DoOnHpChange)Delegate.Combine(health.onHpChangeAction, new Health.DoOnHpChange(UpdateAnimator));
	}

	private void UpdateAnimator()
	{
		for (int i = 0; i < damageLevelsTresholds.Length; i++)
		{
			if (healtController.hp > damageLevelsTresholds[i])
			{
				animator.SetTrigger(parameters[i]);
				return;
			}
		}
		animator.SetTrigger(parameters[parameters.Length - 1]);
	}
}
