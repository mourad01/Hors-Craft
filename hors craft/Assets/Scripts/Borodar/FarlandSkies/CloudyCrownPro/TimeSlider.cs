// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.TimeSlider
using UnityEngine;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	public class TimeSlider : MonoBehaviour
	{
		private Slider _slider;

		private SkyboxCycleManager _skyboxCycleManager;

		protected void Awake()
		{
			_slider = GetComponent<Slider>();
		}

		protected void Start()
		{
			_skyboxCycleManager = MonoBehaviourSingleton<SkyboxCycleManager>.get;
		}

		protected void Update()
		{
			_slider.value = _skyboxCycleManager.CycleProgress;
		}

		public void OnValueChanged(float value)
		{
			_skyboxCycleManager.CycleProgress = value;
		}
	}
}
