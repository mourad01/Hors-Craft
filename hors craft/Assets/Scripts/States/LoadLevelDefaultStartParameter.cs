// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadLevelDefaultStartParameter
using System;

namespace States
{
	public class LoadLevelDefaultStartParameter : LoadLevelStateStartParameter
	{
		public string sceneToLoadName;

		public Type stateType;

		public override void SetupLoadLevelState(LoadLevelState state)
		{
			state.LoadScene(sceneToLoadName);
			state.LoadStateAfter(stateType);
		}
	}
}
