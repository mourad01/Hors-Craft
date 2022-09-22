// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RateUsStateConnector
using Common.Managers.States.UI;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class RateUsStateConnector : UIConnector
	{
		public delegate void OnApplyButtonClicked(int starNo);

		public delegate void OnClick();

		public Button returnButton;

		public Button[] starsButtons;

		public Button applyButton;

		public OnApplyButtonClicked onApplyButtonClicked;

		public OnClick onReturnButtonClicked;

		private int lastSelected = -1;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
			applyButton.onClick.AddListener(delegate
			{
				if (onApplyButtonClicked != null)
				{
					onApplyButtonClicked(lastSelected);
				}
			});
			UpdateStarsColors();
			for (int i = 0; i < starsButtons.Length; i++)
			{
				int starNo = i + 1;
				starsButtons[i].onClick.AddListener(delegate
				{
					StarClicked(starNo);
				});
			}
		}

		private void Update()
		{
			applyButton.interactable = (lastSelected > -1);
		}

		private void UpdateStarsColors()
		{
			for (int i = 0; i < starsButtons.Length; i++)
			{
				if (i < lastSelected)
				{
					applyButton.GetComponent<Button>().interactable = true;
					starsButtons[i].transform.GetChild(0).gameObject.SetActive(value: true);
				}
				else
				{
					starsButtons[i].transform.GetChild(0).gameObject.SetActive(value: false);
				}
			}
		}

		private void StarClicked(int starNo)
		{
			lastSelected = starNo;
			UpdateStarsColors();
			ShowApply();
		}

		private void ShowApply()
		{
			CanvasGroup component = applyButton.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}
	}
}
