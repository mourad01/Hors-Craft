// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.TestsSuite.LogsTestsTab
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Common.Utils.TestsSuite
{
	public class LogsTestsTab : TestsSuiteState.TestsTab
	{
		private class Log
		{
			public string condition;

			public string stacktrace;

			public LogType logType;

			public bool expanded;
		}

		private const int LOGS_LIMIT = 250;

		private Dictionary<LogType, GUIStyle> styles;

		private static List<Log> logs = new List<Log>();

		private Vector2 scrollPos = Vector2.zero;

		[CompilerGenerated]
		private static Application.LogCallback _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Application.LogCallback _003C_003Ef__mg_0024cache1;

		public override string name => "Logs";

		public LogsTestsTab()
		{
			Application.logMessageReceived -= OnLog;
			Application.logMessageReceived += OnLog;
			PrepareStylesDictionary();
		}

		private void PrepareStylesDictionary()
		{
			styles = new Dictionary<LogType, GUIStyle>();
			GUI.skin.verticalScrollbar.fixedWidth = (float)Screen.width * 0.05f;
			int fontSize = (int)((float)Screen.height * 0.025f);
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.button.fontSize = fontSize;
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.button);
			gUIStyle.alignment = TextAnchor.MiddleLeft;
			gUIStyle.fontSize = fontSize;
			GUIStyle gUIStyle2 = new GUIStyle(GUI.skin.button);
			GUIStyleState normal = gUIStyle2.normal;
			Color yellow = Color.yellow;
			gUIStyle2.hover.textColor = yellow;
			yellow = yellow;
			gUIStyle2.active.textColor = yellow;
			normal.textColor = yellow;
			gUIStyle2.alignment = TextAnchor.MiddleLeft;
			gUIStyle2.fontSize = fontSize;
			GUIStyle gUIStyle3 = new GUIStyle(GUI.skin.button);
			GUIStyleState normal2 = gUIStyle3.normal;
			yellow = new Color(1f, 0.4f, 0.3f);
			gUIStyle3.hover.textColor = yellow;
			yellow = yellow;
			gUIStyle3.active.textColor = yellow;
			normal2.textColor = yellow;
			gUIStyle3.alignment = TextAnchor.MiddleLeft;
			gUIStyle3.fontSize = fontSize;
			styles.Add(LogType.Assert, gUIStyle3);
			styles.Add(LogType.Error, gUIStyle3);
			styles.Add(LogType.Exception, gUIStyle3);
			styles.Add(LogType.Log, gUIStyle);
			styles.Add(LogType.Warning, gUIStyle2);
		}

		private static void OnLog(string condition, string stacktrace, LogType logType)
		{
			logs.Add(new Log
			{
				condition = condition,
				stacktrace = stacktrace,
				logType = logType,
				expanded = false
			});
			if (logs.Count > 250)
			{
				logs.RemoveAt(0);
			}
		}

		public override void OnContentGUI()
		{
			GUILayout.Space(10f);
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(expand: true));
			foreach (Log log in logs)
			{
				string text = "[" + log.logType + "] " + log.condition;
				if (GUILayout.Button(text, styles[log.logType]))
				{
					log.expanded = !log.expanded;
				}
				if (log.expanded)
				{
					GUILayout.Label(log.stacktrace, GUI.skin.label);
				}
			}
			GUILayout.EndScrollView();
			GUILayout.Space(10f);
		}
	}
}
