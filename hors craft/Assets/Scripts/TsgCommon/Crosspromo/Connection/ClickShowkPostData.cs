// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.Connection.ClickShowkPostData
using System;

namespace TsgCommon.Crosspromo.Connection
{
	[Serializable]
	public class ClickShowkPostData : PostData
	{
		[Serializable]
		public class PostParams
		{
			public string app;

			public string platform;

			public string playerId;

			public long bannerId;

			public PostParams(string app, string platform, string playedId, long bannerId)
			{
				this.app = app;
				this.platform = platform;
				playerId = playedId;
				this.bannerId = bannerId;
			}
		}

		public PostParams parameters;

		public ClickShowkPostData(string method, string id, string app, string platform, string playerId, long bannerId)
		{
			base.method = method;
			base.id = id;
			parameters = new PostParams(app, platform, playerId, bannerId);
		}
	}
}
