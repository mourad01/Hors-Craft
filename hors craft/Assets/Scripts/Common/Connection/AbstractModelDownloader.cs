// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Connection.AbstractModelDownloader
using System.Collections;

namespace Common.Connection
{
	public abstract class AbstractModelDownloader
	{
		public delegate void OnModelDownloaded(object jsonResult);

		protected string gamename;

		protected string homeURL;

		public AbstractModelDownloader(string gamename, string homeURL)
		{
			this.gamename = gamename;
			this.homeURL = homeURL;
		}

		public abstract IEnumerator DownloadModelCoroutine(OnModelDownloaded onModelDownloaded);
	}
}
