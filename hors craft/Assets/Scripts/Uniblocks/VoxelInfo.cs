// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelInfo
using System.Collections.Generic;

namespace Uniblocks
{
	public class VoxelInfo
	{
		public Index index;

		public Index adjacentIndex;

		public readonly ChunkData chunk;

		public int rawIndex => ChunkData.IndexToRawIndex(index);

		public VoxelInfo(int setX, int setY, int setZ, ChunkData setChunk)
		{
			index.x = setX;
			index.y = setY;
			index.z = setZ;
			chunk = setChunk;
		}

		public VoxelInfo(int setX, int setY, int setZ, int setXa, int setYa, int setZa, ChunkData setChunk)
		{
			index.x = setX;
			index.y = setY;
			index.z = setZ;
			adjacentIndex.x = setXa;
			adjacentIndex.y = setYa;
			adjacentIndex.z = setZa;
			chunk = setChunk;
		}

		public VoxelInfo(Index setIndex, ChunkData setChunk)
		{
			index = setIndex;
			chunk = setChunk;
		}

		public VoxelInfo(Index setIndex, Index setAdjacentIndex, ChunkData setChunk)
		{
			index = setIndex;
			adjacentIndex = setAdjacentIndex;
			chunk = setChunk;
		}

		public ushort GetVoxel()
		{
			return chunk.GetVoxel(index);
		}

		public byte GetVoxelRotation()
		{
			return chunk.GetVoxelRotation(index);
		}

		public float GetVoxelFinalRotationInDegrees()
		{
			byte voxelRotation = GetVoxelRotation();
			MeshRotation vRotation = GetVoxelType().VRotation;
			return RotationsUtility.MeshRotationToDegrees(RotationsUtility.RotateMeshRotation(voxelRotation, vRotation));
		}

		public Voxel GetVoxelType()
		{
			return Engine.GetVoxelType(chunk.GetVoxel(index));
		}

		public ushort GetAdjacentVoxel()
		{
			return chunk.GetVoxel(adjacentIndex);
		}

		public Voxel GetAdjacentVoxelType()
		{
			return Engine.GetVoxelType(chunk.GetVoxel(adjacentIndex));
		}

		public void SetVoxel(ushort data, bool updateMesh, byte rotation = 0)
		{
			chunk.SetVoxel(index, data, updateMesh, rotation);
		}

		public void SetRotation(byte data)
		{
			chunk.SetRotation(index.x, index.y, index.z, data);
		}

		public override string ToString()
		{
			return $"[VoxelInfo: index={index.x},{index.y},{index.z}]";
		}

		public static string GetGlobalIndex(ChunkData chunk, Index index)
		{
			string arg = ChunkDataFiles.GetParentRegion(chunk.ChunkIndex).ToString();
			string arg2 = chunk.ChunkIndex.ToString();
			string arg3 = index.ToString();
			return $"{arg}|{arg2}|{arg3}";
		}

		public string GetGlobalIndex()
		{
			string arg = ChunkDataFiles.GetParentRegion(chunk.ChunkIndex).ToString();
			string arg2 = chunk.ChunkIndex.ToString();
			string arg3 = index.ToString();
			return $"{arg}|{arg2}|{arg3}";
		}

		public static VoxelInfo FromGlobalIndex(string ind)
		{
			string[] array = ind.Split('|');
			Index key = Index.FromString(array[1]);
			Index index = Index.FromString(array[2]);
			ChunkManager.ChunkDatas.TryGetValue(key, out ChunkData value);
			return value?.GetVoxelInfo(index);
		}

		public VoxelInfo GetNearbyVoxelInfo(int x, int y, int z)
		{
			return chunk.GetVoxelInfo(index.x + x, index.y + y, index.z + z);
		}

		public void GetChunkAndNeighbours(List<ChunkData> chunks)
		{
			chunks.Add(chunk);
			ChunkData value = null;
			Index chunkIndex = chunk.ChunkIndex;
			if (index.x == 0)
			{
				ChunkManager.ChunkDatas.TryGetValue(new Index(chunkIndex.x - 1, chunkIndex.y, chunkIndex.z), out value);
			}
			else if (index.x == ChunkData.SideLength - 1)
			{
				ChunkManager.ChunkDatas.TryGetValue(new Index(chunkIndex.x + 1, chunkIndex.y, chunkIndex.z), out value);
			}
			if (value != null)
			{
				chunks.Add(value);
				value = null;
			}
			if (index.y == 0)
			{
				ChunkManager.ChunkDatas.TryGetValue(new Index(chunkIndex.x, chunkIndex.y - 1, chunkIndex.z), out value);
			}
			else if (index.y == ChunkData.SideLength - 1)
			{
				ChunkManager.ChunkDatas.TryGetValue(new Index(chunkIndex.x, chunkIndex.y + 1, chunkIndex.z), out value);
			}
			if (value != null)
			{
				chunks.Add(value);
				value = null;
			}
			if (index.z == 0)
			{
				ChunkManager.ChunkDatas.TryGetValue(new Index(chunkIndex.x, chunkIndex.y, chunkIndex.z - 1), out value);
			}
			else if (index.z == ChunkData.SideLength - 1)
			{
				ChunkManager.ChunkDatas.TryGetValue(new Index(chunkIndex.x, chunkIndex.y, chunkIndex.z + 1), out value);
			}
			if (value != null)
			{
				chunks.Add(value);
				value = null;
			}
		}
	}
}
