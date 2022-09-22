// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.TerrainGenerator
using Common.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Uniblocks
{
	public class TerrainGenerator : MonoBehaviour
	{
		public int waterHeight = 12;

		public void Generate(ChunkData chunk, bool testWorld, bool setDone = true)
		{
			int worldSeed = Engine.WorldSeed;
			if (!testWorld)
			{
				GenerateVoxelData(worldSeed, chunk);
			}
			else
			{
				GenerateTestWorldVoxelData(worldSeed, chunk);
			}
			chunk.Empty = true;
			ushort[] voxelData = chunk.VoxelData;
			for (int i = 0; i < voxelData.Length; i++)
			{
				if (voxelData[i] != 0)
				{
					chunk.Empty = false;
					break;
				}
			}
			if (setDone)
			{
				chunk.VoxelsDone = true;
			}
		}

		protected virtual void GenerateVoxelData(int seed, ChunkData chunk)
		{
		}

		public virtual int GetWorldHeight(VoxelInfo info)
		{
			return 0;
		}

		protected virtual void GenerateTestWorldVoxelData(int seed, ChunkData chunk)
		{
			int x = chunk.ChunkIndex.x;
			int y = chunk.ChunkIndex.y;
			int z = chunk.ChunkIndex.z;
			int sideLength = ChunkData.SideLength;
			GetVoxelsByPriorityAndCategory(out Dictionary<Voxel.Category, List<Voxel>> catToVoxels);
			int count = catToVoxels.Keys.Count;
			for (int i = 0; i < sideLength; i++)
			{
				for (int j = 0; j < sideLength; j++)
				{
					for (int k = 0; k < sideLength; k++)
					{
						int num = -(k + sideLength * x);
						int num2 = i + sideLength * y;
						int num3 = j + sideLength * z;
						int num4 = num / 4;
						int num5 = num3 / 4;
						bool flag = num % 4 == 0 && num3 % 4 == 0 && num4 >= 0 && num5 >= 0 && num4 / 7 < count;
						int num6 = num4 / 7;
						if (num2 <= 8)
						{
							if (!flag)
							{
								chunk.SetVoxelSimple(k, i, j, Engine.usefulIDs.grassBlockID);
							}
							else
							{
								chunk.SetVoxelSimple(k, i, j, (num6 % 2 != 0) ? Engine.usefulIDs.dirtBlockID : Engine.usefulIDs.stoneBlockID);
							}
						}
						if (num2 == 12 && flag)
						{
							int num7 = num4 % 7 + num5 * 7;
							List<Voxel> list = catToVoxels[(Voxel.Category)num6];
							if (num7 >= 0 && num7 < list.Count)
							{
								chunk.SetVoxelSimple(k, i, j, list[num7].GetUniqueID());
								chunk.SetRotation(k, i, j, 2);
							}
						}
					}
				}
			}
		}

		private void GetVoxelsByPriorityAndCategory(out Dictionary<Voxel.Category, List<Voxel>> catToVoxels)
		{
			catToVoxels = new Dictionary<Voxel.Category, List<Voxel>>();
			IEnumerator enumerator = Enum.GetValues(typeof(Voxel.Category)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Voxel.Category key = (Voxel.Category)enumerator.Current;
					catToVoxels.Add(key, new List<Voxel>());
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			Voxel[] blocks = Engine.Blocks;
			foreach (Voxel voxel in blocks)
			{
				if (voxel.GetUniqueID() != 0 && !voxel.editorOnly)
				{
					List<Voxel> list = catToVoxels[voxel.blockCategory];
					list.Add(voxel);
				}
			}
			IEnumerator enumerator2 = Enum.GetValues(typeof(Voxel.Category)).GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Voxel.Category category = (Voxel.Category)enumerator2.Current;
					if (category == Voxel.Category.craftable)
					{
						catToVoxels[category].RemoveAll((Voxel a) => Manager.Get<CraftingManager>().GetCraftableFromBlock(a.GetUniqueID()) == null);
						catToVoxels[category].Sort(delegate(Voxel alice, Voxel bob)
						{
							Craftable craftableFromBlock = Manager.Get<CraftingManager>().GetCraftableFromBlock(alice.GetUniqueID());
							Craftable craftableFromBlock2 = Manager.Get<CraftingManager>().GetCraftableFromBlock(bob.GetUniqueID());
							int num = craftableFromBlock.GetStatusValue() - craftableFromBlock2.GetStatusValue();
							return (num == 0) ? (craftableFromBlock.id - craftableFromBlock2.id) : num;
						});
					}
					else
					{
						catToVoxels[category] = (from v in catToVoxels[category]
							orderby Engine.EngineInstance.voxelToPriority[v]
							select v).ToList();
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}
	}
}
