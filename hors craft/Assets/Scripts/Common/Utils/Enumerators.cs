// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.Enumerators
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils
{
	public static class Enumerators
	{
		public static IEnumerator WaitForRealSeconds(float delay)
		{
			float end = Time.realtimeSinceStartup + delay;
			while (Time.realtimeSinceStartup < end)
			{
				yield return null;
			}
		}

		public static IEnumerator<T> Loop<T>(IList<T> list)
		{
			int index = 0;
			while (true)
			{
				yield return list[index];
				index = (index + 1) % list.Count;
			}
		}

		public static IEnumerator<T> RandomButNotPrevious<T>(IList<T> list)
		{
			int lastIndex = 0;
			while (true)
			{
				int newIndex = lastIndex;
				if (list.Count > 1)
				{
					while (newIndex == lastIndex)
					{
						newIndex = Random.Range(0, list.Count);
					}
				}
				lastIndex = newIndex;
				yield return list[lastIndex];
			}
		}
	}
}
