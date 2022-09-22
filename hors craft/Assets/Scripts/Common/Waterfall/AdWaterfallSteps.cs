// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.AdWaterfallSteps
using System;
using System.Collections.Generic;

namespace Common.Waterfall
{
	public class AdWaterfallSteps
	{
		private Dictionary<AdWaterfallType, List<AdWaterfallStepInfo>> typeToSteps;

		public AdWaterfallSteps()
		{
			ConstructTypeToStepsDictionary();
		}

		private void ConstructTypeToStepsDictionary()
		{
			typeToSteps = new Dictionary<AdWaterfallType, List<AdWaterfallStepInfo>>();
			AdWaterfallType[] array = Enum.GetValues(typeof(AdWaterfallType)) as AdWaterfallType[];
			foreach (AdWaterfallType key in array)
			{
				typeToSteps.Add(key, new List<AdWaterfallStepInfo>());
			}
		}

		public void Register(AdWaterfallStepInfo info)
		{
			List<AdWaterfallStepInfo> list = typeToSteps[info.type];
			int index = FindInsertIndexForOrder(list, info.order);
			list.Insert(index, info);
		}

		private int FindInsertIndexForOrder(List<AdWaterfallStepInfo> list, int order)
		{
			if (list.Count == 0)
			{
				return 0;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (order < list[i].order)
				{
					return i;
				}
			}
			return list.Count;
		}

		public void UnRegister(AdWaterfallStepInfo info)
		{
			foreach (List<AdWaterfallStepInfo> value in typeToSteps.Values)
			{
				value.RemoveAll((AdWaterfallStepInfo checkedInfo) => checkedInfo == info);
			}
		}

		public void UnRegister(IAdWaterfallStep stepInstance)
		{
			foreach (List<AdWaterfallStepInfo> value in typeToSteps.Values)
			{
				int num = 0;
				while (num < value.Count)
				{
					if (value[num].instance == stepInstance)
					{
						value.RemoveAt(num);
					}
					else
					{
						num++;
					}
				}
			}
		}

		public void Clear()
		{
			foreach (List<AdWaterfallStepInfo> value in typeToSteps.Values)
			{
				value.Clear();
			}
		}

		public AdWaterfallStepInfo GetStep(AdWaterfallType type, int index)
		{
			List<AdWaterfallStepInfo> list = typeToSteps[type];
			if (index < 0 || index >= list.Count)
			{
				return null;
			}
			return list[index];
		}

		public List<AdWaterfallStepInfo> GetAllSteps(AdWaterfallType type)
		{
			return typeToSteps[type];
		}
	}
}
