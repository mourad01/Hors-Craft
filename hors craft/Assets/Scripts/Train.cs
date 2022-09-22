// DecompilerFi decompiler from Assembly-CSharp.dll class: Train
using Uniblocks;
using UnityEngine;

public class Train : MonoBehaviour
{
	public float spawnYOffset;

	private bool mountedValue;

	private bool connectedToTrain;

	public bool mounted
	{
		get
		{
			return mountedValue;
		}
		set
		{
			mountedValue = value;
		}
	}

	public bool connectToTrain
	{
		get
		{
			return connectedToTrain;
		}
		set
		{
			base.enabled = !value;
			connectedToTrain = value;
			if ((bool)base.gameObject.GetComponentInParent<PlayerMovement>())
			{
				GetComponent<CharacterController>().enabled = false;
			}
			else
			{
				GetComponent<CharacterController>().enabled = true;
			}
		}
	}

	public Rigidbody body
	{
		get;
		private set;
	}

	private void Start()
	{
		base.transform.Translate(Vector3.up * spawnYOffset);
	}
}
