// DecompilerFi decompiler from Assembly-CSharp.dll class: States.IsometricObjectPlacementStateStartParameter
using Common.Managers.States;
using Uniblocks;

namespace States
{
	public class IsometricObjectPlacementStateStartParameter : StartParameter
	{
		public IsometricPlaceableObject obj;

		public bool canRotate = true;
	}
}
