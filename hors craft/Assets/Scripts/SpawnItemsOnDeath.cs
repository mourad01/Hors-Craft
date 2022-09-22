// DecompilerFi decompiler from Assembly-CSharp.dll class: SpawnItemsOnDeath
using AirStrikeKit;
using Common.Managers;
using System;
using UnityEngine;

public class SpawnItemsOnDeath : ObjectPoolSpawnerBase
{
	[Range(0f, 100f)]
	[SerializeField]
	protected float spawnChance = 30f;

	private AIPlaneController aiPlane;

	private void Start()
	{
		Init();
	}

	public override void Init()
	{
		aiPlane = GetComponent<AIPlaneController>();
		if (!(aiPlane == null))
		{
			AIPlaneController aIPlaneController = aiPlane;
			aIPlaneController.onDie = (Action)Delegate.Remove(aIPlaneController.onDie, new Action(((ObjectPoolSpawnerBase)this).Spawn));
			AIPlaneController aIPlaneController2 = aiPlane;
			aIPlaneController2.onDie = (Action)Delegate.Combine(aIPlaneController2.onDie, new Action(((ObjectPoolSpawnerBase)this).Spawn));
		}
	}

	public override void Spawn()
	{
		if ((float)UnityEngine.Random.Range(0, 100) < spawnChance || aiPlane == null)
		{
			return;
		}
		ObjectPoolManager objectPoolManager = Manager.Get<ObjectPoolManager>();
		if (objectPoolManager == null)
		{
			return;
		}
		ObjectPoolItem newObject = null;
		for (int i = 0; i < howMuchToSpawn; i++)
		{
			((IObjectPool)objectPoolManager).GetObject(idToSpawn, out newObject);
			if (!(newObject == null))
			{
				Vector3 b = Vector3.forward * 5f;
				newObject.transform.localPosition = base.transform.localPosition + b;
				newObject.transform.localRotation = UnityEngine.Random.rotation;
			}
		}
		AIPlaneController aIPlaneController = aiPlane;
		aIPlaneController.onDie = (Action)Delegate.Remove(aIPlaneController.onDie, new Action(((ObjectPoolSpawnerBase)this).Spawn));
	}
}
