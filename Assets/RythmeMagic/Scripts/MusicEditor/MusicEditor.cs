using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RythhmMagic.MusicEditor
{
	public class MusicEditor : MonoBehaviour, IMusicEditor
	{
		protected MusicEditorMain main;
		public List<EditorBeat> beatList { get; private set; }

		[SerializeField] EditorBeat beatPrefab;

		// Start is called before the first frame update
		void Awake()
		{
			main = FindObjectOfType<MusicEditorMain>();
			beatList = new List<EditorBeat>();
		}

		//load all beats
		public void Init(List<MusicSheetObject.Beat> list)
		{
			foreach (var info in list)
			{
				var o = Instantiate(beatPrefab.gameObject);
				o.transform.SetParent(transform, false);

				var beat = o.GetComponent<EditorBeat>();
				beat.Init(info.startTime);
				for (int i = 0; i < info.infos.Count; i++)
				{
					if (i == 0)
					{
						foreach (var p in info.infos[i].posList)
							beat.leftInfos.Add(new BeatPosInfo() { pos = p.pos, time = p.time });
					}
					else if (i == 1)
					{
						foreach (var p in info.infos[i].posList)
							beat.rightInfos.Add(new BeatPosInfo() { pos = p.pos, time = p.time });
					}
				}
				AddBeatToList(beat);
			}
		}

		public virtual void OnClickAddBeat(float time)
		{
			//don't create key when key existed
			if (FindBeatByTime(time) != null)
				return;

			var o = Instantiate(beatPrefab.gameObject);
			o.transform.SetParent(transform, false);

			var beat = o.GetComponent<EditorBeat>();
			beat.Init(time);

			AddBeatToList(beat);
		}

		public void AdjustBeatInBeatList(EditorBeat key)
		{
			if (!beatList.Contains(key)) return;

			beatList.Remove(key);
			key.time = main.GetTimeByPosition(key.GetComponent<RectTransform>().anchoredPosition.x);

			//overried old key if existe
			var oldKey = FindBeatByTime(key.time);
			if (oldKey != null)
			{
				beatList.Remove(oldKey);
				Destroy(oldKey.gameObject);
			}

			//readd key in key list
			AddBeatToList(key);
		}

		void AddBeatToList(EditorBeat beat)
		{
			if (beatList.Count < 1)
			{
				beatList.Add(beat);
				return;
			}

			var index = 0;
			var beatTime = beat.time;

			for (int i = 0; i < beatList.Count; i++)
			{
				if (beatTime > beatList[i].time)
					index += 1;
				else
					break;
			}

			if (index >= beatList.Count)
				beatList.Add(beat);
			else
				beatList.Insert(index, beat);
		}

		public void OnClickRemoveKey(float time)
		{
			var key = FindBeatByTime(time);
			//return when can't find key
			if (key == null)
				return;

			beatList.Remove(key);
			Destroy(key.gameObject);
		}

		public void AdjustKeysPos()
		{
			//adjust all object
			foreach (var k in beatList)
			{
				k.GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(k.time), 0);
			}
		}

		public EditorBeat FindBeatByTime(float time)
		{
			foreach (var k in beatList)
			{
				if (Mathf.Abs(k.time - time) < .02) return k;
			}
			return null;
		}

		public EditorBeat FindClosestBeat(float targetTime, bool findNext)
		{
			EditorBeat closestKey = null;
			if (beatList.Count < 1) return closestKey;

			List<EditorBeat> list = new List<EditorBeat>();
			if (findNext) list = beatList.Where(k => k.time > targetTime).ToList();
			else list = beatList.Where(k => k.time < targetTime).ToList();

			var closestTime = float.MaxValue;

			foreach (var k in list)
			{
				var time = Mathf.Abs(k.time - targetTime);
				if (time < closestTime && time >= .02)
				{
					closestTime = time;
					closestKey = k;
				}
			}
			return closestKey;
		}
	}
}
