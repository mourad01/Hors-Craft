// DecompilerFi decompiler from Assembly-CSharp.dll class: TrainInterior
using UnityEngine;

public class TrainInterior : MonoBehaviour
{
	public GameObject exterior;

	public GameObject interior;

	public void EnableInterior(bool enabled)
	{
		interior.SetActive(enabled);
		exterior.SetActive(!enabled);
	}
}
