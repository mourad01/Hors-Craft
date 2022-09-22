// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AchievementsFragment
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class AchievementsFragment : Fragment
	{
		[SerializeField]
		private AchievementElement elementPrefab;

		[SerializeField]
		private RectTransform content;

		[SerializeField]
		private Image progressImage;

		[SerializeField]
		private Text currentPointsText;

		private ScrollRect scrollView;

		private List<Achievement> configs;

		private void Awake()
		{
			scrollView = GetComponentInChildren<ScrollRect>();
		}

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			InitAchievements();
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			InitAchievements();
		}

		private void InitAchievements()
		{
			AchievementManager achievementManager = Manager.Get<AchievementManager>();
			UpdateProgressBar(achievementManager.GetAchievedPoints(), achievementManager.GetMaxPoints());
			configs = achievementManager.configs;
			while (content.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(content.GetChild(0).gameObject);
			}
			for (int i = 0; i < configs.Count; i++)
			{
				AchievementElement achievementElement = Object.Instantiate(elementPrefab, content, worldPositionStays: false);
				achievementElement.Init(configs[i]);
			}
			scrollView.normalizedPosition = achievementManager.GetScrollViewPosition();
			achievementManager.ResetCurrentConfig();
		}

		public void UpdateProgressBar(double achieved, double max)
		{
			currentPointsText.text = "{0}/{1}".Formatted(achieved, max);
			progressImage.fillAmount = (float)(achieved / max);
		}
	}
}
