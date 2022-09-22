// DecompilerFi decompiler from Assembly-CSharp.dll class: States.UniversalPopupState
using Common.Managers.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace States
{
	public class UniversalPopupState : XCraftUIState<UniversalPopupStateConnector>
	{
		public List<GameObject> availablePopups;

		protected override bool hasBackground => false;

		protected override bool hasBackgroundOverlay => true;

		public override void StartState(StartParameter parameter)
		{
			UniversalPopupStateStartParameter startParameter = parameter as UniversalPopupStateStartParameter;
			base.StartState(parameter);
			GameObject original = (parameter == null || !((UniversalPopupStateStartParameter)parameter).prefabToSpawn.IsNOTNullOrEmpty()) ? availablePopups[0] : availablePopups.First((GameObject p) => p.name == startParameter.prefabToSpawn);
			Object.Instantiate(original, base.connector.transform, worldPositionStays: false);
			base.connector.Init();
			if (startParameter.configPopup != null)
			{
				startParameter.configPopup(base.connector.popup);
			}
		}
	}
}
