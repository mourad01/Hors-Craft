// DecompilerFi decompiler from Assembly-CSharp.dll class: PatientUnlockedNotification
using UnityEngine;
using UnityEngine.UI;

public class PatientUnlockedNotification : TopNotification
{
	public class PatientUnlockedInformation : ShowInformation
	{
		public Sprite icon;
	}

	public Image image;

	public override void SetElement(ShowInformation information)
	{
		PatientUnlockedInformation patientUnlockedInformation = information as PatientUnlockedInformation;
		image.sprite = patientUnlockedInformation.icon;
	}
}
