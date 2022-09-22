// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.DeveloperWarning
using Common.Managers;
using UnityEngine;

namespace Common.Behaviours
{
	public class DeveloperWarning : MonoBehaviourSingleton<DeveloperWarning>
	{
		private GUIStyle _redBoxStyle;

		private bool duplicatedManagersContainer;

		private GUIStyle redBoxStyle
		{
			get
			{
				if (_redBoxStyle == null)
				{
					_redBoxStyle = new GUIStyle(GUI.skin.box);
					Texture2D texture2D = new Texture2D(1, 1);
					texture2D.SetPixel(0, 0, new Color(1f, 0f, 0f, 0.5f));
					texture2D.Apply();
					_redBoxStyle.normal.background = texture2D;
					_redBoxStyle.fontSize = 20;
					_redBoxStyle.fontStyle = FontStyle.Bold;
					_redBoxStyle.normal.textColor = Color.white;
				}
				return _redBoxStyle;
			}
		}

		public void DuplicatedManagersContainer()
		{
			duplicatedManagersContainer = true;
		}

		private void OnGUI()
		{
			if (Application.isEditor)
			{
				return;
			}
			bool flag = Manager.Get<ConnectionInfoManager>().homeURL.Contains("devs");
			bool isDebugBuild = UnityEngine.Debug.isDebugBuild;
			if (flag || isDebugBuild || duplicatedManagersContainer)
			{
				string str = string.Empty;
				if (flag)
				{
					str = "build uses 'devs' server";
				}
				if (isDebugBuild)
				{
					str = "development build";
				}
				if (duplicatedManagersContainer)
				{
					str = "more than one ManagersContainer (wrong!)";
				}
				Rect position = new Rect(0f, 0f, Screen.width, Screen.height / 10);
				position.y = (float)Screen.height - position.height;
				GUI.Box(position, "NON-PRODUCTION INSTANCE!\n(reason: " + str + ")", redBoxStyle);
			}
		}
	}
}
