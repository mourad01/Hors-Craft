// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.RotationsUtility
namespace Uniblocks
{
	public static class RotationsUtility
	{
		public static Facing RotateFacing(Facing facing, byte rotation)
		{
			int facingNumber = GetFacingNumber(facing);
			if (facingNumber < 0)
			{
				return facing;
			}
			return NumberToFace((facingNumber + rotation) % 4);
		}

		public static int GetFacingNumber(Facing facing)
		{
			switch (facing)
			{
			case Facing.forward:
				return 0;
			case Facing.right:
				return 1;
			case Facing.back:
				return 2;
			case Facing.left:
				return 3;
			default:
				return -1;
			}
		}

		public static Facing NumberToFace(int facing)
		{
			switch (facing)
			{
			case 0:
				return Facing.forward;
			case 1:
				return Facing.right;
			case 2:
				return Facing.back;
			case 3:
				return Facing.left;
			default:
				return Facing.up;
			}
		}

		public static MeshRotation RotateMeshRotation(byte rotation, MeshRotation basicRotation)
		{
			for (int i = 0; i < rotation; i++)
			{
				switch (basicRotation)
				{
				default:
					basicRotation = MeshRotation.right;
					break;
				case MeshRotation.right:
					basicRotation = MeshRotation.back;
					break;
				case MeshRotation.back:
					basicRotation = MeshRotation.left;
					break;
				case MeshRotation.left:
					basicRotation = MeshRotation.none;
					break;
				}
			}
			return basicRotation;
		}

		public static float MeshRotationToDegrees(MeshRotation meshRot)
		{
			switch (meshRot)
			{
			case MeshRotation.back:
				return 180f;
			case MeshRotation.right:
				return 90f;
			case MeshRotation.left:
				return -90f;
			default:
				return 0f;
			}
		}
	}
}
