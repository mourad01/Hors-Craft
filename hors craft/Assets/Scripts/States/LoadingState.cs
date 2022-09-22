// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadingState
using Common.Managers;
using Common.Managers.States;
using GameUI;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace States
{
	public class LoadingState : XCraftUIState<LoadingStateConnector>
	{
		private const string defaultRodoKey = "hasShownDefaultRodo";

		private const string defaultRodoMessage = "Dear Player!\n\nDue to the changes in personal data protection regulations you need to accept the updated terms of service and Personal Data Administrator information to play this game.";

		private Sprite logoBlackResource;

		public Voxel loadingBlock;

		private bool wasLogoLoaded;

		public Sprite logoBlack;

		private bool wasConsentSpecifiedSet;

		private const float tooMuchTimeOnAdsDownload = 15f;

		private float endTime;

		protected override bool hasBackground => false;

		//protected override bool canShowBanner => false;

		private bool wasRodoShown => PlayerPrefs.GetInt("hasShownDefaultRodo", 0) == 1;

		private void SpecifyConsent(bool consent)
		{
			if (!wasConsentSpecifiedSet)
			{
				wasConsentSpecifiedSet = true;
				SpecifyConsentInManagers(consent);
				endTime = Time.realtimeSinceStartup + 15f;
				UnityEngine.Debug.Log("Consent specified");
			}
		}

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			endTime = Time.realtimeSinceStartup + 15f;
			base.connector.InitCube(loadingBlock);
			logoBlackResource = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Titlescreen");
			if (logoBlackResource == null)
			{
				base.connector.logoImage.color = Manager.Get<ColorManager>().GetColorForCategory(ColorManager.ColorCategory.MAIN_COLOR);
			}
			else
			{
				base.connector.logoImage.sprite = logoBlackResource;
			}
		}

		public override void FinishState()
		{
			if (base.connector != null)
			{
				base.connector.DestroyCube();
			}
			base.connector.logoImage = null;
			Resources.UnloadAsset(logoBlackResource);
			logoBlackResource = null;
			base.FinishState();
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (wasRodoShown)
			{
				TryToGoToTitle();
			}
			else if (Manager.Get<AbstractModelManager>().modelDownloaded)
			{
				if (base.modelManager.policyPopupSettings.GetEnabled())
				{
					ShowRodoPolicy();
				}
				else
				{
					TryToGoToTitle();
				}
			}
			else
			{
				Manager.Get<AbstractModelManager>().CheckModelDownloadError(delegate
				{
					ShowRodoPolicy();
				});
			}
		}

		private void TryToGoToTitle()
		{
			SpecifyConsent(consent: true);
			bool flag = false;
			bool flag2 = Time.realtimeSinceStartup > endTime && FacebookCheck() != "not-logged-in";
			/*if (PolicyPopupModule.AllowToStart() && (flag2 || flag || HeyzapAdsCheck() || IronSourceAdsCheck()))
			{
			}*/
				SetTitle();

		}

		private void SetTitle()
		{
			Manager.Get<StateMachineManager>().SetState<TitleState>();
		}

		private void ShowRodoPolicy()
		{
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(PolicyPopupState)))
			{
				Queue<PolicyPopupData> defaultRodoData = GetDefaultRodoData();
				Manager.Get<StateMachineManager>().PushState<PolicyPopupState>(new PolicyPopupStateStartParameter(defaultRodoData, OnRodoAccepted));
			}
		}

		public void OnRodoAccepted()
		{
			SpecifyConsent(consent: true);
			if (Manager.Contains<AbstractModelManager>())
			{
				StartCoroutine(Manager.Get<AbstractModelManager>().AssumeModelDownloaded(0f));
			}
			StartingPolicyShowState.wasInitialPolicyShown = true;
			PlayerPrefs.SetInt("hasShownDefaultRodo", 1);
		}

		private void SpecifyConsentInManagers(bool consentAquired)
		{
			Manager[] managers = MonoBehaviourSingleton<ManagersContainer>.get.GetManagers();
			Manager[] array = managers;
			foreach (Manager manager in array)
			{
				manager.OnConsentSpecified(consentAquired);
			}
		}

		private Queue<PolicyPopupData> GetDefaultRodoData()
		{
			Queue<PolicyPopupData> queue = new Queue<PolicyPopupData>();
			PolicyPopupData policyPopupData = new PolicyPopupData();
			policyPopupData.popupId = 0;
			policyPopupData.showCheckbox = false;
			policyPopupData.url = ConstructRodoFullPolicyURL();
			policyPopupData.messageKey = "policy.popup.message.rodo.short";
			policyPopupData.defaultText = "Dear Player!\n\nDue to the changes in personal data protection regulations you need to accept the updated terms of service and Personal Data Administrator information to play this game.";
			policyPopupData.context = "RodoPolicy";
			queue.Enqueue(policyPopupData);
			return queue;
		}

		private string ConstructRodoFullPolicyURL()
		{
			string str = "http://projectx-mobile.apps.tensquaregames.com/policy/policyOneStep";
			str = str + "?gamename=" + WWW.EscapeURL(Manager.Get<ConnectionInfoManager>().gameName);
			str = str + "&packageid=" + VersionDependend.applicationBundleIdentifier;
			return str + "&platform=android";
		}

		private bool HeyzapAdsCheck()
		{
			return false;
		}

		//private bool IronSourceAdsCheck()
		//{
			//return Manager.Get<IronSourceManager>().isInterstitialReady;
		//}

		private string FacebookCheck()
		{
			return "karol";
		}

		private bool ShouldRodoInEditor()
		{
			return true;
		}
	}
}
