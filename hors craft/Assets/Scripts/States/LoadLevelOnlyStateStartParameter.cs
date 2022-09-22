// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadLevelOnlyStateStartParameter
using System;

namespace States
{
	public class LoadLevelOnlyStateStartParameter : LoadLevelStateStartParameter
	{
		public Type stateType;

		public override void SetupLoadLevelState(LoadLevelState state)
		{
			state.startLoading = delegate
			{
				state.isLoaded = (() => true);
			};
			state.LoadStateAfter(stateType);
		}
	}
}
