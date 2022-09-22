// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.MinMax
using System;
using UnityEngine;

namespace Common.Utils
{
	[Serializable]
	public class MinMax
	{
		public float min;

		public float max = 1f;

		public void Init()
		{
			float num = Mathf.Min(min, max);
			max = Mathf.Max(min, max);
			min = num;
		}

		public float GetRandom()
		{
			Init();
			return UnityEngine.Random.Range(min, max);
		}
	}
}
