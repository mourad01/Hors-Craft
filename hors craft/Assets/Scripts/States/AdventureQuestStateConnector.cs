// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AdventureQuestStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class AdventureQuestStateConnector : UIConnector
	{
		public Image background;

		public Text mainText;

		public ButtonTextPair[] optionsButtons = new ButtonTextPair[0];

		public Image characterImage;

		public Button nextStepButton;

		public bool IsBigButtonActive => nextStepButton.gameObject.IsActive();

		public void FireBigButton()
		{
			nextStepButton.onClick.Invoke();
		}

		public void SetCharacterImage(Sprite characterTexture)
		{
			if (!(characterImage == null))
			{
				if (characterTexture == null)
				{
					characterImage.color = new Color(0f, 0f, 0f, 0f);
					return;
				}
				characterImage.color = new Color(1f, 1f, 1f, 1f);
				characterImage.sprite = characterTexture;
			}
		}

		public void CreateBlurrBackground()
		{
			int width = Screen.width;
			int height = Screen.height;
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, mipChain: false);
			texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
			texture2D.Apply();
			LinearBlur linearBlur = new LinearBlur();
			texture2D = linearBlur.ScaleTexture(texture2D, texture2D.width / 5, texture2D.height / 5);
			texture2D = linearBlur.Blur(texture2D, 2, 2);
			background.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f);
		}

		public void SetButtonsAction(Action<int> onUserClicked)
		{
			nextStepButton.onClick.AddListener(delegate
			{
				onUserClicked(-1);
			});
			for (int i = 0; i < optionsButtons.Length; i++)
			{
				int val = i;
				optionsButtons[i].button.onClick.AddListener(delegate
				{
					onUserClicked(val);
				});
			}
		}

		public void SetOptionsButtonVisibility(int index, bool newState)
		{
			for (int i = 0; i < optionsButtons.Length; i++)
			{
				if (i == index)
				{
					optionsButtons[i].SetButtonVisibility(newState);
				}
				else
				{
					optionsButtons[i].SetButtonVisibility(!newState);
				}
			}
		}

		public void SetBigButtonVisibility(bool state)
		{
			nextStepButton.gameObject.SetActive(state);
		}

		public void SetMainText(AdventureScreenData data)
		{
			mainText.text = data.MainText;
		}

		public void SetOptions(AdventureScreenData data)
		{
			for (int i = 0; i < optionsButtons.Length; i++)
			{
				if (data.OptionsLength > i)
				{
					optionsButtons[i].buttonText.text = data.GetOption(i);
				}
				else
				{
					optionsButtons[i].SetButtonVisibility(state: false);
				}
			}
		}

		public void SetTexts(AdventureScreenData data)
		{
			mainText.text = data.MainText;
			for (int i = 0; i < optionsButtons.Length; i++)
			{
				optionsButtons[i].buttonText.text = data.GetOption(i);
			}
		}
	}
}
