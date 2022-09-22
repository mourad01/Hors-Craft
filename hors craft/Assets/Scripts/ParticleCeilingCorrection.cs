// DecompilerFi decompiler from Assembly-CSharp.dll class: ParticleCeilingCorrection
using UnityEngine;

public class ParticleCeilingCorrection : MonoBehaviour
{
	public ParticleSystem particles;

	private bool belowCeiling;

	private const int maxDistance = 50;

	private void Update()
	{
		Vector3 position = base.transform.position;
		float x = position.x;
		Vector3 position2 = base.transform.position;
		float y = position2.y;
		Vector3 position3 = base.transform.position;
		if (Physics.Raycast(new Vector3(x, y, position3.z), Vector3.up, out RaycastHit _, 50f, 1 << LayerMask.NameToLayer("Terrain")))
		{
			if (!belowCeiling)
			{
				particles.Stop();
				belowCeiling = true;
			}
		}
		else if (belowCeiling)
		{
			belowCeiling = false;
			particles.Play();
		}
	}
}
