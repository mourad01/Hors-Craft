// DecompilerFi decompiler from Assembly-CSharp.dll class: EjectShell
using UnityEngine;

public class EjectShell : MonoBehaviour
{
	public ParticleSystem ParticleSystem;

	private void Eject(int s)
	{
		ParticleSystem.Emit(s);
	}
}
