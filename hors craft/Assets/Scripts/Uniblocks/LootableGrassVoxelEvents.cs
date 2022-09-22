// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.LootableGrassVoxelEvents
using UnityEngine;

namespace Uniblocks
{
	public class LootableGrassVoxelEvents : VoxelGrass
	{
		public ushort lootId;

		public float probabilityPercent = 100f;

		public int quantityMin;

		public int quantityMax = 10;

		private const float DROP_RANGE = 1.5f;

		public override void OnBlockDestroy(VoxelInfo voxelInfo)
		{
			DropLoot(voxelInfo);
			base.OnBlockDestroy(voxelInfo);
		}

		private void DropLoot(VoxelInfo voxelInfo)
		{
			if (Random.value < probabilityPercent / 100f)
			{
				Vector3 vector = voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
				int num = Random.Range(quantityMin, quantityMax + 1);
				for (int i = 0; i < num; i++)
				{
					float x = vector.x + Random.Range(-1f, 1f) * 1.5f;
					float z = vector.z + Random.Range(-1f, 1f) * 1.5f;
					Vector3 lootPosition = new Vector3(x, vector.y + 1f, z);
					SpawnLoot(voxelInfo, lootPosition);
				}
			}
		}

		private void SpawnLoot(VoxelInfo voxelInfo, Vector3 lootPosition)
		{
			GameObject gameObject = Object.Instantiate(Engine.GetVoxelGameObject(lootId), lootPosition, Quaternion.identity);
			CollectibleVoxel component = gameObject.GetComponent<CollectibleVoxel>();
			if (component != null)
			{
				component.id = lootId;
			}
		}
	}
}
