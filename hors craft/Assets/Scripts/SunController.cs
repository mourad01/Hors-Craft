// DecompilerFi decompiler from Assembly-CSharp.dll class: SunController
using Common.Managers;
using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SunController : MonoBehaviour, IGameCallbacksListener
{
	public static SunController instance;

	public bool newSkybox;

	public Color ambientLightNight = Color.black;

	public Color ambientLightStartNight = Color.black;

	public Color ambientLightDay = Color.black;

	public Color ambientLightStartDay = Color.black;

	public float blendTime = 0.01f;

	[Space]
	[Header("This will be calculated so don't change this. Right click->calc gradient to get real in game gradient")]
	public Gradient ambientGradient;

	public float secondsInFullDay = 120f;

	[Range(0f, 1f)]
	public float gameStartTimeOfDay = 0.4f;

	public GameObject[] clouds;

	public GameObject cloudsParent;

	[Range(0f, 1f)]
	public float cloudsMovingSpeed = 1f;

	public bool disableClouds;

	[HideInInspector]
	public GameObject player;

	public float timeMultiplier = 1f;

	[HideInInspector]
	public Light sun;

	[Range(0f, 1f)]
	public float currentTimeOfDay;

	public bool disableFog;

	public Color fogDayColor = new Color32(200, 222, 239, byte.MaxValue);

	public Color fogNightColor = new Color32(22, 31, 52, byte.MaxValue);

	private float cloudsLighterAmount;

	public float nightComingTime = 0.8f;

	public float nightStartTime = 0.916f;

	public float dayComingTime = 0.2f;

	public float dayStartTime = 0.25f;

	public bool disableMovingSun;

	private TimeOfDayContext timeOfDayContext;

	private void Awake()
	{
		if (GetComponent<SkyboxController>() == null)
		{
			base.gameObject.AddComponent<SkyboxController>();
		}
		instance = this;
	}

	private void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
		sun = GetComponent<Light>();
		if (disableClouds)
		{
			GameObject[] array = clouds;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(value: false);
			}
		}
		InitFromModel();
		RenderSettings.ambientMode = AmbientMode.Flat;
		timeOfDayContext = new TimeOfDayContext
		{
			time = currentTimeOfDay,
			isNight = IsNight()
		};
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.TIME_OF_DAY, timeOfDayContext);
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
		CalcAmbientGradient();
		RenderSettings.fog = !disableFog;
		UpdateSun();
	}

	private void Update()
	{
		UpdateSun();
		currentTimeOfDay += Time.deltaTime / secondsInFullDay * timeMultiplier;
		if (currentTimeOfDay >= 1f)
		{
			currentTimeOfDay = 0f;
		}
		Shader.SetGlobalFloat("_TimeOfDay", currentTimeOfDay);
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
		}
		RenderSettings.ambientLight = ambientGradient.Evaluate(currentTimeOfDay);
		UpdateFactsContext();
	}

	private void UpdateFactsContext()
	{
		if (Mathf.Abs(timeOfDayContext.time - currentTimeOfDay) >= 0.01f)
		{
			timeOfDayContext.time = currentTimeOfDay;
			timeOfDayContext.isNight = IsNight();
			MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.TIME_OF_DAY);
		}
	}

	public void InitFromModel()
	{
		SunModule sunSettings = Manager.Get<ModelManager>().sunSettings;
		secondsInFullDay = sunSettings.GetSecondsInFullDay(secondsInFullDay);
		nightComingTime = sunSettings.GetNightComing(nightComingTime);
		nightStartTime = sunSettings.GetNightStart(nightStartTime);
		dayComingTime = sunSettings.GetDayComing(dayComingTime);
		dayStartTime = sunSettings.GetDayStart(dayStartTime);
		gameStartTimeOfDay = sunSettings.GetGameStartTime(gameStartTimeOfDay);
		CalcAmbientGradient();
		SkyboxController componentInChildren = GetComponentInChildren<SkyboxController>();
		if (componentInChildren != null)
		{
			componentInChildren.Init();
		}
	}

	protected void UpdateSun()
	{
		float f = Mathf.Lerp(0f, 6.28319f, currentTimeOfDay);
		base.transform.localPosition = new Vector3(10f * Mathf.Cos(f), 10f, 10f * Mathf.Sin(f));
		base.transform.LookAt(Vector3.zero);
	}

	private void SetLighterAmount(float intensity)
	{
		if (clouds.Length >= 1)
		{
			clouds[0].GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_LigtherAmount", intensity * 2f);
		}
	}

	private void UpdateCloudsPosition()
	{
		if (player != null)
		{
			Transform transform = cloudsParent.transform;
			Vector3 position = cloudsParent.transform.position;
			transform.SetPositionX(position.x + cloudsMovingSpeed);
			Transform transform2 = cloudsParent.transform.parent.transform;
			Vector3 position2 = player.transform.position;
			float x = position2.x;
			Vector3 position3 = player.transform.position;
			transform2.position = new Vector3(x, 0f, position3.z);
			Vector3 position4 = cloudsParent.transform.position;
			if (position4.x % 215f >= 215f - cloudsMovingSpeed)
			{
				Transform transform3 = cloudsParent.transform.GetChild(0).transform;
				Vector3 position5 = cloudsParent.transform.GetChild(0).transform.position;
				transform3.SetPositionX(position5.x - 860f);
				cloudsParent.transform.GetChild(0).transform.SetAsLastSibling();
			}
		}
	}

	public void OnGameSavedFrequent()
	{
		PlayerPrefs.SetFloat("sun.currentTime", currentTimeOfDay);
		GlobalSettings.timeOfDay = GetTimeOfDayEnum();
	}

	public void OnGameSavedInfrequent()
	{
	}

	public virtual void OnGameplayStarted()
	{
		currentTimeOfDay = PlayerPrefs.GetFloat("sun.currentTime", gameStartTimeOfDay);
	}

	public virtual void OnGameplayRestarted()
	{
	}

	public int GetTimeOfDay()
	{
		int num = (int)(4f * currentTimeOfDay - 1f);
		return (num >= 0) ? num : (num + 4);
	}

	public GlobalSettings.TimeOfDay GetTimeOfDayEnum()
	{
		if (nightStartTime <= currentTimeOfDay || dayComingTime > currentTimeOfDay)
		{
			return GlobalSettings.TimeOfDay.NIGHT;
		}
		if (dayComingTime <= currentTimeOfDay && dayStartTime > currentTimeOfDay)
		{
			return GlobalSettings.TimeOfDay.MORNING;
		}
		if (dayStartTime <= currentTimeOfDay && nightComingTime > currentTimeOfDay)
		{
			return GlobalSettings.TimeOfDay.MIDDAY;
		}
		return GlobalSettings.TimeOfDay.TWILIGHT;
	}

	public bool IsNight()
	{
		return currentTimeOfDay > nightStartTime || currentTimeOfDay < dayStartTime;
	}

	[ContextMenu("Calc gradient")]
	public void CalcAmbientGradient()
	{
		CalculateGradient(ambientGradient, ambientLightStartDay, ambientLightDay, ambientLightStartNight, ambientLightNight);
	}

	public void CalculateGradient(Gradient gradient, Color colorStarDay, Color colorDay, Color colorStartNight, Color colorNight)
	{
		List<GradientColorKey> list = new List<GradientColorKey>();
		GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2]
		{
			new GradientAlphaKey(1f, 0f),
			new GradientAlphaKey(1f, 1f)
		};
		float num = (nightComingTime - dayStartTime) * blendTime;
		list.Add(new GradientColorKey(colorDay, dayStartTime + num));
		list.Add(new GradientColorKey(colorDay, nightComingTime - num));
		num = (1f - (nightStartTime - dayComingTime)) * blendTime;
		list.Add(new GradientColorKey(colorNight, nightStartTime + num));
		list.Add(new GradientColorKey(colorNight, dayComingTime - num));
		num = (dayStartTime - dayComingTime) * blendTime;
		list.Add(new GradientColorKey(colorStarDay, dayComingTime + num));
		list.Add(new GradientColorKey(colorStarDay, dayStartTime - num));
		num = (nightStartTime - nightComingTime) * blendTime;
		list.Add(new GradientColorKey(colorStartNight, nightComingTime + num));
		list.Add(new GradientColorKey(colorStartNight, nightStartTime - num));
		gradient.SetKeys(list.ToArray(), alphaKeys);
	}

	public void animateTimeChange(float targetTime)
	{
		StartCoroutine(animateTimeChangeCoroutine(targetTime));
	}

	public IEnumerator animateTimeChangeCoroutine(float targetTime)
	{
		float targetChange = (!(currentTimeOfDay <= targetTime)) ? (1f - (currentTimeOfDay - targetTime)) : (targetTime - currentTimeOfDay);
		float elapsedChange = 0f;
		float previousTimeOfDay = currentTimeOfDay;
		float previousTimeMultiplier = timeMultiplier;
		timeMultiplier = 250f;
		while (elapsedChange < targetChange)
		{
			elapsedChange += ((!(previousTimeOfDay <= currentTimeOfDay)) ? (1f - (previousTimeOfDay - currentTimeOfDay)) : (currentTimeOfDay - previousTimeOfDay));
			previousTimeOfDay = currentTimeOfDay;
			yield return new WaitForSeconds(0.1f);
		}
		timeMultiplier = previousTimeMultiplier;
	}
}
