// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.TestsSuite.TapjoyTestsTab
using Common.Managers;
using Common.Managers.States;
using TapjoyUnity.Internal;
using UnityEngine;

namespace Common.Utils.TestsSuite
{
	public class TapjoyTestsTab : TestsSuiteState.TestsTab
	{
		private string customContent = string.Empty;

		public override string name => "Tapjoy Ads";

		public override void OnContentGUI()
		{
			string text;
			try
			{
				State currentState = Manager.Get<StateMachineManager>().currentState;
				if (currentState == null)
				{
					GUILayout.Label("No state enabled.");
					return;
				}
				text = currentState.GetType().Name.ToTitleCase();
			}
			catch (UndefinedManagerException arg)
			{
				text = "Common";
				UnityEngine.Debug.Log("Exceptions tap joy: " + arg);
			}
			TapjoyComponent componentInChildren = Manager.Get<TapjoyManager>().GetComponentInChildren<TapjoyComponent>();
			GUILayout.Label("Android ID: " + componentInChildren.settings.AndroidSettings.SdkKey);
			GUILayout.Label("iOS ID: " + componentInChildren.settings.IosSettings.SdkKey);
			GUILayout.Space(20f);
			string text2 = "state";
			int num = text.IndexOf(text2);
			if (num >= 0)
			{
				text = text.Remove(num, text2.Length);
			}
			if (GUILayout.Button("Open Tapjoy Content:  " + text, GUILayout.ExpandHeight(expand: true)))
			{
				Show(text);
			}
			if (GUILayout.Button("Open Tapjoy Content:  " + text + "_nofloor", GUILayout.ExpandHeight(expand: true)))
			{
				Show(text + "_nofloor");
			}
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal(GUILayout.ExpandHeight(expand: true));
			GUILayout.Label("Custom Tapjoy Content", GUILayout.Height(50f));
			customContent = GUILayout.TextField(customContent, GUILayout.Height(50f));
			GUILayout.Space(10f);
			if (GUILayout.Button("Open It", GUILayout.Height(50f)))
			{
				Show(customContent);
			}
			GUILayout.EndHorizontal();
		}

		private void Show(string name)
		{
			Manager.Get<TapjoyManager>().DefinePlacement(name);
			UnityEngine.Debug.Log("SHOW PLACEMENT");
			Manager.Get<TapjoyManager>().ShowPlacement(name);
		}
	}
}
