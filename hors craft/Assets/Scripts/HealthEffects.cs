// DecompilerFi decompiler from Assembly-CSharp.dll class: HealthEffects
using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class HealthEffects : MonoBehaviour
{
	[Serializable]
	public struct Effect
	{
		public GameObject effect;

		[Range(0f, 1f)]
		public float percentage;
	}

	[SerializeField]
	private Effect[] effects;

	private Health health;

	private float maxHealth;

	private GameObject currentEffect;

	private void Awake()
	{
		health = GetComponent<Health>();
		maxHealth = health.maxHp;
	}

	private void OnEnable()
	{
		health.onHitAction += OnHitAction;
	}

	private void OnDisable()
	{
		health.onHitAction -= OnHitAction;
	}

	public void OnHitAction(Vector3 dir)
	{
		float hp = health.hp;
		float num = hp / maxHealth;
		Effect[] array = effects;
		for (int i = 0; i < array.Length; i++)
		{
			Effect effect = array[i];
			if (!currentEffect && effect.percentage > num)
			{
				SpawnNewEffect(effect);
			}
		}
	}

	private void SpawnNewEffect(Effect effect)
	{
		currentEffect = UnityEngine.Object.Instantiate(effect.effect);
		currentEffect.transform.SetParent(base.transform);
		currentEffect.transform.localPosition = Vector3.zero;
	}
}
