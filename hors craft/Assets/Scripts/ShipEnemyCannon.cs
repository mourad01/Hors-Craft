// DecompilerFi decompiler from Assembly-CSharp.dll class: ShipEnemyCannon
using UnityEngine;

public class ShipEnemyCannon : MonoBehaviour
{
	[SerializeField]
	private Transform horizontalBase;

	[SerializeField]
	private Transform verticalBase;

	[SerializeField]
	private Transform _shootPoint;

	private const float MAX_ROTATION_SPEED = 70f;

	private const float MAX_ROTATION_X_SPEED = 50f;

	private Transform _player;

	public Transform shootPoint => _shootPoint;

	private Transform player
	{
		get
		{
			if (_player == null)
			{
				_player = GameObject.FindGameObjectWithTag("Player").transform;
			}
			return _player;
		}
	}

	private void Update()
	{
		Look();
	}

	private void Look()
	{
		if (!(player == null))
		{
			Vector3 normalized = (player.TransformPoint(0f, -0.5f, 0f) - horizontalBase.position).normalized;
			horizontalBase.forward = new Vector3(normalized.x, 0f, normalized.z).normalized;
			verticalBase.forward = normalized;
		}
	}
}
