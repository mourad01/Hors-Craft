// DecompilerFi decompiler from Assembly-CSharp.dll class: GameplayFacts
using Common.Behaviours;
using Common.Managers;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameplayFacts : MonoBehaviourSingleton<GameplayFacts>, IGameCallbacksListener
{
	private Dictionary<Fact, HashSet<FactContext>> factsDictionary = new Dictionary<Fact, HashSet<FactContext>>();

	private Dictionary<Fact, HashSet<IFactChangedListener>> factsToTheirListeners = new Dictionary<Fact, HashSet<IFactChangedListener>>();

	private HashSet<Fact> factsChanged = new HashSet<Fact>();

	private Dictionary<IFactChangedListener, HashSet<Fact>> listenersToTheirFacts = new Dictionary<IFactChangedListener, HashSet<Fact>>();

	private HashSet<Fact> factsToRemove = new HashSet<Fact>();

	private void Start()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	public void RegisterFactChangedListener(IFactChangedListener listener, params Fact[] facts)
	{
		foreach (Fact key in facts)
		{
			if (!factsToTheirListeners.ContainsKey(key))
			{
				factsToTheirListeners[key] = new HashSet<IFactChangedListener>();
			}
			factsToTheirListeners[key].Add(listener);
		}
	}

	public void UnregisterFactChangedListener(IFactChangedListener listener, params Fact[] facts)
	{
		foreach (Fact key in facts)
		{
			if (factsToTheirListeners.ContainsKey(key))
			{
				factsToTheirListeners[key].Remove(listener);
			}
		}
	}

	public bool AddFact(Fact fact, FactContext context = null)
	{
		if (context == null)
		{
			context = new FactContext();
		}
		if (!factsDictionary.ContainsKey(fact))
		{
			factsDictionary[fact] = new HashSet<FactContext>();
		}
		if (factsDictionary[fact].Add(context))
		{
			TriggerFactListeners(fact);
			return true;
		}
		return false;
	}

	public bool AddFactPersistent(Fact fact, PersistentFactContext context = null)
	{
		if (context == null)
		{
			context = new PersistentFactContext();
		}
		return AddFact(fact, context);
	}

	public bool AddFactIfNotExisting(Fact fact, FactContext context = null)
	{
		if (factsDictionary.ContainsKey(fact))
		{
			return false;
		}
		return AddFact(fact, context);
	}

	public bool AddSignalFact(Fact fact)
	{
		return AddFact(fact, new SignalFactContext());
	}

	public void NotifyFactChanged(Fact fact)
	{
		TriggerFactListeners(fact);
	}

	public void RemoveFactContext(Fact fact, FactContext context)
	{
		if (context != null && factsDictionary.ContainsKey(fact))
		{
			if (factsDictionary[fact].Remove(context))
			{
				TriggerFactListeners(fact);
			}
			if (factsDictionary[fact].Count == 0)
			{
				factsDictionary.Remove(fact);
			}
		}
	}

	public void RemoveFactContexts<T>(Fact fact) where T : FactContext
	{
		if (factsDictionary.ContainsKey(fact))
		{
			List<FactContext> list = new List<FactContext>();
			foreach (FactContext item in factsDictionary[fact])
			{
				if (item is T)
				{
					list.Add(item);
				}
			}
			list.ForEach(delegate(FactContext f)
			{
				RemoveFactContext(fact, f);
			});
		}
	}

	public void ReverseFact(Fact fact)
	{
		if (FactExists(fact))
		{
			RemoveFact(fact);
		}
		else
		{
			AddFact(fact);
		}
	}

	public void SetFact(Fact fact, bool enabled)
	{
		if (enabled)
		{
			AddFactIfNotExisting(fact);
		}
		else
		{
			RemoveFact(fact);
		}
	}

	public void SetContext(Fact fact, FactContext context, bool enabled)
	{
		if (enabled)
		{
			AddFact(fact, context);
		}
		else
		{
			RemoveFactContext(fact, context);
		}
	}

	public void RemoveFact(Fact fact)
	{
		if (factsDictionary.ContainsKey(fact))
		{
			factsDictionary.Remove(fact);
			TriggerFactListeners(fact);
		}
	}

	private void TriggerFactListeners(Fact fact)
	{
		if (MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper && PlayerPrefs.GetInt("print.facts", 0) == 1)
		{
			PrintFacts();
		}
		factsChanged.Add(fact);
	}

	public T GetFactContext<T>(Fact fact)
	{
		return GetFactContexts<T>(fact).FirstOrDefault();
	}

	public List<T> GetFactContexts<T>(Fact fact)
	{
		if (factsDictionary.ContainsKey(fact))
		{
			return (from f in factsDictionary[fact]
				where f is T
				select f).Cast<T>().ToList();
		}
		return new List<T>();
	}

	public bool FactExists(Fact fact)
	{
		return factsDictionary.ContainsKey(fact);
	}

	public void PrintFacts()
	{
		StringWriter stringWriter = new StringWriter();
		foreach (Fact key in factsDictionary.Keys)
		{
			stringWriter.WriteLine(key.ToString());
			foreach (FactContext item in factsDictionary[key])
			{
				stringWriter.WriteLine("\t" + item.GetType().ToString() + ", " + item.GetContent());
			}
			stringWriter.WriteLine();
		}
		UnityEngine.Debug.LogWarning(stringWriter.ToString());
		stringWriter.Flush();
		stringWriter.Close();
	}

	private void LateUpdate()
	{
		NotifyListeners();
		RemoveSignalContexts();
	}

	private void NotifyListeners()
	{
		Dictionary<IFactChangedListener, HashSet<Fact>> dictionary = ConstructListenersToTheirFactsDictionary();
		factsChanged.Clear();
		foreach (IFactChangedListener key in dictionary.Keys)
		{
			key.OnFactsChanged(dictionary[key]);
		}
		dictionary = null;
	}

	private Dictionary<IFactChangedListener, HashSet<Fact>> ConstructListenersToTheirFactsDictionary()
	{
		listenersToTheirFacts.Clear();
		foreach (Fact item in factsChanged)
		{
			if (factsToTheirListeners.ContainsKey(item))
			{
				foreach (IFactChangedListener item2 in factsToTheirListeners[item])
				{
					if (!listenersToTheirFacts.ContainsKey(item2))
					{
						listenersToTheirFacts.Add(item2, new HashSet<Fact>());
					}
					listenersToTheirFacts[item2].Add(item);
				}
			}
		}
		return listenersToTheirFacts;
	}

	private void RemoveSignalContexts()
	{
		RemoveAllContexts((FactContext c) => c.isSignal);
	}

	private void RemoveAllContexts(Func<FactContext, bool> predicate)
	{
		factsToRemove.Clear();
		foreach (Fact key in factsDictionary.Keys)
		{
			HashSet<FactContext> hashSet = factsDictionary[key];
			hashSet.RemoveWhere((FactContext context) => predicate(context));
			if (hashSet.Count == 0)
			{
				factsToRemove.Add(key);
			}
		}
		foreach (Fact item in factsToRemove)
		{
			factsDictionary.Remove(item);
		}
	}

	public void OnGameplayStarted()
	{
		RemoveNullListeners();
	}

	public void OnGameplayRestarted()
	{
		RemoveAllContexts((FactContext c) => !c.isPersistent);
		RemoveNullListeners();
	}

	public void OnGameSavedFrequent()
	{
	}

	public void OnGameSavedInfrequent()
	{
	}

	private void RemoveNullListeners()
	{
		HashSet<Fact> hashSet = new HashSet<Fact>();
		List<Fact> list = factsToTheirListeners.Keys.ToList();
		foreach (Fact item in list)
		{
			factsToTheirListeners[item] = new HashSet<IFactChangedListener>(from listener in factsToTheirListeners[item]
				where listener != null
				select listener);
			if (factsToTheirListeners[item].Count == 0)
			{
				hashSet.Add(item);
			}
		}
		foreach (Fact item2 in hashSet)
		{
			factsToTheirListeners.Remove(item2);
		}
	}
}
