// DecompilerFi decompiler from Assembly-CSharp.dll class: RollercoasterInput
using Common.GameUI;
using System;
using UnityEngine;

public class RollercoasterInput : MonoBehaviour
{
	public RollercoasterController controller;

	public SimpleRepeatButton simpleRepeatButton;

	private int pointerId;

	private Vector2 touchStartPosition;

	private float offset = 0.1f;

	private void Start()
	{
		SimpleRepeatButton obj = simpleRepeatButton;
		obj.onFingerDown = (SimpleRepeatButton.OnFingerDown)Delegate.Combine(obj.onFingerDown, new SimpleRepeatButton.OnFingerDown(CheckForInputDown));
		SimpleRepeatButton obj2 = simpleRepeatButton;
		obj2.onFingerUp = (SimpleRepeatButton.OnFingerDown)Delegate.Combine(obj2.onFingerUp, new SimpleRepeatButton.OnFingerDown(CheckForInputUp));
	}

	private void CheckForInputDown(int id)
	{
		pointerId = id;
		touchStartPosition = UnityEngine.Input.GetTouch(id).position;
	}

	private void CheckForInputUp(int id)
	{
		if (pointerId != id)
		{
			return;
		}
		Vector2 a = UnityEngine.Input.GetTouch(id).position - touchStartPosition;
		a /= new Vector2(Screen.width, Screen.height);
		if (Mathf.Abs(a.x) > Mathf.Abs(a.y) && Mathf.Abs(a.x) > offset)
		{
			if (a.x > 0f)
			{
				controller.SetDirection(RollercoasterController.Direction.Right);
			}
			else
			{
				controller.SetDirection(RollercoasterController.Direction.Left);
			}
		}
		else if (Mathf.Abs(a.y) > Mathf.Abs(a.x) && Mathf.Abs(a.y) > offset)
		{
			if (a.y > 0f)
			{
				controller.SetDirection(RollercoasterController.Direction.Up);
			}
			else
			{
				controller.SetDirection(RollercoasterController.Direction.Down);
			}
		}
	}
}
