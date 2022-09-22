// DecompilerFi decompiler from Assembly-CSharp.dll class: SimpleVelocity
using System;
using UnityEngine;

[Serializable]
public class SimpleVelocity
{
	public float currentVelocity;

	public float acceleration = 0.01f;

	public float maxVelocity = 1f;

	public float drag = 0.6f;

	public bool noForce;

	public SimpleVelocity()
	{
	}

	public SimpleVelocity(float acceleration = 0.01f, float maxVelocity = 1f, float drag = 0.05f)
	{
		this.acceleration = acceleration;
		this.maxVelocity = maxVelocity;
		this.drag = drag;
	}

	public float GetOutput(float value)
	{
		currentVelocity += acceleration * value;
		if (currentVelocity >= maxVelocity)
		{
			currentVelocity = maxVelocity;
		}
		if (currentVelocity < -1f * maxVelocity)
		{
			currentVelocity = -1f * maxVelocity;
		}
		if (Mathf.RoundToInt(value) == 0)
		{
			noForce = true;
			currentVelocity -= currentVelocity * drag;
		}
		else
		{
			noForce = false;
		}
		return currentVelocity;
	}

	internal void Reset()
	{
		currentVelocity = 0f;
	}
}
