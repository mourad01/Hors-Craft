// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.WaterfallStepIdentifiersExtensions
using Common.Waterfall;
using System;

/*namespace Common.Managers
{
	public static class WaterfallStepIdentifiersExtensions
	{
		public static string ToStepSettingsIdentifier(this WaterfallStepIdentifier step)
		{
			return step.ToString().ToLower();
		}

		public static string ToStepServerSideID(this WaterfallStepIdentifier step)
		{
			switch (step)
			{
			case WaterfallStepIdentifier.TAPJOY_WITH_FLOOR:
				return "tapjoy-with-floor";
			case WaterfallStepIdentifier.TAPJOY_FLOOR_1:
				return "tapjoy-with-floor-1";
			case WaterfallStepIdentifier.TAPJOY_FLOOR_2:
				return "tapjoy-with-floor-2";
			case WaterfallStepIdentifier.TAPJOY_FLOOR_3:
				return "tapjoy-with-floor-3";
			case WaterfallStepIdentifier.TAPJOY_FLOOR_4:
				return "tapjoy-with-floor-4";
			case WaterfallStepIdentifier.TAPJOY_WITHOUT_FLOOR:
				return "tapjoy-without-floor";
			default:
				return null;
			}
		}

		public static IAdWaterfallStep ToAdWaterfallStep(this WaterfallStepIdentifier step)
		{
			switch (step)
			{
			case WaterfallStepIdentifier.TAPJOY_WITH_FLOOR:
				return new TapjoyAdWaterfallStep(string.Empty);
			case WaterfallStepIdentifier.TAPJOY_FLOOR_1:
				return new TapjoyAdWaterfallStep("_floor1");
			case WaterfallStepIdentifier.TAPJOY_FLOOR_2:
				return new TapjoyAdWaterfallStep("_floor2");
			case WaterfallStepIdentifier.TAPJOY_FLOOR_3:
				return new TapjoyAdWaterfallStep("_floor3");
			case WaterfallStepIdentifier.TAPJOY_FLOOR_4:
				return new TapjoyAdWaterfallStep("_floor4");
			case WaterfallStepIdentifier.TAPJOY_WITHOUT_FLOOR:
				return new TapjoyAdWaterfallStep("_nofloor");
			case WaterfallStepIdentifier.IRONSOURCE:
				return new IronSourceAdWaterfallStep();
			default:
				return null;
			}
		}

		public static Type ToAppropriateManagerType(this WaterfallStepIdentifier step)
		{
			switch (step)
			{
			case WaterfallStepIdentifier.TAPJOY_WITH_FLOOR:
			case WaterfallStepIdentifier.TAPJOY_FLOOR_1:
			case WaterfallStepIdentifier.TAPJOY_FLOOR_2:
			case WaterfallStepIdentifier.TAPJOY_FLOOR_3:
			case WaterfallStepIdentifier.TAPJOY_FLOOR_4:
				return null;
			case WaterfallStepIdentifier.TAPJOY_WITHOUT_FLOOR:
				return typeof(TapjoyManager);
			default:
				return null;
			}
		}
	}
}*/
