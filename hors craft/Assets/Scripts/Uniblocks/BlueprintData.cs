// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BlueprintData
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Uniblocks
{
	[Serializable]
	public class BlueprintData
	{
		public ushort[][][] voxelsArray;

		public byte[][][] rotationsArray;

		public int width => voxelsArray.Length;

		public int height => voxelsArray[0].Length;

		public int depth => voxelsArray[0][0].Length;

		public static BlueprintData CreateBlueprint(VoxelInfo[][][] voxels)
		{
			BlueprintData blueprintData = new BlueprintData();
			blueprintData.voxelsArray = new ushort[voxels.Length][][];
			blueprintData.rotationsArray = new byte[voxels.Length][][];
			for (int i = 0; i < voxels.Length; i++)
			{
				blueprintData.voxelsArray[i] = new ushort[voxels[i].Length][];
				blueprintData.rotationsArray[i] = new byte[voxels[i].Length][];
				for (int j = 0; j < voxels[i].Length; j++)
				{
					blueprintData.voxelsArray[i][j] = new ushort[voxels[i][j].Length];
					blueprintData.rotationsArray[i][j] = new byte[voxels[i][j].Length];
					for (int k = 0; k < voxels[i][j].Length; k++)
					{
						blueprintData.voxelsArray[i][j][k] = voxels[i][j][k].GetVoxel();
						blueprintData.rotationsArray[i][j][k] = voxels[i][j][k].GetVoxelRotation();
					}
				}
			}
			return blueprintData;
		}

		public ushort GetVoxel(Vector3 localPosition)
		{
			byte rotation;
			return GetVoxel(localPosition, out rotation);
		}

		public ushort GetVoxel(Vector3 localPosition, out byte rotation)
		{
			int num = width / 2 + Mathf.FloorToInt(localPosition.x);
			int num2 = height / 2 + Mathf.FloorToInt(localPosition.y);
			int num3 = depth / 2 + Mathf.FloorToInt(localPosition.z);
			if (num >= 0 && num < width && num2 >= 0 && num2 < height && num3 >= 0 && num3 < depth)
			{
				rotation = rotationsArray[num][num2][num3];
				return voxelsArray[num][num2][num3];
			}
			rotation = 0;
			return 0;
		}

		public ushort GetVoxel(int x, int y, int z)
		{
			return voxelsArray[x][y][z];
		}

		public ushort GetVoxel(int x, int y, int z, out byte rotation)
		{
			rotation = rotationsArray[x][y][z];
			return voxelsArray[x][y][z];
		}

		public int GetBlockCount()
		{
			int num = 0;
			for (int i = 0; i < voxelsArray.Length; i++)
			{
				for (int j = 0; j < voxelsArray[i].Length; j++)
				{
					for (int k = 0; k < voxelsArray[i][j].Length; k++)
					{
						if (voxelsArray[i][j][k] != 0)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		public string Serialize()
		{
			StringWriter stringWriter = new StringWriter();
			ushort value = (ushort)voxelsArray.Length;
			ushort value2 = (ushort)voxelsArray[0].Length;
			ushort value3 = (ushort)voxelsArray[0][0].Length;
			stringWriter.Write((char)value);
			stringWriter.Write((char)value2);
			stringWriter.Write((char)value3);
			int num = 0;
			ushort num2 = 0;
			ushort num3 = 0;
			for (int i = 0; i < voxelsArray.Length; i++)
			{
				for (int j = 0; j < voxelsArray[i].Length; j++)
				{
					for (int k = 0; k < voxelsArray[i][j].Length; k++)
					{
						ushort num4 = voxelsArray[i][j][k];
						if (num4 != num2)
						{
							if (num != 0)
							{
								stringWriter.Write((char)num3);
								stringWriter.Write((char)num2);
							}
							num3 = 1;
							num2 = num4;
						}
						else
						{
							num3 = (ushort)(num3 + 1);
						}
						num++;
					}
				}
			}
			stringWriter.Write((char)num3);
			stringWriter.Write((char)num2);
			stringWriter = WriteRotationData(stringWriter);
			string result = stringWriter.ToString();
			stringWriter.Flush();
			stringWriter.Close();
			return result;
		}

		private StringWriter WriteRotationData(StringWriter writer)
		{
			int num = 0;
			byte b = 0;
			ushort num2 = 0;
			for (int i = 0; i < rotationsArray.Length; i++)
			{
				for (int j = 0; j < rotationsArray[i].Length; j++)
				{
					for (int k = 0; k < rotationsArray[i][j].Length; k++)
					{
						byte b2 = rotationsArray[i][j][k];
						if (b2 != b)
						{
							if (num != 0)
							{
								writer.Write((char)num2);
								writer.Write((char)b);
							}
							num2 = 1;
							b = b2;
						}
						else
						{
							num2 = (ushort)(num2 + 1);
						}
						num++;
					}
				}
			}
			writer.Write((char)num2);
			writer.Write((char)b);
			return writer;
		}

		public static BlueprintData Deserialize(MemoryStream stream)
		{
			BlueprintData blueprintData = new BlueprintData();
			StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			ushort num = (ushort)streamReader.Read();
			ushort num2 = (ushort)streamReader.Read();
			ushort num3 = (ushort)streamReader.Read();
			ushort[][][] array = new ushort[num][][];
			ushort num4 = (ushort)streamReader.Read();
			ushort num5 = (ushort)streamReader.Read();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new ushort[num2][];
				for (int j = 0; j < array[i].Length; j++)
				{
					array[i][j] = new ushort[num3];
					for (int k = 0; k < array[i][j].Length; k++)
					{
						if (num4 <= 0)
						{
							num4 = (ushort)streamReader.Read();
							num5 = (ushort)streamReader.Read();
						}
						array[i][j][k] = num5;
						num4 = (ushort)(num4 - 1);
					}
				}
			}
			blueprintData.voxelsArray = array;
			blueprintData.rotationsArray = DeserializeRotation(streamReader, num, num2, num3);
			streamReader.Close();
			streamReader.Dispose();
			return blueprintData;
		}

		private static byte[][][] DeserializeRotation(StreamReader reader, ushort xLength, ushort yLength, ushort zLength)
		{
			byte[][][] array = new byte[xLength][][];
			ushort num = (ushort)reader.Read();
			byte b = (byte)reader.Read();
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new byte[yLength][];
				for (int j = 0; j < array[i].Length; j++)
				{
					array[i][j] = new byte[zLength];
					for (int k = 0; k < array[i][j].Length; k++)
					{
						if (num <= 0)
						{
							num = (ushort)reader.Read();
							b = (byte)reader.Read();
						}
						array[i][j][k] = b;
						num = (ushort)(num - 1);
					}
				}
			}
			return array;
		}

		private void ForEachVoxel(Action<ushort> action)
		{
			ForEachVoxel(delegate(ushort u, int i, int j, int k)
			{
				action(u);
			});
		}

		private void ForEachVoxel(Action<ushort, int, int, int> action)
		{
			for (int i = 0; i < voxelsArray.Length; i++)
			{
				for (int j = 0; j < voxelsArray[i].Length; j++)
				{
					for (int k = 0; k < voxelsArray[i][j].Length; k++)
					{
						action(voxelsArray[i][j][k], i, j, k);
					}
				}
			}
		}
	}
}
