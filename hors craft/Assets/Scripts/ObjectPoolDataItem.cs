// DecompilerFi decompiler from Assembly-CSharp.dll class: ObjectPoolDataItem
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPoolDataItem
{
	[SerializeField]
	public int id;

	public List<ObjectPoolItem> spawnedObjects;

	[SerializeField]
	public int maxObjectsToSpawn;
}
