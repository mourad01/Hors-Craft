// DecompilerFi decompiler from Assembly-CSharp.dll class: RollercoasterThingOnTrack
using UnityEngine;

public abstract class RollercoasterThingOnTrack : ScriptableObject
{
	public abstract void Hit(RollercoasterController controller);

	public abstract void Setup(RollercoasterController.Direction direction, RollercoasterController.RollercoasterNode currentNode, Vector3 offset);
}
