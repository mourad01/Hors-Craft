// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadingStartParameter
using Common.Managers.States;

namespace States
{
	public class LoadingStartParameter : StartParameter
	{
		public string reason = string.Empty;

		public string defaultText = string.Empty;

		public LoadingStartParameter(string reason, string defaultText)
		{
			this.reason = reason;
			this.defaultText = defaultText;
		}
	}
}
