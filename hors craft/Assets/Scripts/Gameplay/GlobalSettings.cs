// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GlobalSettings
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using States;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace Gameplay
{
	public static class GlobalSettings
	{
		public enum MovingMode
		{
			WALKING,
			FLYING
		}

		public enum TimeOfDay
		{
			MORNING,
			MIDDAY,
			TWILIGHT,
			NIGHT
		}

		public static bool timeChanged = false;

		private static HashSet<string> gamesWithDefaultFlyingMode = new HashSet<string>
		{
			"xcraft.creative",
			"xcraft.dollhouse.two"
		};

		private const string GAME_MODE_PATH = "mode";

		private static CultureInfo _cultureInfo;

		private static ConfigModule _configSettings;

		private const string FLYING_TIME_PATH = "flying.time";

		private const string QUALITY_LEVEL_PATH = "qualityLevel";

		private const string TIME_OF_DAY_PATH = "timeOfDay";

		private const string SENSITIVITY_PATH = "cameraSensitivity";

		private const string VOLUME_PATH = "volumeSensitivity";

		[CompilerGenerated]
		private static Action _003C_003Ef__mg_0024cache0;

		public static MovingMode mode
		{
			get
			{
				int defaultValue = 0;
				if (gamesWithDefaultFlyingMode.Contains(Manager.Get<ConnectionInfoManager>().gameName))
				{
					defaultValue = 1;
				}
				return (MovingMode)PlayerPrefs.GetInt("mode", defaultValue);
			}
			set
			{
				PlayerPrefs.SetInt("mode", (int)value);
				UpdateMoveModeFact();
			}
		}

		public static CultureInfo cultureInfo
		{
			get
			{
				if (_cultureInfo == null)
				{
					_cultureInfo = new CultureInfo("en-US");
				}
				return _cultureInfo;
			}
		}

		private static ConfigModule configSettings
		{
			get
			{
				if (_configSettings == null)
				{
					_configSettings = Manager.Get<ModelManager>().configSettings;
				}
				return _configSettings;
			}
		}

		private static DateTime flyingModeStartTime
		{
			get
			{
				return DateTime.Parse(PlayerPrefs.GetString("flying.time", (DateTime.Now - new TimeSpan(100, 0, 0)).ToString(cultureInfo)), cultureInfo);
			}
			set
			{
				PlayerPrefs.SetString("flying.time", value.ToString(cultureInfo));
			}
		}

		public static int qualityLevel
		{
			get
			{
				return PlayerPrefs.GetInt("qualityLevel", 0);
			}
			set
			{
				PlayerPrefs.SetInt("qualityLevel", value);
			}
		}

		public static TimeOfDay timeOfDay
		{
			get
			{
				return (TimeOfDay)PlayerPrefs.GetInt("timeOfDay", 1);
			}
			set
			{
				PlayerPrefs.SetInt("timeOfDay", (int)value);
			}
		}

		public static float sensitivity
		{
			get
			{
				return PlayerPrefs.GetFloat("cameraSensitivity", 1f);
			}
			set
			{
				PlayerPrefs.SetFloat("cameraSensitivity", value);
			}
		}

		public static float volume
		{
			get
			{
				return PlayerPrefs.GetFloat("volumeSensitivity", 2f);
			}
			set
			{
				PlayerPrefs.SetFloat("volumeSensitivity", value);
			}
		}

		public static void LoadSettings()
		{
			if (CameraController.instance.MainCamera != null)
			{
				Camera mainCamera = CameraController.instance.MainCamera;
			}
			QualitySettings.SetQualityLevel(qualityLevel);
			if (timeChanged)
			{
				SunController component = GameObject.FindGameObjectWithTag("Sun").GetComponent<SunController>();
				component.animateTimeChange((float)(timeOfDay + 1) * 6f / 24f);
				timeChanged = false;
			}
			AudioListener.volume = volume / 2f;
		}

		public static void SwitchMoveMode()
		{
			if (mode == MovingMode.WALKING && !CanSwitchToFlying())
			{
				PushFlyForAdPopup();
			}
			else
			{
				mode = Misc.IncrementEnum(mode);
			}
		}

		private static void PushFlyForAdPopup()
		{
			Manager.Get<StateMachineManager>().PushState<UniversalPopupState>(new UniversalPopupStateStartParameter
			{
				prefabToSpawn = "FlyPopup",
				configPopup = delegate(DefaultUniversalPopup popup)
				{
					popup.returnButton.onClick.AddListener(delegate
					{
						Manager.Get<StateMachineManager>().PopState();
					});
					popup.GetActionButton("watchButton").onClick.AddListener(delegate
					{
						Manager.Get<RewardedAdsManager>().ShowRewardedAd(StatsManager.AdReason.XCRAFT_FLY, delegate(bool success)
						{
							if (success)
							{
								StartFlyingMode();
								SwitchMoveMode();
								Manager.Get<StateMachineManager>().PopState();
							}
						});
					});
					TranslateText component = popup.GetObject("FlyText").GetComponent<TranslateText>();
					component.AddVisitor((string text) => text.Formatted(TimeSpan.FromSeconds(GetMaxFlyingModeTimeLeft()).Minutes));
					component.ForceRefresh();
				}
			});
		}

		public static void UpdateMoveModeFact()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.RemoveFact(Fact.MOVE_MODE);
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MOVE_MODE, new MoveModeContext
			{
				mode = mode,
				onMoveModeChange = SwitchMoveMode
			});
		}

		public static void StartFlyingMode()
		{
			flyingModeStartTime = DateTime.Now;
		}

		public static float GetFlyingModeTimeLeft()
		{
			return (float)(flyingModeStartTime - DateTime.Now).TotalSeconds + (float)configSettings.GetFlyingForAdTime();
		}

		public static float GetMaxFlyingModeTimeLeft()
		{
			return configSettings.GetFlyingForAdTime();
		}

		public static bool CanFlyForAd()
		{
			if (configSettings.IsFlyingForAdEnabled())
			{
				if (GetFlyingModeTimeLeft() > 0f)
				{
					return true;
				}
				if (mode == MovingMode.FLYING)
				{
					SwitchMoveMode();
				}
			}
			return false;
		}

		public static bool IsFlyingForAdEnabled()
		{
			return configSettings.IsFlyingForAdEnabled();
		}

		private static bool CanSwitchToFlying()
		{
			if (configSettings.IsFlyingForAdEnabled() && GetFlyingModeTimeLeft() < 0f)
			{
				return false;
			}
			return true;
		}

		public static void SetTimeOfDayDirty(TimeOfDay newtimeOfDay)
		{
			if (newtimeOfDay != timeOfDay)
			{
				timeChanged = true;
			}
		}

		private static void enableAmbientOcclusion(Camera cam, bool enable)
		{
			ScreenSpaceAmbientOcclusion component = cam.GetComponent<ScreenSpaceAmbientOcclusion>();
			if (component != null)
			{
				component.enabled = enable;
			}
			else if (enable)
			{
				component = cam.gameObject.AddComponent<ScreenSpaceAmbientOcclusion>();
				component.m_SSAOShader = Shader.Find("Hidden/SSAO");
				component.m_RandomTexture = Resources.Load<Texture2D>("Effects/SSAO/RandomVectors");
				component.enabled = true;
			}
			Shader.SetGlobalFloat("_SSAO", (!enable) ? 1 : (-1));
		}
	}
}
