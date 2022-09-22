// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.AdWaterfallStepDefinition
using System;

namespace Common.Waterfall
{
	public abstract class AdWaterfallStepDefinition
	{
		public abstract string GetStepSettingsIdentifier();

		public abstract string GetServerSideID();

		public abstract IAdWaterfallStep GetAdWaterfallStep();

		public virtual Type ToAppropriateManagerTypeForDisabling()
		{
			return null;
		}
	}
}
