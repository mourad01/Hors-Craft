// DecompilerFi decompiler from Assembly-CSharp.dll class: FireParticlesWorker
using UnityEngine;

public class FireParticlesWorker : CutSceneWorker
{
	public ParticleSystem particles;

	public override void DoJob(WorkerParameter parameters = null)
	{
		UnityEngine.Debug.Log("FireParticlesWorker doing its job.");
		particles.gameObject.SetActive(value: true);
		base.DoJob(parameters);
	}

	public override void CleanUp()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		base.CleanUp();
	}
}
