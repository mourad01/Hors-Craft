// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AdForRespawnStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine.UI;

namespace States
{
	public class AdForRespawnStateConnector : UIConnector
	{
		public Button okButton;

		public TranslateText okButtonText;

		public Button cancelButton;

		public TranslateText cancelButtonText;

		public TranslateText mainText;

		public TranslateText secondText;

		public void SetOkButton(string newKey = null, Action newAction = null)
		{
			if (okButton == null)
			{
				throw new NullReferenceException("Attach a button object to the okButton field and THEN change it.");
			}
			if (!string.IsNullOrEmpty(newKey))
			{
				SetOkButtonText(newKey);
			}
			okButton.onClick.AddListener(delegate
			{
				if (newAction != null)
				{
					newAction();
				}
			});
		}

		public void SetSecondText(PopupTextConfig aditionalTexts)
		{
			secondText.defaultText = aditionalTexts.defaultText;
			secondText.translationKey = aditionalTexts.translationKey;
			secondText.ForceRefresh();
		}

		public void SetCancelButton(string newKey = null, Action newAction = null)
		{
			if (cancelButton == null)
			{
				throw new NullReferenceException("Attach a button to the cancelButton field and THEN change it.");
			}
			if (!string.IsNullOrEmpty(newKey))
			{
				SetCancelButtonText(newKey);
			}
			cancelButton.onClick.AddListener(delegate
			{
				if (newAction != null)
				{
					newAction();
				}
			});
		}

		public void SetCustomTitle(string newKey)
		{
			if (!string.IsNullOrEmpty(newKey))
			{
				SetMainText(newKey);
			}
		}

		protected void SetOkButtonText(string newKey)
		{
			if (okButtonText == null)
			{
				throw new NullReferenceException("Attach a Text object from the ok button to the field and THEN change it.");
			}
			okButtonText.translationKey = newKey;
			okButtonText.defaultText = "OK";
			okButtonText.ForceRefresh();
		}

		protected void SetCancelButtonText(string newKey)
		{
			if (cancelButtonText == null)
			{
				throw new NullReferenceException("Attach a Text object from the cancel button to the field and THEN change it.");
			}
			cancelButtonText.translationKey = newKey;
			cancelButtonText.defaultText = "CANCEL";
			cancelButtonText.ForceRefresh();
		}

		protected void SetMainText(string newText)
		{
			if (mainText == null)
			{
				throw new NullReferenceException("Attach a Text object to the mainText field and THEN change it.");
			}
			mainText.translationKey = newText;
			mainText.defaultText = "You died. Watch Ad to respawn.";
			mainText.ForceRefresh();
		}
	}
}
