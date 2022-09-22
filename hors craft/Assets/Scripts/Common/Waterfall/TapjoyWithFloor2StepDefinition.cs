// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.TapjoyWithFloor2StepDefinition
namespace Common.Waterfall
{
	public class TapjoyWithFloor2StepDefinition : TapjoyWithFloorStepDefinition
	{
		public override string GetStepSettingsIdentifier()
		{
			return "tapjoy_floor_2";
		}

		public override string GetServerSideID()
		{
			return "tapjoy-with-floor-2";
		}

		public override IAdWaterfallStep GetAdWaterfallStep()
		{
			return new TapjoyAdWaterfallStep("_floor2");
		}
	}
}
