// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.Core.DotParams.DotParamsList`1
using System;
using System.Collections.Generic;

namespace Borodar.FarlandSkies.Core.DotParams
{
	public class DotParamsList<T> : SortedList<float, T>
	{
		public DotParamsList(int capacity)
			: base(capacity)
		{
		}

		public int FindIndexPerTime(float time)
		{
			return BinarySearch(base.Keys, time);
		}

		private static int BinarySearch<T>(IList<T> list, T value)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			Comparer<T> @default = Comparer<T>.Default;
			int num = 0;
			int num2 = list.Count - 1;
			while (num < num2)
			{
				int num3 = (num2 + num) / 2;
				if (@default.Compare(list[num3], value) < 0)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3 - 1;
				}
			}
			if (@default.Compare(list[num], value) < 0)
			{
				num++;
			}
			return num;
		}
	}
}
