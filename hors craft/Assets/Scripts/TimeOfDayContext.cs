// DecompilerFi decompiler from Assembly-CSharp.dll class: TimeOfDayContext
public class TimeOfDayContext : FactContext
{
	public bool isNight;

	public float time;

	public override bool isPersistent => true;

	public override string GetContent()
	{
		return base.GetContent() + "isNight? " + isNight + ", time: " + time;
	}
}
