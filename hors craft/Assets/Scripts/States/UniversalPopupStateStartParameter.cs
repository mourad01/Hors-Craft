// DecompilerFi decompiler from Assembly-CSharp.dll class: States.UniversalPopupStateStartParameter
using Common.Managers.States;
using System;

namespace States
{
	public class UniversalPopupStateStartParameter : StartParameter
	{
		public string prefabToSpawn;

		public Action<DefaultUniversalPopup> configPopup;
	}
}
