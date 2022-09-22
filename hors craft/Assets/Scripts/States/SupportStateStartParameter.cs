// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SupportStateStartParameter
using Common.Managers.States;

namespace States
{
	public class SupportStateStartParameter : StartParameter
	{
		public delegate void OnReturn();

		public OnReturn onReturn;

		public bool noEmailMode;
	}
}
