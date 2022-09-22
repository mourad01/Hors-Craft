// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalMobSpawning.AbstractSpawnPlacingBehaviour
using Common.Utils;
using System;
using Uniblocks;
using UnityEngine;

namespace SurvivalMobSpawning
{
	public abstract class AbstractSpawnPlacingBehaviour : MonoBehaviour
	{
		protected float spawnRadiusMin = 15f;

		protected float spawnRadiusMax = 25f;

		protected float spawnForwardOfPlayerFactor = 0.25f;

		protected abstract ushort placingBlockId
		{
			get;
		}

		public void Init(float spawnRadiusMin, float spawnRadiusMax, float spawnForwardOfPlayer)
		{
			this.spawnRadiusMin = spawnRadiusMin;
			this.spawnRadiusMax = spawnRadiusMax;
			spawnForwardOfPlayerFactor = spawnForwardOfPlayer;
		}

		public virtual Vector3 GetRandomPosition(GameObject prefab, GameObject player)
		{
			for (int num = 50; num > 0; num--)
			{
				Vector3 vector = GenerateRandomPosition(player);
				ushort num2 = ushort.MaxValue;
				while (num2 != placingBlockId && vector.y > 0f)
				{
					num2 = (Engine.PositionToVoxelInfo(vector)?.GetVoxel() ?? ushort.MaxValue);
					vector += Vector3.down;
				}
				if (num2 == placingBlockId)
				{
					vector += Vector3.up * 4f;
					if (MonsterFitsInPosition(prefab, vector))
					{
						return vector;
					}
				}
			}
			return GetRandomSpawnPosition(player);
		}

		protected Vector3 GenerateRandomPosition(GameObject player)
		{
			float num = UnityEngine.Random.Range(0, 360);
			float d = UnityEngine.Random.Range(spawnRadiusMin, spawnRadiusMax);
			Vector3 vector = new Vector3(Mathf.Cos((float)Math.PI / 180f * num), 0f, Mathf.Sin((float)Math.PI / 180f * num));
			Vector3 position = player.transform.position;
			Vector3 forward = player.transform.forward;
			Quaternion rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.FromToRotation(vector, forward), spawnForwardOfPlayerFactor);
			vector = rotation * vector * d;
			vector.x += position.x;
			vector.y = 100f;
			vector.z += position.z;
			return vector;
		}

		protected virtual Vector3 GetRandomSpawnPosition(GameObject player)
		{
			Vector3 vector = GenerateRandomPosition(player);
			if (Physics.Raycast(vector, Vector3.down, out RaycastHit hitInfo, 2000f))
			{
				Vector3 point = hitInfo.point;
				vector.y = point.y + 2f;
			}
			else
			{
				vector.y = 16f;
			}
			return vector;
		}

		protected virtual bool MonsterFitsInPosition(GameObject monster, Vector3 position)
		{
			Bounds bounds = RenderersBounds.MaximumBounds(monster);
			Vector3 extents = bounds.extents;
			int num = Mathf.CeilToInt(extents.x);
			Vector3 extents2 = bounds.extents;
			int num2 = Mathf.CeilToInt(extents2.y) - 1;
			Vector3 extents3 = bounds.extents;
			int num3 = Mathf.CeilToInt(extents3.z);
			for (float num4 = position.x - (float)num; num4 <= position.x + (float)num; num4 += 1f)
			{
				for (float num5 = position.y - (float)num2; num5 <= position.y + (float)num2; num5 += 1f)
				{
					for (float num6 = position.z - (float)num3; num6 <= position.z + (float)num3; num6 += 1f)
					{
						VoxelInfo voxelInfo = Engine.PositionToVoxelInfo(new Vector3(num4, num5, num6));
						if (voxelInfo.GetVoxel() != 0)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}
