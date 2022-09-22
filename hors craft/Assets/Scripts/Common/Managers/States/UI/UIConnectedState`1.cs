// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.States.UI.UIConnectedState`1
using System;
using System.Collections;
using UnityEngine;

namespace Common.Managers.States.UI
{
	public abstract class UIConnectedState<T> : State where T : UIConnector
	{
		[SerializeField]
		protected GameObject uiPrefab;

		[SerializeField]
		protected string uiPrefabResource;

		protected GameObject instantiatedPrefab;

		private T instantiatedConnector;

		protected bool pushedPolicyPopup;

		protected T connector => instantiatedConnector;

		protected virtual bool showsPolicyPopup => true;

		protected virtual bool hideUiOnPause => true;

		public override void StartState(StartParameter startParameter)
		{
			instantiatedPrefab = CreateConnector();
			instantiatedConnector = instantiatedPrefab.GetComponentInChildren<T>();
			if ((UnityEngine.Object)instantiatedConnector == (UnityEngine.Object)null)
			{
				throw new Exception("No " + typeof(T) + " connector found in " + GetType() + " UIState!");
			}
			InitConnector();
			pushedPolicyPopup = false;
		}

		protected virtual GameObject CreateConnector()
		{
			if (uiPrefab == null)
			{
				LoadStateFromResources();
			}
			if (uiPrefab == null)
			{
				throw new Exception("No uiPrefab provided for UIState " + GetType() + "!");
			}
			return UnityEngine.Object.Instantiate(uiPrefab);
		}

		public void LoadStateFromResources()
		{
			uiPrefab = Resources.Load<GameObject>(uiPrefabResource);
		}

		protected virtual void InitConnector()
		{
			instantiatedPrefab.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
			instantiatedPrefab.transform.SetSiblingIndex(Manager.Get<CanvasManager>().canvas.transform.childCount - 2);
		}

		protected bool CheckPolicy(Action onFinish)
		{
			if (showsPolicyPopup && !pushedPolicyPopup && Manager.Get<AbstractModelManager>().policyPopupSettings.ShouldShowInState(this) && Manager.Get<StateMachineManager>().ContainsState(typeof(PolicyPopupState)))
			{
				Manager.Get<StateMachineManager>().PushState<PolicyPopupState>(PolicyPopupStateStartParameter.DefaultStartState(Manager.Get<AbstractModelManager>().policyPopupSettings.GetSettingsForState(this), onFinish));
				pushedPolicyPopup = true;
				return true;
			}
			return false;
		}

		public override void UpdateState()
		{
		}

		public override void FreezeState()
		{
			if (hideUiOnPause && instantiatedPrefab != null)
			{
				instantiatedPrefab.SetActive(value: false);
			}
		}

		public override void UnfreezeState()
		{
			if (instantiatedPrefab != null)
			{
				instantiatedPrefab.SetActive(value: true);
			}
		}

		public override void FinishState()
		{
			if (!string.IsNullOrEmpty(uiPrefabResource))
			{
				uiPrefab = null;
			}
			instantiatedConnector = (T)null;
			UnityEngine.Object.Destroy(instantiatedPrefab);
			instantiatedPrefab = null;
			if (Manager.Get<StateMachineManager>().unloadUnusedAssets)
			{
				StartCoroutine(WaitAndUnloadAssets(0.2f));
			}
		}

		private IEnumerator WaitAndUnloadAssets(float time)
		{
			yield return new WaitForSecondsRealtime(time);
			Resources.UnloadUnusedAssets();
		}

		public override void PrepareState()
		{
		}
	}
}
