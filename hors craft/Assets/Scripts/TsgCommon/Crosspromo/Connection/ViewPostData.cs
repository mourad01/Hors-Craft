// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.Connection.ViewPostData
using System;

namespace TsgCommon.Crosspromo.Connection
{
	[Serializable]
	public class ViewPostData : PostData
	{
		[Serializable]
		public class PostParams
		{
			public string app;

			public string platform;

			public string playerId;

			public int n;

			public bool preloading;

			public string[] neededTags;

			public bool withDuration;

			public bool allowGif;

			public PostParams(string app, string platform, string playedId, int n, bool preloading, string[] neededTags, bool withDuration, bool allowGif)
			{
				this.app = app;
				this.platform = platform;
				playerId = playedId;
				this.n = n;
				this.withDuration = withDuration;
				this.allowGif = allowGif;
				this.preloading = preloading;
				this.neededTags = neededTags;
			}
		}

		public PostParams parameters;

		public ViewPostData(string method, string id, string app, string platform, string playerId, int n, bool preloading, string[] neededTags, bool allowGif, bool withDuration)
		{
			base.method = method;
			base.id = id;
			parameters = new PostParams(app, platform, playerId, n, preloading, neededTags, allowGif, withDuration);
		}
	}
}
