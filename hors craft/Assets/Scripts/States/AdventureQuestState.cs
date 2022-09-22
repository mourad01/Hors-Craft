// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AdventureQuestState
using Common.Managers;
using Common.Managers.States;
using Common.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace States
{
	public class AdventureQuestState : XCraftUIState<AdventureQuestStateConnector>
	{
		public Coroutine cor;

		public const int WAIT_TIME = 7;

		public override void StartState(StartParameter startParameter)
		{
			base.StartState(startParameter);
			base.connector.CreateBlurrBackground();
			base.connector.SetBigButtonVisibility(state: false);
			base.connector.SetCharacterImage((startParameter as AdventureQuestStateParameter).characterTexture);
			base.connector.SetButtonsAction((startParameter as AdventureQuestStateParameter).onUserClicked);
			base.connector.SetOptionsButtonVisibility(-1, newState: true);
			if (!string.IsNullOrEmpty((startParameter as AdventureQuestStateParameter).data.MainText))
			{
				base.connector.SetMainText((startParameter as AdventureQuestStateParameter).data);
				base.connector.SetBigButtonVisibility(state: true);
				cor = StartCoroutine(Wait(7f, delegate
				{
					if (!(base.connector == null) && base.connector.IsBigButtonActive && (startParameter as AdventureQuestStateParameter).onUserClicked != null)
					{
						(startParameter as AdventureQuestStateParameter).onUserClicked(-1);
					}
				}));
			}
			else if ((startParameter as AdventureQuestStateParameter).data.OptionsLength > 0)
			{
				base.connector.SetOptionsButtonVisibility(-1, newState: true);
				base.connector.SetOptions((startParameter as AdventureQuestStateParameter).data);
				base.connector.SetButtonsAction((startParameter as AdventureQuestStateParameter).onUserClicked);
			}
			TimeScaleHelper.value = 1f;
		}

		internal void ChoosedAnswer(int index)
		{
			base.connector.SetOptionsButtonVisibility(index, newState: true);
			base.connector.SetMainText(new AdventureScreenData());
		}

		private IEnumerator Wait(float time, Action onEnd)
		{
			while (time > 0f)
			{
				time -= Time.unscaledDeltaTime;
				yield return null;
			}
			onEnd?.Invoke();
		}

		public void UpdateContent(AdventureScreenData newData, Sprite newSprite)
		{
			base.connector.SetCharacterImage(newSprite);
			if (!string.IsNullOrEmpty(newData.MainText))
			{
				base.connector.SetMainText(newData);
				base.connector.SetOptionsButtonVisibility(-1, newState: true);
				base.connector.SetBigButtonVisibility(state: true);
				cor = StartCoroutine(Wait(7f, delegate
				{
					if (!(base.connector == null) && base.connector.IsBigButtonActive)
					{
						base.connector.FireBigButton();
					}
				}));
			}
			else if (newData.OptionsLength > 0)
			{
				base.connector.SetOptionsButtonVisibility(-1, newState: false);
				base.connector.SetBigButtonVisibility(state: false);
				base.connector.SetOptions(newData);
			}
		}

		public override void FinishState()
		{
			base.FinishState();
			GameplayState gameplayState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(GameplayState)) as GameplayState;
			if (gameplayState != null)
			{
				gameplayState.ShowUI();
			}
		}
	}
}
