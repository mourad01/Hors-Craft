// DecompilerFi decompiler from Assembly-CSharp.dll class: DayTimeContext
using Gameplay;

public class DayTimeContext : SurvivalContext
{
	public float time;

	public int day;

	public float fullPassedTime;

	public GlobalSettings.TimeOfDay dayTime;

	public override string ToString()
	{
		return $"Time: {time}; Day: {day}; Full passed time: {fullPassedTime}; DatTime: {dayTime}";
	}
}
