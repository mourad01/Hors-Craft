// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.BuildingLimitPerLevelRequirement
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Gameplay
{
	[CreateAssetMenu(fileName = "Max Building Limit Per Level", menuName = "ScriptableObjects/Requirements/MaxBuildingsPerLevel", order = 1)]
	public class BuildingLimitPerLevelRequirement : Requirement
	{
		private struct RequirementPair
		{
			public int level;

			public int maxValue;

			[CompilerGenerated]
			private static Func<string, RequirementPair> _003C_003Ef__mg_0024cache0;

			public static RequirementPair Construct(string data)
			{
				string[] array = data.Split('-');
				RequirementPair result = default(RequirementPair);
				int.TryParse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture, out result.level);
				int.TryParse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture, out result.maxValue);
				return result;
			}

			public static List<RequirementPair> Deserialize(string data)
			{
				if (string.IsNullOrEmpty(data))
				{
					return new List<RequirementPair>();
				}
				string[] source = data.Split(';');
				return source.Select(Construct).ToList();
			}
		}

		public override bool customEditor
		{
			[CompilerGenerated]
			get
			{
				return true;
			}
		}

		public override bool CheckIfMet(float requiredAmount = 0f, string id = "")
		{
			return true;
		}

		public override bool CheckIfMet(float requiredAmount = 0f, string id = "", string data = "")
		{
			int num = MaxAmount(data);
			int stockCount = AutoRefreshingStock.GetStockCount($"{id}.built", float.NaN, int.MaxValue, 0);
			return stockCount < num;
		}

		public int MaxAmount(string data)
		{
			int level = Manager.Get<ProgressManager>().level;
			List<RequirementPair> list = RequirementPair.Deserialize(data);
			if (list == null || list.Count == 0)
			{
				return int.MaxValue;
			}
			RequirementPair requirementPair = list.LastOrDefault((RequirementPair p) => p.level <= level);
			if (requirementPair.Equals(default(RequirementPair)))
			{
				requirementPair = list.FirstOrDefault();
			}
			return requirementPair.maxValue;
		}
	}
}
