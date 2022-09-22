// DecompilerFi decompiler from Assembly-CSharp.dll class: ShouldSaveParameter
using Common.Managers.States;

public class ShouldSaveParameter : StartParameter
{
	public bool shouldSaveWorld;

	public ShouldSaveParameter(bool save)
	{
		shouldSaveWorld = save;
	}
}
