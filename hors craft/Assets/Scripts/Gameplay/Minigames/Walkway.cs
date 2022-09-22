// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Minigames.Walkway
using Common.Managers;
using States;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Minigames
{
	public class Walkway : InteractiveObject
	{
		public Transform walkWayObject;

		public Transform walkwayStartPoint;

		public Vector3 startOffset = new Vector3(0f, 0f, 0f);

		public RuntimeAnimatorController humanAnimatorOverride;

		public WalkwayAditionalObject[] aditionalObjects;

		public override void OnUse()
		{
			base.OnUse();
			StartMinigame();
		}

		private void StartMinigame()
		{
			Dictionary<string, WalkwayAditionalObject> dictionary = new Dictionary<string, WalkwayAditionalObject>();
			for (int i = 0; i < aditionalObjects.Length; i++)
			{
				dictionary.Add(aditionalObjects[i].objectNameId, aditionalObjects[i]);
			}
			Manager.Get<StateMachineManager>().PushState<RhythmicMinigameState>(new RhythmicMinigameStateStartParameter
			{
				graphicScene = new WalkwayScene(PlayerGraphic.GetControlledPlayerInstance(), base.transform, walkwayStartPoint, startOffset, humanAnimatorOverride, dictionary)
			});
		}
	}
}
