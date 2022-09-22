// DecompilerFi decompiler from Assembly-CSharp.dll class: SurvivalContextsBroadcaster
using System;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalContextsBroadcaster
{
	private static SurvivalContextsBroadcaster _instance;

	protected List<SurvivalContext> survivalContexts = new List<SurvivalContext>();

	protected Action onUpdateWhatever;

	protected Dictionary<Type, Action> onUpdateContextDictionary = new Dictionary<Type, Action>();

	public static SurvivalContextsBroadcaster instance => _instance ?? (_instance = new SurvivalContextsBroadcaster());

	private SurvivalContextsBroadcaster()
	{
	}

	public T GetContext<T>() where T : SurvivalContext
	{
		for (int i = 0; i < survivalContexts.Count; i++)
		{
			if (survivalContexts[i] is T)
			{
				return (T)survivalContexts[i];
			}
		}
		return (T)null;
	}

	public SurvivalContext[] GetAllContexts()
	{
		return survivalContexts.ToArray();
	}

	public void Register<T>(Action onContextUpdated) where T : SurvivalContext
	{
		Register(typeof(T), onContextUpdated);
	}

	public void Register(Type contextType, Action onContextUpdated)
	{
		if (!onUpdateContextDictionary.ContainsKey(contextType))
		{
			onUpdateContextDictionary.Add(contextType, delegate
			{
			});
		}
		Dictionary<Type, Action> dictionary;
		Type key;
		(dictionary = onUpdateContextDictionary)[key = contextType] = (Action)Delegate.Combine(dictionary[key], onContextUpdated);
	}

	public void Register(Type[] contextTypes, Action onContextUpdated)
	{
		for (int i = 0; i < contextTypes.Length; i++)
		{
			Register(contextTypes[i], onContextUpdated);
		}
	}

	public void Register(Type[] contextTypes, Action[] onContextUpdatedes)
	{
		if (contextTypes.Length != onContextUpdatedes.Length)
		{
			UnityEngine.Debug.LogError("contextTypes size must be the same as onContextUpdated size!!! Can not register");
			return;
		}
		for (int i = 0; i < contextTypes.Length; i++)
		{
			Register(contextTypes[i], onContextUpdatedes[i]);
		}
	}

	public void RegisterForAll(Action onContextUpdated)
	{
		onUpdateWhatever = (Action)Delegate.Combine(onUpdateWhatever, onContextUpdated);
	}

	public void Unregister<T>(Action onContextUpdated) where T : SurvivalContext
	{
		Unregister(typeof(T), onContextUpdated);
	}

	public void Unregister(Type contextType, Action onContextUpdated)
	{
		if (onUpdateContextDictionary.ContainsKey(contextType))
		{
			Dictionary<Type, Action> dictionary;
			Type key;
			(dictionary = onUpdateContextDictionary)[key = contextType] = (Action)Delegate.Remove(dictionary[key], onContextUpdated);
		}
	}

	public void Unregister(Type[] contextTypes, Action onContextUpdated)
	{
		for (int i = 0; i < contextTypes.Length; i++)
		{
			Unregister(contextTypes[i], onContextUpdated);
		}
	}

	public void Unregister(Type[] contextTypes, Action[] onContextUpdatedes)
	{
		if (contextTypes.Length != onContextUpdatedes.Length)
		{
			UnityEngine.Debug.LogError("contextTypes size must be the same as onContextUpdated size!!! Can not unregister");
			return;
		}
		for (int i = 0; i < contextTypes.Length; i++)
		{
			Unregister(contextTypes[i], onContextUpdatedes[i]);
		}
	}

	public void UnregisterForAll(Action onContextUpdated)
	{
		onUpdateWhatever = (Action)Delegate.Remove(onUpdateWhatever, onContextUpdated);
	}

	public void UpdateContext<T>(T context) where T : SurvivalContext
	{
		if (!survivalContexts.Contains(context))
		{
			survivalContexts.Add(context);
		}
		if (onUpdateContextDictionary.ContainsKey(typeof(T)))
		{
			onUpdateContextDictionary[typeof(T)]();
		}
		if (onUpdateWhatever != null)
		{
			onUpdateWhatever();
		}
	}

	public void Clear()
	{
		survivalContexts.Clear();
		onUpdateContextDictionary.Clear();
	}
}
