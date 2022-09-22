// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenesStateConnector
using Common.Managers.States.UI;

public class CutScenesStateConnector : UIConnector
{
	public void ManualStartHandler()
	{
		AnimatorHandler[] components = base.gameObject.GetComponents<AnimatorHandler>();
		foreach (AnimatorHandler animatorHandler in components)
		{
			animatorHandler.Awake();
		}
	}
}
