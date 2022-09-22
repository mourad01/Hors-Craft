// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.StateMachineManager
using Common.Managers.States;
using Common.Utils.TestsSuite;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class StateMachineManager : Manager
	{
		private class Transition
		{
			public enum Mode
			{
				POP,
				PUSH
			}

			public Mode mode;

			public Type targetType;

			public StartParameter startParameter;
		}

		public State startingState;

		public bool prepareStates;

		public bool unloadUnusedAssets = true;

		protected Dictionary<Type, State> statesDictionary;

		protected Stack<State> stackedStates;

		private Queue<Transition> transitions;

		public State currentState
		{
			get;
			protected set;
		}

		public override void Init()
		{
			PopulateAllStates();
			stackedStates = new Stack<State>();
			transitions = new Queue<Transition>();
			if (unloadUnusedAssets)
			{
				Resources.UnloadUnusedAssets();
			}
			if (startingState != null)
			{
				transitions.Enqueue(new Transition
				{
					mode = Transition.Mode.PUSH,
					targetType = startingState.GetType()
				});
			}
		}

		public virtual void SetState<T>(StartParameter startParameter = null) where T : State
		{
			SetState(typeof(T), startParameter);
		}

		public virtual void SetState(Type stateType, StartParameter startParameter = null)
		{
			PopState();
			PushState(stateType, startParameter);
		}

		public virtual void PushState<T>(StartParameter startParameter = null) where T : State
		{
			PushState(typeof(T), startParameter);
		}

		public virtual void PushState(Type stateType, StartParameter startParameter = null)
		{
			transitions.Enqueue(new Transition
			{
				mode = Transition.Mode.PUSH,
				targetType = stateType,
				startParameter = startParameter
			});
		}

		public virtual void PopState()
		{
			transitions.Enqueue(new Transition
			{
				mode = Transition.Mode.POP,
				targetType = null
			});
		}

		public virtual void PopStatesUntil<T>() where T : State
		{
			transitions.Enqueue(new Transition
			{
				mode = Transition.Mode.POP,
				targetType = typeof(T)
			});
		}

		public virtual bool IsCurrentStateA<T>() where T : State
		{
			return currentState is T;
		}

		protected void PopulateAllStates()
		{
			statesDictionary = new Dictionary<Type, State>();
			State[] componentsInChildren = base.gameObject.GetComponentsInChildren<State>();
			foreach (State state in componentsInChildren)
			{
				try
				{
					statesDictionary.Add(state.GetType(), state);
					if (prepareStates)
					{
						state.PrepareState();
					}
				}
				catch (ArgumentException)
				{
					throw new Exception("Duplicated state of type " + state.GetType() + " was found inside the state manager hierarchy!");
				}
			}
			AddTestsSuiteStateIfNeeded();
		}

		private void AddTestsSuiteStateIfNeeded()
		{
			if (!statesDictionary.ContainsKey(typeof(TestsSuiteState)))
			{
				GameObject gameObject = new GameObject("TestsSuiteState");
				TestsSuiteState value = gameObject.AddComponent<TestsSuiteState>();
				gameObject.transform.SetParent(base.transform, worldPositionStays: true);
				statesDictionary.Add(typeof(TestsSuiteState), value);
			}
		}

		private void OnGUI()
		{
			EvaluateAllTransitions();
			if (currentState != null)
			{
				currentState.UpdateState();
			}
		}

		private void EvaluateAllTransitions()
		{
			while (transitions != null && transitions.Count > 0)
			{
				Transition t = transitions.Dequeue();
				EvaluateTransition(t);
			}
		}

		private void EvaluateTransition(Transition t)
		{
			if (t.mode == Transition.Mode.PUSH)
			{
				PushTransition(t);
			}
			else
			{
				if (t.mode != 0)
				{
					return;
				}
				if (t.targetType == null)
				{
					if (currentState != null)
					{
						PopOne();
					}
					return;
				}
				while (!t.targetType.IsAssignableFrom(currentState.GetType()) && stackedStates.Count > 0)
				{
					PopOne();
				}
				if (stackedStates.Count == 0)
				{
					UnityEngine.Debug.LogWarning("Popped, but couldn't get to " + t.targetType + "!");
				}
			}
		}

		private State PushTransition(Transition t)
		{
			if (currentState != null)
			{
				currentState.FreezeState();
			}
			currentState = GetStateInstance(t.targetType);
			currentState.StartState(t.startParameter);
			stackedStates.Push(currentState);
			return currentState;
		}

		public T GetStateInstance<T>() where T : State
		{
			return GetStateInstance(typeof(T)) as T;
		}

		public State GetStateInstance(Type stateType)
		{
			if (!statesDictionary.TryGetValue(stateType, out State value))
			{
				foreach (Type key in statesDictionary.Keys)
				{
					if (stateType.IsAssignableFrom(key))
					{
						return statesDictionary[key];
					}
				}
				throw new Exception("Requested use of nonexisting state: " + stateType.ToString());
			}
			return value;
		}

		public bool ContainsState(Type stateType)
		{
			if (!statesDictionary.ContainsKey(stateType))
			{
				foreach (Type key in statesDictionary.Keys)
				{
					if (stateType.IsAssignableFrom(key))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}

		private void PopOne()
		{
			currentState.FinishState();
			stackedStates.Pop();
			if (stackedStates.Count > 0)
			{
				currentState = stackedStates.Peek();
				currentState.UnfreezeState();
			}
			else
			{
				currentState = null;
			}
		}

		public bool IsStateInStack(Type stateType)
		{
			State state = null;
			try
			{
				state = GetStateInstance(stateType);
			}
			catch (Exception)
			{
				return false;
			}
			return state != null && stackedStates.Contains(state);
		}

		public void Reset()
		{
			currentState.FinishState();
			stackedStates.Clear();
			transitions.Clear();
		}
	}
}
