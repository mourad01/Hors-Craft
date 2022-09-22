// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AdForRespawnStartParameter
using Common.Managers.States;
using System;

namespace States
{
	public class AdForRespawnStartParameter : StartParameter
	{
		public PopupTextConfig aditionalTexts;

		public Action onAccept
		{
			get;
			private set;
		}

		public Action onRefuse
		{
			get;
			private set;
		}

		public string titleTextKey
		{
			get;
			private set;
		}

		public string buttonOkTextKey
		{
			get;
			private set;
		}

		public string buttonCancelTextKey
		{
			get;
			private set;
		}

		public AdForRespawnStartParameter(string titleText = null, string buttonOkText = null, string buttonCancelText = null, PopupTextConfig aditionalTexts = null, Action onAccept = null, Action onRefuse = null)
		{
			this.onAccept = onAccept;
			this.onRefuse = onRefuse;
			titleTextKey = titleText;
			buttonOkTextKey = buttonOkText;
			buttonCancelTextKey = buttonCancelText;
			this.aditionalTexts = aditionalTexts;
		}
	}
}
