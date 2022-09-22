// DecompilerFi decompiler from Assembly-CSharp.dll class: ShipSailsDestruction
using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class ShipSailsDestruction : MonoBehaviour
{
	[SerializeField]
	private GameObject[] sails;

	[SerializeField]
	private Material destructableMaterial;

	private Material sharedMaterial;

	private Health shipHealth;

	private void Awake()
	{
		shipHealth = GetComponent<Health>();
		ApplyNewMaterial();
		OnHealthChange();
		Health health = shipHealth;
		health.onHpChangeAction = (Health.DoOnHpChange)Delegate.Combine(health.onHpChangeAction, new Health.DoOnHpChange(OnHealthChange));
	}

	private void ApplyNewMaterial()
	{
		sharedMaterial = new Material(destructableMaterial);
		GameObject[] array = sails;
		foreach (GameObject gameObject in array)
		{
			gameObject.GetComponent<Renderer>().material = sharedMaterial;
		}
	}

	private void OnHealthChange()
	{
		float value = shipHealth.hp / shipHealth.maxHp;
		sharedMaterial.SetFloat("_Cutoff", value);
	}
}
