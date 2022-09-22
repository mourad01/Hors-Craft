// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadLevelAndInitStateStartParameter
using Common.Managers.States;
using System;

namespace States
{
	public class LoadLevelAndInitStateStartParameter : LoadLevelStateStartParameter
	{
		public string sceneToLoadName;

		public Type stateType;

		public StartParameter parameter;

		public override void SetupLoadLevelState(LoadLevelState state)
		{
			state.LoadScene(sceneToLoadName);
			state.LoadStateAfter(stateType, parameter);
		}
	}
}
