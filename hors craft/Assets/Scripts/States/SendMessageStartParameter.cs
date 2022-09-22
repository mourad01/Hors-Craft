// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SendMessageStartParameter
using Common.Managers.States;
using System;

namespace States
{
	public class SendMessageStartParameter : StartParameter
	{
		public Action<string> onMessageReceived;
	}
}
