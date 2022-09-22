// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TutorialState
using Common.Managers;
using Common.Managers.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
	public class TutorialState : XCraftUIState<TutorialStateConnector>
	{
		public List<GameObject> tutorialObjects;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			base.StartState(parameter);
			GameObject original = (parameter == null || !((TutorialStartParameter)parameter).prefabToSpawn.IsNOTNullOrEmpty()) ? tutorialObjects[0] : tutorialObjects.First((GameObject p) => p.name == ((TutorialStartParameter)parameter).prefabToSpawn);
			Object.Instantiate(original, base.connector.transform, worldPositionStays: false);
			base.connector.Init();
			base.connector.onPlayClicked = OnPlay;
		}

		private void OnPlay()
		{
			Manager.Get<StateMachineManager>().PopState();
		}
	}
}
