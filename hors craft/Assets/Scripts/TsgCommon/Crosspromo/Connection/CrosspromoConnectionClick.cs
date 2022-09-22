// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.Connection.CrosspromoConnectionClick
using System.Collections;
using UnityEngine;

namespace TsgCommon.Crosspromo.Connection
{
	internal class CrosspromoConnectionClick : CrosspromoConnection
	{
		public CrosspromoConnectionClick(string gamename, string homeURL, string playerId)
			: base(gamename, homeURL, playerId)
		{
		}

		public void Click(MonoBehaviour coroutinesProvider)
		{
			base.running = true;
			coroutinesProvider.StartCoroutine(TrackClickAndDisableRunning(coroutinesProvider));
		}

		private IEnumerator TrackClickAndDisableRunning(MonoBehaviour coroutinesProvider)
		{
			yield return coroutinesProvider.StartCoroutine(TrackClick());
			base.running = false;
		}

		private IEnumerator TrackClick()
		{
			yield return CreateCrosspromoRequest("click");
		}
	}
}
