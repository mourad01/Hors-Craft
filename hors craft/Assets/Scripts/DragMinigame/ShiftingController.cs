// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.ShiftingController
using UnityEngine;

namespace DragMinigame
{
	public class ShiftingController : MonoBehaviour
	{
		public int currentTransfer = 1;

		[HideInInspector]
		public float currentShiftMaxValue;

		private DragPlayerConfig playerConfig;

		private int maxShiftNumber;

		public void Init(DragPlayerConfig config)
		{
			playerConfig = config;
			CalculateTransferValues();
			maxShiftNumber = config.shiftValues.Length;
		}

		public virtual void NextTransfer()
		{
			if (currentTransfer < maxShiftNumber)
			{
				currentTransfer++;
			}
			CalculateTransferValues();
		}

		public virtual void PreviousTransfer()
		{
			if (currentTransfer > 1)
			{
				currentTransfer--;
			}
			CalculateTransferValues();
		}

		private void CalculateTransferValues()
		{
			currentShiftMaxValue = playerConfig.shiftValues[currentTransfer - 1];
		}
	}
}
