// DecompilerFi decompiler from Assembly-CSharp.dll class: ShowCutscene
using Common.Managers;
using Gameplay;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Initial Popups/Show Cutscene")]
public class ShowCutscene : InitialPopupExecution
{
	public string cutscene;

	public override void Show()
	{
		Manager.Get<SimpleCutsceneManager>().ShowCutscene(cutscene);
	}
}
[CreateAssetMenu(menuName = "Initial Popup Req/show Cutscene")]
public class ShowCutScene : InitialPopupExecution
{
	public int sceneId;

	public override void Show()
	{
		CutScenesManager cutScenesManager = Manager.Get<CutScenesManager>();
		if (!(cutScenesManager == null))
		{
			cutScenesManager.StartScene(sceneId);
		}
	}
}
