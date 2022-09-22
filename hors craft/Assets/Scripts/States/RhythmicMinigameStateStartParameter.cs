// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RhythmicMinigameStateStartParameter
using Common.Managers.States;
using Gameplay.RhythmicMinigame;
using System;

namespace States
{
	public class RhythmicMinigameStateStartParameter : StartParameter
	{
		public RhythmReactor graphicScene;

		public Action onWin;

		public Action onFail;
	}
}
