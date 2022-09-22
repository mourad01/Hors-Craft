// DecompilerFi decompiler from Assembly-CSharp.dll class: SwitchMoveModeExe
using Gameplay;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Switch move mode")]
public class SwitchMoveModeExe : InitialPopupExecution
{
	public override void Show()
	{
		GlobalSettings.mode = GlobalSettings.MovingMode.FLYING;
	}
}
