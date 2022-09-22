// DecompilerFi decompiler from Assembly-CSharp.dll class: ExplodeNode
using com.ootii.Cameras;
using Common.BehaviourTrees;
using UnityEngine;

public class ExplodeNode : MobNode
{
	private const int RANGE = 3;

	private ParticleSystem explosionSystem;

	private Camera _camera;

	private Camera mainCamera
	{
		get
		{
			if (_camera == null)
			{
				_camera = CameraController.instance.MainCamera;
			}
			return _camera;
		}
	}

	public ExplodeNode(EnemyMob mob, ParticleSystem system = null)
		: base(mob)
	{
		explosionSystem = system;
	}

	public override void Update()
	{
		base.status = Status.FAILURE;
		Vector3 position = base.mob.transform.position;
		float y = position.y;
		Vector3 position2 = mainCamera.transform.position;
		bool digDown = y > position2.y;
		VoxelExplosion.ExplodeTerrain(null, base.mob.transform, 3f, digDown, out bool somethingDestroyed);
		explosionSystem.Play();
		if (somethingDestroyed)
		{
			base.status = Status.SUCCESS;
		}
	}
}
