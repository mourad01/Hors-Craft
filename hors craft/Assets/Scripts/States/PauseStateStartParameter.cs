// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PauseStateStartParameter
using Common.Managers.States;

namespace States
{
	public class PauseStateStartParameter : StartParameter
	{
		public bool canSave;

		public bool allowTimeChange;

		public bool disableAds;

		public string categoryToOpen = string.Empty;

		public ushort blockCaused;
	}
}
