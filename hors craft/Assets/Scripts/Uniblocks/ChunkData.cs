// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ChunkData
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uniblocks
{
	public class ChunkData : IIndexable, IPoolAble
	{
		public struct doorState
		{
			public readonly bool doorOpen;

			public readonly byte doorRotation;

			public doorState(bool doorOpen, byte doorRotation)
			{
				this.doorOpen = doorOpen;
				this.doorRotation = doorRotation;
			}
		}

		public ushort[] VoxelData;

		public byte[] VoxelRotation;

		public Index ChunkIndex;

		public ChunkData[] NeighborChunks;

		public bool Empty;

		private bool changed;

		public bool rebuildOnMainThread;

		public bool hasWater;

		private bool _VoxelsDone;

		public bool ignoreVoxelEventsOnNextRebuild;

		public int[] minMaxXZ;

		public static int SideLength = 16;

		public static int SquaredSideLength = 256;

		public readonly Dictionary<int, doorState> doorsStates;

		public bool isDirty;

		private bool inited;

		public static bool testWorld;

		private Matrix4x4 object2World = Matrix4x4.identity;

		private Matrix4x4 word2Object = Matrix4x4.identity;

		public bool Changed
		{
			get
			{
				return changed;
			}
			set
			{
				changed = value;
				if (changed && inited)
				{
					Chunk value2 = null;
					ChunkManager.Chunks.TryGetValue(ChunkIndex, out value2);
					if (value2 != null)
					{
						value2.FlagToUpdate();
					}
					else
					{
						Generate();
					}
				}
			}
		}

		public bool VoxelsDone
		{
			get
			{
				return _VoxelsDone;
			}
			set
			{
				if (_VoxelsDone != value)
				{
					_VoxelsDone = value;
					if (_VoxelsDone)
					{
						ChunkManager.RegisterChunk(this);
						GetNeighbors();
						Generate();
					}
				}
			}
		}

		public bool InPool
		{
			get;
			private set;
		}

		public ChunkData()
		{
			NeighborChunks = new ChunkData[6];
			VoxelData = new ushort[SideLength * SideLength * SideLength];
			VoxelRotation = new byte[SideLength * SideLength * SideLength];
			doorsStates = new Dictionary<int, doorState>();
		}

		public IVector3Index GetIndex()
		{
			return ChunkIndex;
		}

		public void Activate()
		{
			InPool = false;
		}

		public void Deactivate()
		{
			if (inited)
			{
				ChunkManager.UnregisterChunk(this);
				int num = 0;
				for (int i = 0; i < NeighborChunks.Length; i++)
				{
					if (NeighborChunks[i] != null)
					{
						num = ((i % 2 != 0) ? (i - 1) : (i + 1));
						NeighborChunks[i].NeighborChunks[num] = null;
						NeighborChunks[i] = null;
					}
				}
			}
			inited = false;
			InPool = true;
			VoxelsDone = false;
			Empty = true;
		}

		public void Init(Index index)
		{
			ChunkIndex = index;
			GenerateMatrices();
			ClearVoxelData();
			GetVoxelData();
			inited = true;
		}

		public void GetVoxelData()
		{
			if (!Empty)
			{
				ClearVoxelData();
			}
			if (!Engine.SaveVoxelData || !TryLoadVoxelData())
			{
				GenerateVoxelData();
			}
		}

		private void GetVoxelDataThread(object info)
		{
			GenerateVoxelData();
		}

		private bool TryLoadVoxelData()
		{
			return ChunkDataFiles.LoadData(this);
		}

		private void GenerateVoxelData()
		{
			Engine.TerrainGenerator.Generate(this, testWorld);
		}

		public void ClearVoxelData()
		{
			Array.Clear(VoxelData, 0, VoxelData.Length);
		}

		public int GetDataLength()
		{
			return VoxelData.Length;
		}

		public void SetRotation(int rawIndex, byte data)
		{
			VoxelRotation[rawIndex] = data;
		}

		public void SetRotation(int x, int y, int z, byte data)
		{
			VoxelRotation[z * SquaredSideLength + y * SideLength + x] = data;
		}

		public void SetVoxelSimple(int rawIndex, ushort data)
		{
			VoxelData[rawIndex] = data;
		}

		public void SetVoxelSimple(int x, int y, int z, ushort data)
		{
			VoxelData[z * SquaredSideLength + y * SideLength + x] = data;
		}

		public void SetVoxelSimple(Index index, ushort data)
		{
			VoxelData[index.z * SquaredSideLength + index.y * SideLength + index.x] = data;
		}

		public void SetVoxel(int x, int y, int z, ushort data, bool updateMesh, byte rotation = 0)
		{
			if (x < 0)
			{
				if (NeighborChunks[3] != null)
				{
					NeighborChunks[3].SetVoxel(x + SideLength, y, z, data, updateMesh, rotation);
				}
				return;
			}
			if (x >= SideLength)
			{
				if (NeighborChunks[2] != null)
				{
					NeighborChunks[2].SetVoxel(x - SideLength, y, z, data, updateMesh, rotation);
				}
				return;
			}
			if (y < 0)
			{
				if (NeighborChunks[1] != null)
				{
					NeighborChunks[1].SetVoxel(x, y + SideLength, z, data, updateMesh, rotation);
				}
				return;
			}
			if (y >= SideLength)
			{
				if (NeighborChunks[0] != null)
				{
					NeighborChunks[0].SetVoxel(x, y - SideLength, z, data, updateMesh, rotation);
				}
				return;
			}
			if (z < 0)
			{
				if (NeighborChunks[5] != null)
				{
					NeighborChunks[5].SetVoxel(x, y, z + SideLength, data, updateMesh, rotation);
				}
				return;
			}
			if (z >= SideLength)
			{
				if (NeighborChunks[4] != null)
				{
					NeighborChunks[4].SetVoxel(x, y, z - SideLength, data, updateMesh, rotation);
				}
				return;
			}
			VoxelData[z * SquaredSideLength + y * SideLength + x] = data;
			VoxelRotation[z * SquaredSideLength + y * SideLength + x] = rotation;
			if (Empty && data != 0)
			{
				Empty = false;
			}
			if (updateMesh)
			{
				UpdateNeighborsIfNeeded(x, y, z);
				Changed = true;
			}
		}

		public void SetVoxel(Index index, ushort data, bool updateMesh, byte rotation = 0)
		{
			SetVoxel(index.x, index.y, index.z, data, updateMesh, rotation);
		}

		public void SetVoxelWater(Index index, ushort data, bool updateMesh, byte rotation = 0)
		{
			SetVoxel(index.x, index.y, index.z, data, updateMesh, 0);
			if (data == Engine.usefulIDs.waterBlockID)
			{
				Index index2 = new Index(index.x, index.y - 1, index.z);
				Index index3 = new Index(index.x - 1, index.y, index.z);
				Index index4 = new Index(index.x + 1, index.y, index.z);
				Index index5 = new Index(index.x, index.y, index.z + 1);
				Index index6 = new Index(index.x, index.y, index.z - 1);
				if (GetVoxel(index2) == 0)
				{
					SetVoxelWater(index2, data, updateMesh: true, 0);
				}
				if (GetVoxel(index5) == 0)
				{
					SetVoxelWater(index5, data, updateMesh: true, 0);
				}
				if (GetVoxel(index6) == 0)
				{
					SetVoxelWater(index6, data, updateMesh: true, 0);
				}
				if (GetVoxel(index4) == 0)
				{
					SetVoxelWater(index4, data, updateMesh: true, 0);
				}
				if (GetVoxel(index3) == 0)
				{
					SetVoxelWater(index3, data, updateMesh: true, 0);
				}
			}
		}

		public ushort GetVoxelSimple(int rawIndex)
		{
			return VoxelData[rawIndex];
		}

		public byte GetVoxelRotationSimple(int rawIndex)
		{
			return VoxelRotation[rawIndex];
		}

		public ushort GetVoxelSimple(int x, int y, int z)
		{
			return VoxelData[z * SquaredSideLength + y * SideLength + x];
		}

		public ushort GetVoxelSimple(Index index)
		{
			return VoxelData[index.z * SquaredSideLength + index.y * SideLength + index.x];
		}

		public byte GetVoxelRotation(int x, int y, int z)
		{
			return VoxelRotation[z * SquaredSideLength + y * SideLength + x];
		}

		public ushort GetVoxel(int x, int y, int z)
		{
			if (x < 0)
			{
				if (NeighborChunks[3] != null)
				{
					return NeighborChunks[3].GetVoxel(x + SideLength, y, z);
				}
				return ushort.MaxValue;
			}
			if (x >= SideLength)
			{
				if (NeighborChunks[2] != null)
				{
					return NeighborChunks[2].GetVoxel(x - SideLength, y, z);
				}
				return ushort.MaxValue;
			}
			if (y < 0)
			{
				if (NeighborChunks[1] != null)
				{
					return NeighborChunks[1].GetVoxel(x, y + SideLength, z);
				}
				return ushort.MaxValue;
			}
			if (y >= SideLength)
			{
				if (NeighborChunks[0] != null)
				{
					return NeighborChunks[0].GetVoxel(x, y - SideLength, z);
				}
				return ushort.MaxValue;
			}
			if (z < 0)
			{
				if (NeighborChunks[5] != null)
				{
					return NeighborChunks[5].GetVoxel(x, y, z + SideLength);
				}
				return ushort.MaxValue;
			}
			if (z >= SideLength)
			{
				if (NeighborChunks[4] != null)
				{
					return NeighborChunks[4].GetVoxel(x, y, z - SideLength);
				}
				return ushort.MaxValue;
			}
			return VoxelData[z * SquaredSideLength + y * SideLength + x];
		}

		public VoxelInfo GetVoxelInfo(int x, int y, int z)
		{
			if (x < 0)
			{
				if (NeighborChunks[3] != null)
				{
					return NeighborChunks[3].GetVoxelInfo(x + SideLength, y, z);
				}
				return null;
			}
			if (x >= SideLength)
			{
				if (NeighborChunks[2] != null)
				{
					return NeighborChunks[2].GetVoxelInfo(x - SideLength, y, z);
				}
				return null;
			}
			if (y < 0)
			{
				if (NeighborChunks[1] != null)
				{
					return NeighborChunks[1].GetVoxelInfo(x, y + SideLength, z);
				}
				return null;
			}
			if (y >= SideLength)
			{
				if (NeighborChunks[0] != null)
				{
					return NeighborChunks[0].GetVoxelInfo(x, y - SideLength, z);
				}
				return null;
			}
			if (z < 0)
			{
				if (NeighborChunks[5] != null)
				{
					return NeighborChunks[5].GetVoxelInfo(x, y, z + SideLength);
				}
				return null;
			}
			if (z >= SideLength)
			{
				if (NeighborChunks[4] != null)
				{
					return NeighborChunks[4].GetVoxelInfo(x, y, z - SideLength);
				}
				return null;
			}
			return new VoxelInfo(new Index(x, y, z), this);
		}

		public VoxelInfo GetVoxelInfo(Index index)
		{
			return GetVoxelInfo(index.x, index.y, index.z);
		}

		public Voxel GetVoxelType(Index index)
		{
			return GetVoxelInfo(index).GetVoxelType();
		}

		public ushort GetVoxel(Index index)
		{
			return GetVoxel(index.x, index.y, index.z);
		}

		public byte GetVoxelRotation(Index index)
		{
			return GetVoxelRotation(index.x, index.y, index.z);
		}

		public void GetNeighbors()
		{
			int x = ChunkIndex.x;
			int y = ChunkIndex.y;
			int z = ChunkIndex.z;
			if (NeighborChunks[0] == null)
			{
				ChunkManager.ChunkDatas.TryGetValue(new Index(x, y + 1, z), out ChunkData value);
				NeighborChunks[0] = value;
				if (value != null)
				{
					value.NeighborChunks[1] = this;
					value.Changed = true;
				}
			}
			if (NeighborChunks[1] == null)
			{
				ChunkData value = null;
				ChunkManager.ChunkDatas.TryGetValue(new Index(x, y - 1, z), out value);
				NeighborChunks[1] = value;
				if (value != null)
				{
					value.NeighborChunks[0] = this;
					value.Changed = true;
				}
			}
			if (NeighborChunks[2] == null)
			{
				ChunkData value = null;
				ChunkManager.ChunkDatas.TryGetValue(new Index(x + 1, y, z), out value);
				NeighborChunks[2] = value;
				if (value != null)
				{
					value.NeighborChunks[3] = this;
					value.Changed = true;
				}
			}
			if (NeighborChunks[3] == null)
			{
				ChunkData value = null;
				ChunkManager.ChunkDatas.TryGetValue(new Index(x - 1, y, z), out value);
				NeighborChunks[3] = value;
				if (value != null)
				{
					value.NeighborChunks[2] = this;
					value.Changed = true;
				}
			}
			if (NeighborChunks[4] == null)
			{
				ChunkData value = null;
				ChunkManager.ChunkDatas.TryGetValue(new Index(x, y, z + 1), out value);
				NeighborChunks[4] = value;
				if (value != null)
				{
					value.NeighborChunks[5] = this;
					value.Changed = true;
				}
			}
			if (NeighborChunks[5] == null)
			{
				ChunkData value = null;
				ChunkManager.ChunkDatas.TryGetValue(new Index(x, y, z - 1), out value);
				NeighborChunks[5] = value;
				if (value != null)
				{
					value.NeighborChunks[4] = this;
					value.Changed = true;
				}
			}
		}

		public Index GetAdjacentIndex(Index index, Direction direction)
		{
			return GetAdjacentIndex(index.x, index.y, index.z, direction);
		}

		public Index GetAdjacentIndex(int x, int y, int z, Direction direction)
		{
			switch (direction)
			{
			case Direction.down:
				return new Index(x, y - 1, z);
			case Direction.up:
				return new Index(x, y + 1, z);
			case Direction.left:
				return new Index(x - 1, y, z);
			case Direction.right:
				return new Index(x + 1, y, z);
			case Direction.back:
				return new Index(x, y, z - 1);
			case Direction.forward:
				return new Index(x, y, z + 1);
			case Direction.forwardleft:
				return new Index(x - 1, y, z + 1);
			case Direction.forwardright:
				return new Index(x + 1, y, z + 1);
			case Direction.backleft:
				return new Index(x - 1, y, z - 1);
			case Direction.backright:
				return new Index(x + 1, y, z - 1);
			default:
				UnityEngine.Debug.LogError("Chunk.GetAdjacentIndex failed! Returning default index.");
				return new Index(x, y, z);
			}
		}

		public void UpdateNeighborsIfNeeded(int x, int y, int z)
		{
			if (x == 0 && NeighborChunks[3] != null)
			{
				NeighborChunks[3].Changed = true;
			}
			else if (x == SideLength - 1 && NeighborChunks[2] != null)
			{
				NeighborChunks[2].Changed = true;
			}
			if (y == 0 && NeighborChunks[1] != null)
			{
				NeighborChunks[1].Changed = true;
			}
			else if (y == SideLength - 1 && NeighborChunks[0] != null)
			{
				NeighborChunks[0].Changed = true;
			}
			if (z == 0 && NeighborChunks[5] != null)
			{
				NeighborChunks[5].Changed = true;
			}
			else if (z == SideLength - 1 && NeighborChunks[4] != null)
			{
				NeighborChunks[4].Changed = true;
			}
		}

		public Index PositionToVoxelIndex(Vector3 position)
		{
			Vector3 vector = word2Object.MultiplyPoint3x4(position);
			Index result = new Index(0, 0, 0);
			result.x = Mathf.Clamp(Mathf.RoundToInt(vector.x), 0, SideLength - 1);
			result.y = Mathf.Clamp(Mathf.RoundToInt(vector.y), 0, SideLength - 1);
			result.z = Mathf.Clamp(Mathf.RoundToInt(vector.z), 0, SideLength - 1);
			return result;
		}

		public Vector3 VoxelIndexToPosition(Index index)
		{
			Vector3 point = index.ToVector3();
			return object2World.MultiplyPoint3x4(point);
		}

		public Vector3 VoxelIndexToPosition(int x, int y, int z)
		{
			return object2World.MultiplyPoint3x4(new Vector3(x, y, z));
		}

		public Index PositionToVoxelIndex(Vector3 position, Vector3 normal, bool returnAdjacent)
		{
			position = (returnAdjacent ? (position + normal * 0.95f) : (position - normal * 0.05f));
			Vector3 vector = word2Object.MultiplyPoint3x4(position);
			Index result = new Index(0, 0, 0);
			result.x = Mathf.RoundToInt(vector.x);
			result.y = Mathf.RoundToInt(vector.y);
			result.z = Mathf.RoundToInt(vector.z);
			return result;
		}

		public bool OpenDoor(int index)
		{
			if (doorsStates.ContainsKey(index))
			{
				Dictionary<int, doorState> dictionary = doorsStates;
				doorState doorState = doorsStates[index];
				bool doorOpen = !doorState.doorOpen;
				doorState doorState2 = doorsStates[index];
				dictionary[index] = new doorState(doorOpen, doorState2.doorRotation);
				doorState doorState3 = doorsStates[index];
				return !doorState3.doorOpen;
			}
			doorsStates.Add(index, new doorState(doorOpen: true, GetVoxelRotationSimple(index)));
			return false;
		}

		public bool GetDoorState(int index)
		{
			doorState doorState = doorsStates[index];
			return doorState.doorOpen;
		}

		public byte GetDoorBasicRoot(int index)
		{
			doorState doorState = doorsStates[index];
			return doorState.doorRotation;
		}

		public void ChangeDoorBasicRoot(int index, byte rot)
		{
			doorsStates[index] = new doorState(doorOpen: false, rot);
		}

		public void AddDoor(int index, byte rot)
		{
			doorsStates.AddIfNotExists(index, new doorState(doorOpen: false, rot));
		}

		public void CloseLogicDoor(int index)
		{
			if (doorsStates.ContainsKey(index))
			{
				doorState doorState = doorsStates[index];
				SetRotation(index, doorState.doorRotation);
			}
		}

		public bool Generate()
		{
			if (AllNeighborsHaveData() && VoxelsDone && !Empty)
			{
				Engine.MeshCreator.Generate(this, !ignoreVoxelEventsOnNextRebuild);
				ignoreVoxelEventsOnNextRebuild = false;
				ChunkManager.operationsCounter++;
				return true;
			}
			return Empty;
		}

		public bool AllNeighborsHaveData()
		{
			ChunkData[] neighborChunks = NeighborChunks;
			foreach (ChunkData chunkData in neighborChunks)
			{
				if (chunkData == null || !chunkData.VoxelsDone)
				{
					return false;
				}
			}
			return true;
		}

		private void GenerateMatrices()
		{
			Vector3 vector = IndexToPosition(ChunkIndex);
			object2World = Matrix4x4.identity;
			object2World.m03 = vector.x;
			object2World.m13 = vector.y;
			object2World.m23 = vector.z;
			word2Object = object2World.inverse;
		}

		public void SaveData()
		{
			if (!Engine.SaveVoxelData || Application.platform == RuntimePlatform.WebGLPlayer)
			{
				UnityEngine.Debug.LogWarning("Uniblocks: Saving is disabled. You can enable it in the Engine Settings.");
				return;
			}
			ChunkDataFiles.SaveData(this);
			isDirty = false;
		}

		public void ChunkComputerWaterCollider()
		{
			minMaxXZ = new int[4]
			{
				SideLength,
				-1,
				SideLength,
				-1
			};
		}

		public static int ChunkGetOppositeNeighbourIndex(int index)
		{
			int num = 1;
			if (index % 2 != 0)
			{
				num = -1;
			}
			return index + num;
		}

		public static Vector3 IndexToPosition(Index index)
		{
			return new Vector3(index.x * SideLength, index.y * SideLength, index.z * SideLength);
		}

		public static Index PositionToIndex(Vector3 position)
		{
			return new Index(position);
		}

		public static Index WorldPositionToIndex(Vector3 position)
		{
			return Engine.PositionToIndex(position);
		}

		public static int IndexToRawIndex(Index index)
		{
			return index.z * SquaredSideLength + index.y * SideLength + index.x;
		}
	}
}
