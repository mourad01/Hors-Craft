// DecompilerFi decompiler from Assembly-CSharp.dll class: GooglePlayGames.Native.PInvoke.NativeScoreSummary
using GooglePlayGames.Native.Cwrapper;
using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeScoreSummary : BaseReferenceHolder
	{
		internal NativeScoreSummary(IntPtr selfPtr)
			: base(selfPtr)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			ScoreSummary.ScoreSummary_Dispose(selfPointer);
		}

		internal ulong ApproximateResults()
		{
			return ScoreSummary.ScoreSummary_ApproximateNumberOfScores(SelfPtr());
		}

		internal NativeScore LocalUserScore()
		{
			return NativeScore.FromPointer(ScoreSummary.ScoreSummary_CurrentPlayerScore(SelfPtr()));
		}

		internal static NativeScoreSummary FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new NativeScoreSummary(pointer);
		}
	}
}
