// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.QueueExtension
using System.Collections.Generic;

namespace Common.Utils
{
	public static class QueueExtension
	{
		public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> enu)
		{
			foreach (T item in enu)
			{
				queue.Enqueue(item);
			}
		}
	}
}
