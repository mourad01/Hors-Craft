// DecompilerFi decompiler from Assembly-CSharp.dll class: MiniGameExe
using Common.Managers;
using System;
using UnityEngine;

public abstract class MiniGameExe : ScriptableObject
{
	private static MiniGameExe _currentMinigame;

	protected Action injectedOnWin;

	protected Action injectedOnFail;

	public static MiniGameExe CurrentMinigame
	{
		get
		{
			if (_currentMinigame == null)
			{
				return null;
			}
			return _currentMinigame;
		}
		protected set
		{
			_currentMinigame = value;
		}
	}

	protected abstract StatsManager.MinigameType minigameType
	{
		get;
	}

	public virtual void Run(Action onWin, Action onFail, int difficultyLevel, bool useFinish = true)
	{
		CurrentMinigame = this;
		injectedOnWin = onWin;
		injectedOnFail = onFail;
		injectedOnWin = (Action)Delegate.Combine(injectedOnWin, new Action(ClearActions));
		injectedOnFail = (Action)Delegate.Combine(injectedOnFail, new Action(ClearActions));
		if (useFinish)
		{
			injectedOnFail = (Action)Delegate.Combine(injectedOnFail, new Action(Finish));
		}
		injectedOnWin = (Action)Delegate.Combine(injectedOnWin, (Action)delegate
		{
			Manager.Get<StatsManager>().MinigameFinished(minigameType, success: true);
		});
	}

	public virtual void Finish()
	{
		CurrentMinigame = null;
		Manager.Get<StatsManager>().MinigameFinished(minigameType, success: false);
	}

	public void InjectOnWin(Action onWin)
	{
		injectedOnWin = (Action)Delegate.Combine(injectedOnWin, onWin);
	}

	public void InjectOnFail(Action onFail)
	{
		injectedOnFail = (Action)Delegate.Combine(injectedOnFail, onFail);
	}

	private void ClearActions()
	{
		injectedOnWin = null;
		injectedOnFail = null;
	}
}
