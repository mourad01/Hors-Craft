// DecompilerFi decompiler from Assembly-CSharp.dll class: LakeNavigator
using Common.Managers;
using Common.Utils;
using Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class LakeNavigator : MonoBehaviour
{
	private int chunkSideLength;

	private int chunkSideLengthTemp;

	private int chunkDoubleLength;

	private int chunkDoubleSquareLength;

	private HashSet<int> chunkIndexes = new HashSet<int>();

	private List<HashSet<int>> chunkLakes = new List<HashSet<int>>();

	private List<Vector3> globalLakes = new List<Vector3>();

	private List<int> lakesBatch = new List<int>();

	private int currentLakeBatch;

	private int chunkIndex;

	private Index globalLakeChunkIndex;

	private NavigationManager navigationManager;

	private int chunkLakesCount;

	private const string PREF_KEY = "lakes.batch";

	private bool isFishingEnabled;

	private Vector3[] neighbors = new Vector3[4]
	{
		new Vector3(-1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(0f, 0f, -1f),
		new Vector3(0f, 0f, 1f)
	};

	private HashSet<int> visited = new HashSet<int>();

	private int zOut;

	private int yOut;

	private int xOut;

	private int[] xyzOut = new int[3];

	private Vector3 meanPosition;

	private List<Vector3> chunkPositions = new List<Vector3>();

	private Vector3 chunkPos = Vector3.zero;

	private Index currentChunkIndex;

	private void Awake()
	{
		chunkSideLength = ChunkData.SideLength;
		chunkSideLengthTemp = chunkSideLength * 10;
		chunkDoubleLength = chunkSideLengthTemp * 2;
		chunkDoubleSquareLength = chunkDoubleLength * chunkDoubleLength;
		navigationManager = Manager.Get<NavigationManager>();
		isFishingEnabled = Manager.Get<ModelManager>().fishingSettings.IsFishingEnabled();
		if (isFishingEnabled)
		{
			globalLakes = navigationManager.navigationHistory;
			LoadLakesBatch();
		}
	}

	private void Update()
	{
		if (isFishingEnabled)
		{
			foreach (Chunk value in ChunkManager.Chunks.Values)
			{
				chunkIndex = EncodeXYZToInt(value.chunkData.ChunkIndex.x, value.chunkData.ChunkIndex.y, value.chunkData.ChunkIndex.z);
				if (!chunkIndexes.Contains(chunkIndex) && value.chunkData.hasWater)
				{
					chunkIndexes.Add(EncodeXYZToInt(value.chunkData.ChunkIndex.x, value.chunkData.ChunkIndex.y, value.chunkData.ChunkIndex.z));
				}
			}
			visited.Clear();
			chunkLakes.Clear();
			foreach (int chunkIndex2 in chunkIndexes)
			{
				if (!visited.Contains(chunkIndex2))
				{
					Queue<int> queue = new Queue<int>();
					HashSet<int> hashSet = new HashSet<int>();
					visited.Add(chunkIndex2);
					queue.Enqueue(chunkIndex2);
					hashSet.Add(chunkIndex2);
					while (queue.Count > 0)
					{
						int index = queue.Dequeue();
						int[] array = DecodeIntArrayFromInt(index);
						int num = array[0];
						int num2 = array[1];
						int num3 = array[2];
						Chunk chunkComponent = ChunkManager.GetChunkComponent(num, num2, num3);
						Vector3[] array2 = neighbors;
						for (int i = 0; i < array2.Length; i++)
						{
							Vector3 vector = array2[i];
							int num4 = num + (int)vector.x;
							int num5 = num2 + (int)vector.y;
							int num6 = num3 + (int)vector.z;
							int item = EncodeXYZToInt(num4, num5, num6);
							Chunk chunkComponent2 = ChunkManager.GetChunkComponent(num4, num5, num6);
							if (chunkComponent2 != null && chunkComponent != null && chunkIndexes.Contains(item) && !visited.Contains(item) && !hashSet.Contains(item) && chunkComponent2.WaterCollides(chunkComponent))
							{
								visited.Add(item);
								queue.Enqueue(item);
								hashSet.Add(item);
							}
						}
					}
					if (hashSet.Count > 2)
					{
						chunkLakes.Add(hashSet);
					}
				}
			}
			UpdateGlobalLakes();
		}
	}

	public int EncodeXYZToInt(int xIn, int yIn, int zIn)
	{
		xOut = xIn + chunkSideLengthTemp;
		yOut = yIn + chunkSideLengthTemp;
		zOut = zIn + chunkSideLengthTemp;
		return xOut + yOut * chunkDoubleLength + zOut * chunkDoubleSquareLength;
	}

	public int EncodeIndexToInt(Index i)
	{
		return EncodeXYZToInt(i.x, i.y, i.z);
	}

	public int[] DecodeIntArrayFromInt(int index)
	{
		if (chunkSideLength == 0)
		{
			chunkSideLength = ChunkData.SideLength;
			chunkSideLengthTemp = chunkSideLength * 10;
			chunkDoubleLength = chunkSideLengthTemp * 2;
			chunkDoubleSquareLength = chunkDoubleLength * chunkDoubleLength;
		}
		zOut = Mathf.FloorToInt(index / chunkDoubleSquareLength);
		yOut = Mathf.FloorToInt((index - zOut * chunkDoubleSquareLength) / chunkDoubleLength);
		xOut = index - yOut * chunkDoubleLength - zOut * chunkDoubleSquareLength;
		xyzOut[0] = xOut - chunkSideLengthTemp;
		xyzOut[1] = yOut - chunkSideLengthTemp;
		xyzOut[2] = zOut - chunkSideLengthTemp;
		return xyzOut;
	}

	private void UpdateGlobalLakes()
	{
		foreach (HashSet<int> chunkLake in chunkLakes)
		{
			chunkPositions.Clear();
			meanPosition = Vector3.zero;
			bool flag = false;
			foreach (int item5 in chunkLake)
			{
				int[] array = DecodeIntArrayFromInt(item5);
				chunkPos.x = (float)array[0] + 0.5f;
				chunkPos.y = array[1];
				chunkPos.z = (float)array[2] + 0.5f;
				chunkPos *= (float)chunkSideLength;
				chunkPositions.Add(chunkPos);
				meanPosition += chunkPos;
			}
			meanPosition /= (float)chunkPositions.Count;
			for (int i = 0; i < globalLakes.Count; i++)
			{
				globalLakeChunkIndex = Engine.PositionToIndex(globalLakes[i]);
				int item = EncodeIndexToInt(globalLakeChunkIndex);
				if (chunkLake.Contains(item))
				{
					globalLakes[i] = meanPosition;
					navigationManager.navigationHistory[i] = meanPosition;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (Vector3 globalLake in globalLakes)
				{
					if (Vector3.Distance(globalLake, meanPosition) <= 64f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					globalLakes.Add(meanPosition);
					navigationManager.Add(meanPosition);
					int item2 = UnityEngine.Random.Range(0, 3);
					lakesBatch.Add(item2);
				}
			}
			for (int j = 0; j < globalLakes.Count; j++)
			{
				globalLakeChunkIndex = Engine.PositionToIndex(globalLakes[j]);
				int item3 = EncodeIndexToInt(globalLakeChunkIndex);
				Index i2 = Engine.PositionToIndex(base.transform.position);
				int item4 = EncodeIndexToInt(i2);
				if (chunkLake.Contains(item4) && chunkLake.Contains(item3))
				{
					currentLakeBatch = lakesBatch[j];
					break;
				}
			}
		}
	}

	public int GetCurrentLakeBatch()
	{
		return currentLakeBatch;
	}

	private void SaveLakesBatch()
	{
		string value = JSONHelper.ToJson(lakesBatch.ToArray());
		WorldPlayerPrefs.get.SetString("lakes.batch", value);
	}

	private void LoadLakesBatch()
	{
		string @string = WorldPlayerPrefs.get.GetString("lakes.batch", string.Empty);
		try
		{
			int[] array = JSONHelper.FromJson<int>(@string);
			lakesBatch = new List<int>();
			int[] array2 = array;
			foreach (int item in array2)
			{
				lakesBatch.Add(item);
			}
		}
		catch
		{
			UnityEngine.Debug.LogWarning("[LAKES NAVIGATOR]: Didnt find any lake batches in prefs.");
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (isFishingEnabled)
		{
			SaveLakesBatch();
		}
	}

	private void OnApplicationQuit()
	{
		if (isFishingEnabled)
		{
			SaveLakesBatch();
		}
	}
}
