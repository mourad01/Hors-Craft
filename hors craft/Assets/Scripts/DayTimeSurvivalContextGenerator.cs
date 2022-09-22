// DecompilerFi decompiler from Assembly-CSharp.dll class: DayTimeSurvivalContextGenerator
using Common.Managers;
using Gameplay;
using UnityEngine;

public class DayTimeSurvivalContextGenerator : MonoBehaviour, IGameCallbacksListener
{
	private const string GENERATOR_KEY = "DayTimeContextGenerator";

	public float updateContextTime = 0.2f;

	private DayTimeContext context;

	private SunController _sunController;

	private float toUpdate;

	private SunController sunController
	{
		get
		{
			if (_sunController == null)
			{
				_sunController = SunController.instance;
			}
			return _sunController;
		}
	}

	private void Start()
	{
		InitContext();
		SurvivalContextsBroadcaster.instance.UpdateContext(context);
		toUpdate = updateContextTime;
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	private void Update()
	{
		toUpdate -= Time.deltaTime;
		if (toUpdate <= 0f)
		{
			toUpdate = updateContextTime;
			if (context.time > sunController.currentTimeOfDay)
			{
				context.day++;
			}
			context.time = sunController.currentTimeOfDay;
			context.fullPassedTime = (float)context.day + context.time;
			context.dayTime = sunController.GetTimeOfDayEnum();
			SurvivalContextsBroadcaster.instance.UpdateContext(context);
		}
	}

	private void InitContext()
	{
		if (context != null)
		{
			float @float = PlayerPrefs.GetFloat("global.survivedTime", 0f);
			int num = (int)@float;
			context.time = @float - (float)num;
			context.day = num;
			context.fullPassedTime = @float;
			context.dayTime = sunController.GetTimeOfDayEnum();
		}
		else if (PlayerPrefs.HasKey("DayTimeContextGenerator"))
		{
			float float2 = PlayerPrefs.GetFloat("global.survivedTime", 0f);
			int num2 = (int)float2;
			context = new DayTimeContext
			{
				time = float2 - (float)num2,
				day = num2,
				fullPassedTime = float2,
				dayTime = sunController.GetTimeOfDayEnum()
			};
		}
		else
		{
			context = new DayTimeContext
			{
				time = sunController.currentTimeOfDay,
				day = 0,
				fullPassedTime = sunController.currentTimeOfDay,
				dayTime = sunController.GetTimeOfDayEnum()
			};
		}
	}

	private void Restart()
	{
		if (context != null)
		{
			context.time = sunController.currentTimeOfDay;
			context.day = 0;
			context.fullPassedTime = sunController.currentTimeOfDay;
			context.dayTime = sunController.GetTimeOfDayEnum();
		}
		else
		{
			context = new DayTimeContext
			{
				time = sunController.currentTimeOfDay,
				day = 0,
				fullPassedTime = sunController.currentTimeOfDay,
				dayTime = sunController.GetTimeOfDayEnum()
			};
		}
	}

	private void SaveContext()
	{
		if (context != null)
		{
			PlayerPrefs.SetInt("DayTimeContextGenerator", 1);
			PlayerPrefs.SetFloat("global.survivedTime", context.fullPassedTime - 1f);
		}
	}

	public void OnGameSavedFrequent()
	{
		SaveContext();
	}

	public void OnGameSavedInfrequent()
	{
	}

	public void OnGameplayStarted()
	{
		InitContext();
	}

	public void OnGameplayRestarted()
	{
		Restart();
	}
}
