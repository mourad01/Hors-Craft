// DecompilerFi decompiler from Assembly-CSharp.dll class: SpinningMachine.SpinableMachineController
using System;
using System.Collections;
using UnityEngine;

namespace SpinningMachine
{
	public class SpinableMachineController : MonoBehaviour
	{
		public enum WinType
		{
			Random,
			AlwaysMax,
			AlwaysMin
		}

		public enum RotateAxis
		{
			X,
			Y,
			Z
		}

		public Action<bool, RewardType, int, int> onSpinDone;

		[Header("GFX")]
		[SerializeField]
		private Animator[] slotsAnimators;

		[SerializeField]
		private GameObject[] slotsReal;

		[SerializeField]
		private GameObject[] slotsBlured;

		[SerializeField]
		private RotateAxis rotateAxis;

		[Header("Logic")]
		[SerializeField]
		private RewardsConfig rewardsConfig;

		[SerializeField]
		private WinType winType = WinType.AlwaysMax;

		[SerializeField]
		private float animationDuratuion = 1f;

		private const float RANDOM_ALWAYS_WIN = 0.95f;

		private const float RANDOM_ALWAYS_MIN = 0.75f;

		private float markAngle;

		private bool _isSpinning;

		private Vector3 axis
		{
			get
			{
				if (rotateAxis == RotateAxis.X)
				{
					return new Vector3(1f, 0f, 0f);
				}
				if (rotateAxis == RotateAxis.Y)
				{
					return new Vector3(0f, 1f, 0f);
				}
				return new Vector3(0f, 0f, 1f);
			}
		}

		private int slotMarksCount => rewardsConfig.slotsCount;

		private int minToWin => rewardsConfig.minToWin;

		public bool isSpinning => _isSpinning;

		private void Awake()
		{
			markAngle = 360f / (float)slotMarksCount;
			DisableBlured();
		}

		public void Spin()
		{
			_isSpinning = true;
			StartCoroutine(Spining());
		}

		private IEnumerator Spining()
		{
			int[] slotsValues = new int[slotsAnimators.Length];
			int winValue;
			int winCount;
			bool isWin = RandomSlots(ref slotsValues, out winValue, out winCount);
			EnableBlured();
			for (int j = 0; j < slotsAnimators.Length; j++)
			{
				slotsAnimators[j].SetBool("Spin", value: true);
			}
			yield return new WaitForSeconds(animationDuratuion);
			for (int i = 0; i < slotsAnimators.Length; i++)
			{
				yield return new WaitForSeconds(0.2f);
				slotsAnimators[i].SetBool("Spin", value: false);
				slotsReal[i].transform.localRotation = Quaternion.Euler(axis * ((float)slotsValues[i] * markAngle));
				slotsBlured[i].SetActive(value: false);
			}
			yield return new WaitForSeconds(0.3f);
			_isSpinning = false;
			if (onSpinDone != null)
			{
				RewardSettings rewardSettings = rewardsConfig.RewardSettings(winValue);
				onSpinDone(isWin, rewardSettings.rewardType, rewardSettings.rewardId, rewardsConfig.WinCount(winType, winCount));
			}
		}

		private void DisableBlured()
		{
			SetActiveBlured(active: false);
		}

		private void EnableBlured()
		{
			SetActiveBlured(active: true);
		}

		private void SetActiveBlured(bool active)
		{
			for (int i = 0; i < slotsBlured.Length; i++)
			{
				slotsBlured[i].SetActive(active);
			}
		}

		private bool RandomSlots(ref int[] slotsValues, out int winValue, out int winCount)
		{
			if (winType == WinType.AlwaysMax)
			{
				return AlwaysWin(ref slotsValues, out winValue, out winCount);
			}
			if (winType == WinType.AlwaysMin)
			{
				return AlwaysMin(ref slotsValues, out winValue, out winCount);
			}
			float value = UnityEngine.Random.value;
			if (value >= 0.95f)
			{
				return AlwaysWin(ref slotsValues, out winValue, out winCount);
			}
			if (value >= 0.75f)
			{
				return AlwaysMin(ref slotsValues, out winValue, out winCount);
			}
			return RandomLoose(ref slotsValues, out winValue, out winCount);
		}

		private bool AlwaysWin(ref int[] slotsValues, out int winValue, out int winCount)
		{
			winCount = slotsReal.Length;
			winValue = UnityEngine.Random.Range(0, slotMarksCount);
			for (int i = 0; i < winCount; i++)
			{
				slotsValues[i] = winValue;
			}
			return true;
		}

		private bool AlwaysMin(ref int[] slotsValues, out int winValue, out int winCount)
		{
			winValue = UnityEngine.Random.Range(0, slotMarksCount);
			winCount = UnityEngine.Random.Range(minToWin, slotsReal.Length + 1);
			int num = UnityEngine.Random.Range(0, slotsReal.Length - winCount + 1);
			for (int i = 0; i < slotsReal.Length; i++)
			{
				if (i < num || i >= num + winCount)
				{
					slotsValues[i] = RandomRangeExcept(0, slotMarksCount, winValue);
				}
				else
				{
					slotsValues[i] = winValue;
				}
			}
			return true;
		}

		private bool RandomLoose(ref int[] slotsValues, out int winValue, out int winCount)
		{
			winCount = 0;
			winValue = 0;
			slotsValues[0] = UnityEngine.Random.Range(0, slotMarksCount + 1);
			for (int i = 1; i < slotsReal.Length; i++)
			{
				slotsValues[i] = RandomRangeExcept(0, slotMarksCount + 1, slotsValues[i - 1]);
			}
			return false;
		}

		private int RandomRangeExcept(int min, int max, int except)
		{
			if (max - min <= 1)
			{
				return min;
			}
			int num;
			for (num = 0; num == except; num = UnityEngine.Random.Range(min, max))
			{
			}
			return num;
		}
	}
}
