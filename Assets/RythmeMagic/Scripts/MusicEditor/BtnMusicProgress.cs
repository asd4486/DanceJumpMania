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
		public override void OnDragSetPos(float xPos)
		{
			//magnet mode set position to closest beat 
			if (main.moveMode == MoveModes.Magnet)
			{
				var closestBeat = main.AttractToBeat(xPos);
				if (closestBeat != null) xPos = closestBeat.rectTransfom.anchoredPosition.x;
			}

			SetXPos(xPos);
			base.OnDragSetPos(xPos);
		}

		public void SetXPos(float xPos)
		{
			rectTransfom.anchoredPosition = new Vector2(xPos, 0);
			if (onSetPosAction != null) onSetPosAction();
		}
	}
}
