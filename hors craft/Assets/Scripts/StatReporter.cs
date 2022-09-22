// DecompilerFi decompiler from Assembly-CSharp.dll class: StatReporter
using Common.Managers;

public abstract class StatReporter
{
	public abstract StatsManager.Stat GetStat();

	public abstract bool UpdateAndCheck();
}
