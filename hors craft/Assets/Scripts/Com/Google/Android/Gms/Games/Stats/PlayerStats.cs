// DecompilerFi decompiler from Assembly-CSharp.dll class: Com.Google.Android.Gms.Games.Stats.PlayerStats
namespace Com.Google.Android.Gms.Games.Stats
{
	public interface PlayerStats
	{
		float getAverageSessionLength();

		float getChurnProbability();

		int getDaysSinceLastPlayed();

		int getNumberOfPurchases();

		int getNumberOfSessions();

		float getSessionPercentile();

		float getSpendPercentile();

		float getSpendProbability();

		float getHighSpenderProbability();

		float getTotalSpendNext28Days();
	}
}
