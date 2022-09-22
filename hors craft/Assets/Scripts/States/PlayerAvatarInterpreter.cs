// DecompilerFi decompiler from Assembly-CSharp.dll class: States.PlayerAvatarInterpreter
using Common.Managers;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace States
{
	public class PlayerAvatarInterpreter : RankingDataInterpreter
	{
		public override KeyValuePair<string, string>? PrepareForSerialization()
		{
			if (PlayerGraphic.GetControlledPlayerInstance() != null)
			{
				return new KeyValuePair<string, string>("avatar", PlayerGraphic.GetControlledPlayerInstance().GetCurrentCloth(BodyPart.Head).ToString());
			}
			return null;
		}

		public override void SetElement(RankingScoreElement element, Dictionary<string, string> data, RankingModel.ScoreData score)
		{
			element.playerAvatar.gameObject.SetActive(value: true);
			SkinList skinList = (!(SkinList.customPlayerSkinList != null)) ? SkinList.instance : SkinList.customPlayerSkinList;
			if (data.ContainsKey("avatar") && int.TryParse(data["avatar"], NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
			{
				Sprite sprite = skinList.getSprite(BodyPart.Head, result);
				element.playerAvatar.sprite = sprite;
			}
			else
			{
				element.playerAvatar.sprite = skinList.getSprite(BodyPart.Head, 0);
			}
		}
	}
}
