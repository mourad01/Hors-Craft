// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityEngine.SocialPlatforms.GKAchievementReporter
namespace UnityEngine.SocialPlatforms
{
	public class GKAchievementReporter
	{
		public static void ReportAchievement(string achievementID, float progress, bool showsCompletionBanner)
		{
			Social.ReportProgress(achievementID, progress, null);
		}
	}
}
