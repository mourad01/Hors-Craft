// DecompilerFi decompiler from Assembly-CSharp.dll class: GooglePlayGames.BasicApi.SavedGame.IConflictResolver
namespace GooglePlayGames.BasicApi.SavedGame
{
	public interface IConflictResolver
	{
		void ChooseMetadata(ISavedGameMetadata chosenMetadata);

		void ResolveConflict(ISavedGameMetadata chosenMetadata, SavedGameMetadataUpdate metadataUpdate, byte[] updatedData);
	}
}
