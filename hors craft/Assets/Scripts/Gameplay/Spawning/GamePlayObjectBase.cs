// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Spawning.GamePlayObjectBase
using Common.Managers;
using System;
using UnityEngine;

namespace Gameplay.Spawning
{
	[Serializable]
	public class GamePlayObjectBase
	{
		public int id;

		public Vector3 position;

		public bool isGathered;

		[NonSerialized]
		public GameObject objectInstance;

		public GamePlayObjectBase(int id, Vector3 position, GameObject objectInstance)
		{
			this.id = id;
			this.position = position;
			this.objectInstance = objectInstance;
		}

		public virtual GameObject Spawn(Transform parentToSet = null)
		{
			if (isGathered)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(Manager.Get<CraftingManager>().lootPrefab);
			ResourceSprite component = gameObject.GetComponent<ResourceSprite>();
			component.InitWithResourceId(position, id);
			component.IgnoreGravity = true;
			objectInstance = gameObject;
			position = gameObject.transform.position;
			if (parentToSet != null)
			{
				gameObject.transform.SetParent(parentToSet);
			}
			return gameObject;
		}
	}
}
