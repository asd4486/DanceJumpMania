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
		[HideInInspector] public float time;
		[HideInInspector] public Vector2 pos;

		[HideInInspector] public List<BeatPosInfo> leftInfos = new List<BeatPosInfo>();
		[HideInInspector] public List<BeatPosInfo> rightInfos = new List<BeatPosInfo>();

		public void Init(float _time)
		{
			time = _time;
			GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(time), 0);
		}

		protected override void OnDragEnd(BaseEventData arg0)
		{
			main.AdjustKeyInKeyList(this);
		}

		public void SaveBeatInfos(List<EditorBeat> list0, List<EditorBeat> list1)
		{
			leftInfos.Clear();
			rightInfos.Clear();

			foreach (var b in list0)
				leftInfos.Add(new BeatPosInfo() { pos = b.pos, time = b.time });
			foreach (var b in list1)
				rightInfos.Add(new BeatPosInfo() { pos = b.pos, time = b.time });
		}
	}
}