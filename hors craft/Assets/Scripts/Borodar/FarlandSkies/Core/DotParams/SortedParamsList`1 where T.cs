// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.Core.DotParams.SortedParamsList`1 where T
using System;

namespace Borodar.FarlandSkies.Core.DotParams
{
	[Serializable]
	public abstract class SortedParamsList<T> where T : DotParam
	{
		public T[] Params = new T[0];

		protected DotParamsList<T> SortedParams = new DotParamsList<T>(0);

		public void Init()
		{
			SortedParams = new DotParamsList<T>(Params.Length);
			T[] @params = Params;
			foreach (T val in @params)
			{
				SortedParams[val.Time] = val;
			}
		}

		public void Update()
		{
			if (SortedParams == null)
			{
				SortedParams = new DotParamsList<T>(Params.Length);
			}
			else
			{
				SortedParams.Clear();
			}
			T[] @params = Params;
			foreach (T val in @params)
			{
				SortedParams[val.Time] = val;
			}
		}
	}
}
