// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TutorialStateConnector
using Common.Managers.States.UI;
using UnityEngine.UI;

namespace States
{
	public class TutorialStateConnector : UIConnector
	{
		public delegate void OnClick();

		public OnClick onPlayClicked;

		public void Init()
		{
			Button componentInChildren = GetComponentInChildren<Button>();
			componentInChildren.onClick.AddListener(delegate
			{
				if (onPlayClicked != null)
				{
					onPlayClicked();
				}
			});
		}
	}
}
