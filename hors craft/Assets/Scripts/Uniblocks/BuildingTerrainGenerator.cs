// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BuildingTerrainGenerator
using NoiseTest;
using UnityEngine;

namespace Uniblocks
{
	[RequireComponent(typeof(CityManager))]
	public class BuildingTerrainGenerator : CommonTerrainGenerator
	{
		protected override void GenerateVoxelData(int seed, ChunkData chunk)
		{
			int y = chunk.ChunkIndex.y;
			chunkSideLength = ChunkData.SideLength;
			chunkSideLengthTemp = ChunkData.SideLength * 10;
			chunkDoubleLength = chunkSideLengthTemp * 2;
			chunkDoubleSquareLength = chunkDoubleLength * chunkDoubleLength;
			bool flag = IsHorizontalStreetChunk(seed, chunk);
			bool flag2 = IsVerticalStreetChunk(seed, chunk);
			int num = chunkSideLength / 2 - streets.streetLaneWidth;
			int num2 = chunkSideLength / 2 + streets.streetLaneWidth;
			OpenSimplexNoise openSimplexNoise = new OpenSimplexNoise(seed);
			City nearestCity = CityManager.GetNearestCity(chunk);
			nearestCity.index.y = GetCityCenterHeight(chunk, openSimplexNoise, nearestCity);
			if (nearestCity.IsChunkInInterpolationRange(chunk.ChunkIndex) && nearestCity.IsChunkInCityRange(chunk.ChunkIndex))
			{
				SpawnCityChunks(chunk, nearestCity);
			}
			for (int i = 0; i < chunkSideLength; i++)
			{
				for (int j = 0; j < chunkSideLength; j++)
				{
					Vector3 vector = chunk.VoxelIndexToPosition(j, 0, i) + new Vector3(seed, 0f, seed);
					float num3 = Mathf.PerlinNoise(vector.x * perlinTerrainScaleX * scaleWorldInX, vector.z * perlinTerrainScaleZ * scaleWorldInZ) * 60.9f;
					float num4 = (float)openSimplexNoise.Evaluate(vector.x * 0.055f * scaleWorldInX, vector.z * 0.055f * scaleWorldInZ);
					float num5 = num4 * 9.1f;
					int num6 = GetTerrainHeightRamp().RemapHeight(Mathf.FloorToInt(num3 + num5 * 2f));
					if (num6 > 30)
					{
						num6 -= (int)(openSimplexNoise.Evaluate(vector.x * 0.1f, vector.z * 0.1f) * 5.0);
					}
					int num7 = Random.Range(3, 6);
					bool flag3 = (float)num6 > 57f;
					bool flag4 = (flag2 && j >= num && j <= num2) || (flag && i >= num && i <= num2);
					if (nearestCity.IsChunkInInterpolationRange(chunk.ChunkIndex))
					{
						float num8 = (float)(nearestCity.index.x % CityManager.scale) - ((float)(chunk.ChunkIndex.x % CityManager.scale) - 0.5f + (float)j / (float)chunkSideLength);
						float num9 = (float)(nearestCity.index.z % CityManager.scale) - ((float)(chunk.ChunkIndex.z % CityManager.scale) - 0.5f + (float)i / (float)chunkSideLength);
						num8 *= num8;
						num9 *= num9;
						float num10 = Mathf.Sqrt(num8 + num9);
						int num11 = nearestCity.index.y * chunkSideLength - num6;
						if (nearestCity.IsChunkInCityRange(chunk.ChunkIndex))
						{
							num6 += num11;
						}
						else
						{
							float num12 = num10 - (float)nearestCity.radius;
							float num13 = nearestCity.interpolationRadius - nearestCity.radius;
							float value = 1f - num12 / num13;
							value = Mathf.Clamp01(value);
							int num14 = (int)((float)num11 * value);
							num6 += num14;
						}
					}
					for (int k = 0; k < chunkSideLength; k++)
					{
						bool flag5 = false;
						bool flag6 = false;
						int num15 = k + chunkSideLength * y;
						if (fastWater && num15 < waterHeight)
						{
							if (num15 < waterHeight - 1 && Random.value < animals.waterPlaceProbability)
							{
								AddAnimalsSpawner(chunk, j, k, i);
							}
							else
							{
								chunk.SetVoxelSimple(j, k, i, otherIDs.waterBlockID);
							}
						}
						if (!flag4 && num15 > num6)
						{
							continue;
						}
						if (num15 < num6 && (!flag4 || num15 < streets.streetLevel || num15 > streets.streetLevel + streets.tunnelHeight))
						{
							if (num15 == num6 - 1)
							{
								if (flag3)
								{
									chunk.SetVoxelSimple(j, k, i, otherIDs.snowBlockID);
								}
								else if (num15 > waterHeight)
								{
									chunk.SetVoxelSimple(j, k, i, otherIDs.grassBlockID);
									flag5 = true;
								}
								else
								{
									chunk.SetVoxelSimple(j, k, i, otherIDs.beachBlockID);
								}
							}
							else if (num15 > num6 - num7)
							{
								chunk.SetVoxelSimple(j, k, i, otherIDs.dirtBlockID);
							}
							else
							{
								chunk.SetVoxelSimple(j, k, i, otherIDs.stoneBlockID);
							}
						}
						if (!flag4 && flag5)
						{
							int num16 = Random.Range(trees.treeHeightMin, trees.treeHeightMax);
							int num17;
							int treeLeavesWidth;
							if (num16 <= 5)
							{
								num17 = 2;
								treeLeavesWidth = 1;
							}
							else if (num16 <= 8)
							{
								num17 = Random.Range(2, num16 - 4);
								treeLeavesWidth = Random.Range(num16 - 4, num16 - 3);
							}
							else
							{
								num17 = Random.Range(num16 - 6, num16 - 5);
								treeLeavesWidth = num16 - num17 - 1;
							}
							if (TreeCanFit(j, k, i, num16, treeLeavesWidth))
							{
								float num18 = 1f - Mathf.Abs((float)k - trees.desiredTreeY) / trees.treeToDesiredMaxDiff;
								float num19 = trees.placeProbability * num18;
								if (Random.value < num19)
								{
									TreesConfig.Tree tree = PickTreeConfig();
									AddTree(chunk, j, k + 1, i, tree, num16, treeLeavesWidth, num17);
									flag6 = true;
								}
							}
						}
						if (!flag4 && flag5 && !flag6 && FlowerCanFit(k))
						{
							if (Random.value < animals.placeProbability)
							{
								AddAnimalsSpawner(chunk, j, k + 1, i);
							}
							else if (Random.value < specialFlowers.placeProbability)
							{
								AddSpecialFlower(chunk, j, k + 1, i);
							}
							else if (Random.value < flowers.placeFlowerProbability)
							{
								AddFlower(chunk, j, k + 1, i);
							}
							else if (Random.value < chests.placeProbability)
							{
								AddChest(chunk, j, k + 1, i);
							}
							else if (num5 > 5f && Random.value < flowers.placeTallGrassProbability)
							{
								chunk.SetVoxelSimple(j, k + 1, i, flowers.tallGrassBlockID);
							}
						}
						if (flag4 && flag2)
						{
							if (num15 == streets.streetLevel)
							{
								if (j == chunkSideLength / 2 && i % 2 == 1)
								{
									chunk.SetVoxelSimple(j, k, i, otherIDs.roadLineBlockID);
								}
								else if ((j == num || j == num2) && !flag)
								{
									chunk.SetVoxelSimple(j, k, i, otherIDs.roadLineBlockID);
								}
								else
								{
									chunk.SetVoxelSimple(j, k, i, otherIDs.roadBlockID);
								}
							}
							else if (num15 > streets.streetLevel && num15 <= streets.streetLevel + streets.tunnelHeight)
							{
								bool flag7 = Mathf.Abs(chunkSideLength / 2 - j) == (streets.streetLaneWidth + 1) / 2;
								if (num15 == streets.streetLevel + 1 && flag7 && Random.value < streets.carProbability)
								{
									chunk.SetVoxel(j, k, i, animals.animalSpawnerBlockID, updateMesh: false, 1);
								}
								else
								{
									chunk.SetVoxelSimple(j, k, i, 0);
								}
							}
							else if (num15 < streets.streetLevel && (j == num || j == num2) && i % 4 == 3 && num15 > num6)
							{
								chunk.SetVoxelSimple(j, k, i, streets.bridgeVoxelId);
							}
						}
						if (!flag4 || !flag)
						{
							continue;
						}
						if (num15 == streets.streetLevel)
						{
							if (i == chunkSideLength / 2 && j % 2 == 1)
							{
								chunk.SetVoxelSimple(j, k, i, otherIDs.roadLineBlockID);
								chunk.SetRotation(j, k, i, 1);
							}
							else if ((i == num || i == num2) && !flag2)
							{
								chunk.SetVoxelSimple(j, k, i, otherIDs.roadLineBlockID);
								chunk.SetRotation(j, k, i, 1);
							}
							else
							{
								chunk.SetVoxelSimple(j, k, i, otherIDs.roadBlockID);
							}
						}
						else if (num15 > streets.streetLevel && num15 <= streets.streetLevel + streets.tunnelHeight)
						{
							bool flag8 = Mathf.Abs(chunkSideLength / 2 - i) == (streets.streetLaneWidth + 1) / 2;
							if (num15 == streets.streetLevel + 1 && flag8 && Random.value < streets.carProbability)
							{
								chunk.SetVoxel(j, k, i, animals.animalSpawnerBlockID, updateMesh: false, 2);
							}
							else
							{
								chunk.SetVoxelSimple(j, k, i, 0);
							}
						}
						else if (num15 < streets.streetLevel && (i == num || i == num2) && j % 4 == 3 && num15 > num6)
						{
							chunk.SetVoxelSimple(j, k, i, streets.bridgeVoxelId);
						}
					}
				}
			}
		}

		private int GetCityCenterHeight(ChunkData chunk, OpenSimplexNoise openSimplexNoise, City currentCity)
		{
			int num = (int)Mathf.Floor((float)chunk.ChunkIndex.x / (float)CityManager.scale);
			int num2 = (int)Mathf.Floor((float)chunk.ChunkIndex.z / (float)CityManager.scale);
			currentCity.index.x += num * CityManager.scale;
			currentCity.index.z += num2 * CityManager.scale;
			Vector3 position = currentCity.index.ToVector3();
			Vector3 vector = base.transform.TransformPoint(position);
			float num3 = Mathf.PerlinNoise(vector.x * perlinTerrainScaleX * scaleWorldInX, vector.z * perlinTerrainScaleZ * scaleWorldInZ) * 60.9f;
			float num4 = (float)openSimplexNoise.Evaluate(vector.x * 0.055f * scaleWorldInX, vector.z * 0.055f * scaleWorldInZ);
			float num5 = num4 * 9.1f;
			int num6 = GetTerrainHeightRamp().RemapHeight(Mathf.FloorToInt(num3 + num5 * 2f));
			if (num6 > 30)
			{
				num6 -= (int)(openSimplexNoise.Evaluate(vector.x * 0.1f, vector.z * 0.1f) * 5.0);
			}
			int num7 = num6 / chunkSideLength;
			if (num7 == 0)
			{
				num7 = 1;
			}
			return num7;
		}

		private void SpawnCityChunks(ChunkData chunk, City city)
		{
			if (city.index.y == chunk.ChunkIndex.y)
			{
				Index relativeIndex = CityManager.GetRelativeIndex(chunk.ChunkIndex);
				int x = relativeIndex.x;
				int z = relativeIndex.z;
				Index relativeIndex2 = CityManager.GetRelativeIndex(city.index);
				int x2 = relativeIndex2.x;
				int z2 = relativeIndex2.z;
				int num = x2 - x;
				int num2 = z2 - z;
				foreach (CityChunk cityChunk in city.cityChunks)
				{
					if (cityChunk.x == num && cityChunk.z == num2)
					{
						foreach (BlueprintWithPosition item in cityChunk.blueprintsWithPosition)
						{
							PlaceBlueprint(chunk, city, item);
						}
						break;
					}
				}
			}
		}

		private void PlaceBlueprint(ChunkData chunk, City city, BlueprintWithPosition blueprintWithPosition)
		{
			BlueprintData blueprintData = BlueprintDataFiles.ReadDataFromResources(blueprintWithPosition.blueprintResourceName);
			if (chunk.ChunkIndex.x % CityManager.scale < city.index.x % CityManager.scale)
			{
				PlaceBlueprintBack(chunk, blueprintWithPosition.x, blueprintWithPosition.z, blueprintData);
			}
			else if (city.index.x % CityManager.scale == chunk.ChunkIndex.x % CityManager.scale && city.index.z % CityManager.scale != chunk.ChunkIndex.z % CityManager.scale)
			{
				if (chunk.ChunkIndex.z % CityManager.scale < city.index.z % CityManager.scale)
				{
					PlaceBlueprintLeft(chunk, blueprintWithPosition.x, blueprintWithPosition.z, blueprintData);
				}
				else
				{
					PlaceBlueprintRight(chunk, blueprintWithPosition.x, blueprintWithPosition.z, blueprintData);
				}
			}
			else
			{
				PlaceBlueprintFront(chunk, blueprintWithPosition.x, blueprintWithPosition.z, blueprintData);
			}
		}

		private static void PlaceBlueprintFront(ChunkData chunk, int xStart, int zStart, BlueprintData blueprintData)
		{
			for (int i = 0; i < blueprintData.width; i++)
			{
				for (int j = 0; j < blueprintData.depth; j++)
				{
					for (int k = 0; k < blueprintData.height; k++)
					{
						chunk.SetVoxel(xStart + i, k, zStart + j, blueprintData.voxelsArray[i][k][j], updateMesh: true, blueprintData.rotationsArray[i][k][j]);
					}
				}
			}
		}

		private static void PlaceBlueprintRight(ChunkData chunk, int xStart, int zStart, BlueprintData blueprintData)
		{
			for (int i = 0; i < blueprintData.width; i++)
			{
				for (int j = 0; j < blueprintData.depth; j++)
				{
					for (int k = 0; k < blueprintData.height; k++)
					{
						chunk.SetVoxel(xStart + j, k, zStart + i, blueprintData.voxelsArray[i][k][j], updateMesh: true, (byte)((blueprintData.rotationsArray[blueprintData.width - 1 - i][k][blueprintData.depth - 1 - j] + 3) % 4));
					}
				}
			}
		}

		private static void PlaceBlueprintLeft(ChunkData chunk, int xStart, int zStart, BlueprintData blueprintData)
		{
			for (int i = 0; i < blueprintData.width; i++)
			{
				for (int j = 0; j < blueprintData.depth; j++)
				{
					for (int k = 0; k < blueprintData.height; k++)
					{
						chunk.SetVoxel(xStart + j, k, zStart + i, blueprintData.voxelsArray[blueprintData.width - 1 - i][k][blueprintData.depth - 1 - j], updateMesh: true, (byte)((blueprintData.rotationsArray[blueprintData.width - 1 - i][k][blueprintData.depth - 1 - j] + 3) % 4));
					}
				}
			}
		}

		private static void PlaceBlueprintBack(ChunkData chunk, int xStart, int zStart, BlueprintData blueprintData)
		{
			for (int i = 0; i < blueprintData.width; i++)
			{
				for (int j = 0; j < blueprintData.depth; j++)
				{
					for (int k = 0; k < blueprintData.height; k++)
					{
						chunk.SetVoxel(xStart + i, k, zStart + j, blueprintData.voxelsArray[blueprintData.width - 1 - i][k][blueprintData.depth - 1 - j], updateMesh: true, (byte)((blueprintData.rotationsArray[blueprintData.width - 1 - i][k][blueprintData.depth - 1 - j] + 2) % 4));
					}
				}
			}
		}
	}
}
