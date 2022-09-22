// DecompilerFi decompiler from Assembly-CSharp.dll class: SkyboxController
using com.ootii.Cameras;
using CommonAttribute;
using System;
using Uniblocks;
using UnityEngine;

[RequireComponent(typeof(SunController))]
public class SkyboxController : MonoBehaviour
{
	public Material toAssignSkybox;

	private Material skyboxMaterial;

	private SunController _sunController;

	[Space]
	public bool newSkybox;

	[Header("Sky Color Top")]
	public Color skyColorTopNight = Color.black;

	public Color skyColorTopStartNight = Color.black;

	public Color skyColorTopDay = Color.black;

	public Color skyColorTopStartDay = Color.black;

	[ReadOnly]
	public Gradient skyColorTopGradient;

	[Space]
	[Header("Sky Color Bottom")]
	public Color skyColorBottomNight = Color.black;

	public Color skyColorBottomStartNight = Color.black;

	public Color skyColorBottomDay = Color.black;

	public Color skyColorBottomStartDay = Color.black;

	[ReadOnly]
	public Gradient skyColorBottomGradient;

	[Space]
	[Header("Sun Settings (Probably dont change this)")]
	public float _sunLongitude;

	public float _sunAltitude = 45f;

	public Vector2 _sunOrbit = new Vector2(-20f, 200f);

	public bool sunSetAtNight = true;

	private float _sunrise;

	private float _sunset;

	private float _sunDuration;

	private Vector3 _sunAttitudeVector;

	public Color sunColorNight = Color.black;

	public Color sunColorStartNight = Color.black;

	public Color sunColorDay = Color.black;

	public Color sunColorStartDay = Color.black;

	[ReadOnly]
	public Gradient sunColorGradient;

	[Space]
	[Header("Moon Settings (Probably dont change this)")]
	public float _moonLongitude;

	public float _moonAltitude = 45f;

	public Vector2 _moonOrbit = new Vector2(-20f, 200f);

	private float _moonrise;

	private float _moonset;

	private float _moonDuration;

	private Vector3 _moonAttitudeVector;

	[Space]
	[Header("Fake objects")]
	[ReadOnly]
	public GameObject fakeMoon;

	[ReadOnly]
	public GameObject fakeSun;

	[Space]
	[Header("Underwater settings")]
	public Material waterMaterial;

	[Range(0f, 1f)]
	public float saturationWater = 0.9f;

	[Range(0f, 1f)]
	public float underwaterWaterAlpha = 0.1f;

	private Color? underwaterBaseColor;

	private readonly Color waterBaseColor = new Color(0.07644895f, 0.5772461f, 0.7426471f, 0.6235294f);

	private PlayerMovement _playerMovement;

	private SunController sunController => _sunController ?? (_sunController = GetComponent<SunController>());

	private PlayerMovement playerMovement
	{
		get
		{
			if (_playerMovement == null && CameraController.instance != null)
			{
				_playerMovement = CameraController.instance.Anchor.GetComponent<PlayerMovement>();
			}
			return _playerMovement;
		}
		set
		{
			_playerMovement = value;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		Init();
		waterMaterial.SetColor("_Color", waterBaseColor);
		playerMovement = null;
	}

	public void Init()
	{
		if (newSkybox)
		{
			SetUpSky();
			SetUpSun();
			SetUpMoon();
			if (fakeMoon == null)
			{
				fakeMoon = new GameObject("FakeMoon");
				fakeMoon.transform.position = Vector3.zero;
			}
			if (fakeSun == null)
			{
				fakeSun = new GameObject("FakeSun");
				fakeSun.transform.position = Vector3.zero;
			}
		}
	}

	private void Update()
	{
		SetSkybox();
		if (skyboxMaterial == null)
		{
			return;
		}
		if (newSkybox)
		{
			UpdateSky();
			UpdateSun();
			UpdateMoon();
		}
		else
		{
			float currentTimeOfDay = sunController.currentTimeOfDay;
			if (currentTimeOfDay > sunController.dayStartTime && currentTimeOfDay < sunController.nightComingTime)
			{
				skyboxMaterial.SetColor("_Transition", new Color(0f, 0f, 0f, 0f));
			}
			else if (currentTimeOfDay >= sunController.nightComingTime && currentTimeOfDay < sunController.nightStartTime)
			{
				skyboxMaterial.SetColor("_Transition", Color.Lerp(new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 1f), (currentTimeOfDay - sunController.nightComingTime) / (sunController.nightStartTime - sunController.nightComingTime)));
			}
			else if (currentTimeOfDay >= sunController.nightStartTime || currentTimeOfDay < sunController.dayComingTime)
			{
				skyboxMaterial.SetColor("_Transition", new Color(0f, 0f, 0f, 1f));
			}
			else
			{
				skyboxMaterial.SetColor("_Transition", Color.Lerp(new Color(0f, 0f, 0f, 1f), new Color(0f, 0f, 0f, 0f), (currentTimeOfDay - sunController.dayComingTime) / (sunController.dayStartTime - sunController.dayComingTime)));
			}
		}
		if (underwaterBaseColor.HasValue && playerMovement != null)
		{
			Color color = skyColorBottomGradient.Evaluate(sunController.currentTimeOfDay);
			color = Color.Lerp(waterBaseColor, color, saturationWater);
			float a;
			if (playerMovement.underwater)
			{
				a = underwaterWaterAlpha;
			}
			else
			{
				Color color2 = waterBaseColor;
				a = color2.a;
			}
			color.a = a;
			waterMaterial.SetColor("_Color", color);
			float a2;
			if (playerMovement.underwater)
			{
				float num = (float)Engine.TerrainGenerator.waterHeight - 1.49f;
				Vector3 localPosition = playerMovement.transform.localPosition;
				a2 = Mathf.Clamp01((num - localPosition.y) / 5f);
			}
			else
			{
				a2 = 0f;
			}
			color.a = a2;
			skyboxMaterial.SetColor("_UnderwaterOverlay", color);
		}
	}

	private void SetUpSky()
	{
		CalcGradientTop();
		CalcGradientBottom();
		CalcGradientSun();
	}

	private void UpdateSky()
	{
		skyboxMaterial.SetColor("_TopColor", skyColorTopGradient.Evaluate(sunController.currentTimeOfDay));
		skyboxMaterial.SetColor("_BottomColor", skyColorBottomGradient.Evaluate(sunController.currentTimeOfDay));
	}

	private void SetUpSun()
	{
		_sunrise = sunController.dayStartTime;
		_sunset = ((!sunSetAtNight) ? sunController.nightComingTime : sunController.nightStartTime);
		_sunDuration = ((!(_sunrise < _sunset)) ? (1f - _sunrise + _sunset) : (_sunset - _sunrise));
		float f = _sunAltitude * ((float)Math.PI / 180f);
		_sunAttitudeVector = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
	}

	private void UpdateSun()
	{
		float currentTimeOfDay = sunController.currentTimeOfDay;
		float num = (!(_sunrise < currentTimeOfDay)) ? (1f + currentTimeOfDay - _sunrise) : (currentTimeOfDay - _sunrise);
		float t = (!(num < _sunDuration)) ? ((_sunDuration - num) / _sunDuration) : (num / _sunDuration);
		float angle = Mathf.Lerp(_sunOrbit.x, _sunOrbit.y, t);
		Quaternion rotation = Quaternion.AngleAxis(_sunLongitude - 180f, Vector3.up) * Quaternion.AngleAxis(angle, _sunAttitudeVector);
		Vector3 eulerAngles = rotation.eulerAngles;
		float x = eulerAngles.x;
		Vector3 eulerAngles2 = rotation.eulerAngles;
		rotation.eulerAngles = new Vector3(x, eulerAngles2.y, 0f);
		if (fakeSun != null)
		{
			fakeSun.transform.rotation = rotation;
			skyboxMaterial.SetMatrix("sunMatrix", fakeSun.transform.worldToLocalMatrix);
		}
		skyboxMaterial.SetColor("_SunTint", sunColorGradient.Evaluate(sunController.currentTimeOfDay));
	}

	private void SetUpMoon()
	{
		_moonrise = sunController.nightStartTime;
		_moonset = sunController.dayComingTime;
		_moonDuration = ((!(_moonrise < _moonset)) ? (1f - _moonrise + _moonset) : (_moonset - _moonrise));
		float f = _moonAltitude * ((float)Math.PI / 180f);
		_moonAttitudeVector = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
	}

	private void UpdateMoon()
	{
		float currentTimeOfDay = sunController.currentTimeOfDay;
		float num = (!(_moonrise < currentTimeOfDay)) ? (1f + currentTimeOfDay - _moonrise) : (currentTimeOfDay - _moonrise);
		float t = (!(num < _moonDuration)) ? ((_moonDuration - num) / _moonDuration) : (num / _moonDuration);
		float angle = Mathf.Lerp(_moonOrbit.x, _moonOrbit.y, t);
		Quaternion rotation = Quaternion.AngleAxis(_moonLongitude - 180f, Vector3.up) * Quaternion.AngleAxis(angle, _moonAttitudeVector);
		Vector3 eulerAngles = rotation.eulerAngles;
		float x = eulerAngles.x;
		Vector3 eulerAngles2 = rotation.eulerAngles;
		rotation.eulerAngles = new Vector3(x, eulerAngles2.y, 0f);
		if (fakeMoon != null)
		{
			fakeMoon.transform.rotation = rotation;
			skyboxMaterial.SetMatrix("moonMatrix", fakeMoon.transform.worldToLocalMatrix);
		}
	}

	private void SetSkybox()
	{
		if (!(CameraController.instance == null) && !(CameraController.instance.MainCamera == null) && skyboxMaterial == null)
		{
			Skybox component = CameraController.instance.MainCamera.GetComponent<Skybox>();
			if (component != null)
			{
				skyboxMaterial = component.material;
				UnityEngine.Object.Destroy(component);
			}
			else if (toAssignSkybox != null)
			{
				skyboxMaterial = toAssignSkybox;
				underwaterBaseColor = skyboxMaterial.GetColor("_UnderwaterOverlay");
			}
			else
			{
				skyboxMaterial = Resources.Load<Material>("defaultSkybox");
			}
			RenderSettings.skybox = skyboxMaterial;
		}
	}

	[ContextMenu("Calc gradient top")]
	public void CalcGradientTop()
	{
		sunController.CalculateGradient(skyColorTopGradient, skyColorTopStartDay, skyColorTopDay, skyColorTopStartNight, skyColorTopNight);
	}

	[ContextMenu("Calc gradient bottom")]
	public void CalcGradientBottom()
	{
		sunController.CalculateGradient(skyColorBottomGradient, skyColorBottomStartDay, skyColorBottomDay, skyColorBottomStartNight, skyColorBottomNight);
	}

	[ContextMenu("Calc gradient sun")]
	public void CalcGradientSun()
	{
		sunController.CalculateGradient(sunColorGradient, sunColorStartDay, sunColorDay, sunColorStartNight, sunColorNight);
	}

	private void OnDestroy()
	{
		if (underwaterBaseColor.HasValue)
		{
			skyboxMaterial.SetColor("_UnderwaterOverlay", underwaterBaseColor.Value);
		}
		waterMaterial.SetColor("_Color", waterBaseColor);
	}
}
