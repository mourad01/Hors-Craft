// DecompilerFi decompiler from Assembly-CSharp.dll class: IFighting
using UnityEngine;

public interface IFighting
{
	bool IsEnemy();

	Mob GetMob();

	Health GetHealth();

	void Attack(Transform target);

	void HealtMultiplier(float multiplier, float baseValue = -1f);

	void DamageMultiplier(float multiplier, float baseValue = -1f);
}
