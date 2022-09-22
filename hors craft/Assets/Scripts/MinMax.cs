// DecompilerFi decompiler from Assembly-CSharp.dll class: MinMax
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	public struct MinMax
	{
		private float min;

		private float max;

		public MinMax(float min, float max)
		{
			if (min > max)
			{
				float num = min;
				min = max;
				max = num;
			}
			this.min = min;
			this.max = max;
		}

		public bool InBounds(int value, bool chunkCorrection)
		{
			return (float)value >= ((!chunkCorrection) ? min : (min - (float)(int)Mathf.Sign(min))) && (float)value <= ((!chunkCorrection) ? max : (max - (float)(int)Mathf.Sign(max)));
		}

		public bool InBoundsWorlds(float value, bool chunkCorrection)
		{
			return value >= ((!chunkCorrection) ? min : (min - (float)(int)Mathf.Sign(min))) * (float)ChunkData.SideLength && value <= ((!chunkCorrection) ? max : (max - (float)(int)Mathf.Sign(max))) * (float)ChunkData.SideLength;
		}

		public bool IsBound(int value, bool chunkCorrection)
		{
			if (chunkCorrection)
			{
				return Mathf.FloorToInt((!(min < 0f)) ? min : (min + 0.9f)) - (int)Mathf.Sign(min) == value || Mathf.FloorToInt((!(max < 0f)) ? max : (max + 0.9f)) - (int)Mathf.Sign(max) == value;
			}
			return Mathf.FloorToInt((!(min < 0f)) ? min : (min + 0.9f)) == value || Mathf.FloorToInt((!(max < 0f)) ? max : (max + 0.9f)) == value;
		}

		public bool IsMin(int value, bool chunkCorrection)
		{
			if (chunkCorrection)
			{
				return Mathf.FloorToInt((!(min < 0f)) ? min : (min + 0.9f)) - (int)Mathf.Sign(min) == value;
			}
			return Mathf.FloorToInt((!(min < 0f)) ? min : (min + 0.9f)) == value;
		}

		public bool IsMax(int value, bool chunkCorrection)
		{
			if (chunkCorrection)
			{
				return Mathf.FloorToInt((!(max < 0f)) ? max : (max + 0.9f)) - (int)Mathf.Sign(max) == value;
			}
			return Mathf.FloorToInt((!(max < 0f)) ? max : (max + 0.9f)) == value;
		}

		public Vector2 GetBound(bool chunkCorrection)
		{
			Vector2 zero = Vector2.zero;
			if (chunkCorrection)
			{
				zero.x = Mathf.FloorToInt((!(min < 0f)) ? min : (min + 0.9f)) - (int)Mathf.Sign(min);
				zero.y = Mathf.FloorToInt((!(max < 0f)) ? max : (max + 0.9f)) - (int)Mathf.Sign(max);
			}
			else
			{
				zero.x = Mathf.FloorToInt((!(min < 0f)) ? min : (min + 0.9f));
				zero.y = Mathf.FloorToInt((!(max < 0f)) ? max : (max + 0.9f));
			}
			zero.x *= ChunkData.SideLength;
			zero.y = zero.y * (float)ChunkData.SideLength + (float)ChunkData.SideLength - 1f;
			return zero;
		}
	}
}
