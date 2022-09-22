// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ChangeClothesState
using Common.Managers;
using Common.Managers.States;
using Gameplay;
using UnityEngine;

namespace States
{
	public class ChangeClothesState : XCraftUIState<ChangeClothesStateConnector>
	{
		public bool showChoosePetsAfter = true;

		private GameObject changeClothesFragment;

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			base.connector.onApplyButtonClicked = OnApply;
			changeClothesFragment = Object.Instantiate(base.connector.changeClothesPrefab, base.connector.changeClothesContainer.transform);
			changeClothesFragment.transform.SetAsFirstSibling();
			changeClothesFragment.GetComponent<CustomizationFragment>().petsTabEnabled = false;
			changeClothesFragment.GetComponent<Fragment>().Init(null);
			AddMoveIfBannerEnabled(changeClothesFragment.GetComponent<CustomizationFragment>().objectsToMoveIfBanner.ToArray());
		}

		private void OnApply()
		{
			bool flag = true;
			StartupFunnelStatsReporter.Instance.RaiseFunnelEvent(StartupFunnelActionType.CLOTHES_PLAY_CLICKED);
			if (Manager.Get<ModelManager>().petSetting.GetPetsEnabled() && flag && showChoosePetsAfter && Manager.Contains<PetManager>())
			{
				Manager.Get<StateMachineManager>().PopState();
				Manager.Get<StateMachineManager>().PushState<ChangePetState>();
			}
			else
			{
				Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
			}
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			changeClothesFragment.GetComponent<Fragment>().UpdateFragment();
		}

		public override void FinishState()
		{
			changeClothesFragment.GetComponent<Fragment>().Destroy();
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
