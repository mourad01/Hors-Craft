// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ChatBotStateStartParameter
using Common.Managers.States;
using System;

namespace States
{
	public class ChatBotStateStartParameter : StartParameter
	{
		public PlayerGraphic mobGraphic;

		public Pettable pettable;

		public bool hideBackground;

		public bool hideTamepanel;

		public ChatBotState.CustomAnswers customAnswers;

		public int chatBootSeed;

		public bool isMale = true;

		public string botName;

		public Action OnChatClosed;
	}
}
