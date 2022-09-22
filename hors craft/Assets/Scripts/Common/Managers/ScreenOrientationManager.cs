// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.ScreenOrientationManager
using UnityEngine;

namespace Common.Managers
{
	public class ScreenOrientationManager : Manager
	{
		public enum EOrientation
		{
			PORTRAIT,
			LANDSCAPE
		}

		public EOrientation orientation;

		public override void Init()
		{
			if (orientation == EOrientation.LANDSCAPE)
			{
				Screen.autorotateToLandscapeLeft = true;
				Screen.autorotateToLandscapeRight = true;
				Screen.autorotateToPortrait = false;
				Screen.autorotateToPortraitUpsideDown = false;
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
			else if (orientation == EOrientation.PORTRAIT)
			{
				Screen.autorotateToLandscapeLeft = false;
				Screen.autorotateToLandscapeRight = false;
				Screen.autorotateToPortrait = true;
				Screen.autorotateToPortraitUpsideDown = true;
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
		}
	}
}
