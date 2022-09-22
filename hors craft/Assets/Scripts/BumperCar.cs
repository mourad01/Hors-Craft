// DecompilerFi decompiler from Assembly-CSharp.dll class: BumperCar
using Common.Managers;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

[RequireComponent(typeof(HoverCar))]
public class BumperCar : MonoBehaviour
{
	public List<ushort> blockId;

	public bool isSpawningResources = true;

	private const float GET_REWARD_EVERY = 10f;

	private Rigidbody body;

	private HoverCar car;

	private float timer;

	private void Awake()
	{
		car = GetComponent<HoverCar>();
		body = GetComponent<Rigidbody>();
		timer = 0f;
	}

	private void Update()
	{
		if (car.enabled)
		{
			DisableIfNotOnCorrectBlock();
			if (timer > 10f && isSpawningResources)
			{
				Manager.Get<CraftingManager>().SpawnRandomResource(base.transform.position);
				timer = 0f;
			}
			timer += Time.deltaTime;
		}
	}

	private void DisableIfNotOnCorrectBlock()
	{
		ushort voxel = Engine.PositionToVoxelInfo(base.transform.position + Vector3.down).GetVoxel();
		if (!blockId.Contains(voxel))
		{
			car.enabled = false;
			body.useGravity = true;
		}
	}
}
