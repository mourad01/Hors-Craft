// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelEventsInstancesCache
using System.Collections.Generic;
using UnityEngine;

namespace Uniblocks
{
	public class VoxelEventsInstancesCache : Singleton<VoxelEventsInstancesCache>
	{
		private const int MAX_INSTANCES = 16;

		private Dictionary<ushort, VoxelEvents> voxelIdToInstance;

		private List<ushort> lastUsedVoxelIds;

		public VoxelEventsInstancesCache()
		{
			voxelIdToInstance = new Dictionary<ushort, VoxelEvents>();
			lastUsedVoxelIds = new List<ushort>();
		}

		public VoxelEvents GetInstanceForVoxelId(ushort voxelId)
		{
			if (voxelId == 0)
			{
				return null;
			}
			if (!voxelIdToInstance.TryGetValue(voxelId, out VoxelEvents value) || value == null)
			{
				value = CreateInstance(voxelId);
				DestroyMostUnusedInstanceIfNeccesary();
			}
			else
			{
				RegisterInstanceUsed(voxelId);
			}
			return value;
		}

		private VoxelEvents CreateInstance(ushort voxelId)
		{
			GameObject gameObject = Object.Instantiate(Engine.GetVoxelGameObject(voxelId), Vector3.down * 1000f, Quaternion.identity);
			VoxelEvents component = gameObject.GetComponent<VoxelEvents>();
			if (voxelIdToInstance.ContainsKey(voxelId))
			{
				voxelIdToInstance[voxelId] = component;
			}
			else
			{
				voxelIdToInstance.Add(voxelId, component);
			}
			if (lastUsedVoxelIds.Contains(voxelId))
			{
				lastUsedVoxelIds.Remove(voxelId);
			}
			lastUsedVoxelIds.Add(voxelId);
			return component;
		}

		private void RegisterInstanceUsed(ushort voxelId)
		{
			lastUsedVoxelIds.Remove(voxelId);
			lastUsedVoxelIds.Add(voxelId);
		}

		private void DestroyMostUnusedInstanceIfNeccesary()
		{
			if (lastUsedVoxelIds.Count > 16)
			{
				ushort key = lastUsedVoxelIds[0];
				lastUsedVoxelIds.RemoveAt(0);
				voxelIdToInstance.Remove(key);
			}
		}
	}
}
