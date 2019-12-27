using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RythhmMagic.MusicEditor
{
	public class BeatPosInfo
	{
		public float time;
		public Vector2 pos;
	}

	public class EditorBeat : EditorKeyBase
	{
		BtnMusicProgress progressBtn;

		[HideInInspector] public float time;
		[HideInInspector] public Vector2 pos;

		[HideInInspector] public List<BeatPosInfo> leftBeatInfos = new List<BeatPosInfo>();
		[HideInInspector] public List<BeatPosInfo> rightBeatInfos = new List<BeatPosInfo>();

		private void Start()
		{
			progressBtn = FindObjectOfType<BtnMusicProgress>();
		}

		public void Init(float _time)
		{
			time = _time;
			rectTransfom.anchoredPosition = new Vector2(main.GetPositionByTime(time), 0);
		}

		public override void OnDragSetPos(BaseEventData arg0)
		{
			base.OnDragSetPos(arg0);

			Vector2 movePos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(main.RectRefPoint, Input.mousePosition, main.GetComponent<Canvas>().worldCamera, out movePos);
			var xPos = Mathf.Clamp(movePos.x, 0, main.mapWidth);
			//magnet mode set position to progress button when close 
			if (main.moveMode == MoveModes.Magnet)
			{
				if (progressBtn != null && Mathf.Abs(xPos - progressBtn.rectTransfom.anchoredPosition.x) < 10)
					xPos = progressBtn.rectTransfom.anchoredPosition.x;
			}

			rectTransfom.anchoredPosition = new Vector2(xPos, 0);
		}

		protected override void OnDragEnd(BaseEventData arg0)
		{
			var newTime = main.GetTimeByPosition(rectTransfom.anchoredPosition.x);

			//update beat infos
			var timeDifference = newTime - time;

			foreach (var info in leftBeatInfos)
				info.time += timeDifference;
			foreach (var info in rightBeatInfos)
				info.time += timeDifference;

			//update time info
			time = newTime;
			main.AdjustBeatInBeatList(this);
		}

		public void SaveBeatInfos(List<EditorBeat> leftList, List<EditorBeat> rightList)
		{
			leftBeatInfos.Clear();
			rightBeatInfos.Clear();

			foreach (var info in leftList)
			{
				leftBeatInfos.Add(new BeatPosInfo()
				{
					pos = new Vector2((float)Math.Round(info.pos.x, 2), (float)Math.Round(info.pos.y, 2)),
					time = info.time
				});
			}

			foreach (var info in rightList)
			{
				rightBeatInfos.Add(new BeatPosInfo()
				{
					pos = new Vector2((float)Math.Round(info.pos.x, 2), (float)Math.Round(info.pos.y, 2)),
					time = info.time
				});
			}
		}
	}
}