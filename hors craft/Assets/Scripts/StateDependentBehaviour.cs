// DecompilerFi decompiler from Assembly-CSharp.dll class: StateDependentBehaviour
using Common.Managers;
using Common.Managers.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateDependentBehaviour : MonoBehaviour
{
	private List<Type> requiredStates = new List<Type>();

	public static void Add(GameObject go, params Type[] states)
	{
		StateDependentBehaviour stateDependentBehaviour = go.AddComponent<StateDependentBehaviour>();
		stateDependentBehaviour.Init(states);
	}

	public static void Add(GameObject go, State state)
	{
		Add(go, state.GetType());
	}

	private void Init(params Type[] states)
	{
		requiredStates = states.ToList();
	}

	private void Update()
	{
		State currentState = Manager.Get<StateMachineManager>().currentState;
		if (!requiredStates.Contains(currentState.GetType()))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
