// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GrowthStage
using Common.Managers;
using System;

namespace Gameplay
{
	[Serializable]
	public class GrowthStage
	{
		protected int defaultGrowDuration = 60;

		protected Action doOnGrowth;

		public GrowthStage(int defaultGrowthDuration = 60)
		{
			defaultGrowDuration = defaultGrowthDuration;
		}

		public GrowthStage(Action doOnGrowth, int defaultGrowthDuration = 60)
			: this(defaultGrowthDuration)
		{
			this.doOnGrowth = doOnGrowth;
		}

		public virtual float GrowDuration(GrowableInfo info)
		{
			return Manager.Get<ModelManager>().growthSettings.GetGrowthDuration(info.GetUniqueId(), info.StageIndex(this), defaultGrowDuration);
		}

		public virtual void OnGrowth(GrowableInfo info)
		{
			if (doOnGrowth != null)
			{
				doOnGrowth();
			}
		}
	}
}
