// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.Connection.MultiCrosspromoConnectionShow
using System.Collections;
using UnityEngine;

namespace TsgCommon.Crosspromo.Connection
{
	internal class MultiCrosspromoConnectionShow : MultiCrosspromoConnection
	{
		private long bannerId;

		public MultiCrosspromoConnectionShow(string gamename, string homeURL, string playerId, long bannerId)
			: base(gamename, homeURL, playerId)
		{
			this.bannerId = bannerId;
		}

		public void Show(MonoBehaviour coroutinesProvider)
		{
			base.running = true;
			coroutinesProvider.StartCoroutine(TrackShowAndDisableRunning(coroutinesProvider));
		}

		private IEnumerator TrackShowAndDisableRunning(MonoBehaviour coroutinesProvider)
		{
			yield return coroutinesProvider.StartCoroutine(TrackShow());
			base.running = false;
		}

		private IEnumerator TrackShow()
		{
			yield return CreateShowClickRequest("show", bannerId);
		}
	}
}
