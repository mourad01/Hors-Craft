// DecompilerFi decompiler from Assembly-CSharp.dll class: Weapon
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
	public int id;

	public Sprite sprite;

	public Sprite crosshair;

	public float damage;

	public bool isUnlockedAtStart;

	public bool setSpriteOnAttackButton;

	public bool hidePlayerHands;

	public bool canOwnMoreThanOne;

	[HideInInspector]
	public GameObject owner;

	public abstract void OnPress();

	public abstract void OnRelease();
}
