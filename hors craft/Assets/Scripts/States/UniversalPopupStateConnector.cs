// DecompilerFi decompiler from Assembly-CSharp.dll class: States.UniversalPopupStateConnector
using Common.Managers.States.UI;
using UnityEngine;

namespace States
{
	public class UniversalPopupStateConnector : UIConnector
	{
		[HideInInspector]
		public DefaultUniversalPopup popup;

		public void Init()
		{
			popup = GetComponentInChildren<DefaultUniversalPopup>();
		}
	}
}
