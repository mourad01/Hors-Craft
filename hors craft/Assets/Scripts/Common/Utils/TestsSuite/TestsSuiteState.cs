// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.TestsSuite.TestsSuiteState
using Common.Behaviours;
using Common.Managers;
using Common.Managers.States;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils.TestsSuite
{
	public class TestsSuiteState : State
	{
		public abstract class TestsTab
		{
			public abstract string name
			{
				get;
			}

			public abstract void OnContentGUI();
		}

		private const float MARGIN = 10f;

		private float rememberedTimeScale;

		private List<TestsTab> tabs;

		private TestsTab choosenTab;

		private Rect windowRect;

		private Vector2 scroll;

		public override void StartState(StartParameter startParameter)
		{
			rememberedTimeScale = TimeScaleHelper.value;
			TimeScaleHelper.value = 0f;
			PrepareTabs();
		}

		public override void UpdateState()
		{
			DrawAll();
		}

		public override void FreezeState()
		{
		}

		public override void UnfreezeState()
		{
		}

		public override void FinishState()
		{
			TimeScaleHelper.value = rememberedTimeScale;
		}

		private void Return()
		{
			Manager.Get<StateMachineManager>().PopState();
		}

		private void PrepareTabs()
		{
			choosenTab = null;
			tabs = new List<TestsTab>();
			choosenTab = new LogsTestsTab();
			tabs.Add(choosenTab);
			tabs.Add(new ShowModelTestsTab());
			//tabs.Add(new WaterfallTestsTab());
			tabs.Add(new TapjoyTestsTab());
		}

		private void DrawAll()
		{
			windowRect = new Rect(10f, 10f, (float)Screen.width - 20f, (float)Screen.height - 20f);
			GUI.Box(windowRect, string.Empty);
			Rect screenRect = new Rect(windowRect);
			screenRect.position += Vector2.one * 10f;
			screenRect.size -= 2f * Vector2.one * 10f;
			GUILayout.BeginArea(screenRect, GUI.skin.box);
			DrawWindow();
			GUILayout.EndArea();
		}

		private void DrawWindow()
		{
			DrawReturnButton();
			GUILayout.Space(10f);
			DrawTabs();
			GUILayout.Space(10f);
			DrawTabContent();
			GUILayout.Space(10f);
		}

		private void DrawReturnButton()
		{
			float height = (float)Screen.height * 0.2f;
			GUILayout.BeginHorizontal(GUILayout.Height(height));
			if (GUILayout.Button("RETURN", GUILayout.Height(height)))
			{
				Return();
			}
			string text = (!MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper) ? "Activate Developer Mode" : "Activated";
			if (GUILayout.Button(text, GUILayout.Height(height)))
			{
				Return();
				MonoBehaviourSingleton<DeveloperModeBehaviour>.get.DeveloperOn();
			}
			GUILayout.EndHorizontal();
		}

		private void DrawTabs()
		{
			GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.Height((float)Screen.height * 0.1f));
			foreach (TestsTab tab in tabs)
			{
				string text = (tab != choosenTab) ? tab.name.ToLower() : tab.name.ToUpper();
				if (GUILayout.Button(text, GUILayout.ExpandHeight(expand: true)))
				{
					choosenTab = tab;
				}
			}
			GUILayout.EndHorizontal();
		}

		private void DrawTabContent()
		{
			if (choosenTab != null)
			{
				scroll = GUILayout.BeginScrollView(scroll);
				choosenTab.OnContentGUI();
				GUILayout.EndScrollView();
			}
		}
	}
}
