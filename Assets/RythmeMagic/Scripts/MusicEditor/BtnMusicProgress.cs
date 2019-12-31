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
		public event System.Action onSetPosAction;
		public override void OnDragSetPos(BaseEventData arg0)
		{
			Vector2 movePos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(main.RectRefPoint, Input.mousePosition, main.GetComponent<Canvas>().worldCamera, out movePos);
			var xPos = Mathf.Clamp(movePos.x, 0, main.mapWidth);
			//magnet mode set position to closest beat 
			if (main.moveMode == MoveModes.Magnet)
			{
				var closestBeat = main.AttractToBeat(xPos);
				if (closestBeat != null) xPos = closestBeat.rectTransfom.anchoredPosition.x;
			}

			SetXPos(xPos);
			base.OnDragSetPos(arg0);
		}

		public void SetXPos(float xPos)
		{
			rectTransfom.anchoredPosition = new Vector2(xPos, 0);
			if (onSetPosAction != null) onSetPosAction();
		}
	}
}
