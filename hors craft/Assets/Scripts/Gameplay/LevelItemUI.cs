// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.LevelItemUI
using Common.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class LevelItemUI : MonoBehaviour
	{
		public Text lvlText;

		public Image fillImage;

		private ProgressManager progressManager;

		private void Awake()
		{
			progressManager = Manager.Get<ProgressManager>();
			ProgressManager.onExpIncrease += OnExpIncrease;
			UpdateLevel();
		}

		private void OnExpIncrease(int amount, int previousExp)
		{
			UpdateLevel();
		}

		private void UpdateLevel()
		{
			if (!(progressManager == null))
			{
				lvlText.text = $"LEVEL {progressManager.level.ToString()}";
				float fillAmount = (float)progressManager.experience / (float)progressManager.experienceNeededToNextLevel;
				fillImage.fillAmount = fillAmount;
			}
		}

		private void OnDestroy()
		{
			ProgressManager.onExpIncrease -= OnExpIncrease;
		}
	}
}
