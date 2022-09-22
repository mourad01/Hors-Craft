// DecompilerFi decompiler from Assembly-CSharp.dll class: InteractiveAnimations.InteractiveAnimationsManager
using com.ootii.Cameras;
using Common.Managers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InteractiveAnimations
{
	public class InteractiveAnimationsManager : Manager
	{
		public STHumanModule saveTransformModule;

		public GameObject humanPrefab;

		public GameObject[] humanPrefabs;

		public List<SingleAnimation> randomPlayableAnimations = new List<SingleAnimation>();

		public List<WalkAnimation> walkingAnimations = new List<WalkAnimation>();

		public InteractiveAnimationsList interactiveAnimationsList;

		public List<SingleAnimation> availableSingleAnimations;

		public List<PredefinedListAnimation> availablePredefinedListAnimations;

		public GameObject selectionPrefab;

		private StaticAnimatedUnitSelection selectionTool;

		private bool selectionToolActive;

		public override void Init()
		{
			randomPlayableAnimations = (from item in randomPlayableAnimations
				where item != null
				select item).ToList();
			Manager.Get<SaveTransformsManager>().AddRuntimePrefab(humanPrefab);
			if (humanPrefabs != null)
			{
				for (int i = 0; i < humanPrefabs.Length; i++)
				{
					Manager.Get<SaveTransformsManager>().AddRuntimePrefab(humanPrefabs[i]);
				}
			}
		}

		public Animation GetRandomAnimation()
		{
			return randomPlayableAnimations.GetRandomItem();
		}

		public Animation GetGenderWalkingAnimation(Skin.Gender gender)
		{
			return walkingAnimations.First((WalkAnimation item) => item.gender == gender);
		}

		private void Update()
		{
		}

		private void TryToAddSelectionTool()
		{
			if (!CameraController.instance.GetComponent<StaticAnimatedUnitSelection>())
			{
				selectionToolActive = true;
				CameraController.instance.InputSource.IsEnabled = false;
				selectionTool = CameraController.instance.gameObject.AddComponent<StaticAnimatedUnitSelection>();
				selectionTool.SwitchMode(StaticAnimatedUnitSelection.AnimationsShowMode.Single);
				selectionTool.selectionPrefab = selectionPrefab;
			}
			else
			{
				selectionToolActive = false;
				CameraController.instance.InputSource.IsEnabled = true;
				UnityEngine.Object.Destroy(CameraController.instance.gameObject.GetComponent<StaticAnimatedUnitSelection>());
			}
		}

		private void OnGUI()
		{
		}
	}
}
