// DecompilerFi decompiler from Assembly-CSharp.dll class: InitialShowFragment
using Common.Managers;
using States;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Initial Show Fragment")]
public class InitialShowFragment : InitialPopupExecution
{
	public int categoryToOpen;

	public override void Show()
	{
		Manager.Get<StateMachineManager>().PushState<DressupState>();
	}
}
