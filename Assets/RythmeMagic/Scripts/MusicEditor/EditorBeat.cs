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

		[HideInInspector] public List<List<BeatPosInfo>> beatInfoList = new List<List<BeatPosInfo>>();

		public void Init(float _time)
		{
			time = _time;
			GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(time), 0);
		}

		protected override void OnDragEnd(BaseEventData arg0)
		{
			main.AdjustKeyInKeyList(this);
		}

		public void SaveBeatInfos(List<List<EditorBeat>> list)
		{
			beatInfoList.Clear();

			for(int i=0; i< list.Count; i++)
			{
				beatInfoList.Add(new List<BeatPosInfo>());
				foreach (var info in list[i])
				{
					beatInfoList[i].Add(new BeatPosInfo() { pos = info.pos, time = info.time });
				}
			}
		}
	}
}