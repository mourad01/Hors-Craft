// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AdWaterfallOrdersModule
using Common.Model;
using Common.Waterfall;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/*namespace Common.Managers
{
	public class AdWaterfallOrdersModule : ModelModule
	{
		public override void FillModelDescription(ModelDescription descriptions)
		{
			descriptions.AddDescription("waterfall.serialized.config", string.Empty);
		}

		public override void OnModelDownloaded()
		{
			AdWaterfall.get.InitWithConfig(ConstructConfig());
		}

		private AdWaterfallConfig ConstructConfig()
		{
			GatherSteps();
			return new AdWaterfallConfig(base.settings, GatherSteps());
		}

		private AdWaterfallStepDefinition[] GatherSteps()
		{
			List<AdWaterfallStepDefinition> list = new List<AdWaterfallStepDefinition>();
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();
			Type typeFromHandle = typeof(AdWaterfallStepDefinition);
			Type[] array = types;
			foreach (Type type in array)
			{
				if (typeFromHandle.IsAssignableFrom(type))
				{
					try
					{
						list.Add(Activator.CreateInstance(type) as AdWaterfallStepDefinition);
					}
					catch
					{
					}
				}
			}
			UnityEngine.Debug.LogFormat("Waterfall found {0} possible AdWaterfallSteps. Steps: {1}", list.Count.ToString(), list.ToStringPretty());
			return list.ToArray();
		}
	}
}*/
