// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Configs.SurvivalRankConfig
using Common.Managers;
using System;
using UnityEngine;

namespace Gameplay.Configs
{
	public class SurvivalRankConfig : ScriptableObject
	{
		[SerializeField]
		private new string name = "Default name";

		[SerializeField]
		private bool hasSteps = true;

		[SerializeField]
		private float stepRequirements = 1f;

		[SerializeField]
		private int maxSteps = 1;

		[SerializeField]
		private float pointsPerStep = 1f;

		[SerializeField]
		private bool isRelatedToQuest = true;

		[SerializeField]
		private QuestType questType;

		[SerializeField]
		private bool affectsLeaderboard = true;

		[SerializeField]
		private bool affectsRank = true;

		[SerializeField]
		private bool clearOnDead;

		private float progress;

		private int _step;

		public Action onStepChange;

		private int step
		{
			get
			{
				return _step;
			}
			set
			{
				if (_step != value)
				{
					_step = value;
					if (onStepChange != null)
					{
						onStepChange();
					}
				}
			}
		}

		public string Name => name;

		public float MaxSteps
		{
			get
			{
				if (hasSteps)
				{
					return maxSteps;
				}
				return -1f;
			}
		}

		public float PointsPerStep => pointsPerStep;

		public float MaxPoints => pointsPerStep * (float)maxSteps;

		public float Points
		{
			get
			{
				if (hasSteps)
				{
					return pointsPerStep * (float)step;
				}
				return pointsPerStep * progress;
			}
		}

		public float RankPoints
		{
			get
			{
				if (affectsRank)
				{
					return Points;
				}
				return 0f;
			}
		}

		public float Progress => progress;

		public float LeaderboardPoints
		{
			get
			{
				if (affectsLeaderboard)
				{
					return Points;
				}
				return 0f;
			}
		}

		public void Init(float currentProgress)
		{
			progress = currentProgress;
			_step = CalcStep();
		}

		public void IncreaseProgress(float value)
		{
			progress += value;
			SetStep();
			if (isRelatedToQuest)
			{
				Manager.Get<QuestManager>().IncreaseQuestOfType(questType, (int)value);
			}
		}

		public void PlayerDie()
		{
			if (clearOnDead)
			{
				progress = 0f;
				SetStep();
			}
		}

		private int CalcStep()
		{
			if (!hasSteps)
			{
				return 0;
			}
			return Mathf.Clamp((int)(progress / stepRequirements), 0, maxSteps);
		}

		private void SetStep()
		{
			step = CalcStep();
		}
	}
}
