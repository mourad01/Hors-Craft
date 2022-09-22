// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.CollectibleVoxel
using UnityEngine;

namespace Uniblocks
{
	public class CollectibleVoxel : Voxel
	{
		public enum Type
		{
			INVENTORY_ITEM,
			HEALTH,
			FOOD
		}

		public Type type;

		public int quantity = 1;

		[HideInInspector]
		public ushort id;

		private void Start()
		{
			GameObject gameObject = base.transform.Find("Sprite").gameObject;
			gameObject.AddComponent<SpriteRenderer>();
			SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
			component.sprite = voxelSprite;
		}

		private void Update()
		{
			Vector3 position = base.transform.position;
			if (position.y < -64f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void OnCollected()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
