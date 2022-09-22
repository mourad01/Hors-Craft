// DecompilerFi decompiler from Assembly-CSharp.dll class: GamePlayDoorObject
using Gameplay.Spawning;
using System;
using UnityEngine;

[Serializable]
public class GamePlayDoorObject : GamePlayObjectBase
{
	public bool locked = true;

	public int itemToUnlockId = -1;

	public GamePlayDoorObject(int id, Vector3 position, GameObject objectInstance)
		: base(id, position, objectInstance)
	{
	}

	public override GameObject Spawn(Transform parentToSet = null)
	{
		if (!locked)
		{
			return null;
		}
		GameObject gameObject = new GameObject($"Door Locker (item{itemToUnlockId})");
		gameObject.transform.position = position;
		gameObject.transform.SetParent(parentToSet);
		return gameObject;
	}

	internal bool Interact()
	{
		if (itemToUnlockId < 0)
		{
			return true;
		}
		if (Singleton<PlayerData>.get.playerItems.GetResourcesCount(itemToUnlockId) > 0)
		{
			return true;
		}
		return false;
	}
}
