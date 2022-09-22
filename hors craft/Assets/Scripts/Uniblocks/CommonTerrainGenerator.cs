// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.CommonTerrainGenerator
using Common.Utils;
using LimitWorld;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uniblocks
{
	public class CommonTerrainGenerator : TerrainGenerator
	{
		public class PrecalculatedTerrainRampData
		{
			private readonly int[] data;

			public PrecalculatedTerrainRampData(AnimationCurve curve)
			{
				int num = 80;
				data = new int[num];
				for (int i = 0; i < num; i++)
				{
					data[i] = Mathf.RoundToInt(curve.Evaluate(i));
				}
			}

			public int RemapHeight(int height)
			{
				height = Mathf.Clamp(height, 0, data.Length - 1);
				return data[height];
			}
		}

		[Serializable]
		public class FlowersConfig
		{
			public ushort tallGrassBlockID = 159;

			public float placeTallGrassProbability = 0.3f;

			public float placeFlowerProbability = 0.1f;

			public List<ushort> flowersIDs = new List<ushort>
			{
				150,
				151,
				152,
				153,
				154,
				155,
				156,
				157,
				158
			};
		}

		[Serializable]
		public class AlgaeConfig
		{
			public bool spawnAlgaeEnable;

			public int maxAlgaeSpawnHeight = 10;

			public float placeProbability = 0.1f;

			public List<ushort> algaeIDs = new List<ushort>();
		}

		[Serializable]
		public class SpecialFlowersConfig
		{
			public float placeProbability;

			public List<ushort> flowersIDs = new List<ushort>
			{
				140,
				141,
				153
			};
		}

		public enum TreeType
		{
			OAK,
			JUNGLE,
			SPRUCE,
			PALM,
			CACTUS,
			COCONUT_PALM,
			SAVANNAH_TREE,
			PALM_BLOCK
		}

		[Serializable]
		public class TreesConfig
		{
			[Serializable]
			public class Tree
			{
				public TreeType type;

				public int probabilityWeight = 1;

				public ushort trunkBlockID;

				public ushort leavesBlockID;

				public ushort fruitsBlockID;
			}

			[Range(0f, 1f)]
			public float placeProbability = 0.05f;

			public int treeHeightMin = 4;

			public int treeHeightMax = 13;

			public float desiredTreeY = 13f;

			public float treeToDesiredMaxDiff = 20f;

			public List<Tree> treeConfigs = new List<Tree>
			{
				new Tree
				{
					type = TreeType.OAK,
					probabilityWeight = 8,
					trunkBlockID = 6,
					leavesBlockID = 9
				},
				new Tree
				{
					type = TreeType.JUNGLE,
					probabilityWeight = 1,
					trunkBlockID = 69,
					leavesBlockID = 67
				},
				new Tree
				{
					type = TreeType.SPRUCE,
					probabilityWeight = 1,
					trunkBlockID = 72,
					leavesBlockID = 68
				}
			};
		}

		[Serializable]
		public class LowpolyTrees
		{
			public bool enable;

			[Range(0f, 1f)]
			public float placeProbability = 0.05f;

			public ushort[] treesIds;
		}

		[Serializable]
		public class AnimalsSpawnerConfig
		{
			public float placeProbability = 0.00055f;

			public ushort animalSpawnerBlockID = 262;

			public float waterPlaceProbability;
		}

		[Serializable]
		public class ChestSpawnConfig
		{
			public float placeProbability = 0.0005f;

			public ushort chestBlockId = 1277;

			public bool spawnUnderwater;
		}

		[Serializable]
		public class OtherIDsConfig
		{
			public ushort waterBlockID = 12;

			public ushort dirtBlockID = 1;

			public ushort stoneBlockID = 5;

			public ushort grassBlockID = 2;

			public ushort beachBlockID = 101;

			public ushort snowBlockID = 41;

			public ushort roadBlockID = 771;

			public ushort roadLineBlockID = 772;

			public ushort blueprintID = 1118;

			public ushort iceBlockID = 1555;
		}

		[Serializable]
		public class StreetsConfig
		{
			public float carProbability = 0.008f;

			public ushort bridgeVoxelId = 74;

			public int streetLevel = 14;

			public int streetLaneWidth = 5;

			public float cityRange = 100f;

			public float maxStreetProbability = 0.1f;

			public float minStreetProbability = 0.02f;

			public int tunnelHeight = 5;

			public bool useTunnelVoxel;

			public ushort tunnelVoxelId = 5;
		}

		[Serializable]
		public class IceConfig
		{
			[Header("Works only with fast water for now")]
			public bool enabled;

			[Range(0f, 1f)]
			[Header("Size of air holes. 0 - ice without air holes, 1 - no ice. Default value 0.1")]
			public float airHoleSize = 0.1f;

			[Header("Default value 0.1")]
			public float perlinInputScaler = 0.1f;
		}

		[Range(1f, 16f)]
		public int perlTerrainInterval = 1;

		public AnimationCurve terrainHeightRamp = AnimationCurve.EaseInOut(0f, 0f, 70f, 70f);

		public bool generateStreets;

		public float scaleWorldInX = 1f;

		public float scaleWorldInZ = 1f;

		protected static PrecalculatedTerrainRampData terrainHeightRampInstance;

		public FlowersConfig flowers = new FlowersConfig();

		public AlgaeConfig algae = new AlgaeConfig();

		public SpecialFlowersConfig specialFlowers = new SpecialFlowersConfig();

		public TreesConfig trees = new TreesConfig();

		public LowpolyTrees lowpolyTrees = new LowpolyTrees();

		public AnimalsSpawnerConfig animals = new AnimalsSpawnerConfig();

		public ChestSpawnConfig chests = new ChestSpawnConfig();

		public OtherIDsConfig otherIDs = new OtherIDsConfig();

		public StreetsConfig streets = new StreetsConfig();

		public IceConfig ice = new IceConfig();

		protected const int MOUNTAIN_DISTORTION_HEIGHT = 30;

		protected const int MAX_TERRAIN_HEIGHT = 70;

		protected const float SNOW_HEIGHT = 13f;

		public float perlinTerrainScaleX = 0.01f;

		public float perlinTerrainScaleZ = 0.01f;

		public bool fastWater = true;

		protected int chunkSideLength;

		protected int chunkSideLengthTemp;

		protected int chunkDoubleLength;

		protected int chunkDoubleSquareLength;

		protected Vector3[] neighbors = new Vector3[4]
		{
			new Vector3(-1f, 0f, 0f),
			new Vector3(1f, 0f, 0f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, 1f)
		};

		protected HashSet<int> visited = new HashSet<int>();

		protected int waterBlocksCount;

		protected int calculatedSumOfTreesProbabilityWeights = -1;

		protected int sumOfTreesProbabilityWeights
		{
			get
			{
				if (calculatedSumOfTreesProbabilityWeights < 0)
				{
					calculatedSumOfTreesProbabilityWeights = 0;
					foreach (TreesConfig.Tree treeConfig in trees.treeConfigs)
					{
						calculatedSumOfTreesProbabilityWeights += treeConfig.probabilityWeight;
					}
				}
				return calculatedSumOfTreesProbabilityWeights;
			}
		}

		protected PrecalculatedTerrainRampData GetTerrainHeightRamp()
		{
			if (terrainHeightRampInstance == null)
			{
				terrainHeightRampInstance = new PrecalculatedTerrainRampData(terrainHeightRamp);
			}
			return terrainHeightRampInstance;
		}

		public static void ClearCurrentRamp()
		{
			terrainHeightRampInstance = null;
		}

		public override int GetWorldHeight(VoxelInfo info)
		{
			int worldSeed = Engine.WorldSeed;
			Vector3 a = ChunkData.IndexToPosition(info.chunk.ChunkIndex);
			Vector3 vector = a + new Vector3(info.index.x + worldSeed, 0f, info.index.z + worldSeed);
			float num = Mathf.PerlinNoise(vector.x * perlinTerrainScaleX * scaleWorldInX, vector.z * perlinTerrainScaleZ * scaleWorldInZ) * 60.9f;
			float num2 = Mathf.PerlinNoise(vector.x * 0.055f * scaleWorldInX, vector.z * 0.055f * scaleWorldInZ);
			float num3 = num2 * 9.1f;
			int num4 = (int)(Mathf.PerlinNoise(vector.x * 0.1f, vector.z * 0.1f) * 5f);
			int num5 = GetTerrainHeightRamp().RemapHeight((int)Math.Floor(num + num3 * 2f));
			if (num5 > 30)
			{
				num5 -= num4;
			}
			return num5;
		}

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
			Mathf.Clamp(perlTerrainInterval, 1, 16);
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			int num6 = 0;
			int[] array = new int[chunkSideLength];
			int num7 = 0;
			ushort[] voxelData = chunk.VoxelData;
			int num8 = 0;
			int squaredSideLength = ChunkData.SquaredSideLength;
			int sideLength = ChunkData.SideLength;
			Vector3 a = ChunkData.IndexToPosition(chunk.ChunkIndex);
			for (int i = 0; i < chunkSideLength; i++)
			{
				for (int j = 0; j < chunkSideLength; j++)
				{
					num8 = i * squaredSideLength + j - sideLength;
					Vector3 vector = a + new Vector3(j + seed, 0f, i + seed);
					if (j % perlTerrainInterval == 0 && i % perlTerrainInterval == 0)
					{
						num4 = Mathf.PerlinNoise(vector.x * perlinTerrainScaleX * scaleWorldInX, vector.z * perlinTerrainScaleZ * scaleWorldInZ) * 60.9f;
						float num9 = Mathf.PerlinNoise(vector.x * 0.055f * scaleWorldInX, vector.z * 0.055f * scaleWorldInZ);
						num3 = num9 * 9.1f;
						num5 = (int)(Mathf.PerlinNoise(vector.x * 0.1f, vector.z * 0.1f) * 5f);
						num6 = (array[j] = GetTerrainHeightRamp().RemapHeight((int)Math.Floor(num4 + num3 * 2f)));
					}
					else
					{
						num7 = (int)((float)j / (float)perlTerrainInterval) * perlTerrainInterval;
						num6 = array[num7];
					}
					if (num6 > 30)
					{
						num6 -= num5;
					}
					int num10 = 4;
					bool flag3 = (float)num6 > 57f;
					bool flag4 = (flag2 && j >= num && j <= num2) || (flag && i >= num && i <= num2);
					for (int k = 0; k < chunkSideLength; k++)
					{
						num8 += sideLength;
						bool flag5 = false;
						bool flag6 = false;
						int num11 = k + chunkSideLength * y;
						if (num11 < waterHeight)
						{
							if (num11 == waterHeight - 1)
							{
								if (ice.enabled)
								{
									float num12 = Mathf.PerlinNoise(vector.x * ice.perlinInputScaler, vector.z * ice.perlinInputScaler);
									if (num12 < ice.airHoleSize)
									{
										voxelData[num8] = 0;
									}
									else
									{
										voxelData[num8] = otherIDs.iceBlockID;
									}
								}
								else
								{
									voxelData[num8] = otherIDs.waterBlockID;
								}
							}
							else if (num11 < waterHeight - 1 && UnityEngine.Random.value < animals.waterPlaceProbability)
							{
								AddAnimalsSpawner(voxelData, num8);
							}
							else
							{
								voxelData[num8] = otherIDs.waterBlockID;
							}
						}
						if (!flag4 && num11 > num6)
						{
							continue;
						}
						if (num11 == num6 && FlowerCanFit(k - 1))
						{
							if (num11 <= algae.maxAlgaeSpawnHeight && algae.spawnAlgaeEnable && UnityEngine.Random.value < algae.placeProbability)
							{
								AddAlga(voxelData, num8);
							}
							else if (chests.spawnUnderwater && UnityEngine.Random.value < chests.placeProbability)
							{
								AddChest(chunk, j, k, i);
							}
						}
						if (num11 < num6 && (!flag4 || num11 < streets.streetLevel || num11 > streets.streetLevel + streets.tunnelHeight))
						{
							if (num11 == num6 - 1)
							{
								if (flag3)
								{
									voxelData[num8] = otherIDs.snowBlockID;
								}
								else if (num11 > waterHeight)
								{
									voxelData[num8] = otherIDs.grassBlockID;
									flag5 = true;
								}
								else
								{
									voxelData[num8] = otherIDs.beachBlockID;
								}
							}
							else if (num11 > num6 - num10)
							{
								voxelData[num8] = otherIDs.dirtBlockID;
							}
							else
							{
								voxelData[num8] = otherIDs.stoneBlockID;
							}
						}
						if (!flag4 && flag5)
						{
							float num13 = 1f - Mathf.Abs((float)k - trees.desiredTreeY) / trees.treeToDesiredMaxDiff;
							float num14 = trees.placeProbability * num13;
							if (UnityEngine.Random.value < num14)
							{
								int num15 = UnityEngine.Random.Range(trees.treeHeightMin, trees.treeHeightMax);
								int num16;
								int treeLeavesWidth;
								if (num15 <= 5)
								{
									num16 = 2;
									treeLeavesWidth = 1;
								}
								else if (num15 <= 8)
								{
									num16 = UnityEngine.Random.Range(2, num15 - 4);
									treeLeavesWidth = UnityEngine.Random.Range(num15 - 4, num15 - 3);
								}
								else
								{
									num16 = UnityEngine.Random.Range(num15 - 6, num15 - 5);
									treeLeavesWidth = num15 - num16 - 1;
								}
								if (TreeCanFit(j, k, i, num15, treeLeavesWidth))
								{
									TreesConfig.Tree tree = PickTreeConfig();
									AddTree(chunk, j, k + 1, i, tree, num15, treeLeavesWidth, num16);
									flag6 = true;
								}
							}
						}
						if (lowpolyTrees.enable && !flag4 && !flag6 && flag5 && k < chunkSideLength - 1 && UnityEngine.Random.value < lowpolyTrees.placeProbability)
						{
							chunk.SetVoxelSimple(j, k + 1, i, lowpolyTrees.treesIds[UnityEngine.Random.Range(0, lowpolyTrees.treesIds.Length)]);
							flag6 = true;
						}
						if (!flag4 && flag5 && !flag6 && FlowerCanFit(k))
						{
							if (UnityEngine.Random.value < animals.placeProbability)
							{
								AddAnimalsSpawner(voxelData, num8 + sideLength);
							}
							else if (UnityEngine.Random.value < specialFlowers.placeProbability)
							{
								AddSpecialFlower(voxelData, num8 + sideLength);
							}
							else if (UnityEngine.Random.value < flowers.placeFlowerProbability)
							{
								AddFlower(voxelData, num8 + sideLength);
							}
							else if (UnityEngine.Random.value < chests.placeProbability)
							{
								AddChest(chunk, j, k + 1, i);
							}
							else if (num3 > 5f && UnityEngine.Random.value < flowers.placeTallGrassProbability)
							{
								voxelData[num8 + sideLength] = flowers.tallGrassBlockID;
							}
						}
						if (flag4 && flag2)
						{
							int num17 = Mathf.Abs(chunkSideLength / 2 - j);
							if (num11 == streets.streetLevel)
							{
								if (j == chunkSideLength / 2 && i % 2 == 1)
								{
									voxelData[num8] = otherIDs.roadLineBlockID;
								}
								else if ((j == num || j == num2) && !flag)
								{
									voxelData[num8] = otherIDs.roadLineBlockID;
								}
								else
								{
									voxelData[num8] = otherIDs.roadBlockID;
								}
							}
							else if (num11 > streets.streetLevel && num11 <= streets.streetLevel + streets.tunnelHeight)
							{
								bool flag7 = num17 == (streets.streetLaneWidth + 1) / 2;
								if (num11 == streets.streetLevel + 1 && flag7 && UnityEngine.Random.value < streets.carProbability)
								{
									chunk.SetVoxel(j, k, i, animals.animalSpawnerBlockID, updateMesh: false, 1);
								}
								else
								{
									voxelData[num8] = 0;
									if (num11 == streets.streetLevel + streets.tunnelHeight - 1)
									{
										if (num17 > 3)
										{
											SetGroundVoxelInTunnels(chunk, num11, num6, j, k, i, num10, flag3);
										}
									}
									else if (num11 == streets.streetLevel + streets.tunnelHeight && num17 > 1)
									{
										SetGroundVoxelInTunnels(chunk, num11, num6, j, k, i, num10, flag3);
									}
								}
							}
							else if (num11 < streets.streetLevel && (j == num || j == num2) && i % 4 == 3 && num11 > num6)
							{
								voxelData[num8] = streets.bridgeVoxelId;
							}
						}
						if (!flag4 || !flag)
						{
							continue;
						}
						int num18 = Mathf.Abs(chunkSideLength / 2 - i);
						if (num11 == streets.streetLevel)
						{
							if (i == chunkSideLength / 2 && j % 2 == 1)
							{
								voxelData[num8] = otherIDs.roadLineBlockID;
								chunk.SetRotation(j, k, i, 1);
							}
							else if ((i == num || i == num2) && !flag2)
							{
								voxelData[num8] = otherIDs.roadLineBlockID;
								chunk.SetRotation(j, k, i, 1);
							}
							else
							{
								voxelData[num8] = otherIDs.roadBlockID;
							}
						}
						else if (num11 > streets.streetLevel && num11 <= streets.streetLevel + streets.tunnelHeight)
						{
							bool flag8 = num18 == (streets.streetLaneWidth + 1) / 2;
							if (num11 == streets.streetLevel + 1 && flag8 && UnityEngine.Random.value < streets.carProbability)
							{
								chunk.SetVoxel(j, k, i, animals.animalSpawnerBlockID, updateMesh: false, 2);
								continue;
							}
							voxelData[num8] = 0;
							if (num11 == streets.streetLevel + streets.tunnelHeight - 1)
							{
								if (num18 > 3)
								{
									SetGroundVoxelInTunnels(chunk, num11, num6, j, k, i, num10, flag3);
								}
							}
							else if (num11 == streets.streetLevel + streets.tunnelHeight && num18 > 1)
							{
								SetGroundVoxelInTunnels(chunk, num11, num6, j, k, i, num10, flag3);
							}
						}
						else if (num11 < streets.streetLevel && (i == num || i == num2) && j % 4 == 3 && num11 > num6)
						{
							voxelData[num8] = streets.bridgeVoxelId;
						}
					}
				}
			}
			if (!fastWater)
			{
				visited.Clear();
				for (int l = 0; l < chunkSideLength; l++)
				{
					for (int m = 0; m < chunkSideLength; m++)
					{
						int num19 = waterHeight - 1 - chunkSideLength * y;
						if (num19 < 0 || num19 >= chunkSideLength)
						{
							continue;
						}
						int item = EncodeXYZToInt(m, num19, l);
						if (chunk.GetVoxel(m, num19, l) != 0 || visited.Contains(item))
						{
							continue;
						}
						Queue<int> queue = new Queue<int>();
						HashSet<int> hashSet = new HashSet<int>();
						visited.Add(item);
						queue.Enqueue(item);
						hashSet.Add(item);
						int[] array2 = new int[4]
						{
							chunkSideLength,
							-1,
							chunkSideLength,
							-1
						};
						while (queue.Count > 0)
						{
							int index = queue.Dequeue();
							int[] array3 = DecodeIntArrayFromInt(index);
							int num20 = array3[0];
							int num21 = array3[1];
							int num22 = array3[2];
							if (num20 >= 0 && num20 <= chunkSideLength && num21 >= 0 && num21 <= chunkSideLength && num22 >= 0 && num22 <= chunkSideLength && chunk.GetVoxel(num20, num21, num22) == 0)
							{
								if (num20 < array2[0])
								{
									array2[0] = num20;
								}
								if (num20 > array2[1])
								{
									array2[1] = num20;
								}
								if (num22 < array2[2])
								{
									array2[2] = num22;
								}
								if (num22 > array2[3])
								{
									array2[3] = num22;
								}
							}
							Vector3[] array4 = neighbors;
							for (int n = 0; n < array4.Length; n++)
							{
								Vector3 vector2 = array4[n];
								int num23 = num20 + (int)vector2.x;
								int num24 = num21 + (int)vector2.y;
								int num25 = num22 + (int)vector2.z;
								int item2 = EncodeXYZToInt(num23, num24, num25);
								ushort voxel = chunk.GetVoxel(num23, num24, num25);
								if ((voxel == 0 || voxel == otherIDs.waterBlockID) && !visited.Contains(item2))
								{
									visited.Add(item2);
									queue.Enqueue(item2);
									hashSet.Add(item2);
								}
							}
						}
						chunk.minMaxXZ = array2;
						if (hashSet.Count >= 180)
						{
							foreach (int item3 in hashSet)
							{
								int[] array5 = DecodeIntArrayFromInt(item3);
								int num26 = array5[0];
								int num27 = array5[1];
								int num28 = array5[2];
								while (chunk.GetVoxel(num26, num27, num28) == 0)
								{
									chunk.SetVoxel(num26, num27, num28, otherIDs.waterBlockID, updateMesh: true, 0);
									waterBlocksCount++;
									chunk.GetVoxelInfo(new Index(num26, num27, num28)).chunk.hasWater = true;
									num27--;
									if ((float)(num27 + chunkSideLength * y) <= -16f)
									{
										return;
									}
								}
							}
							chunk.hasWater = true;
						}
					}
				}
			}
			MonoBehaviourSingleton<LimitedWorld>.get.RaiseEvent(new DataLW
			{
				target = chunk
			}, EventTypeLW.ChunkSpawned);
		}

		private void SetGroundVoxelInTunnels(ChunkData chunk, int absoluteY, int columnHeight, int x, int y, int z, int dirtHeight, bool snowyTop)
		{
			if (absoluteY >= columnHeight)
			{
				return;
			}
			if (streets.useTunnelVoxel)
			{
				chunk.SetVoxelSimple(x, y, z, streets.tunnelVoxelId);
			}
			else if (absoluteY == columnHeight - 1)
			{
				if (snowyTop)
				{
					chunk.SetVoxelSimple(x, y, z, otherIDs.snowBlockID);
				}
				else if (absoluteY > waterHeight)
				{
					chunk.SetVoxelSimple(x, y, z, otherIDs.grassBlockID);
				}
				else
				{
					chunk.SetVoxelSimple(x, y, z, otherIDs.beachBlockID);
				}
			}
			else if (absoluteY > columnHeight - dirtHeight)
			{
				chunk.SetVoxelSimple(x, y, z, otherIDs.dirtBlockID);
			}
			else
			{
				chunk.SetVoxelSimple(x, y, z, otherIDs.stoneBlockID);
			}
		}

		protected bool IsHorizontalStreetChunk(int seed, ChunkData chunk)
		{
			if (!generateStreets)
			{
				return false;
			}
			DeterministicRNG deterministicRNG = new DeterministicRNG(seed + chunk.ChunkIndex.z);
			float t = Mathf.Max(0f, (streets.cityRange - (float)Mathf.Abs(chunk.ChunkIndex.z)) / streets.cityRange);
			float num = Mathf.Lerp(streets.minStreetProbability, streets.maxStreetProbability, t);
			if (deterministicRNG.NextFloat() < num)
			{
				return true;
			}
			return false;
		}

		protected bool IsVerticalStreetChunk(int seed, ChunkData chunk)
		{
			if (!generateStreets)
			{
				return false;
			}
			DeterministicRNG deterministicRNG = new DeterministicRNG(seed + chunk.ChunkIndex.x);
			float t = Mathf.Max(0f, (streets.cityRange - (float)Mathf.Abs(chunk.ChunkIndex.x)) / streets.cityRange);
			float num = Mathf.Lerp(streets.minStreetProbability, streets.maxStreetProbability, t);
			if (deterministicRNG.NextFloat() < num)
			{
				return true;
			}
			return false;
		}

		protected bool FlowerCanFit(int y)
		{
			return y + 1 < ChunkData.SideLength;
		}

		protected bool TreeCanFit(int x, int y, int z, int treeHeight, int treeLeavesWidth)
		{
			if (x > treeLeavesWidth && x < ChunkData.SideLength - treeLeavesWidth && z > treeLeavesWidth && z < ChunkData.SideLength - treeLeavesWidth && y + treeHeight + 1 < ChunkData.SideLength)
			{
				return true;
			}
			return false;
		}

		protected TreesConfig.Tree PickTreeConfig()
		{
			float num = UnityEngine.Random.value * (float)sumOfTreesProbabilityWeights;
			int num2 = 0;
			foreach (TreesConfig.Tree treeConfig in trees.treeConfigs)
			{
				num2 += treeConfig.probabilityWeight;
				if ((float)num2 > num)
				{
					return treeConfig;
				}
			}
			return trees.treeConfigs[0];
		}

		protected void AddTree(ChunkData chunk, int x, int y, int z, TreesConfig.Tree tree, int treeHeight, int treeLeavesWidth, int treeLeavesStartHeight)
		{
			if (tree.type == TreeType.JUNGLE || tree.type == TreeType.OAK || tree.type == TreeType.SPRUCE)
			{
				AddDefaultTree(chunk, x, y, z, tree, treeHeight, treeLeavesWidth, treeLeavesStartHeight);
			}
			else if (tree.type == TreeType.PALM)
			{
				AddPalm(chunk, x, y, z, tree, treeHeight, treeLeavesWidth, treeLeavesStartHeight, withCoconuts: false);
			}
			else if (tree.type == TreeType.COCONUT_PALM)
			{
				AddPalm(chunk, x, y, z, tree, treeHeight, treeLeavesWidth, treeLeavesStartHeight, withCoconuts: true);
			}
			else if (tree.type == TreeType.CACTUS)
			{
				AddCactus(chunk, x, y, z, tree, treeHeight, treeLeavesWidth, treeLeavesStartHeight);
			}
			else if (tree.type == TreeType.SAVANNAH_TREE)
			{
				AddSavannahTree(chunk, x, y, z, tree, treeHeight, treeLeavesWidth);
			}
			else if (tree.type == TreeType.PALM_BLOCK)
			{
				AddPalmBlock(chunk, x, y, z, tree, treeHeight, treeLeavesWidth);
			}
		}

		protected void AddPalmBlock(ChunkData chunk, int x, int y, int z, TreesConfig.Tree tree, int treeHeight, int treeLeavesWidth)
		{
			BlueprintData blueprintData = BlueprintDataFiles.ReadDataFromResources("Palm");
			for (int i = 0; i < blueprintData.width; i++)
			{
				for (int j = 0; j < blueprintData.depth; j++)
				{
					for (int k = 0; k < blueprintData.height; k++)
					{
						chunk.SetVoxel(i + x, y + k, j + z, blueprintData.voxelsArray[i][k][j], updateMesh: true, blueprintData.rotationsArray[i][k][j]);
					}
				}
			}
		}

		protected void AddDefaultTree(ChunkData chunk, int x, int y, int z, TreesConfig.Tree tree, int treeHeight, int treeLeavesWidth, int treeLeavesStartHeight)
		{
			ushort trunkBlockID = tree.trunkBlockID;
			ushort leavesBlockID = tree.leavesBlockID;
			for (int i = 0; i < treeHeight; i++)
			{
				chunk.SetVoxelSimple(x, y + i, z, trunkBlockID);
			}
			for (int j = treeLeavesStartHeight; j < treeHeight; j++)
			{
				for (int k = -treeLeavesWidth; k <= treeLeavesWidth; k++)
				{
					for (int l = -treeLeavesWidth; l <= treeLeavesWidth; l++)
					{
						if (treeHeight > 5)
						{
							if ((k == -treeLeavesWidth || k == treeLeavesWidth || l == -treeLeavesWidth || l == treeLeavesWidth) && treeLeavesWidth > 2)
							{
								if (((k > -treeLeavesWidth && l > -treeLeavesWidth) || (k < treeLeavesWidth && l < treeLeavesWidth)) && ((k > -treeLeavesWidth && l < treeLeavesWidth) || (k < treeLeavesWidth && l > -treeLeavesWidth)) && UnityEngine.Random.value >= 0.1f)
								{
									SetVoxelIfEmpty(chunk, x + k, y + j, z + l, leavesBlockID);
								}
							}
							else if (k != 0 || l != 0)
							{
								SetVoxelIfEmpty(chunk, x + k, y + j, z + l, leavesBlockID);
							}
						}
						else if (k != 0 || l != 0)
						{
							SetVoxelIfEmpty(chunk, x + k, y + j, z + l, leavesBlockID);
						}
					}
				}
				if (treeHeight > 5 && treeHeight <= 8 && j > treeLeavesStartHeight + 1)
				{
					treeLeavesWidth--;
				}
				if (treeHeight > 8 && j > treeLeavesStartHeight)
				{
					treeLeavesWidth--;
				}
			}
			chunk.SetVoxel(x, y + treeHeight, z, leavesBlockID, updateMesh: false, 0);
		}

		protected void AddCactus(ChunkData chunk, int x, int y, int z, TreesConfig.Tree tree, int treeHeight, int treeLeavesWidth, int treeLeavesStartHeight)
		{
			ushort trunkBlockID = tree.trunkBlockID;
			ushort leavesBlockID = tree.leavesBlockID;
			for (int i = 0; i < treeHeight; i++)
			{
				chunk.SetVoxelSimple(x, y + i, z, trunkBlockID);
			}
			if (treeLeavesWidth >= 2)
			{
				int num = UnityEngine.Random.Range(0, 2);
				int num2 = 1 - num;
				int num3 = UnityEngine.Random.Range(0, 2);
				int num4 = 1 - num3;
				SetVoxelIfEmpty(chunk, x + num, treeLeavesStartHeight + num3, z + num2, leavesBlockID);
				SetVoxelIfEmpty(chunk, x - num, treeLeavesStartHeight + num4, z - num2, leavesBlockID);
				num *= 2;
				num2 *= 2;
				SetVoxelIfEmpty(chunk, x + num, treeLeavesStartHeight + num3, z + num2, leavesBlockID);
				SetVoxelIfEmpty(chunk, x - num, treeLeavesStartHeight + num4, z - num2, leavesBlockID);
				SetVoxelIfEmpty(chunk, x + num, treeLeavesStartHeight + 1 + num3, z + num2, leavesBlockID);
				SetVoxelIfEmpty(chunk, x - num, treeLeavesStartHeight + 1 + num4, z - num2, leavesBlockID);
			}
		}

		protected void AddPalm(ChunkData chunk, int x, int y, int z, TreesConfig.Tree tree, int treeHeight, int treeLeavesWidth, int treeLeavesStartHeight, bool withCoconuts)
		{
			ushort leavesBlockID = tree.leavesBlockID;
			ushort fruitsBlockID = tree.fruitsBlockID;
			ushort trunkBlockID = tree.trunkBlockID;
			for (int i = 0; i < treeHeight; i++)
			{
				chunk.SetVoxelSimple(x, y + i, z, trunkBlockID);
			}
			int num = treeHeight + y - 1;
			int num2 = 0;
			int j = 1;
			int num3 = 0;
			for (; j <= treeLeavesWidth; j++)
			{
				SetVoxelIfEmpty(chunk, x + j, num - num3, z, leavesBlockID);
				num2 = num3;
				num3 += ((j % 2 == 0) ? 1 : 0);
			}
			if (withCoconuts)
			{
				SetVoxelIfEmpty(chunk, x + treeLeavesWidth - 1, num - num2 - 1, z, fruitsBlockID);
			}
			int k = 1;
			int num4 = 0;
			for (; k <= treeLeavesWidth; k++)
			{
				SetVoxelIfEmpty(chunk, x - k, num - num4, z, leavesBlockID);
				num2 = num4;
				num4 += ((k % 2 == 0) ? 1 : 0);
			}
			if (withCoconuts)
			{
				SetVoxelIfEmpty(chunk, x - treeLeavesWidth + 1, num - num2 - 1, z, fruitsBlockID);
			}
			int l = 1;
			int num5 = 0;
			for (; l <= treeLeavesWidth; l++)
			{
				SetVoxelIfEmpty(chunk, x, num - num5, z + l, leavesBlockID);
				num2 = num5;
				num5 += ((l % 2 == 0) ? 1 : 0);
			}
			if (withCoconuts)
			{
				SetVoxelIfEmpty(chunk, x, num - num2 - 1, z + treeLeavesWidth - 1, fruitsBlockID);
			}
			int m = 1;
			int num6 = 0;
			for (; m <= treeLeavesWidth; m++)
			{
				SetVoxelIfEmpty(chunk, x, num - num6, z - m, leavesBlockID);
				num2 = num6;
				num6 += ((m % 2 == 0) ? 1 : 0);
			}
			if (withCoconuts)
			{
				SetVoxelIfEmpty(chunk, x, num - num2 - 1, z - treeLeavesWidth + 1, fruitsBlockID);
			}
			int n = 1;
			int num7 = 0;
			for (; n < treeLeavesWidth; n++)
			{
				SetVoxelIfEmpty(chunk, x + n, num - num7, z - n, leavesBlockID);
				num2 = num7;
				num7 += ((n % 2 == 0) ? 1 : 0);
			}
			if (withCoconuts)
			{
				SetVoxelIfEmpty(chunk, x + treeLeavesWidth - 1, num - num2 - 1, z - treeLeavesWidth + 1, fruitsBlockID);
			}
			int num8 = 1;
			int num9 = 0;
			for (; num8 < treeLeavesWidth; num8++)
			{
				SetVoxelIfEmpty(chunk, x + num8, num - num9, z + num8, leavesBlockID);
				num2 = num9;
				num9 += ((num8 % 2 == 0) ? 1 : 0);
			}
			if (withCoconuts)
			{
				SetVoxelIfEmpty(chunk, x + treeLeavesWidth - 1, num - num2 - 1, z + treeLeavesWidth - 1, fruitsBlockID);
			}
			int num10 = 1;
			int num11 = 0;
			for (; num10 < treeLeavesWidth; num10++)
			{
				SetVoxelIfEmpty(chunk, x - num10, num - num11, z + num10, leavesBlockID);
				num2 = num11;
				num11 += ((num10 % 2 == 0) ? 1 : 0);
			}
			if (withCoconuts)
			{
				SetVoxelIfEmpty(chunk, x - treeLeavesWidth + 1, num - num2 - 1, z + treeLeavesWidth - 1, fruitsBlockID);
			}
			int num12 = 1;
			int num13 = 0;
			for (; num12 < treeLeavesWidth; num12++)
			{
				SetVoxelIfEmpty(chunk, x - num12, num - num13, z - num12, leavesBlockID);
				num2 = num13;
				num13 += ((num12 % 2 == 0) ? 1 : 0);
			}
			if (withCoconuts)
			{
				SetVoxelIfEmpty(chunk, x - treeLeavesWidth + 1, num - num2 - 1, z - treeLeavesWidth + 1, fruitsBlockID);
			}
			chunk.SetVoxel(x, y + treeHeight, z, leavesBlockID, updateMesh: false, 0);
		}

		protected void AddSavannahTree(ChunkData chunk, int x, int y, int z, TreesConfig.Tree tree, int treeHeight, int treeLeavesWidth)
		{
			ushort trunkBlockID = tree.trunkBlockID;
			ushort leavesBlockID = tree.leavesBlockID;
			int num = UnityEngine.Random.Range(2, 4);
			int num2 = treeHeight - num;
			int num3 = UnityEngine.Random.Range(0, 4);
			Vector3 vector = Vector3.zero;
			switch (num3)
			{
			case 0:
				vector = Vector3.left;
				break;
			case 1:
				vector = Vector3.right;
				break;
			case 2:
				vector = Vector3.forward;
				break;
			case 3:
				vector = Vector3.back;
				break;
			}
			for (int i = 0; i < num2; i++)
			{
				int num4 = Mathf.RoundToInt(vector.x * 3f * ((float)i / (float)treeHeight));
				int num5 = Mathf.RoundToInt(vector.z * 3f * ((float)i / (float)treeHeight));
				chunk.SetVoxelSimple(x + num4, y + i, z + num5, trunkBlockID);
			}
			int num6 = treeHeight / 3;
			int num7 = Mathf.RoundToInt(vector.x * 3f * ((float)num6 / (float)treeHeight));
			int num8 = Mathf.RoundToInt(vector.z * 3f * ((float)num6 / (float)treeHeight));
			for (int j = num6; j < num2; j++)
			{
				int num9 = num7 - Mathf.RoundToInt(vector.x * 3f * ((float)(j - num6) / (float)(treeHeight - num6)));
				int num10 = num8 - Mathf.RoundToInt(vector.z * 3f * ((float)(j - num6) / (float)(treeHeight - num6)));
				chunk.SetVoxelSimple(x + num9, y + j, z + num10, trunkBlockID);
			}
			for (int k = num2; k < treeHeight; k++)
			{
				int num11 = (int)(Mathf.Lerp(0.5f, 1f, (float)(treeHeight - k) / (float)num) * (float)treeLeavesWidth);
				for (int l = -treeLeavesWidth; l <= treeLeavesWidth; l++)
				{
					for (int m = -treeLeavesWidth; m <= treeLeavesWidth; m++)
					{
						int x2 = x + l + num7;
						int z2 = z + m + num8;
						if (new Vector2(l, m).sqrMagnitude < (float)(num11 * num11))
						{
							chunk.SetVoxelSimple(x2, y + k, z2, leavesBlockID);
						}
					}
				}
			}
		}

		protected void AddFlower(ChunkData chunk, int x, int y, int z)
		{
			ushort data = flowers.flowersIDs[UnityEngine.Random.Range(0, flowers.flowersIDs.Count)];
			chunk.SetVoxelSimple(x, y, z, data);
		}

		protected void AddFlower(ushort[] voxelData, int rawIndex)
		{
			ushort num = voxelData[rawIndex] = flowers.flowersIDs[UnityEngine.Random.Range(0, flowers.flowersIDs.Count)];
		}

		protected void AddAlga(ChunkData chunk, int x, int y, int z)
		{
			ushort data = algae.algaeIDs[UnityEngine.Random.Range(0, algae.algaeIDs.Count)];
			chunk.SetVoxelSimple(x, y, z, data);
		}

		protected void AddAlga(ushort[] voxelData, int rawIndex)
		{
			ushort num = voxelData[rawIndex] = algae.algaeIDs[UnityEngine.Random.Range(0, algae.algaeIDs.Count)];
		}

		protected void AddSpecialFlower(ChunkData chunk, int x, int y, int z)
		{
			ushort data = specialFlowers.flowersIDs[UnityEngine.Random.Range(0, specialFlowers.flowersIDs.Count)];
			chunk.SetVoxelSimple(x, y, z, data);
		}

		protected void AddSpecialFlower(ushort[] voxelData, int rawIndex)
		{
			ushort num = voxelData[rawIndex] = specialFlowers.flowersIDs[UnityEngine.Random.Range(0, specialFlowers.flowersIDs.Count)];
		}

		protected void AddAnimalsSpawner(ChunkData chunk, int x, int y, int z)
		{
			chunk.SetVoxelSimple(x, y, z, animals.animalSpawnerBlockID);
		}

		protected void AddAnimalsSpawner(ushort[] voxelData, int rawIndex)
		{
			voxelData[rawIndex] = animals.animalSpawnerBlockID;
		}

		protected void AddChest(ChunkData chunk, int x, int y, int z)
		{
			Index index = new Index(x, y, z);
			string globalIndex = VoxelInfo.GetGlobalIndex(chunk, index);
			if (ChestInteractiveObject.CanPutChestThere(globalIndex))
			{
				chunk.SetVoxelSimple(x, y, z, chests.chestBlockId);
				chunk.SetRotation(x, y, z, (byte)UnityEngine.Random.Range(0, 3));
			}
		}

		protected void SetVoxelIfEmpty(ChunkData chunk, int x, int y, int z, ushort data)
		{
			if (chunk.GetVoxel(x, y, z) == 0)
			{
				chunk.SetVoxelSimple(x, y, z, data);
			}
		}

		public int EncodeXYZToInt(int xIn, int yIn, int zIn)
		{
			int num = xIn + chunkSideLengthTemp;
			int num2 = yIn + chunkSideLengthTemp;
			int num3 = zIn + chunkSideLengthTemp;
			return num + num2 * chunkDoubleLength + num3 * chunkDoubleSquareLength;
		}

		public int[] DecodeIntArrayFromInt(int index)
		{
			int num = Mathf.FloorToInt(index / chunkDoubleSquareLength);
			int num2 = Mathf.FloorToInt((index - num * chunkDoubleSquareLength) / chunkDoubleLength);
			int num3 = index - num2 * chunkDoubleLength - num * chunkDoubleSquareLength;
			return new int[3]
			{
				num3 - chunkSideLengthTemp,
				num2 - chunkSideLengthTemp,
				num - chunkSideLengthTemp
			};
		}
	}
}
