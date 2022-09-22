// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadLevelCustomStartParameter
using System;

namespace States
{
	public class LoadLevelCustomStartParameter : LoadLevelStateStartParameter
	{
		public Action startLoading;

		public Func<bool> isLoaded;

		public Action onLoadAction;

		public override void SetupLoadLevelState(LoadLevelState state)
		{
			state.isLoaded = isLoaded;
			state.onLoaded = onLoadAction;
			state.startLoading = startLoading;
		}
	}
}
