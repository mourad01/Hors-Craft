// DecompilerFi decompiler from Assembly-CSharp.dll class: CarNavigator
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class CarNavigator : MobNavigator
{
	public Voxel[] blockToFollow;

	public Voxel blockToSet;

	public float vehicleSpeed = 0.05f;

	private Vector3 currentPositionTarget;

	private VoxelInfo currentVoxel;

	private VoxelInfo possibleVoxel;

	private EngineSoundsBasedOnVelocity engineSound;

	public float coneSize = 45f;

	public float lengthOfProbes = 4f;

	public float sideProbesLenght = 0.75f;

	public float angleToChange = 1f;

	public Collider forwardIndicator;

	public bool carIsStoped;

	public bool debug;

	public bool forwardFixer = true;

	public float checkTimeSpace = 0.2f;

	private ushort[] blockToFollowIds;

	private bool isVisible = true;

	private bool foundedLine;

	private float lastCheck;

	private float angleValueToFixLater;

	protected override void UpdateMovement()
	{
		base.transform.Translate(base.transform.forward.normalized * vehicleSpeed, Space.World);
		ChangeVolume();
	}

	private void ChangeVolume()
	{
		if (engineSound != null)
		{
			engineSound.PlayAtVolume(1f);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag.ToLower().Equals("vehicle") && other.GetComponent<CarMob>() == null)
		{
			carIsStoped = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag.ToLower().Equals("vehicle") && other.GetComponent<CarMob>() == null)
		{
			carIsStoped = false;
		}
	}

	protected override void Start()
	{
		engineSound = GetComponentInChildren<EngineSoundsBasedOnVelocity>();
		blockToFollowIds = (from b in blockToFollow
			select b.GetUniqueID()).ToArray();
	}

	private void FixedUpdate()
	{
		if (!carIsStoped && isVisible)
		{
			if (Time.time - lastCheck > checkTimeSpace)
			{
				foundedLine = FindBlockInLine();
				lastCheck = Time.time;
			}
			if (foundedLine)
			{
				UpdateMovement();
			}
			if (debug)
			{
				UnityEngine.Debug.DrawLine(base.transform.position, GetLineEndPoint(GetCurrentAngle(coneSize), base.transform.position, lengthOfProbes * sideProbesLenght), Color.red, Time.fixedDeltaTime);
				UnityEngine.Debug.DrawLine(base.transform.position, GetLineEndPoint(GetCurrentAngle(0f - coneSize), base.transform.position, lengthOfProbes * sideProbesLenght), Color.red, Time.fixedDeltaTime);
				UnityEngine.Debug.DrawLine(base.transform.position, GetLineEndPoint(GetCurrentAngle(0f), base.transform.position, lengthOfProbes), Color.red, Time.fixedDeltaTime);
				UnityEngine.Debug.DrawRay(base.transform.position, base.transform.forward * 2f, Color.cyan, Time.fixedDeltaTime);
			}
		}
	}

	private void OnBecameVisible()
	{
		isVisible = true;
	}

	private void OnBecameInvisible()
	{
		isVisible = true;
	}

	public bool CheckUnderMe(VoxelInfo info, Vector3 position)
	{
		info = Engine.VoxelRaycast(position, Vector3.down, 3f, ignoreTransparent: true);
		if (info == null || blockToFollow[0].GetUniqueID() == info.GetVoxel())
		{
			return false;
		}
		return true;
	}

	private bool CheckForBlock(ushort blockId)
	{
		return blockToFollowIds.Contains(blockId);
	}

	public bool FindBlockInLine()
	{
		currentVoxel = Engine.VoxelRaycast(base.transform.position + Vector3.up, Vector3.down, 4f, ignoreTransparent: true);
		if (currentVoxel == null || !CheckForBlock(currentVoxel.GetVoxel()))
		{
			return false;
		}
		if (CheckInLine(GetCurrentAngle(0f), lengthOfProbes))
		{
			if (!forwardFixer)
			{
				return true;
			}
			bool flag = false;
			if (!CheckInLine(GetCurrentAngle(coneSize), lengthOfProbes * sideProbesLenght))
			{
				Transform transform = base.transform;
				Vector3 eulerAngles = base.transform.eulerAngles;
				transform.eulerAngles = new Vector3(0f, eulerAngles.y + angleToChange * 0.05f, 0f);
				angleValueToFixLater += angleToChange * 0.05f;
				flag = true;
			}
			else if (!CheckInLine(GetCurrentAngle(0f - coneSize), lengthOfProbes * sideProbesLenght))
			{
				Transform transform2 = base.transform;
				Vector3 eulerAngles2 = base.transform.eulerAngles;
				transform2.eulerAngles = new Vector3(0f, eulerAngles2.y - angleToChange * 0.05f, 0f);
				angleValueToFixLater -= angleToChange * 0.05f;
				flag = true;
			}
			if (!flag && !Mathf.Approximately(angleValueToFixLater, 0f))
			{
				float num = angleValueToFixLater / 20f;
				Transform transform3 = base.transform;
				Vector3 eulerAngles3 = base.transform.eulerAngles;
				transform3.eulerAngles = new Vector3(0f, eulerAngles3.y - num, 0f);
				angleValueToFixLater -= num;
			}
			return true;
		}
		if (CheckInLine(GetCurrentAngle(coneSize), lengthOfProbes * sideProbesLenght))
		{
			Transform transform4 = base.transform;
			Vector3 eulerAngles4 = base.transform.eulerAngles;
			transform4.eulerAngles = new Vector3(0f, eulerAngles4.y - angleToChange * 0.1f, 0f);
			return true;
		}
		if (CheckInLine(GetCurrentAngle(0f - coneSize), lengthOfProbes * sideProbesLenght))
		{
			Transform transform5 = base.transform;
			Vector3 eulerAngles5 = base.transform.eulerAngles;
			transform5.eulerAngles = new Vector3(0f, eulerAngles5.y + angleToChange, 0f);
			return true;
		}
		return false;
	}

	public bool CheckInLine(float angle, float len)
	{
		List<Index> list = CheckBlocksOnNaiveLine(angle, len);
		foreach (Index item in list)
		{
			if (!CheckForBlock(currentVoxel.chunk.GetVoxel(item)))
			{
				return false;
			}
		}
		return true;
	}

	public void RoadCreator(float angle)
	{
		currentVoxel = Engine.VoxelRaycast(base.transform.position, Vector3.down, 3f, ignoreTransparent: true);
		foreach (Index item in CheckBlocksOnNaiveLine(angle, lengthOfProbes))
		{
			currentVoxel.chunk.SetVoxel(item, blockToSet.GetUniqueID(), updateMesh: true, 0);
		}
	}

	public float GetCurrentAngle(float angleOffset)
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		return 0f - eulerAngles.y + 90f + angleOffset;
	}

	private List<Index> CheckBlocksOnNaiveLine(float angle, float distance)
	{
		List<Index> list = new List<Index>();
		if (currentVoxel == null)
		{
			return list;
		}
		Vector3 lineEndPoint = GetLineEndPoint(angle, currentVoxel.index.ToVector3(), distance);
		int num = Mathf.RoundToInt(lineEndPoint.x - (float)currentVoxel.index.x);
		int num2 = Mathf.RoundToInt(lineEndPoint.z - (float)currentVoxel.index.z);
		int num3 = 0;
		num3 = Mathf.Max(Mathf.Abs(num), Mathf.Abs(num2));
		float num4 = (float)num / (float)num3;
		float num5 = (float)num2 / (float)num3;
		float num6 = currentVoxel.index.x;
		float num7 = currentVoxel.index.z;
		for (int i = 0; i < num3; i++)
		{
			num6 += num4;
			num7 += num5;
			Index item = new Index(Mathf.RoundToInt(num6), currentVoxel.index.y, Mathf.RoundToInt(num7));
			list.Add(item);
		}
		return list;
	}

	private static Vector3 GetLineEndPoint(float angle, Vector3 start, float length)
	{
		float x = start.x + length * Mathf.Cos(angle * (float)Math.PI / 180f);
		float z = start.z + length * Mathf.Sin(angle * (float)Math.PI / 180f);
		return new Vector3(x, start.y, z);
	}
}
