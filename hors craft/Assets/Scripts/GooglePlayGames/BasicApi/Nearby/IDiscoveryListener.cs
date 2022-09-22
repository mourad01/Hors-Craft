// DecompilerFi decompiler from Assembly-CSharp.dll class: GooglePlayGames.BasicApi.Nearby.IDiscoveryListener
namespace GooglePlayGames.BasicApi.Nearby
{
	public interface IDiscoveryListener
	{
		void OnEndpointFound(EndpointDetails discoveredEndpoint);

		void OnEndpointLost(string lostEndpointId);
	}
}
