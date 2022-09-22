// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.City
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uniblocks
{
	[Serializable]
	public class City
	{
		public Index index;

		public int radius;

		public int radiusSquared;

		public int interpolationRadius;

		public int interpolationRadiusSquared;

		public List<CityChunk> cityChunks;

		public City(City other)
		{
			index = new Index(other.index.ToVector3());
			radius = other.radius;
			radiusSquared = other.radiusSquared;
			interpolationRadius = other.interpolationRadius;
			interpolationRadiusSquared = other.interpolationRadiusSquared;
			cityChunks = other.cityChunks;
		}

		public City(Index index, int radius, int interpolationRadius)
		{
			this.index = index;
			this.radius = radius;
			radiusSquared = radius * radius;
			this.interpolationRadius = interpolationRadius;
			interpolationRadiusSquared = interpolationRadius * interpolationRadius;
			cityChunks = new List<CityChunk>();
			for (int i = -radius; i <= radius; i++)
			{
				for (int j = -radius; j <= radius; j++)
				{
					if (i * i + j * j < radiusSquared)
					{
						CityChunk item = new CityChunk(i, j, FillChunkWithBlueprints(i, j));
						cityChunks.Add(item);
					}
				}
			}
		}

		public bool IsChunkInInterpolationRange(Index chunkIndex)
		{
			float num = Mathf.Pow(Mathf.Abs(index.x - chunkIndex.x), 2f) + Mathf.Pow(Mathf.Abs(index.z - chunkIndex.z), 2f);
			if (num < (float)interpolationRadiusSquared)
			{
				return true;
			}
			return false;
		}

		public bool IsChunkInCityRange(Index chunkIndex)
		{
			float num = Mathf.Pow(Mathf.Abs(index.x - chunkIndex.x), 2f) + Mathf.Pow(Mathf.Abs(index.z - chunkIndex.z), 2f);
			if (num < (float)radiusSquared)
			{
				return true;
			}
			return false;
		}

		private List<BlueprintWithPosition> FillChunkWithBlueprints(int x, int z)
		{
			List<BlueprintWithPosition> list = new List<BlueprintWithPosition>();
			bool[,] chunkGrid = new bool[CityManager.chunkSideLength, CityManager.chunkSideLength];
			Pass[] passes = CityManager.passes;
			foreach (Pass pass in passes)
			{
				AddBlueprintsFromPass(list, chunkGrid, pass);
			}
			return list;
		}

		private void AddBlueprintsFromPass(List<BlueprintWithPosition> blueprintsWithPosition, bool[,] chunkGrid, Pass pass)
		{
			if (!(UnityEngine.Random.Range(0f, 1f) <= pass.chance))
			{
				return;
			}
			int xStart = 0;
			int zStart = 0;
			bool[,] array = new bool[CityManager.chunkSideLength, CityManager.chunkSideLength];
			for (int i = 0; i < CityManager.chunkSideLength; i++)
			{
				for (int j = 0; j < CityManager.chunkSideLength; j++)
				{
					array[i, j] = chunkGrid[i, j];
				}
			}
			while (!IsGridFull(array, ref xStart, ref zStart))
			{
				AddNewBlueprintInNextFreeSpaceInGrid(blueprintsWithPosition, chunkGrid, array, pass, xStart, zStart);
			}
		}

		private void AddNewBlueprintInNextFreeSpaceInGrid(List<BlueprintWithPosition> blueprintsWithPosition, bool[,] chunkGrid, bool[,] passChunkGrid, Pass pass, int xStart, int zStart)
		{
			int sizeOfNextFreeSpaceinGrid = GetSizeOfNextFreeSpaceinGrid(passChunkGrid, xStart, zStart);
			BlueprintCraftableObject blueprintCraftableObject = null;
			BlueprintWithChance[] blueprintsWithChance = pass.blueprintsWithChance;
			foreach (BlueprintWithChance blueprintWithChance in blueprintsWithChance)
			{
				if (UnityEngine.Random.Range(0f, 1f) <= blueprintWithChance.chanceToSpawn && blueprintWithChance.size <= sizeOfNextFreeSpaceinGrid)
				{
					blueprintCraftableObject = blueprintWithChance.blueprintCraftableObject;
					break;
				}
			}
			if (blueprintCraftableObject == null)
			{
				passChunkGrid[xStart, zStart] = true;
				return;
			}
			BlueprintData blueprintData = BlueprintDataFiles.ReadDataFromResources(blueprintCraftableObject.blueprintResourceName);
			BlueprintWithPosition item = new BlueprintWithPosition(xStart, zStart, blueprintCraftableObject.blueprintResourceName);
			blueprintsWithPosition.Add(item);
			FillGrids(chunkGrid, xStart, zStart, passChunkGrid, blueprintData);
		}

		private void FillGrids(bool[,] chunkGrid, int xStart, int zStart, bool[,] passChunkGrid, BlueprintData blueprintData)
		{
			for (int i = 0; i < blueprintData.width; i++)
			{
				for (int j = 0; j < blueprintData.depth; j++)
				{
					chunkGrid[xStart + i, zStart + j] = true;
					passChunkGrid[xStart + i, zStart + j] = true;
				}
			}
		}

		private int GetSizeOfNextFreeSpaceinGrid(bool[,] chunkGrid, int xBlue, int zBlue)
		{
			int result = 1;
			bool flag = false;
			for (int i = 0; i <= CityManager.chunkSideLength; i++)
			{
				if (xBlue + i == CityManager.chunkSideLength || zBlue + i == CityManager.chunkSideLength)
				{
					result = i;
					break;
				}
				for (int j = 0; j <= i; j++)
				{
					if (chunkGrid[xBlue + i, zBlue + j])
					{
						result = i;
						flag = true;
					}
					if (chunkGrid[xBlue + j, zBlue + i])
					{
						result = i;
						flag = true;
					}
				}
				if (flag)
				{
					break;
				}
			}
			return result;
		}

		private bool IsGridFull(bool[,] chunkGrid, ref int xStart, ref int zStart)
		{
			xStart = 0;
			zStart = 0;
			if (!chunkGrid[0, 0])
			{
				xStart = 0;
				zStart = 0;
				return false;
			}
			for (int i = 0; i < CityManager.chunkSideLength; i++)
			{
				for (int j = 0; j < CityManager.chunkSideLength; j++)
				{
					if (!chunkGrid[i, j])
					{
						xStart = i;
						zStart = j;
						return false;
					}
				}
			}
			return true;
		}
	}
}
