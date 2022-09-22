// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.SimpleCutsceneManager
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class SimpleCutsceneManager : Manager
	{
		[Serializable]
		public class CutsceneInfo
		{
			public string name;

			public ScriptableCutscene cutscene;
		}

		public List<CutsceneInfo> cutscenes = new List<CutsceneInfo>();

		private CutsceneInfo currentCutscene;

		public Action doAfterCutscene;

		public override void Init()
		{
		}

		private void Update()
		{
			if (currentCutscene != null && currentCutscene.cutscene.EndCondition())
			{
				currentCutscene.cutscene.OnEnd();
				currentCutscene = null;
				if (doAfterCutscene != null)
				{
					doAfterCutscene();
					doAfterCutscene = null;
				}
			}
		}

		public void ShowCutscene(string name)
		{
			CutsceneInfo cutsceneInfo = cutscenes.FirstOrDefault((CutsceneInfo c) => c.name == name);
			if (cutsceneInfo == null)
			{
				UnityEngine.Debug.LogError("Can't play cutscene " + name + ". It's not added to cutscene manager");
				return;
			}
			PlayerPrefs.SetString(cutsceneInfo.name + ".watched", "true");
			cutsceneInfo.cutscene.Show();
			currentCutscene = cutsceneInfo;
		}
	}
}
