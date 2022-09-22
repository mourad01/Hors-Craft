// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ChangePetState
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using UnityEngine;

namespace States
{
	public class ChangePetState : XCraftUIState<ChangePetStateConnector>
	{
		private GameObject changePetFragment;

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			base.connector.onApplyButtonClicked = OnApply;
			changePetFragment = Object.Instantiate(base.connector.changePetPrefab, base.connector.changePetContainer.transform);
			changePetFragment.transform.SetAsFirstSibling();
			changePetFragment.GetComponent<CustomizationFragment>().clothesTabEnabled = false;
			changePetFragment.GetComponent<Fragment>().Init(null);
			AddMoveIfBannerEnabled(changePetFragment.GetComponent<CustomizationFragment>().objectsToMoveIfBanner.ToArray());
		}

		private void OnApply()
		{
			StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.PETS_PLAY_CLICKED);
			if (Manager.Get<StateMachineManager>().ContainsState(typeof(ChooseFactionState)))
			{
				string name = Manager.Get<SavedWorldManager>().GetCurrentWorld().name;
				if (!PlayerPrefs.HasKey(name + ".faction"))
				{
					Manager.Get<StateMachineManager>().SetState<ChooseFactionState>();
				}
				else
				{
					Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
				}
			}
			else
			{
				Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			changePetFragment.GetComponent<Fragment>().UpdateFragment();
		}

		public override void FinishState()
		{
			changePetFragment.GetComponent<Fragment>().Destroy();
			base.FinishState();
		}

		private void AddMoveIfBannerEnabled(GameObject[] gameObjects)
		{
			for (int i = 0; i < gameObjects.Length; i++)
			{
				//gameObjects[i].AddComponent<MoveIfBannerEnabled>().ChangeMode(MoveIfBannerEnabled.Mode.FLATTEN);
			}
		}
	}
}
