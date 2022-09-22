// DecompilerFi decompiler from Assembly-CSharp.dll class: DayTimeChanger
using System;
using UnityEngine;

public class DayTimeChanger : MonoBehaviour, ISurvivalContextListener
{
	private DayTimeContext _context;

	public float shortenTheDayAmountPerDay = 0.05f;

	public int daysToMaxProgression = 5;

	private int lastDayChange;

	private bool wasInit;

	private float initialDayStartTime;

	private float initialDayComingTime;

	private float initialNightStartTime;

	private float initialNightComingTime;

	protected DayTimeContext context => _context ?? (_context = SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>());

	public void OnContextsUpdated()
	{
		SunController instance = SunController.instance;
		if (context != null && instance != null && context.day > lastDayChange)
		{
			lastDayChange = context.day;
			InitSun(instance);
			CalculateDayNightTimes(instance, Mathf.Clamp(lastDayChange, 0, daysToMaxProgression));
		}
	}

	private void InitSun(SunController sun)
	{
		if (!wasInit)
		{
			initialDayStartTime = sun.dayStartTime;
			initialDayComingTime = sun.dayComingTime;
			initialNightStartTime = sun.nightStartTime;
			initialNightComingTime = sun.nightComingTime;
			wasInit = true;
		}
	}

	private void CalculateDayNightTimes(SunController sun, int day)
	{
		sun.dayStartTime = AddAndNormalize(initialDayStartTime, (float)day * shortenTheDayAmountPerDay * 0.5f);
		sun.dayComingTime = AddAndNormalize(initialDayComingTime, (float)day * shortenTheDayAmountPerDay * 0.5f);
		sun.nightStartTime = AddAndNormalize(initialNightStartTime, (float)(-day) * shortenTheDayAmountPerDay * 0.5f);
		sun.nightComingTime = AddAndNormalize(initialNightComingTime, (float)(-day) * shortenTheDayAmountPerDay * 0.5f);
	}

	private float AddAndNormalize(float value, float addition)
	{
		return Mod(value + addition, 1f);
	}

	private float Mod(float x, float m)
	{
		float num = x % m;
		return (!(num < 0f)) ? num : (num + m);
	}

	public bool AddBySurvivalManager()
	{
		return true;
	}

	public Type[] ContextTypes()
	{
		return new Type[1]
		{
			typeof(DayTimeContext)
		};
	}

	Action[] ISurvivalContextListener.OnContextsUpdated()
	{
		return new Action[1]
		{
			OnContextsUpdated
		};
	}
}
