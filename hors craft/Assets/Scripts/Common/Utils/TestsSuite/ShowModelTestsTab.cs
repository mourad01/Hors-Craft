// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.TestsSuite.ShowModelTestsTab
using Common.Managers;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Common.Utils.TestsSuite
{
	public class ShowModelTestsTab : TestsSuiteState.TestsTab
	{
		private Vector2 scrollPos = Vector2.zero;

		private string forbidden = string.Empty;

		private string allowed = string.Empty;

		private string prevForbidden = string.Empty;

		private string prevAllowed = string.Empty;

		private string prevString = string.Empty;

		private float prevTime = -1f;

		private bool refresh = true;

		public override string name => "Model";

		public override void OnContentGUI()
		{
			GUILayout.Space(10f);
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(expand: true));
			DrawFilters();
			DrawSettings();
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
		}

		private void DrawFilters()
		{
			DrawTextFiled(ref forbidden, "Forbidden");
			DrawTextFiled(ref allowed, "Allowed");
		}

		private void DrawTextFiled(ref string enterString, string text)
		{
			GUILayout.BeginHorizontal(GUILayout.Height(100f));
			GUILayout.Label(text, GUILayout.Height(100f), GUILayout.Width(150f));
			enterString = GUILayout.TextField(enterString, GUILayout.Height(100f));
			GUILayout.EndHorizontal();
		}

		private void DrawSettings()
		{
			AbstractModelManager abstractModelManager = null;
			if (Manager.Contains<AbstractModelManager>())
			{
				abstractModelManager = Manager.Get<AbstractModelManager>();
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				if (!prevAllowed.Equals(allowed) || !prevForbidden.Equals(forbidden))
				{
					prevTime = Time.unscaledTime + 1.5f;
					prevAllowed = allowed;
					prevForbidden = forbidden;
				}
				if (prevTime != -1f && Time.unscaledTime - prevTime > 0f)
				{
					prevTime = -1f;
					refresh = true;
				}
				if (refresh)
				{
					prevString = abstractModelManager.ToString(GetForbiddens(), GetAllowed());
					refresh = false;
				}
				GUILayout.Label(prevString);
				stopwatch.Stop();
			}
		}

		private string[] GetForbiddens()
		{
			return SplitStringBy(forbidden, ';');
		}

		private string[] GetAllowed()
		{
			return SplitStringBy(allowed, ';');
		}

		private string[] SplitStringBy(string toSplit, char splitChar)
		{
			string[] array = new string((from c in toSplit
				where !char.IsWhiteSpace(c)
				select c).ToArray()).Split(splitChar);
			if (array == null)
			{
				return null;
			}
			if (array.Length == 1 && array[0] == string.Empty)
			{
				return null;
			}
			if (array[array.Length - 1].IsNullOrEmpty())
			{
				array[array.Length - 1] = "Blafoihoaf1284ulkndsgno";
			}
			return array;
		}
	}
}
