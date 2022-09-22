// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.CommonUIManager
using Common.Managers;
using UnityEngine;

namespace GameUI
{
	public class CommonUIManager : Manager
	{
		private bool backgroundVisible;

		private bool backgroundOverlayVisible;

		private GameObject instantiatedBackground;

		private GameObject instantiatedBackgroundOverlay;

		private GameObject instantiatedBlackBackgroundOverlayForAds;

		private bool objectsDirty;

		private GameObject background
		{
			get
			{
				if (instantiatedBackground == null)
				{
					instantiatedBackground = InstantiateAndParentise("Background");
				}
				return instantiatedBackground;
			}
		}

		private GameObject backgroundOverlay
		{
			get
			{
				if (instantiatedBackgroundOverlay == null)
				{
					instantiatedBackgroundOverlay = InstantiateAndParentise("BackgroundOverlay");
				}
				return instantiatedBackgroundOverlay;
			}
		}

		public GameObject blackBackgroundOverlayForAds
		{
			get
			{
				if (instantiatedBlackBackgroundOverlayForAds == null)
				{
					instantiatedBlackBackgroundOverlayForAds = InstantiateAndParentise("BlackBackgroundOverlayForAds");
				}
				return instantiatedBlackBackgroundOverlayForAds;
			}
		}

		public void ShowBlackBackgroundOverlayForAds(bool show)
		{
			blackBackgroundOverlayForAds.transform.SetAsLastSibling();
			blackBackgroundOverlayForAds.SetActive(show);
		}

		private GameObject InstantiateAndParentise(string prefabName)
		{
			GameObject original = Resources.Load<GameObject>("UI/" + prefabName);
			GameObject gameObject = Object.Instantiate(original);
			gameObject.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
			return gameObject;
		}

		public override void Init()
		{
			objectsDirty = true;
		}

		public void SetBackground(bool backgroundVisible, bool backgroundOverlayVisible)
		{
			this.backgroundVisible = backgroundVisible;
			this.backgroundOverlayVisible = backgroundOverlayVisible;
			objectsDirty = true;
		}

		private void Update()
		{
			if (objectsDirty)
			{
				objectsDirty = false;
				Refresh();
			}
		}

		private void Refresh()
		{
			if (backgroundOverlayVisible)
			{
				backgroundOverlay.SetActive(value: true);
				backgroundOverlay.transform.SetAsFirstSibling();
			}
			else
			{
				backgroundOverlay.SetActive(value: false);
			}
			if (backgroundVisible)
			{
				background.SetActive(value: true);
				background.transform.SetAsFirstSibling();
			}
			else
			{
				background.SetActive(value: false);
			}
		}
	}
}
