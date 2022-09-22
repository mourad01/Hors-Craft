// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ChunkDataFiles
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Uniblocks
{
	public static class ChunkDataFiles
	{
		public delegate ushort VoxelAction(ushort voxel);

		public class ConcurrentMiniDictionary<TKey, TVal>
		{
			private Dictionary<TKey, TVal> dictionary;

			public Dictionary<TKey, TVal>.KeyCollection Keys
			{
				get
				{
					lock (dictionary)
					{
						return dictionary.Keys;
					}
				}
			}

			public ConcurrentMiniDictionary()
			{
				dictionary = new Dictionary<TKey, TVal>();
			}

			public void Clear()
			{
				lock (dictionary)
				{
					dictionary.Clear();
				}
			}

			public bool Contains(TKey key)
			{
				return dictionary.ContainsKey(key);
			}

			public TVal Get(TKey key)
			{
				lock (dictionary)
				{
					return (!Contains(key)) ? default(TVal) : dictionary[key];
				}
			}

			public void Set(TKey key, TVal val)
			{
				lock (dictionary)
				{
					dictionary.AddOrReplace(key, val);
				}
			}
		}

		public static bool SavingChunks;

		public static ConcurrentMiniDictionary<string, string[]> LoadedRegions;

		public static HashSet<string> ChangedRegions = new HashSet<string>();

		public static void Init()
		{
			(LoadedRegions ?? (LoadedRegions = new ConcurrentMiniDictionary<string, string[]>())).Clear();
			ChangedRegions.Clear();
			StartMultithreadedLoading();
		}

		public static bool LoadData(ChunkData chunk)
		{
			string chunkData = GetChunkData(chunk.ChunkIndex);
			if (chunkData != string.Empty)
			{
				DecompressData(chunk, chunkData);
				chunk.ChunkComputerWaterCollider();
				chunk.VoxelsDone = true;
				return true;
			}
			return false;
		}

		public static void SaveData(ChunkData chunk, bool force = false)
		{
			if (chunk.isDirty || force)
			{
				string data = CompressData(chunk);
				WriteChunkData(chunk.ChunkIndex, data);
			}
		}

		public static void DecompressData(ChunkData chunk, string data)
		{
			if (string.IsNullOrEmpty(data) || ((data.Length == 2 || data.Length == 4) && data[1] == '\0'))
			{
				chunk.Empty = true;
				return;
			}
			StringReader stringReader = new StringReader(data);
			int num = 0;
			ushort[] voxelData = chunk.VoxelData;
			int num2 = voxelData.Length;
			ushort blueprintID = Engine.usefulIDs.blueprintID;
			try
			{
				while (num < num2)
				{
					ushort num3 = (ushort)stringReader.Read();
					ushort num4 = (ushort)stringReader.Read();
					int num5 = 0;
					while (num5 < num3)
					{
						voxelData[num] = num4;
						if (num4 == blueprintID && !chunk.rebuildOnMainThread)
						{
							chunk.rebuildOnMainThread = true;
						}
						num5++;
						num++;
					}
				}
			}
			catch
			{
				stringReader.Close();
				return;
			}
			chunk.Empty = false;
			DecompressRotationData(chunk, stringReader);
			stringReader.Close();
		}

		private static void DecompressRotationData(ChunkData chunk, StringReader reader)
		{
			int num = 0;
			byte[] voxelRotation = chunk.VoxelRotation;
			try
			{
				if (reader.Peek() != -1)
				{
					while (num < voxelRotation.Length)
					{
						ushort num2 = (ushort)reader.Read();
						byte b = (byte)reader.Read();
						int num3 = 0;
						while (num3 < num2)
						{
							voxelRotation[num] = b;
							num3++;
							num++;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static string CompressData(ChunkData chunk)
		{
			return CompressData(chunk.VoxelData, chunk.VoxelRotation, chunk.Empty);
		}

		public static string CompressData(ushort[] voxelData, byte[] voxelRotation, bool empty)
		{
			StringWriter stringWriter = new StringWriter();
			int num = 0;
			int num2 = voxelData.Length;
			ushort num3 = 0;
			ushort num4 = 0;
			if (empty)
			{
				stringWriter.Write((char)num2);
				stringWriter.Write('\0');
			}
			else
			{
				for (num = 0; num < num2; num++)
				{
					ushort num5 = voxelData[num];
					if (num5 != num4)
					{
						if (num != 0)
						{
							stringWriter.Write((char)num3);
							stringWriter.Write((char)num4);
						}
						num3 = 1;
						num4 = num5;
					}
					else
					{
						num3 = (ushort)(num3 + 1);
					}
					if (num == num2 - 1)
					{
						stringWriter.Write((char)num3);
						stringWriter.Write((char)num4);
					}
				}
			}
			CompressRotation(voxelRotation, stringWriter, num2, empty);
			string result = stringWriter.ToString();
			stringWriter.Flush();
			stringWriter.Close();
			return result;
		}

		private static void CompressRotation(byte[] voxelRotation, StringWriter writer, int length, bool empty)
		{
			ushort num = 0;
			byte b = 0;
			if (empty)
			{
				writer.Write((char)length);
				writer.Write('\0');
				return;
			}
			for (int i = 0; i < length; i++)
			{
				byte b2 = voxelRotation[i];
				if (b2 != b)
				{
					if (i != 0)
					{
						writer.Write((char)num);
						writer.Write((char)b);
					}
					num = 1;
					b = b2;
				}
				else
				{
					num = (ushort)(num + 1);
				}
				if (i == length - 1)
				{
					writer.Write((char)num);
					writer.Write((char)b);
				}
			}
		}

		private static string GetChunkData(Index index)
		{
			int chunkRegionIndex = GetChunkRegionIndex(index);
			string[] regionData = GetRegionData(GetParentRegion(index));
			if (regionData == null)
			{
				return string.Empty;
			}
			if (regionData.Length != 1000 || regionData.Length <= chunkRegionIndex)
			{
				UnityEngine.Debug.LogErrorFormat("Region: {0}; Chunk: {1}({2}); Data l: {3};", GetParentRegion(index), index, chunkRegionIndex, regionData.Length);
			}
			return regionData[chunkRegionIndex];
		}

		private static void WriteChunkData(Index index, string data)
		{
			int chunkRegionIndex = GetChunkRegionIndex(index);
			string[] regionData = GetRegionData(GetParentRegion(index));
			regionData[chunkRegionIndex] = data;
			ChangedRegions.Add(GetParentRegion(index).ToString());
		}

		private static int GetChunkRegionIndex(Index index)
		{
			Index index2 = new Index(index.x, index.y, index.z);
			if (index2.x < 0)
			{
				index2.x = -index2.x - 1;
			}
			if (index2.y < 0)
			{
				index2.y = -index2.y - 1;
			}
			if (index2.z < 0)
			{
				index2.z = -index2.z - 1;
			}
			int num = index2.z * 100 + index2.y * 10 + index2.x;
			int num2 = num / 1000;
			return num - num2 * 1000;
		}

		private static string[] GetRegionData(Index regionIndex)
		{
			if (LoadRegionData(regionIndex))
			{
				return LoadedRegions.Get(regionIndex.ToString());
			}
			return null;
		}

		private static bool LoadRegionData(Index regionIndex)
		{
			string text = regionIndex.ToString();
			if (LoadedRegions.Contains(text))
			{
				return true;
			}
			string[] array = FilesLoader.LoadRegion(text);
			if (array == null)
			{
				array = InitStringArray();
			}
			LoadedRegions.Set(text, array);
			return true;
		}

		public static Index GetParentRegion(Index index)
		{
			Index result = new Index(index.x, index.y, index.z);
			if (index.x < 0)
			{
				result.x -= 9;
			}
			if (index.y < 0)
			{
				result.y -= 9;
			}
			if (index.z < 0)
			{
				result.z -= 9;
			}
			result.x /= 10;
			result.y /= 10;
			result.z /= 10;
			return result;
		}

		private static void CreateRegionFile(Index index)
		{
			FilesLoader filesLoader = new FilesLoader();
			filesLoader.CreateRegionFile(index.ToString());
		}

		public static IEnumerator SaveAllChunks()
		{
			if (!Engine.SaveVoxelData)
			{
				UnityEngine.Debug.LogWarning("Uniblocks: Saving is disabled. You can enable it in the Engine Settings.");
				yield break;
			}
			while (SavingChunks)
			{
				yield return new WaitForEndOfFrame();
			}
			SavingChunks = true;
			int count = 0;
			List<ChunkData> chunksToSave = new List<ChunkData>(ChunkManager.ChunkDatas.Values);
			foreach (ChunkData chunk in chunksToSave)
			{
				SaveData(chunk);
				if (!chunk.Empty)
				{
					count++;
				}
				if (count > Engine.MaxChunkSaves)
				{
					yield return new WaitForEndOfFrame();
					count = 0;
				}
			}
			WriteLoadedChunks();
			SavingChunks = false;
			UnityEngine.Debug.Log("Uniblocks: World saved successfully.");
		}

		public static void SaveAllChunksInstant(bool force = false)
		{
			if (!Engine.SaveVoxelData)
			{
				UnityEngine.Debug.LogWarning("Uniblocks: Saving is disabled. You can enable it in the Engine Settings.");
				return;
			}
			foreach (ChunkData value in ChunkManager.ChunkDatas.Values)
			{
				SaveData(value, force);
			}
			WriteLoadedChunks();
			UnityEngine.Debug.Log("Uniblocks: World saved successfully. (Instant)");
		}

		public static void WriteLoadedChunks()
		{
			foreach (string changedRegion in ChangedRegions)
			{
				WriteRegionFile(changedRegion);
			}
			ChangedRegions.Clear();
		}

		private static string[] InitStringArray()
		{
			string[] array = new string[1000];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = string.Empty;
			}
			return array;
		}

		private static void WriteRegionFile(string regionIndex)
		{
			FilesLoader filesLoader = new FilesLoader();
			filesLoader.SaveRegion(regionIndex, LoadedRegions.Get(regionIndex));
		}

		public static void EditorLoadAndSaveChunksFromRegion(string[] region, Index regionIndex, int chunkLength, string directory, VoxelAction doForEveryVoxel)
		{
			for (int i = 0; i < region.Length; i++)
			{
				region[i] = EditorChangeVoxelsInChunk(region[i], chunkLength, doForEveryVoxel);
			}
			EditorWriteRegionfile(regionIndex.ToString(), region, directory);
		}

		public static string EditorChangeVoxelsInChunk(string data, int length, VoxelAction doForEveryVoxel)
		{
			if (string.IsNullOrEmpty(data))
			{
				return string.Empty;
			}
			StringWriter stringWriter = new StringWriter();
			StringReader stringReader = new StringReader(data);
			ushort num = 0;
			ushort num2 = 0;
			for (int i = 0; i < length; i += num)
			{
				num = (ushort)stringReader.Read();
				stringWriter.Write((char)num);
				num2 = (ushort)stringReader.Read();
				num2 = doForEveryVoxel(num2);
				stringWriter.Write((char)num2);
			}
			stringWriter.Write(stringReader.ReadToEnd());
			string result = stringWriter.ToString();
			stringWriter.Flush();
			stringWriter.Close();
			stringReader.Close();
			return result;
		}

		public static string[] EditorLoadRegionData(Index regionIndex, string directory)
		{
			string regionIndexString = regionIndex.ToString();
			return FilesLoader.LoadRegion(regionIndexString, directory);
		}

		private static void EditorWriteRegionfile(string regionIndex, string[] data, string directory)
		{
			FilesLoader filesLoader = new FilesLoader();
			filesLoader.SaveRegion(regionIndex, data, directory, refreshAssetsIfEditor: false);
		}

		public static void EditorDecompressData(ushort[] voxelData, string data)
		{
			if (string.IsNullOrEmpty(data) || ((data.Length == 2 || data.Length == 4) && data[1] == '\0'))
			{
				for (int i = 0; i < voxelData.Length; i++)
				{
					voxelData[i] = 0;
				}
			}
			else
			{
				StringReader stringReader = new StringReader(data);
				int num = 0;
				int num2 = voxelData.Length;
				try
				{
					while (num < num2)
					{
						ushort num3 = (ushort)stringReader.Read();
						ushort num4 = (ushort)stringReader.Read();
						int num5 = 0;
						while (num5 < num3)
						{
							voxelData[num] = num4;
							num5++;
							num++;
						}
					}
				}
				catch
				{
					stringReader.Close();
					return;
				}
				stringReader.Close();
			}
		}

		private static void StartMultithreadedLoading()
		{
			string[] allPregeneratedFilesPath = FilesLoader.GetAllPregeneratedFilesPath();
			string[] array = allPregeneratedFilesPath;
			foreach (string text in array)
			{
				string tmp = text;
				ThreadPool.QueueUserWorkItem(delegate
				{
					LoadRegionFromThread(tmp);
				});
			}
		}

		private static void LoadRegionFromThread(string path)
		{
			string[] array = FilesLoader.LoadRegionFromFile(path);
			if (array == null)
			{
				array = InitStringArray();
			}
			string indexFromPath = GetIndexFromPath(path);
			LoadedRegions.Set(indexFromPath, array);
		}

		private static string GetIndexFromPath(string path)
		{
			return path.Split('/').Last().Split('.')[0].TrimEnd(',');
		}
	}
}
