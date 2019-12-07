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
			foreach (var beatInfos in list)
			{
				var o = Instantiate(beatPrefab.gameObject);
				o.transform.SetParent(transform, false);

				var newBeat = o.GetComponent<EditorBeat>();
				newBeat.Init(beatInfos.startTime);

				foreach (var info in beatInfos.infos)
				{
					var posInfos = new List<BeatPosInfo>();
					foreach (var b in info.posList)
						posInfos.Add(new BeatPosInfo() { time = b.time, pos = b.pos });
					newBeat.beatInfoList.Add(posInfos);
				}

				AddBeatToList(newBeat);
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

		public void AdjustBeatInBeatList(EditorBeat _beat)
		{
			if (!beatList.Contains(_beat)) return;

			beatList.Remove(_beat);

			//overried old key if existe
			var oldKey = FindBeatByTime(_beat.time);
			if (oldKey != null)
			{
				beatList.Remove(oldKey);
				Destroy(oldKey.gameObject);
			}

			//readd key in key list
			AddBeatToList(_beat);
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

		public EditorBeat FindClosestBeat(float targetTime)
		{
			return GetClosestBeat(targetTime, beatList);
		}

		public EditorBeat FindClosestBeat(float targetTime, bool findNext)
		{
			List<EditorBeat> list = new List<EditorBeat>();
			if (findNext) list = beatList.Where(k => k.time > targetTime).ToList();
			else list = beatList.Where(k => k.time < targetTime).ToList();

			return GetClosestBeat(targetTime, list);
		}

		EditorBeat GetClosestBeat(float targetTime, List<EditorBeat> list)
		{
			EditorBeat closestBeat = null;
			if (beatList.Count < 1) return null;
			var closestTime = float.MaxValue;

			foreach (var k in list)
			{
				var time = Mathf.Abs(k.time - targetTime);
				if (time < closestTime && time >= .02)
				{
					closestTime = time;
					closestBeat = k;
				}
			}
			return closestBeat;
		}
	}
}
