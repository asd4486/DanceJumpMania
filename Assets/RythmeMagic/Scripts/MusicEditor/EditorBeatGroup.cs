using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace RythhmMagic.MusicEditor
{
	public class EditorBeatGroup : MonoBehaviour
	{
		[HideInInspector] public List<EditorBeat> beatList = new List<EditorBeat>();
		public BeatPiste CurrentPiste { get; private set; }
		RectTransform myRect;

		public event System.Action<EditorBeatGroup> onAddBeatAction;
		public event System.Action<EditorBeatGroup> onRemoveBeatAction;
		public event System.Action<EditorBeatGroup> onDestroyAction;

		private void Awake()
		{
			myRect = GetComponent<RectTransform>();
		}

		public void Init(List<EditorBeat> beats, BeatPiste piste)
		{
			foreach (var b in beats)
				beatList.Add(b);

			beatList[0].onDragAction += SetGroupLenght;
			if (beatList.Count > 1) beatList[beatList.Count - 1].onDragAction += SetGroupLenght;
			SetGroupLenght();

			CurrentPiste = piste;
		}

		public void SetGroupLenght()
		{
			var startBeat = beatList[0];
			myRect.anchoredPosition = startBeat.rectTransfom.anchoredPosition;

			if (beatList.Count < 2)
			{
				myRect.sizeDelta = Vector2.zero;
				return;
			}

			var endBeat = beatList[beatList.Count - 1];
			var beatLenght = endBeat.rectTransfom.anchoredPosition.x - startBeat.rectTransfom.anchoredPosition.x;
			myRect.sizeDelta = new Vector2(beatLenght, myRect.sizeDelta.y);
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
			var oldBeat = FindBeatByTime(beat.time);
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
				var time = beat.time;
				for (int i = 0; i < beatList.Count; i++)
				{
					if (time > beatList[i].time)
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
				if (onDestroyAction != null) onDestroyAction(this);
				Destroy(gameObject, 0.1f);
			}
			else
			{
				SetGroupLenght();
				if (onRemoveBeatAction != null) onRemoveBeatAction(this);
			}
		}

		EditorBeat FindBeatByTime(float time)
		{
			foreach (var b in beatList)
				if (Mathf.Abs(b.time - time) < .02) return b;
			return null;
		}

		public Vector2 GetTimeCurrentPos(float _time, out EditorBeat _currentBeat)
		{
			if (beatList.Count < 2)
			{
				_currentBeat = beatList[0];
				return beatList[0].pos;
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
				if (Mathf.Abs(b.time - _time) < .02)
				{
					_currentBeat = b;
					return b.pos;
				}

				var timeDist = Mathf.Abs(_time - b.time);
				if (b.time < _time && timeDist < startTimeDist)
				{
					startPos = b.pos;
					startTime = b.time;
					startTimeDist = timeDist;
				}
				else if (b.time > _time && timeDist < endTimeDist)
				{
					endPos = b.pos;
					endTime = b.time;
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
				return Mathf.Abs(time - beatList[0].time) < 0.02f;
			return beatList[0].time <= time && beatList[beatList.Count - 1].time >= time;
		}
	}
}