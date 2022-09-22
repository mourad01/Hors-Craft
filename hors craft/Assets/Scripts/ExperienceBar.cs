// DecompilerFi decompiler from Assembly-CSharp.dll class: ExperienceBar
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
	public Text currentLevelText;

	public Slider levelSlider;

	private ProgressManager progress;

	private void Start()
	{
		currentLevelText = GetComponentInChildren<Text>();
		levelSlider = GetComponentInChildren<Slider>();
		progress = Manager.Get<ProgressManager>();
	}

	private void Update()
	{
		currentLevelText.text = string.Empty + progress.level;
		levelSlider.minValue = 0f;
		levelSlider.maxValue = progress.experienceNeededToNextLevel;
		levelSlider.value = progress.experience;
	}
}
