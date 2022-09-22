// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.States.PolicyPopupStateStartParameter
using System;
using System.Collections.Generic;

namespace Common.Managers.States
{
	public class PolicyPopupStateStartParameter : StartParameter
	{
		public Queue<PolicyPopupData> popupDataQueue;

		public Action onFinish;

		public PolicyPopupStateStartParameter(Queue<PolicyPopupData> popupDataQueue, Action onFinish)
		{
			this.popupDataQueue = popupDataQueue;
			this.onFinish = onFinish;
		}

		public static PolicyPopupStateStartParameter DefaultStartState(Queue<PolicyPopupData> setup, Action onFinish)
		{
			return new PolicyPopupStateStartParameter(setup, onFinish);
		}
	}
}
