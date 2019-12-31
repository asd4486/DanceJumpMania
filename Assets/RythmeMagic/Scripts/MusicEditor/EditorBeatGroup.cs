using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace RythhmMagic.MusicEditor
{
	public class EditorBeatGroup : MonoBehaviour
	{
		[HideInInspector] public List<EditorBeat> beatList = new List<EditorBeat>();
		RectTransform myRect;

		public event System.Action<EditorBeatGroup> onDestroyAction;

		private void Awake()
		{
			myRect = GetComponent<RectTransform>();
		}

		public void Init(List<EditorBeat> beats)
		{
			foreach (var b in beats)
			{
				beatList.Add(b);
			}

			if (beatList.Count > 1)
			{
				beatList[0].onDragAction += SetGroupLenght;
				beatList[beatList.Count - 1].onDragAction += SetGroupLenght;
			}
		}

		public void SetGroupLenght()
		{

		}

		public void AdjustBeatInList(EditorBeat beat)
		{
			if (!beatList.Contains(beat)) return;

			var beatRect = beat.rectTransfom;

			beatList.Remove(beat);

			//overried old key if existe
			var oldBeat = FindBeatByTime(beat.time);
			if (oldBeat != null && oldBeat != beat)
			{
				beatList.Remove(oldBeat);
				Destroy(oldBeat.gameObject);
			}

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
				Destroy(oldBeat);
			}

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
				beatList[beatList.Count - 1].onDragAction -= SetGroupLenght;
				beatList.Add(beat);
				beat.onDragAction += SetGroupLenght;
			}
			else
			{
				beatList.Insert(index, beat);
				if (index == 0)
				{
					//remove old zero index
					beatList[1].onDragAction -= SetGroupLenght;
					beat.onDragAction += SetGroupLenght;
				}
			}
		}

		public void RemoveBeat(EditorBeat beat)
		{
			if (!beatList.Contains(beat))
				return;

			beatList.Remove(beat);
			Destroy(beat);

			if (beatList.Count < 1)
			{
				if (onDestroyAction != null) onDestroyAction(this);
				Destroy(this);
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

			foreach (var b in beatList)
			{
				//return beat pos if can directly find beat by time
				if (Mathf.Abs(b.time - _time) < .02)
				{
					_currentBeat = b;
					return b.pos;
				}

				if (b.time < _time && b.time < startTime)
				{
					startPos = b.pos;
					startTime = b.time;
				}
				else if (b.time > _time && b.time < endTime)
				{
					endPos = b.pos;
					endTime = b.time;
				}
			}

			_currentBeat = null;
			var dist = new Vector2(startPos.x + endPos.x, startPos.y + endPos.y);
			var precent = (_time - startTime) / (endTime - startTime);

			return startPos + dist * precent;
		}

		public bool CheckTimeInRange(float time)
		{
			if (beatList.Count < 2)
				return Mathf.Abs(time - beatList[0].time) < 0.02f;
			return beatList[0].time <= time && beatList[beatList.Count - 1].time >= time;
		}
	}
}