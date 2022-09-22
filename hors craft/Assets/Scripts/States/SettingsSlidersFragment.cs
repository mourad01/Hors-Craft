// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SettingsSlidersFragment
using Common.Managers;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class SettingsSlidersFragment : Fragment
	{
		public GameObject timePanelGO;

		public TranslateText timeOfDayText;

		public TranslateText qualityText;

		public TranslateText sensitivityText;

		public TranslateText volumeText;

		public Slider timeOfDaySlider;

		public Slider qualitySlider;

		public Slider sensitivitySlider;

		public Slider VolumeSlider;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			InitUI();
		}

		public override void Destroy()
		{
			SaveSettings();
			base.Destroy();
		}

		private void SaveSettings()
		{
			GlobalSettings.qualityLevel = (int)qualitySlider.value;
			GlobalSettings.sensitivity = sensitivitySlider.value;
			GlobalSettings.volume = VolumeSlider.value;
			if (startParameter.pauseStartParameter.allowTimeChange)
			{
				GlobalSettings.SetTimeOfDayDirty((GlobalSettings.TimeOfDay)timeOfDaySlider.value);
				GlobalSettings.timeOfDay = (GlobalSettings.TimeOfDay)timeOfDaySlider.value;
			}
		}

		private void InitUI()
		{
			timePanelGO.SetActive(startParameter.pauseStartParameter.allowTimeChange);
			timeOfDaySlider.value = (float)GlobalSettings.timeOfDay;
			qualitySlider.value = GlobalSettings.qualityLevel;
			sensitivitySlider.value = GlobalSettings.sensitivity;
			VolumeSlider.value = GlobalSettings.volume;
			UpdateSettings();
			timeOfDaySlider.onValueChanged.AddListener(delegate
			{
				TimeOfDaySliderChanged();
			});
			qualitySlider.onValueChanged.AddListener(delegate
			{
				QualitySliderChanged();
			});
			sensitivitySlider.onValueChanged.AddListener(delegate
			{
				SensitivityChanged();
			});
			VolumeSlider.onValueChanged.AddListener(delegate
			{
				VolumeChanged();
			});
		}

		private void UpdateSettings()
		{
			TimeOfDaySliderChanged();
			QualitySliderChanged();
			SensitivityChanged();
			VolumeChanged();
		}

		private void TimeOfDaySliderChanged()
		{
			string timeOfDayID;
			switch ((int)timeOfDaySlider.value)
			{
			default:
				timeOfDayID = "morning";
				break;
			case 1:
				timeOfDayID = "midday";
				break;
			case 2:
				timeOfDayID = "dawn";
				break;
			case 3:
				timeOfDayID = "night";
				break;
			}
			timeOfDayText.ClearVisitors();
			timeOfDayText.AddVisitor(delegate(string text)
			{
				string text2 = Manager.Get<TranslationsManager>().GetText("settings.time." + timeOfDayID, timeOfDayID);
				return text.ReplaceInsensitive("{x}", text2);
			});
		}

		private void QualitySliderChanged()
		{
			string qualityID;
			switch ((int)qualitySlider.value)
			{
			default:
				qualityID = "low";
				break;
			case 1:
				qualityID = "normal";
				break;
			case 2:
				qualityID = "high";
				break;
			}
			qualityText.ClearVisitors();
			qualityText.AddVisitor(delegate(string text)
			{
				string text2 = Manager.Get<TranslationsManager>().GetText("settings.quality." + qualityID, qualityID);
				return text.ReplaceInsensitive("{x}", text2);
			});
		}

		private void SensitivityChanged()
		{
			sensitivityText.ClearVisitors();
			sensitivityText.AddVisitor((string text) => text.ReplaceInsensitive("{x}", sensitivitySlider.value.ToString()));
		}

		private void VolumeChanged()
		{
			string volumeID;
			switch ((int)VolumeSlider.value)
			{
			default:
				volumeID = "muted";
				AudioListener.volume = 0f;
				break;
			case 1:
				volumeID = "low";
				AudioListener.volume = 0.5f;
				break;
			case 2:
				volumeID = "high";
				AudioListener.volume = 1f;
				break;
			}
			volumeText.ClearVisitors();
			volumeText.AddVisitor(delegate(string text)
			{
				string text2 = Manager.Get<TranslationsManager>().GetText("settings.volume." + volumeID, volumeID);
				return text.ReplaceInsensitive("{x}", text2);
			});
		}
	}
}
