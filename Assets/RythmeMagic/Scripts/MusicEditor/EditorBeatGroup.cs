using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace RythhmMagic.MusicEditor
{
	//for copy paste
	public class BeatGroupInfo
	{
		public List<BeatInfo> beatInfoList = new List<BeatInfo>();
		public MarkerType markerType;
		public BeatGroupInfo(List<BeatInfo> infos, MarkerType type)
		{
			beatInfoList = infos;
			markerType = type;
		}
	}

	public class EditorBeatGroup : EditorKeyBase
	{
		[HideInInspector] public List<EditorBeat> beatList = new List<EditorBeat>();
		[HideInInspector] public BeatPiste CurrentPiste;
		[HideInInspector] public MarkerType markerType;

		public event System.Action<EditorBeatGroup> onAddBeatAction;
		public event System.Action<EditorBeatGroup> onRemoveBeatAction;
		public event System.Action<EditorBeatGroup> onDestroyAction;

		public void Init(List<EditorBeat> beats, BeatPiste piste, MarkerType type = MarkerType.Default)
		{
			foreach (var b in beats)
				beatList.Add(b);

			beatList[0].onDragAction += SetGroupLenght;
			if (beatList.Count > 1) beatList[beatList.Count - 1].onDragAction += SetGroupLenght;
			SetGroupLenght();

			CurrentPiste = piste;
			markerType = type;
		}

		public void SetGroupLenght()
		{
			var startBeat = beatList[0];
			rectTransfom.anchoredPosition = startBeat.rectTransfom.anchoredPosition;

			if (beatList.Count < 2)
			{
				rectTransfom.sizeDelta = Vector2.zero;
				return;
			}

			var endBeat = beatList[beatList.Count - 1];
			var beatLenght = endBeat.rectTransfom.anchoredPosition.x - startBeat.rectTransfom.anchoredPosition.x;
			rectTransfom.sizeDelta = new Vector2(beatLenght, rectTransfom.sizeDelta.y);
			col.size = new Vector2(rectTransfom.sizeDelta.x, col.size.y);
			col.offset = new Vector2(rectTransfom.sizeDelta.x / 2, 0);
		}

		public void AdjustBeatInList(EditorBeat beat)
		{
			if (!beatList.Contains(beat)) return;

			var beatRect = beat.rectTransfom;

			beatList.Remove(beat);

			//readd key in key list
			AddBeat(beat);
		}

		public void AddBeat(EditorBeat beat)
		{
			//replace oldbeat by new beat
			var oldBeat = FindBeatByTime(beat.info.time);
			if (oldBeat != null)
			{
				beatList.Remove(oldBeat);
				Destroy(oldBeat.gameObject);
			}

			if (beatList.Count < 1)
			{
				beatList.Add(beat);
				beatList[0].onDragAction += SetGroupLenght;
				SetGroupLenght();
			}
			else
			{
				beatList[0].onDragAction -= SetGroupLenght;
				beatList[beatList.Count - 1].onDragAction -= SetGroupLenght;

				var index = 0;
				var time = beat.info.time;
				for (int i = 0; i < beatList.Count; i++)
				{
					if (time > beatList[i].info.time)
						index += 1;
					else
						break;
				}

				if (index >= beatList.Count)
				{
					beatList.Add(beat);
					SetGroupLenght();
				}
				else
				{
					beatList.Insert(index, beat);
					if (index == 0) SetGroupLenght();
				}

				beatList[0].onDragAction += SetGroupLenght;
				beatList[beatList.Count - 1].onDragAction += SetGroupLenght;
			}

			if (onAddBeatAction != null) onAddBeatAction(this);
		}

		public void RemoveBeat(EditorBeat beat)
		{
			if (!beatList.Contains(beat))
				return;

			beatList.Remove(beat);
			Destroy(beat.gameObject);

			if (beatList.Count < 1)
			{
				Destroy();
			}
			else
			{
				SetGroupLenght();
				if (onRemoveBeatAction != null) onRemoveBeatAction(this);
			}
		}

		public void Destroy()
		{
			foreach (var b in beatList)
			{
				Destroy(b.gameObject);
			}

			if (onDestroyAction != null) onDestroyAction(this);
			Destroy(gameObject, 0.05f);
		}

		EditorBeat FindBeatByTime(float time)
		{
			foreach (var b in beatList)
				if (Mathf.Abs(b.info.time - time) < .02) return b;
			return null;
		}

		public Vector2 GetTimeCurrentPos(float _time, out EditorBeat _currentBeat)
		{
			if (beatList.Count < 2)
			{
				_currentBeat = beatList[0];
				return beatList[0].info.pos;
			}

			var startPos = Vector2.zero;
			var endPos = Vector2.zero;
			var startTime = float.MaxValue;
			var endTime = float.MaxValue;

			var startTimeDist = float.MaxValue;
			var endTimeDist = float.MaxValue;

			foreach (var b in beatList)
			{
				//return beat pos if can directly find beat by time
				if (Mathf.Abs(b.info.time - _time) < .02)
				{
					_currentBeat = b;
					return b.info.pos;
				}

				var timeDist = Mathf.Abs(_time - b.info.time);
				if (b.info.time < _time && timeDist < startTimeDist)
				{
					startPos = b.info.pos;
					startTime = b.info.time;
					startTimeDist = timeDist;
				}
				else if (b.info.time > _time && timeDist < endTimeDist)
				{
					endPos = b.info.pos;
					endTime = b.info.time;
					endTimeDist = timeDist;
				}
			}

			_currentBeat = null;
			if (startPos == endPos)
				return startPos;

			var posDist = endPos - startPos;
			var precent = 1 - (_time - startTime) / (endTime - startTime);
			return endPos - posDist * precent;
		}

		public bool CheckTimeInRange(float time)
		{
			if (beatList.Count < 2)
				return Mathf.Abs(time - beatList[0].info.time) < 0.02f;
			return beatList[0].info.time <= time && beatList[beatList.Count - 1].info.time >= time;
		}

		float dragOffset;
		public override void OnDragStart(float xPos)
		{
			dragOffset = xPos - rectTransfom.anchoredPosition.x;
			//disable set group lenght action
			beatList[0].onDragAction -= SetGroupLenght;
			beatList[beatList.Count - 1].onDragAction -= SetGroupLenght;
		}

		public override void OnDragSetPos(float xPos)
		{
			var oldPos = rectTransfom.anchoredPosition.x;

			var movePos = xPos - dragOffset;
			if (main.moveMode == MoveModes.Magnet)
			{
				var progressTime = main.GetProgressTime();
				if (Mathf.Abs(progressTime - main.GetTimeByPosition(movePos)) < 0.1f)
					movePos = main.GetPositionByTime(progressTime);
			}

			rectTransfom.anchoredPosition = new Vector2(movePos, 0);

			foreach (var b in beatList)
				b.GroupDragSetPos(rectTransfom.anchoredPosition.x - oldPos);
		}

		public override void OnDragEnd()
		{
			foreach (var b in beatList)
			{
				var newTime = main.GetTimeByPosition(b.rectTransfom.anchoredPosition.x);
				b.info.time = newTime;
			}

			beatList[0].onDragAction += SetGroupLenght;
			beatList[beatList.Count - 1].onDragAction += SetGroupLenght;
		}
	}
}