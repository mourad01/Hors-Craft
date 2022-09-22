// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Minigames.SpinableMachine
using Common.Managers;
using SpinningMachine;
using States;
using System;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Minigames
{
	public class SpinableMachine : InteractiveObject
	{
		[SerializeField]
		private SpinableMachineController machine;

		[SerializeField]
		private GameObject playerPosition;

		private int multiplier = 1;

		protected bool isInUse;

		private bool isSpinning;

		private ReturnButtonContext returnContext;

		protected GameplayState gameplay => Manager.Get<StateMachineManager>().GetStateInstance(typeof(GameplayState)) as GameplayState;

		private PlayerMovement playerMovement => PlayerGraphic.GetControlledPlayerInstance().GetComponentInParent<PlayerMovement>();

		protected override void Awake()
		{
			base.Awake();
			if (machine == null)
			{
				machine = GetComponent<SpinableMachineController>();
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			if (!isInUse)
			{
				StartUsing();
				returnContext = new ReturnButtonContext
				{
					onReturnButton = OnQuit
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.RETURN_BUTTON_ACTIVATED, returnContext);
				gameplay.SetSubstate(GameplayState.Substates.MINIGAME);
			}
			else if (!isSpinning)
			{
				TicketsManager.TakeEntranceFeeIfPossible(Spin);
			}
		}

		public void OnQuit()
		{
			playerMovement.EndCustomCutscene();
			gameplay.ShowUI();
			isInUse = false;
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.RETURN_BUTTON_ACTIVATED, returnContext);
			returnContext = null;
			gameplay.SetSubstate(GameplayState.Substates.WALKING);
		}

		private void StartUsing()
		{
			gameplay.HideUI(useAction: true);
			playerMovement.StartInteractiveCutscene(playerPosition.transform);
			if (machine.onSpinDone == null)
			{
				SpinableMachineController spinableMachineController = machine;
				spinableMachineController.onSpinDone = (Action<bool, RewardType, int, int>)Delegate.Combine(spinableMachineController.onSpinDone, new Action<bool, RewardType, int, int>(OnSpinDone));
			}
			isInUse = true;
			isSpinning = false;
			multiplier = 1;
		}

		private void Spin()
		{
			if (!isSpinning)
			{
				isSpinning = true;
				machine.Spin();
			}
		}

		private void OnSpinDone(bool isWin, RewardType rewardType, int winId, int winCount)
		{
			isSpinning = false;
			if (isWin)
			{
				switch (rewardType)
				{
				case RewardType.Resource:
					SpawnResources(winCount * multiplier, winId);
					multiplier = 1;
					Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("slotmachine.win", "You've won!"));
					break;
				case RewardType.Multiplier:
					multiplier *= winId;
					Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("slotmachine.multiplier", "Nice! Spin again for a higher reward!"));
					break;
				}
			}
			else
			{
				multiplier = 1;
			}
			MonoBehaviourSingleton<ProgressCounter>.get.Increment(ProgressCounter.Countables.MINIGAMES_PLAYED);
			UnityEngine.Debug.Log($"Do you win?: {isWin}; Your reward type is: {rewardType}; Reward id is: {winId}");
		}

		protected virtual void SpawnResources(int winCount, int winId)
		{
			for (int i = 0; i < winCount; i++)
			{
				SpawnResource(winId);
			}
		}

		private void SpawnResource(int winId)
		{
			Manager.Get<CraftingManager>().SpawnResource(base.transform.position + -base.transform.right, winId);
		}
	}
}
