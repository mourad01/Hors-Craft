// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.TapjoyWithFloor1StepDefinition
namespace Common.Waterfall
{
	public class TapjoyWithFloor1StepDefinition : TapjoyWithFloorStepDefinition
	{
		public override string GetStepSettingsIdentifier()
		{
			return "tapjoy_floor_1";
		}

		public override string GetServerSideID()
		{
			return "tapjoy-with-floor-1";
		}

		public override IAdWaterfallStep GetAdWaterfallStep()
		{
			return new TapjoyAdWaterfallStep("_floor1");
		}
	}
}
