// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlocksPopupState
using Common.Managers;
using Common.Managers.States;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace States
{
	public class BlocksPopupState : XCraftUIState<BlocksPopupStateConnector>
	{
		public GameObject blocksFragment;

		public GameObject blocksNewFragment;

		[HideInInspector]
		public BlocksFragment blocks;

		protected override AutoAdsConfig autoAdsConfig
		{
			[CompilerGenerated]
			get
			{
				return (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_TUTORIAL)) ? base.autoAdsConfig : new AutoAdsConfig
				{
					autoShowOnStart = false
				};
			}
		}

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			GameObject original = (!Singleton<BlocksController>.get.enableRarityBlocks) ? blocksFragment : blocksNewFragment;
			blocks = Object.Instantiate(original, base.connector.parent, worldPositionStays: false).GetComponent<BlocksFragment>();
			PauseState stateInstance = Manager.Get<StateMachineManager>().GetStateInstance<PauseState>();
			blocks.Init(new FragmentStartParameter
			{
				pauseState = stateInstance
			});
			base.connector.onReturnButton = OnReturn;
			base.connector.AdjustWindow();
			blocks.UpdateFragment();
		}

		public override void UnfreezeState()
		{
			base.UnfreezeState();
			blocks.UpdateFragment();
		}

		private void OnReturn()
		{
			Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		}

		public static void Show()
		{
			Manager.Get<StateMachineManager>().PushState<BlocksPopupState>();
		}
	}
}
