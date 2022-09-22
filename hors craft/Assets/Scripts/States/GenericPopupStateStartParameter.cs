// DecompilerFi decompiler from Assembly-CSharp.dll class: States.GenericPopupStateStartParameter
using Common.Managers;
using Common.Managers.States;
using System;
using UnityEngine.UI;

namespace States
{
	public class GenericPopupStateStartParameter : StartParameter
	{
		public delegate void TextConfiguration(TranslateText text);

		public delegate void ButtonConfiguration(Button button, TranslateText text);

		public ButtonConfiguration configureLeftButton;

		public ButtonConfiguration configureRightButton;

		public TextConfiguration configureMessage;

		public TextConfiguration configureTitle;

		public static GenericPopupStateStartParameter OnInternetErrorNoCancel(Action onRetry)
		{
			GenericPopupStateStartParameter genericPopupStateStartParameter = new GenericPopupStateStartParameter();
			genericPopupStateStartParameter.configureRightButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "internet.retry";
				text.defaultText = "RETRY";
				button.onClick.AddListener(delegate
				{
					onRetry();
				});
			};
			genericPopupStateStartParameter.configureLeftButton = delegate(Button button, TranslateText text)
			{
				button.gameObject.SetActive(value: false);
			};
			genericPopupStateStartParameter.configureMessage = delegate(TranslateText text)
			{
				text.translationKey = "internet.connection.error.body";
				text.defaultText = Manager.Get<TranslationsManager>().GetConnectionErrorText();
			};
			genericPopupStateStartParameter.configureTitle = delegate(TranslateText text)
			{
				text.translationKey = "internet.connection.error.title";
				text.defaultText = "NO INTERNET CONNECTION";
			};
			return genericPopupStateStartParameter;
		}

		public static GenericPopupStateStartParameter OnInternetError(Action onCancel, Action onRetry)
		{
			GenericPopupStateStartParameter genericPopupStateStartParameter = new GenericPopupStateStartParameter();
			genericPopupStateStartParameter.configureRightButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "internet.retry";
				text.defaultText = "RETRY";
				button.onClick.AddListener(delegate
				{
					onRetry();
				});
			};
			genericPopupStateStartParameter.configureLeftButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "internet.cancel";
				text.defaultText = "CANCEL";
				button.onClick.AddListener(delegate
				{
					onCancel();
				});
			};
			genericPopupStateStartParameter.configureMessage = delegate(TranslateText text)
			{
				text.translationKey = "internet.connection.error.body";
				text.defaultText = "Please turn on internet connection or move to area with better signal";
			};
			genericPopupStateStartParameter.configureTitle = delegate(TranslateText text)
			{
				text.translationKey = "internet.connection.error.title";
				text.defaultText = "NO INTERNET CONNECTION";
			};
			return genericPopupStateStartParameter;
		}

		public static GenericPopupStateStartParameter OnPaymentError()
		{
			GenericPopupStateStartParameter genericPopupStateStartParameter = new GenericPopupStateStartParameter();
			genericPopupStateStartParameter.configureRightButton = delegate(Button button, TranslateText text)
			{
				button.gameObject.SetActive(value: false);
			};
			genericPopupStateStartParameter.configureLeftButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "payment.OK";
				text.defaultText = "OK";
				button.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
				});
			};
			genericPopupStateStartParameter.configureMessage = delegate(TranslateText text)
			{
				text.translationKey = "payment.error.body";
				text.defaultText = "There was problem with processing your purchase. Please contact support!";
			};
			genericPopupStateStartParameter.configureTitle = delegate(TranslateText text)
			{
				text.translationKey = "payment.error.title";
				text.defaultText = "Error";
			};
			return genericPopupStateStartParameter;
		}

		public static GenericPopupStateStartParameter OnAdShowError(Action onCancel, Action onRetry)
		{
			GenericPopupStateStartParameter genericPopupStateStartParameter = new GenericPopupStateStartParameter();
			genericPopupStateStartParameter.configureRightButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "internet.retry";
				text.defaultText = "RETRY";
				button.onClick.AddListener(delegate
				{
					onRetry();
				});
			};
			genericPopupStateStartParameter.configureLeftButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "internet.cancel";
				text.defaultText = "CANCEL";
				button.onClick.AddListener(delegate
				{
					onCancel();
				});
			};
			genericPopupStateStartParameter.configureMessage = delegate(TranslateText text)
			{
				text.translationKey = "ad.connection.error.body";
				text.defaultText = "There was problem with showing your ad.";
			};
			genericPopupStateStartParameter.configureTitle = delegate(TranslateText text)
			{
				text.translationKey = "ad.connection.error.title";
				text.defaultText = "ERROR";
			};
			return genericPopupStateStartParameter;
		}
	}
}
