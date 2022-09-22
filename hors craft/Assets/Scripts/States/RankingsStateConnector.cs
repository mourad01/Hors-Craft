// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RankingsStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class RankingsStateConnector : UIConnector
	{
		public delegate void OnClick();

		public Button returnButton;

		public Transform fragmentParent;

		public OnClick onReturnButtonClicked;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
		}
	}
}
