// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.CityManager
using Common.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Uniblocks
{
	[Serializable]
	public class CityManager : MonoBehaviour
	{
		public static int chunkSideLength = ChunkData.SideLength;

		public static List<City> cities;

		public static int borderSize = 10;

		public static int scale = 120;

		public static Pass[] passes;

		[SerializeField]
		private Pass[] passesToSet;

		private void Awake()
		{
			passes = passesToSet;
			InitializePasses();
			if (!TryToLoadCitiesData())
			{
				GenerateCities();
				SaveCitiesData();
			}
		}

		public static City GetNearestCity(ChunkData chunk)
		{
			City result = null;
			float num = float.MaxValue;
			foreach (City city in cities)
			{
				Index relativeIndex = GetRelativeIndex(chunk.ChunkIndex);
				int x = relativeIndex.x;
				int z = relativeIndex.z;
				Index relativeIndex2 = GetRelativeIndex(city.index);
				int x2 = relativeIndex2.x;
				int z2 = relativeIndex2.z;
				float num2 = Mathf.Pow(Mathf.Abs(x2 - x), 2f) + Mathf.Pow(Mathf.Abs(z2 - z), 2f);
				if (num2 < num)
				{
					num = num2;
					result = new City(city);
				}
			}
			return result;
		}

		public static Index GetRelativeIndex(Index index)
		{
			int i;
			for (i = index.x; i < 0; i += scale)
			{
			}
			i %= scale;
			int j;
			for (j = index.z; j < 0; j += scale)
			{
			}
			j %= scale;
			return new Index(i, index.y, j);
		}

		private void SaveCitiesData()
		{
			int currentWorldDataIndex = Manager.Get<SavedWorldManager>().currentWorldDataIndex;
			string path = Application.persistentDataPath + "/cities" + currentWorldDataIndex + ".dat";
			FileStream fileStream = (!File.Exists(path)) ? File.Create(path) : File.OpenWrite(path);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(fileStream, cities);
			fileStream.Close();
		}

		private bool TryToLoadCitiesData()
		{
			int currentWorldDataIndex = Manager.Get<SavedWorldManager>().currentWorldDataIndex;
			string path = Application.persistentDataPath + "/cities" + currentWorldDataIndex + ".dat";
			if (File.Exists(path))
			{
				FileStream fileStream = File.OpenRead(path);
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				List<City> list = (List<City>)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
				cities = list;
				return true;
			}
			return false;
		}

		private void GenerateCities()
		{
			int num = borderSize / 2;
			cities = new List<City>();
			for (int i = 0; i < 16; i++)
			{
				int setX = (int)(HaltonSequence(i, 2) * (float)(scale - borderSize)) + num;
				int setZ = (int)(HaltonSequence(i, 3) * (float)(scale - borderSize)) + num;
				Index index = new Index(setX, 0, setZ);
				int num2 = 2 + i % 4;
				City item = new City(index, num2, num2 + 3);
				cities.Add(item);
			}
		}

		private void InitializePasses()
		{
			Pass[] array = passes;
			foreach (Pass pass in array)
			{
				BlueprintWithChance[] blueprintsWithChance = pass.blueprintsWithChance;
				foreach (BlueprintWithChance blueprintWithChance in blueprintsWithChance)
				{
					BlueprintData blueprintData = BlueprintDataFiles.ReadDataFromResources(blueprintWithChance.blueprintCraftableObject.blueprintResourceName);
					blueprintWithChance.size = ((blueprintData.width <= blueprintData.depth) ? blueprintData.depth : blueprintData.width);
				}
			}
		}

		private float HaltonSequence(int i, int b)
		{
			float num = 1f;
			float num2 = 0f;
			while (i > 0)
			{
				num /= (float)b;
				num2 += num * (float)(i % b);
				i /= b;
			}
			return num2;
		}
	}
}
