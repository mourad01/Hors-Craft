// DecompilerFi decompiler from Assembly-CSharp.dll class: TestObjectPoolManager
using UnityEngine;

public class TestObjectPoolManager : MonoBehaviour
{
	protected ObjectPoolItem spawnedBullet;

	private void Update()
	{
		if (UnityEngine.Input.GetKey(KeyCode.Space))
		{
			Shoot();
		}
	}

	private void Shoot()
	{
	}
}
