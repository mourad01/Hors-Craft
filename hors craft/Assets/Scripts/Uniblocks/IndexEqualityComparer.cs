// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.IndexEqualityComparer
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Uniblocks
{
	[Serializable]
	public struct Index : IVector3Index
	{
		public class IndexEqualityComparer : IEqualityComparer<Index>
		{
			public bool Equals(Index from, Index to)
			{
				return from.x == to.x && from.y == to.y && from.z == to.z;
			}

			public int GetHashCode(Index obj)
			{
				return obj.GetHashCode();
			}
		}

		private static readonly IndexEqualityComparer _equalityComparer = new IndexEqualityComparer();

		public int x;

		public int y;

		public int z;

		public static IndexEqualityComparer equalityComparer => _equalityComparer;

		public Index(int setX, int setY, int setZ)
		{
			x = setX;
			y = setY;
			z = setZ;
		}

		public Index(Vector3 setIndex)
		{
			x = (int)setIndex.x;
			y = (int)setIndex.y;
			z = (int)setIndex.z;
		}

		public Index(Index copyIndex)
		{
			x = copyIndex.x;
			y = copyIndex.y;
			z = copyIndex.z;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(x, y, z);
		}

		public int MulFlat(int x, int y, int z)
		{
			return x * this.x + ChunkData.SideLength * (y * this.y) + ChunkData.SquaredSideLength * (z * this.z);
		}

		public int ToFlat()
		{
			return x + ChunkData.SideLength * y + ChunkData.SquaredSideLength * z;
		}

		public int FlatAdd(int rigth)
		{
			return ToFlat() + rigth;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(20);
			stringBuilder.Append(x);
			stringBuilder.Append(',');
			stringBuilder.Append(y);
			stringBuilder.Append(',');
			stringBuilder.Append(z);
			return stringBuilder.ToString();
		}

		public bool IsEqual(IVector3Index to)
		{
			if (to == null)
			{
				return false;
			}
			return x == to.GetX() && y == to.GetY() && z == to.GetZ();
		}

		public Index GetAdjacentIndex(Direction direction)
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
				return default(Index);
			}
		}

		public static bool Compare(Index a, Index b)
		{
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static Index FromString(string indexString)
		{
			string[] array = indexString.Split(',');
			try
			{
				return new Index(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
			}
			catch (Exception)
			{
				UnityEngine.Debug.LogError("Uniblocks: Index.FromString: Invalid format. String must be in \"x,y,z\" format.");
				return default(Index);
			}
		}

		public override int GetHashCode()
		{
			return (x << 20) ^ (y << 10) ^ z;
		}

		public static bool operator ==(Index left, Index rigth)
		{
			return left.IsEqual(rigth);
		}

		public static bool operator !=(Index left, Index rigth)
		{
			return !left.IsEqual(rigth);
		}

		public override bool Equals(object obj)
		{
			if (obj is Index)
			{
				Index index = (Index)obj;
				return x == index.x && y == index.y && z == index.z;
			}
			IVector3Index vector3Index = obj as IVector3Index;
			return vector3Index != null && IsEqual(vector3Index);
		}

		public int GetX()
		{
			return x;
		}

		public int GetY()
		{
			return y;
		}

		public int GetZ()
		{
			return z;
		}

		public static Index operator +(Index left, Index rigth)
		{
			return new Index(left.x + rigth.x, left.y + rigth.y, left.z + rigth.z);
		}

		public static Index operator +(Index left, int rigth)
		{
			return new Index(left.x + rigth, left.y + rigth, left.z + rigth);
		}

		public static Vector3 operator +(Index left, Vector3 rigth)
		{
			return new Vector3((float)left.x + rigth.x, (float)left.y + rigth.y, (float)left.z + rigth.z);
		}

		public static Index operator -(Index left, Index rigth)
		{
			return new Index(left.x - rigth.x, left.y - rigth.y, left.z - rigth.z);
		}

		public static Index operator -(Index left, int rigth)
		{
			return new Index(left.x - rigth, left.y - rigth, left.z - rigth);
		}

		public static Index operator *(Index left, Index rigth)
		{
			return new Index(left.x * rigth.x, left.y * rigth.y, left.z * rigth.z);
		}

		public static Index operator *(Index left, int rigth)
		{
			return new Index(left.x * rigth, left.y * rigth, left.z * rigth);
		}

		public static Vector3 operator *(Index left, Vector3 rigth)
		{
			return new Vector3((float)left.x * rigth.x, (float)left.y * rigth.y, (float)left.z * rigth.z);
		}

		public static Index operator /(Index left, Index rigth)
		{
			return new Index(left.x / rigth.x, left.y / rigth.y, left.z / rigth.z);
		}

		public static Index operator /(Index left, int rigth)
		{
			return new Index(left.x / rigth, left.y / rigth, left.z / rigth);
		}

		public Index Abs()
		{
			return new Index(Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z));
		}

		public int Max()
		{
			return Mathf.Max(x, y, z);
		}
	}
}
