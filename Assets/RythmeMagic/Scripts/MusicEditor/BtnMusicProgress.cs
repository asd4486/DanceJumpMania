using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public class BtnMusicProgress : EditorKeyBase
	{
		public override void OnDragSetPos(BaseEventData arg0)
		{
			base.OnDragSetPos(arg0);

			Vector2 movePos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(main.RectRefPoint, Input.mousePosition, main.GetComponent<Canvas>().worldCamera, out movePos);
			var xPos = Mathf.Clamp(movePos.x, 0, main.mapWidth);
			//magnet mode set position to closest beat 
			if (main.moveMode == MoveModes.Magnet)
			{
				var closestBeat = main.currentEditor.FindClosestBeat(main.GetTimeByPosition(xPos));
				if (closestBeat != null && Mathf.Abs(xPos - closestBeat.rectTransfom.anchoredPosition.x) < 10)
					xPos = closestBeat.rectTransfom.anchoredPosition.x;
			}

			rectTransfom.anchoredPosition = new Vector2(xPos, 0);
		}
	}
}
