// DecompilerFi decompiler from Assembly-CSharp.dll class: Borodar.FarlandSkies.CloudyCrownPro.TimeText
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Borodar.FarlandSkies.CloudyCrownPro
{
	public class TimeText : MonoBehaviour
	{
		private Text _text;

		protected void Awake()
		{
			_text = GetComponent<Text>();
		}

		protected void Update()
		{
			TimeSpan timeSpan = TimeSpan.FromHours(MonoBehaviourSingleton<SkyboxCycleManager>.get.CycleProgress * 0.24f);
			_text.text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}";
		}
	}
}
