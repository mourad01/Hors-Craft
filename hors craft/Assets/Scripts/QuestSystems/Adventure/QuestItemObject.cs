// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestSystems.Adventure.QuestItemObject
namespace QuestSystems.Adventure
{
	public class QuestItemObject : GenericObject
	{
		protected Resource itemResource;

		public Resource Item => itemResource;

		public void SetItem(Resource resource)
		{
			itemResource = resource;
		}
	}
}
