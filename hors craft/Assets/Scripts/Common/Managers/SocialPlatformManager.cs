// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.SocialPlatformManager
using Common.Managers.SocialPlatforms;

namespace Common.Managers
{
	public class SocialPlatformManager : Manager
	{
		private ISocialPlatform instantiatedSocial;

		public ISocialPlatform social
		{
			get
			{
				if (instantiatedSocial == null)
				{
					AssignSocialPlatform();
				}
				return instantiatedSocial;
			}
		}

		private void AssignSocialPlatform()
		{
			if (Manager.Contains<RankingManager>())
			{
				instantiatedSocial = Manager.Get<RankingManager>();
			}
			else
			{
				instantiatedSocial = GetSocialPlatform();
			}
		}

		private ISocialPlatform GetSocialPlatform()
		{
			return new DummySocialPlatform();
		}

		public override void Init()
		{
			social.Init();
		}
	}
}
