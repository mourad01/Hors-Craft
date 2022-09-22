// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.TapjoyWithoutFloorStepDefinition
using Common.Managers;
using System;

namespace Common.Waterfall
{
	public class TapjoyWithoutFloorStepDefinition : TapjoyWithFloorStepDefinition
	{
		public override string GetStepSettingsIdentifier()
		{
			return "tapjoy_without_floor";
		}

		public override string GetServerSideID()
		{
			return "tapjoy-without-floor";
		}

		public override IAdWaterfallStep GetAdWaterfallStep()
		{
			return new TapjoyAdWaterfallStep("_nofloor");
		}

		public override Type ToAppropriateManagerTypeForDisabling()
		{
			return typeof(TapjoyManager);
		}
	}
}
