// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.States.StartupPolicyParam
using System;

namespace Common.Managers.States
{
	public class StartupPolicyParam : StartParameter
	{
		public Action onFinish;

		public StartupPolicyParam(Action onFinish)
		{
			this.onFinish = onFinish;
		}
	}
}
