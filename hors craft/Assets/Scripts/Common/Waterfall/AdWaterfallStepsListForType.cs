// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.AdWaterfallStepsListForType
using System.Collections.Generic;
using UnityEngine;

namespace Common.Waterfall
{
	public class AdWaterfallStepsListForType
	{
		private List<AdWaterfallStepList> lists;

		private HashSet<string> stepsIDsAlreadyAddedToAllSteps;

		private List<AdWaterfallStepInfo> allSteps;

		private float sumOfWeights;

		private bool sumOfWeightsDirty;

		public AdWaterfallStepsListForType(AdWaterfallType type)
		{
			lists = new List<AdWaterfallStepList>();
			stepsIDsAlreadyAddedToAllSteps = new HashSet<string>();
			allSteps = new List<AdWaterfallStepInfo>();
			sumOfWeightsDirty = true;
		}

		public void Register(AdWaterfallStepList list)
		{
			lists.Add(list);
			sumOfWeightsDirty = true;
			foreach (AdWaterfallStepInfo step in list.steps)
			{
				if (!stepsIDsAlreadyAddedToAllSteps.Contains(step.stepId))
				{
					stepsIDsAlreadyAddedToAllSteps.Add(step.stepId);
					allSteps.Add(step);
				}
			}
		}

		public void UnRegister(AdWaterfallStepList list)
		{
			lists.RemoveAll((AdWaterfallStepList stepList) => stepList == list);
			sumOfWeightsDirty = true;
		}

		public void Clear()
		{
			lists.Clear();
			allSteps.Clear();
			stepsIDsAlreadyAddedToAllSteps.Clear();
			sumOfWeightsDirty = true;
		}

		public List<AdWaterfallStepInfo> GetRandomStepList()
		{
			if (lists.Count == 0)
			{
				return null;
			}
			if (sumOfWeightsDirty)
			{
				sumOfWeights = 0f;
				foreach (AdWaterfallStepList list in lists)
				{
					sumOfWeights += list.weight;
				}
			}
			float num = Random.value * sumOfWeights;
			float num2 = 0f;
			foreach (AdWaterfallStepList list2 in lists)
			{
				num2 += list2.weight;
				if (num2 >= num)
				{
					return list2.steps;
				}
			}
			return lists[0].steps;
		}

		public List<AdWaterfallStepInfo> GetAllSteps()
		{
			return allSteps;
		}
	}
}
