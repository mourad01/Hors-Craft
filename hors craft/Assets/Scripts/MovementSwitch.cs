// DecompilerFi decompiler from Assembly-UnityScript.dll class: MovementSwitch
using System;
using UnityEngine;

[Serializable]
public class MovementSwitch : MonoBehaviour
{
	public CharacterMotor motor;

	private bool speedOn;

	public void Update()
	{
		if (UnityEngine.Input.GetKeyDown("r"))
		{
			if (speedOn)
			{
				motor.movement.maxForwardSpeed = 4f;
				motor.movement.maxBackwardsSpeed = 4f;
				motor.movement.maxSidewaysSpeed = 4f;
				motor.jumping.baseHeight = 0.5f;
				speedOn = false;
			}
			else
			{
				motor.movement.maxForwardSpeed = 15f;
				motor.movement.maxBackwardsSpeed = 15f;
				motor.movement.maxSidewaysSpeed = 15f;
				motor.jumping.baseHeight = 1.5f;
				speedOn = true;
			}
		}
	}

	public void Main()
	{
	}
}
