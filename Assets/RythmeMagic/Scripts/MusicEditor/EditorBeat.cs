using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RythhmMagic.MusicEditor
{

	public class EditorBeat : EditorKeyBase
	{
		BtnMusicProgress progressBtn;
		[HideInInspector] public float time;
		[HideInInspector] public Vector2 pos;

		public EditorBeatGroup currentGroup { get; private set; }
		public event System.Action<EditorBeat> onDragEndAction;

		private void Start()
		{
			progressBtn = FindObjectOfType<BtnMusicProgress>();
		}

		public void Init(float _time, Vector2 _pos, EditorBeatGroup _group)
		{
			time = _time;
			pos = _pos;
			currentGroup = _group;
			rectTransfom.anchoredPosition = new Vector2(main.GetPositionByTime(time), 0);
		}

		public override void OnDragSetPos(float xPos)
		{
			//magnet mode set position to progress button when close 
			if (main.moveMode == MoveModes.Magnet)
			{
				if (progressBtn != null && Mathf.Abs(xPos - progressBtn.rectTransfom.anchoredPosition.x) < 10)
					xPos = progressBtn.rectTransfom.anchoredPosition.x;
			}

			rectTransfom.anchoredPosition = new Vector2(xPos, 0);
			base.OnDragSetPos(xPos);
		}

		public override void OnDragEnd()
		{
			var newTime = main.GetTimeByPosition(rectTransfom.anchoredPosition.x);
			//update time info
			time = newTime;

			if (onDragEndAction != null) onDragEndAction(this);
		}
	}
}