// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.SkyboxDayNightCycle
using Borodar.FarlandSkies.CloudyCrownPro.DotParams;
using System;
using UnityEngine;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	[ExecuteInEditMode]
	[HelpURL("http://www.borodar.com/stuff/farlandskies/cloudycrownpro/docs/QuickStart.v1.3.1.pdf")]
	public class SkyboxDayNightCycle : MonoBehaviourSingleton<SkyboxDayNightCycle>
	{
		[SerializeField]
		[Tooltip("List of sky colors, based on time of day. Each list item contains “time” filed that should be specified in percents (0 - 100)")]
		private SkyParamsList _skyParamsList = new SkyParamsList();

		[SerializeField]
		[Tooltip("Allows you to manage stars tint color over time. Each list item contains “time” filed that should be specified in percents (0 - 100)")]
		private StarsParamsList _starsParamsList = new StarsParamsList();

		[SerializeField]
		[Range(0f, 100f)]
		private float _sunrise = 25f;

		[SerializeField]
		[Range(0f, 100f)]
		private float _sunset = 85f;

		[SerializeField]
		[Tooltip("Max angle between the horizon and the center of sun’s disk")]
		private float _sunAltitude = 45f;

		[SerializeField]
		[Tooltip("Angle between z-axis and the center of sun’s disk at sunrise")]
		private float _sunLongitude;

		[SerializeField]
		[Tooltip("A pair of angles that limit visible orbit of the sun")]
		private Vector2 _sunOrbit = new Vector2(-20f, 200f);

		[SerializeField]
		[Tooltip("Sun appearance and light params depending on time of day. Each list item contains “time” filed that should be specified in percents (0 - 100)")]
		private CelestialParamsList _sunParamsList = new CelestialParamsList();

		[SerializeField]
		[Range(0f, 100f)]
		private float _moonrise = 90f;

		[SerializeField]
		[Range(0f, 100f)]
		private float _moonset = 22.5f;

		[SerializeField]
		[Tooltip("Max angle between the horizon and the center of moon’s disk")]
		private float _moonAltitude = 45f;

		[SerializeField]
		[Tooltip("Angle between z-axis and the center of moon’s disk at moonrise")]
		private float _moonLongitude;

		[SerializeField]
		[Tooltip("A pair of angles that limit visible orbit of the moon")]
		private Vector2 _moonOrbit = new Vector2(-20f, 200f);

		[SerializeField]
		[Tooltip("Moon appearance and light params depending on time of day. Each list item contains “time” filed that should be specified in percents (0 - 100)")]
		private CelestialParamsList _moonParamsList = new CelestialParamsList();

		[SerializeField]
		[Tooltip("Reduce the skybox day-night cycle update to run every \"n\" frames")]
		private int _framesInterval = 2;

		private SkyboxController _skyboxController;

		private float _sunDuration;

		private Vector3 _sunAttitudeVector;

		private float _moonDuration;

		private Vector3 _moonAttitudeVector;

		private int _framesToSkip;

		private float _timeOfDay;

		public float TimeOfDay
		{
			get
			{
				return _timeOfDay;
			}
			set
			{
				_timeOfDay = value % 100f;
			}
		}

		public SkyParam CurrentSkyParam
		{
			get;
			private set;
		}

		public StarsParam CurrentStarsParam
		{
			get;
			private set;
		}

		public CelestialParam CurrentSunParam
		{
			get;
			private set;
		}

		public CelestialParam CurrentMoonParam
		{
			get;
			private set;
		}

		protected void Awake()
		{
			_sunDuration = ((!(_sunrise < _sunset)) ? (100f - _sunrise + _sunset) : (_sunset - _sunrise));
			float f = _sunAltitude * ((float)Math.PI / 180f);
			_sunAttitudeVector = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
			_moonDuration = ((!(_moonrise < _moonset)) ? (100f - _moonrise + _moonset) : (_moonset - _moonrise));
			f = _moonAltitude * ((float)Math.PI / 180f);
			_moonAttitudeVector = new Vector3(Mathf.Sin(f), Mathf.Cos(f));
			_skyParamsList.Init();
			_starsParamsList.Init();
			_sunParamsList.Init();
			_moonParamsList.Init();
		}

		public void Start()
		{
			_skyboxController = MonoBehaviourSingleton<SkyboxController>.get;
			CurrentSkyParam = _skyParamsList.GetParamPerTime(TimeOfDay);
			CurrentStarsParam = _starsParamsList.GetParamPerTime(TimeOfDay);
			CurrentSunParam = _sunParamsList.GetParamPerTime(TimeOfDay);
			CurrentMoonParam = _moonParamsList.GetParamPerTime(TimeOfDay);
		}

		public void Update()
		{
			if (--_framesToSkip <= 0)
			{
				_framesToSkip = _framesInterval;
				UpdateSky();
				UpdateStars();
				UpdateSun();
				UpdateMoon();
			}
		}

		private void UpdateSky()
		{
			if (_skyboxController == null)
			{
				_skyboxController = MonoBehaviourSingleton<SkyboxController>.get;
			}
			CurrentSkyParam = _skyParamsList.GetParamPerTime(TimeOfDay);
			_skyboxController.TopColor = CurrentSkyParam.TopColor;
			_skyboxController.BottomColor = CurrentSkyParam.BottomColor;
		}

		private void UpdateStars()
		{
			if (_skyboxController.StarsEnabled)
			{
				CurrentStarsParam = _starsParamsList.GetParamPerTime(TimeOfDay);
				_skyboxController.StarsTint = CurrentStarsParam.TintColor;
			}
		}

		private void UpdateSun()
		{
			if (_skyboxController.SunEnabled)
			{
				if (TimeOfDay > _sunrise || TimeOfDay < _sunset)
				{
					float num = (!(_sunrise < TimeOfDay)) ? (100f + TimeOfDay - _sunrise) : (TimeOfDay - _sunrise);
					float t = (!(num < _sunDuration)) ? ((_sunDuration - num) / _sunDuration) : (num / _sunDuration);
					float angle = Mathf.Lerp(_sunOrbit.x, _sunOrbit.y, t);
					Quaternion rotation = Quaternion.AngleAxis(_sunLongitude - 180f, Vector3.up) * Quaternion.AngleAxis(angle, _sunAttitudeVector);
					Vector3 eulerAngles = rotation.eulerAngles;
					float x = eulerAngles.x;
					Vector3 eulerAngles2 = rotation.eulerAngles;
					rotation.eulerAngles = new Vector3(x, eulerAngles2.y, 0f);
					_skyboxController.SunLight.transform.rotation = rotation;
				}
				CurrentSunParam = _sunParamsList.GetParamPerTime(TimeOfDay);
				_skyboxController.SunTint = CurrentSunParam.TintColor;
				_skyboxController.SunLight.color = CurrentSunParam.LightColor;
				_skyboxController.SunLight.intensity = CurrentSunParam.LightIntencity;
			}
		}

		private void UpdateMoon()
		{
			if (_skyboxController.MoonEnabled)
			{
				if (TimeOfDay > _moonrise || TimeOfDay < _moonset)
				{
					float num = (!(_moonrise < TimeOfDay)) ? (100f + TimeOfDay - _moonrise) : (TimeOfDay - _moonrise);
					float t = (!(num < _moonDuration)) ? ((_moonDuration - num) / _moonDuration) : (num / _moonDuration);
					float angle = Mathf.Lerp(_moonOrbit.x, _moonOrbit.y, t);
					Quaternion rotation = Quaternion.AngleAxis(_moonLongitude - 180f, Vector3.up) * Quaternion.AngleAxis(angle, _moonAttitudeVector);
					Vector3 eulerAngles = rotation.eulerAngles;
					float x = eulerAngles.x;
					Vector3 eulerAngles2 = rotation.eulerAngles;
					rotation.eulerAngles = new Vector3(x, eulerAngles2.y, 0f);
					_skyboxController.MoonLight.transform.rotation = rotation;
				}
				CurrentMoonParam = _moonParamsList.GetParamPerTime(TimeOfDay);
				_skyboxController.MoonTint = CurrentMoonParam.TintColor;
				_skyboxController.MoonLight.color = CurrentMoonParam.LightColor;
				_skyboxController.MoonLight.intensity = CurrentMoonParam.LightIntencity;
			}
		}
	}
}
