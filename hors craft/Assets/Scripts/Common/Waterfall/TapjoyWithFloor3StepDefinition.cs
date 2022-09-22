// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.TapjoyWithFloor3StepDefinition
namespace Common.Waterfall
{
	public class TapjoyWithFloor3StepDefinition : TapjoyWithFloorStepDefinition
	{
		public override string GetStepSettingsIdentifier()
		{
			return "tapjoy_floor_3";
		}

		public override string GetServerSideID()
		{
			return "tapjoy-with-floor-3";
		}

		public override IAdWaterfallStep GetAdWaterfallStep()
		{
			return new TapjoyAdWaterfallStep("_floor3");
		}
	}
}
