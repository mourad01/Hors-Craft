// DecompilerFi decompiler from Assembly-CSharp.dll class: Spine.TextureLoader
namespace Spine
{
	public interface TextureLoader
	{
		void Load(AtlasPage page, string path);

		void Unload(object texture);
	}
}
