// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AdventureQuestStateParameter
using Common.Managers.States;
using System;
using UnityEngine;

namespace States
{
	public class AdventureQuestStateParameter : StartParameter
	{
		public Sprite characterTexture;

		public Action<EQuestState> activeCallerAction;

		public Action<int> onUserClicked
		{
			get;
			private set;
		}

		public AdventureScreenData data
		{
			get;
			private set;
		}

		public AdventureQuestStateParameter(Action<int> onClicked, AdventureScreenData textData, Action<EQuestState> activeCallerAction, Sprite characterImage = null)
		{
			data = textData;
			onUserClicked = onClicked;
			this.activeCallerAction = activeCallerAction;
			characterTexture = characterImage;
		}
	}
}
