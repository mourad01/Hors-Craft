// DecompilerFi decompiler from Assembly-CSharp.dll class: IncreaseHospitalPrestige
using Common.Managers;
using UnityEngine;

public class IncreaseHospitalPrestige : MonoBehaviour
{
	public void Increase()
	{
		if (Manager.Contains<HospitalManager>())
		{
			Manager.Get<HospitalManager>().IncreaseHospitalPrestige();
		}
	}
}
