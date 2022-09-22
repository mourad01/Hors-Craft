// DecompilerFi decompiler from Assembly-CSharp.dll class: GooglePlayGames.Native.PInvoke.NativeLeaderboard
using GooglePlayGames.Native.Cwrapper;
using System;
using System.Runtime.InteropServices;

namespace GooglePlayGames.Native.PInvoke
{
	internal class NativeLeaderboard : BaseReferenceHolder
	{
		internal NativeLeaderboard(IntPtr selfPtr)
			: base(selfPtr)
		{
		}

		protected override void CallDispose(HandleRef selfPointer)
		{
			GooglePlayGames.Native.Cwrapper.Leaderboard.Leaderboard_Dispose(selfPointer);
		}

		internal string Title()
		{
			return PInvokeUtilities.OutParamsToString((byte[] out_string, UIntPtr out_size) => GooglePlayGames.Native.Cwrapper.Leaderboard.Leaderboard_Name(SelfPtr(), out_string, out_size));
		}

		internal static NativeLeaderboard FromPointer(IntPtr pointer)
		{
			if (pointer.Equals(IntPtr.Zero))
			{
				return null;
			}
			return new NativeLeaderboard(pointer);
		}
	}
}
