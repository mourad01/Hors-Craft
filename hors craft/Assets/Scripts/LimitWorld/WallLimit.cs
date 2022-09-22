// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.WallLimit
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/Limit/WallLimit")]
	public class WallLimit : Limit
	{
		[SerializeField]
		private ushort _wallBlock;

		[SerializeField]
		private bool useThisBlock = true;

		[SerializeField]
		private bool invisibleWall = true;

		[SerializeField]
		private bool infinityWall = true;

		[SerializeField]
		private bool waterHeightMin = true;

		[SerializeField]
		private bool worldSizeWall;

		[SerializeField]
		private int wallHeight;

		private readonly List<ChunkData> wallChunks = new List<ChunkData>();

		private readonly List<GameObject> invisibleWalls = new List<GameObject>();

		public override EventTypeLW eventType
		{
			[CompilerGenerated]
			get
			{
				return EventTypeLW.ChunkSpawned;
			}
		}

		protected override bool doChunkCorrection
		{
			[CompilerGenerated]
			get
			{
				return true;
			}
		}

		protected override Action initialAction => CreateInvisibleWalls;

		public ushort wallBlock
		{
			get
			{
				if (useThisBlock)
				{
					return _wallBlock;
				}
				if (Engine.EngineInstance.notDistroyableBlocks != null && Engine.EngineInstance.notDistroyableBlocks.Count > 0)
				{
					return Engine.EngineInstance.notDistroyableBlocks[0];
				}
				return Engine.usefulIDs.stoneBlockID;
			}
		}

		public bool HasToPlaceWall(int columnHeight, int y, ChunkData chunkData)
		{
			int num = y + chunkData.ChunkIndex.y * ChunkData.SideLength;
			return infinityWall || ((!worldSizeWall) ? (((!waterHeightMin) ? columnHeight : Mathf.Max(Engine.TerrainGenerator.waterHeight, columnHeight)) + wallHeight >= num) : (wallHeight >= num));
		}

		public void AddWall(ChunkData data)
		{
			wallChunks.Add(data);
		}

		private void CreateInvisibleWalls()
		{
			if (invisibleWall)
			{
				Vector3[,] worldBoundsPositions = ((LimitRect3D)limitShape).GetWorldBoundsPositions(300f, doChunkCorrection);
				for (int i = 0; i < 4; i++)
				{
					GameObject gameObject = new GameObject("WallColl");
					gameObject.transform.position = Vector3.zero;
					BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
					boxCollider.center = worldBoundsPositions[i, 0];
					boxCollider.size = worldBoundsPositions[i, 1];
					invisibleWalls.Add(gameObject);
				}
			}
		}

		public override bool ProcessEvent(DataLW data)
		{
			ChunkData chunkData = (ChunkData)data.target;
			if (!limitShape.IsBoundaryChunk(chunkData.ChunkIndex, doChunkCorrection))
			{
				return false;
			}
			AddWall(chunkData);
			IndexPair[] boundaryAxis = GetBoundaryAxis(chunkData.ChunkIndex);
			ushort[] voxelData = chunkData.VoxelData;
			IndexPair[] array = boundaryAxis;
			for (int i = 0; i < array.Length; i++)
			{
				IndexPair indexPair = array[i];
				for (int j = 0; j < ChunkData.SideLength; j++)
				{
					for (int k = 0; k < ChunkData.SideLength; k++)
					{
						for (int l = 0; l < ChunkData.SideLength; l++)
						{
							int num = indexPair.startPoint.FlatAdd(indexPair.axis.MulFlat(j, k, l));
							if (HasToPlaceWall(Engine.TerrainGenerator.GetWorldHeight(new VoxelInfo(j, k, l, chunkData)), k, chunkData))
							{
								voxelData[num] = wallBlock;
							}
						}
					}
				}
			}
			return true;
		}

		public override void ResetLimit()
		{
			base.ResetLimit();
			for (int i = 0; i < wallChunks.Count; i++)
			{
				wallChunks[i].GetVoxelData();
				wallChunks[i].Generate();
			}
			wallChunks.Clear();
			for (int j = 0; j < invisibleWalls.Count; j++)
			{
				UnityEngine.Object.Destroy(invisibleWalls[j]);
			}
			invisibleWalls.Clear();
		}

		public override void ReSetup()
		{
			base.ReSetup();
			List<ChunkData> list = new List<ChunkData>();
			foreach (ChunkData value in ChunkManager.ChunkDatas.Values)
			{
				if (limitShape.IsBoundaryChunk(value.ChunkIndex, doChunkCorrection))
				{
					ProcessEvent(new DataLW
					{
						target = value
					});
					list.Add(value);
				}
			}
			foreach (ChunkData item in list)
			{
				item.Generate();
			}
		}

		public IndexPair[] GetBoundaryAxis(Index position)
		{
			return ((LimitRect3D)limitShape).GetBoundaryAxis(position, doChunkCorrection).ToArray();
		}
	}
}
