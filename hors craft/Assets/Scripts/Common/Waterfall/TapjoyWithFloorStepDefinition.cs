// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.TapjoyWithFloorStepDefinition
namespace Common.Waterfall
{
	public class TapjoyWithFloorStepDefinition : AdWaterfallStepDefinition
	{
		public override string GetStepSettingsIdentifier()
		{
			return "tapjoy_with_floor";
		}

		public override string GetServerSideID()
		{
			return "tapjoy-with-floor";
		}

		public override IAdWaterfallStep GetAdWaterfallStep()
		{
			return new TapjoyAdWaterfallStep(string.Empty);
		}
	}
}
