// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.TutorialController
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DragMinigame
{
	public class TutorialController : MonoBehaviour
	{
		[Serializable]
		public class TutorialStep
		{
			public string key;

			[Multiline]
			public string defaultText;

			public RectTransform anchor;

			public bool flipX;

			public GameObject buttonObj;

			public bool wasShown;
		}

		[HideInInspector]
		public List<GameObject> activatedButtons = new List<GameObject>();

		[HideInInspector]
		public GameObject currentActiveButton;

		[SerializeField]
		private RectTransform tutorialInfo;

		[SerializeField]
		private TutorialStep[] tutorialSteps;

		private DragGameManager gameManager;

		private int currentTutorialStepIndex;

		private void Awake()
		{
			gameManager = GetComponent<DragGameManager>();
		}

		public TutorialStep GetNextStep()
		{
			if (currentTutorialStepIndex >= tutorialSteps.Length)
			{
				return null;
			}
			TutorialStep tutorialStep = tutorialSteps[currentTutorialStepIndex];
			if (tutorialStep.wasShown)
			{
				return null;
			}
			tutorialStep.wasShown = true;
			gameManager.SetTutorialUI(tutorialStep);
			activatedButtons.Add(tutorialStep.buttonObj);
			currentActiveButton = tutorialStep.buttonObj;
			currentTutorialStepIndex = Mathf.Clamp(currentTutorialStepIndex, 0, tutorialSteps.Length - 1);
			return tutorialStep;
		}

		public void HideTutorial()
		{
			currentTutorialStepIndex++;
			currentActiveButton = null;
		}

		public bool CheckIfCanShowNextStep()
		{
			return !tutorialSteps[currentTutorialStepIndex].wasShown;
		}
	}
}
