// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.DeveloperModeBehaviour
using Common.Managers;
using Common.Utils.TestsSuite;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Behaviours
{
	public class DeveloperModeBehaviour : MonoBehaviourSingleton<DeveloperModeBehaviour>
	{
		public delegate void OnDeveloperModeActivated();

		private List<OnDeveloperModeActivated> onActivatedCallbacks = new List<OnDeveloperModeActivated>();

		private bool[] leftRightTapCheatSequence = new bool[18]
		{
			false,
			false,
			false,
			false,
			false,
			true,
			true,
			true,
			true,
			true,
			false,
			false,
			false,
			true,
			true,
			true,
			false,
			true
		};

		private int currentCorrectSequenceIndex = -1;

		public bool isDeveloper
		{
			get;
			private set;
		}

		public void AddOnActivatedCallback(OnDeveloperModeActivated onActivated)
		{
			if (!onActivatedCallbacks.Contains(onActivated))
			{
				onActivatedCallbacks.Add(onActivated);
			}
			if (isDeveloper)
			{
				TriggerCallbacks();
			}
		}

		private void Start()
		{
			if (Debug.isDebugBuild)
			{
				ShowSuite();
			}
		}

		private void Update()
		{
			UpdateTouches();
		}

		private void UpdateTouches()
		{
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mousePosition = UnityEngine.Input.mousePosition;
				bool flag = mousePosition.x > (float)(Screen.width / 2);
				if (flag == leftRightTapCheatSequence[currentCorrectSequenceIndex + 1])
				{
					currentCorrectSequenceIndex++;
				}
				else if (flag == leftRightTapCheatSequence[0])
				{
					currentCorrectSequenceIndex = 0;
				}
				else
				{
					currentCorrectSequenceIndex = -1;
				}
				if (currentCorrectSequenceIndex == leftRightTapCheatSequence.Length - 1)
				{
					ShowSuite();
					currentCorrectSequenceIndex = -1;
				}
			}
		}

		private void ShowSuite()
		{
			if (!Manager.Get<StateMachineManager>().IsCurrentStateA<TestsSuiteState>())
			{
				Manager.Get<StateMachineManager>().PushState<TestsSuiteState>();
			}
		}

		private void HideSuite()
		{
			if (Manager.Get<StateMachineManager>().IsCurrentStateA<TestsSuiteState>())
			{
				Manager.Get<StateMachineManager>().PopState();
			}
		}

		public void DeveloperOff()
		{
			isDeveloper = false;
			HideSuite();
		}

		public void DeveloperOn()
		{
			isDeveloper = true;
			TriggerCallbacks();
		}

		private void TriggerCallbacks()
		{
			foreach (OnDeveloperModeActivated onActivatedCallback in onActivatedCallbacks)
			{
				onActivatedCallback();
			}
			ShowSuite();
			onActivatedCallbacks.Clear();
		}
	}
}
