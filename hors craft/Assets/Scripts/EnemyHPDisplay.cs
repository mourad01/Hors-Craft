// DecompilerFi decompiler from Assembly-CSharp.dll class: EnemyHPDisplay
using UnityEngine;

public class EnemyHPDisplay : MonoBehaviour
{
	public GameObject prefab;

	private Health _health;

	private Material material;

	protected Health health => _health ?? (_health = GetComponentInChildren<Health>());

	private void Start()
	{
		Object.Instantiate(prefab, base.transform);
	}
}
