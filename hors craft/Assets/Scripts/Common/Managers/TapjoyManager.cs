// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.TapjoyManager
using Common.Ksero;
using Common.Managers.Validators;
using Common.Model;
using System.Collections.Generic;
using TapjoyUnity;
using TapjoyUnity.Internal;
using UnityEngine;

namespace Common.Managers
{
	[RequireComponent(typeof(ValidateTapjoyComponentIsPresent))]
	public class TapjoyManager : Manager
	{
		public delegate void OnConnected();

		private bool connected;

		private bool verbose = true;

		[SerializeField]
		protected string adKey;

		private List<OnConnected> onConnectedCallbacks = new List<OnConnected>();

		private Dictionary<string, TJPlacement> idToPlacement = new Dictionary<string, TJPlacement>();

		public override void Init()
		{
		}

		public override void OnConsentSpecified(bool isConsentAquired)
		{
			TapjoyComponent componentInChildren = GetComponentInChildren<TapjoyComponent>();
			string text = adKey;
			if (string.IsNullOrEmpty(text))
			{
				Settings settingsForGame = KseroFiles.GetSettingsForGame(Manager.Get<ConnectionInfoManager>().gameName);
				string text2 = settingsForGame.GetString("tapjoy.android.sdkkey").Trim();
				string sdkKey = settingsForGame.GetString("tapjoy.ios.sdkkey").Trim();
				componentInChildren.settings.IosSettings.SdkKey = sdkKey;
				componentInChildren.settings.AndroidSettings.SdkKey = text2;
				text = text2;
			}
			componentInChildren.settings.AutoConnectEnabled = false;
			Tapjoy.OnConnectSuccess += OnConnectSuccess;
			Tapjoy.OnConnectFailure += OnConnectFailure;
			Tapjoy.SubjectToGDPR(isConsentAquired);
			Tapjoy.SetUserConsent((!isConsentAquired) ? "0" : "1");
			Tapjoy.Connect(text);
			UnityEngine.Debug.Log("TapJoy: connect called");
		}

		private void OnConnectSuccess()
		{
			connected = true;
			RegisterCallbacks();
			foreach (OnConnected onConnectedCallback in onConnectedCallbacks)
			{
				onConnectedCallback();
			}
			onConnectedCallbacks.Clear();
		}

		private void OnConnectFailure()
		{
			if (!Tapjoy.IsConnected)
			{
				Tapjoy.Connect();
			}
		}

		public void AddOnConnectedCallback(OnConnected onConnected)
		{
			if (connected)
			{
				onConnected();
			}
			else
			{
				onConnectedCallbacks.Add(onConnected);
			}
		}

		private void RegisterCallbacks()
		{
			TJPlacement.OnRequestSuccess += PlacementOnRequestSuccess;
			TJPlacement.OnRequestFailure += PlacementOnRequestFailure;
			TJPlacement.OnContentReady += PlacementOnContentReady;
			TJPlacement.OnContentDismiss += PlacementOnContentDismiss;
			TJPlacement.OnContentShow += PlacementOnContentShow;
		}

		public void DefinePlacement(string placementId)
		{
			if (!connected)
			{
				UnityEngine.Debug.LogWarning("Not connected to Tapjoy!");
				return;
			}
			if (idToPlacement.ContainsKey(placementId))
			{
				UnityEngine.Debug.LogWarning("Placement already defined: " + placementId);
				return;
			}
			TJPlacement tJPlacement = TJPlacement.CreatePlacement(placementId);
			idToPlacement.Add(placementId, tJPlacement);
			tJPlacement.RequestContent();
		}

		public bool ShowPlacement(string placementId)
		{
			if (!connected)
			{
				UnityEngine.Debug.LogWarning("Not connected to Tapjoy!");
				return false;
			}
			if (!idToPlacement.TryGetValue(placementId, out TJPlacement value))
			{
				UnityEngine.Debug.LogWarning("Placement " + placementId + " isn't defined! Trying to define placement!");
				DefinePlacement(placementId);
				return false;
			}
			if (value.IsContentReady())
			{
				TrackAdShown();
				value.ShowContent();
				value.RequestContent();
				TrackAdShownSuccess();
				return true;
			}
			if (verbose)
			{
				UnityEngine.Debug.Log("Tapjoy placement not ready for placement id: " + placementId);
				return false;
			}
			return false;
		}

		private void PlacementOnRequestSuccess(TJPlacement placement)
		{
		}

		private void PlacementOnRequestFailure(TJPlacement placement, string error)
		{
			if (verbose)
			{
				UnityEngine.Debug.LogWarning("Placement " + placement.GetName() + " request failure with error: " + error);
			}
		}

		private void PlacementOnContentReady(TJPlacement placement)
		{
		}

		private void PlacementOnContentDismiss(TJPlacement placement)
		{
		}

		private void PlacementOnContentShow(TJPlacement placement)
		{
		}

		private static void TrackAdShown()
		{
			StatsManager statsManager = Manager.Get<StatsManager>();
			statsManager.AdShown();
		}

		private static void TrackAdShownSuccess()
		{
			StatsManager statsManager = Manager.Get<StatsManager>();
			statsManager.AdShownSuccessBasedOnCallbacks("tapjoy");
		}
	}
}
