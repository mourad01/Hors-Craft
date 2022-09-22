// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.States.PolicyPopupState
using Common.Managers.States.UI;
using Common.Utils;
using GameUI;
using System.Collections;
using UnityEngine;

namespace Common.Managers.States
{
	public class PolicyPopupState : UIConnectedState<PolicyPopupStateConnector>
	{
		protected PolicyPopupStateStartParameter startParameter;

		private bool initialized;

		private PolicyPopupData popupData;

		protected override bool showsPolicyPopup => false;

		public override void StartState(StartParameter parameter)
		{
			initialized = false;
			base.StartState(parameter);
			startParameter = (parameter as PolicyPopupStateStartParameter);
			popupData = startParameter.popupDataQueue.Dequeue();
			if (popupData.url.IsNullOrEmpty())
			{
				base.connector.leftButton.gameObject.SetActive(value: false);
			}
			base.connector.leftButtonText.translationKey = "policy.popup.urlButton";
			base.connector.leftButtonText.defaultText = "More info";
			base.connector.leftButton.onClick.AddListener(delegate
			{
				Application.OpenURL(popupData.url);
			});
			base.connector.rightButton.interactable = !popupData.showCheckbox;
			base.connector.rightButtonText.translationKey = "policy.popup.apply";
			base.connector.rightButtonText.defaultText = "AGREE";
			base.connector.rightButton.onClick.AddListener(delegate
			{
				OnAcceptButton();
			});
			base.connector.checkboxGroup.SetActive(popupData.showCheckbox);
			base.connector.checkboxText.translationKey = "policy.popup.checkbox";
			base.connector.checkboxText.defaultText = "I accept";
			if (popupData.showCheckbox)
			{
				base.connector.checkbox.onValueChanged.AddListener(delegate(bool value)
				{
					base.connector.rightButton.image.color = GetButtonColor(value);
					base.connector.rightButton.interactable = value;
				});
			}
			base.connector.message.translationKey = popupData.messageKey;
			base.connector.message.defaultText = popupData.defaultText;
			base.connector.leftButtonText.ForceRefresh();
			base.connector.rightButtonText.ForceRefresh();
			base.connector.message.ForceRefresh();
			base.connector.checkboxText.ForceRefresh();
			base.connector.rightButton.image.color = GetButtonColor(!popupData.showCheckbox);
			base.connector.leftButton.image.color = Manager.Get<AbstractModelManager>().policyPopupSettings.GetMoreInfoColor();
			StartCoroutine(ScrollToTop());
			Sprite sprite = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Titlescreen");
			Sprite sprite2 = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Logo");
			if (sprite != null)
			{
				base.connector.background.sprite = sprite;
			}
			else if (base.connector != null && base.connector.background != null)
			{
				base.connector.background.color = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.MAIN_COLOR);
			}
			if (sprite2 != null)
			{
				base.connector.logo.sprite = sprite2;
			}
			else if (base.connector != null && base.connector.logo != null)
			{
				base.connector.logo.color = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.MAIN_COLOR);
			}
		}

		public Color GetButtonColor(bool enabled)
		{
			if (enabled)
			{
				return Manager.Get<AbstractModelManager>().policyPopupSettings.GetAcceptColor();
			}
			return Manager.Get<AbstractModelManager>().policyPopupSettings.GetDisabledColor();
		}

		private IEnumerator ScrollToTop()
		{
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			base.connector.messageScrollRect.verticalNormalizedPosition = 1f;
			initialized = true;
		}

		public override void UpdateState()
		{
			base.UpdateState();
		}

		public override void FinishState()
		{
			base.FinishState();
		}

		public void OnAcceptButton()
		{
			if (!initialized)
			{
				return;
			}
			PolicyPopupModule.SetPolicyShown(popupData.popupId, popupData.context);
			StartCoroutine(RegisterClick());
			if (startParameter.popupDataQueue.Count > 0)
			{
				Manager.Get<StateMachineManager>().SetState<PolicyPopupState>(new PolicyPopupStateStartParameter(startParameter.popupDataQueue, startParameter.onFinish));
				return;
			}
			Manager.Get<StateMachineManager>().PopState();
			if (startParameter.onFinish != null)
			{
				startParameter.onFinish();
			}
		}

		private IEnumerator RegisterClick()
		{
			string homeUrl6 = Manager.Get<ConnectionInfoManager>().homeURL;
			homeUrl6 += WWW.EscapeURL("policypopup/registerWindowAccept");
			homeUrl6 = homeUrl6 + "?playerid=" + WWW.EscapeURL(PlayerId.GetId());
			homeUrl6 += "&platform=android";
			homeUrl6 = homeUrl6 + "&gamename=" + WWW.EscapeURL(Manager.Get<ConnectionInfoManager>().gameName);
			homeUrl6 = homeUrl6 + "&popupid=" + WWW.EscapeURL(popupData.popupId.ToString());
			yield return new WWW(homeUrl6, FormFactory.CreateBasicWWWForm());
		}
	}
}
