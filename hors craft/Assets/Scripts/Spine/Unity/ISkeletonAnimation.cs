// DecompilerFi decompiler from Assembly-CSharp.dll class: Spine.Unity.ISkeletonAnimation
namespace Spine.Unity
{
	public interface ISkeletonAnimation
	{
		Skeleton Skeleton
		{
			get;
		}

		event UpdateBonesDelegate UpdateLocal;

		event UpdateBonesDelegate UpdateWorld;

		event UpdateBonesDelegate UpdateComplete;

		void LateUpdate();
	}
}
