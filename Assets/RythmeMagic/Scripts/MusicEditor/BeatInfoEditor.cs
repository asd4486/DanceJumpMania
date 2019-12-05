using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public class BeatInfoEditor : MonoBehaviour, IMusicEditor
	{
		MusicEditorMain main;

		List<EditorBeat> leftInfos = new List<EditorBeat>();
		List<EditorBeat> rightInfoList = new List<EditorBeat>();

		[SerializeField] EditorBeat beatPrefab;

		EditorBeat selectedBeat;
		[SerializeField] RectTransform keyPosLine;

		[SerializeField] Button[] btnLines;
		Button selectedLine;

		private void Start()
		{
			main = FindObjectOfType<MusicEditorMain>();
			foreach (var btn in btnLines)
				btn.onClick.AddListener(() => OnClickSelectLine(btn));
		}

		public void Init(EditorBeat beat)
		{
			//clear old beat list
			foreach (var b in leftInfos) Destroy(b.gameObject);
			foreach (var b in rightInfoList) Destroy(b.gameObject);
			leftInfos.Clear();
			rightInfoList.Clear();

			selectedBeat = beat;
			keyPosLine.anchoredPosition = new Vector2(selectedBeat.GetComponent<RectTransform>().anchoredPosition.x, 0);

			//load beat infos
			foreach (var info in selectedBeat.leftInfos)
			{
				var o = Instantiate(beatPrefab.gameObject);
				o.transform.SetParent(btnLines[0].transform, false);

				var b = o.GetComponent<EditorBeat>();
				//set selected beat time if is start point
				b.Init(selectedBeat.leftInfos.IndexOf(info) == 0 ? selectedBeat.time : info.time);
				leftInfos.Add(b);
			}

			foreach (var info in selectedBeat.rightInfos)
			{
				var o = Instantiate(beatPrefab.gameObject);
				o.transform.SetParent(btnLines[1].transform, false);

				var b = o.GetComponent<EditorBeat>();
				//set selected beat time if is start point
				b.Init(selectedBeat.rightInfos.IndexOf(info) == 0 ? selectedBeat.time : info.time);
				rightInfoList.Add(b);
			}
		}

		void OnClickSelectLine(Button line)
		{
			selectedLine = line;
			foreach (var l in btnLines)
			{
				l.GetComponent<Image>().color = l == selectedLine ? new Color(1, 1, 0, 0.5f) : new Color(0, 0, 0, 0.01f);
			}
		}

		public void OnClickAddBeat(float time)
		{
			if (selectedLine == null) return;

			//don't create key when key existed
			if (FindBeatByTime(time) != null)
				return;

			var o = Instantiate(beatPrefab.gameObject);
			o.transform.SetParent(selectedLine.transform, false);

			var beat = o.GetComponent<EditorBeat>();
			beat.Init(time);

			AddBeatToList(beat);
		}

		public void AdjustBeatInBeatList(EditorBeat key)
		{
			//top line
			var beatList = selectedLine == btnLines[0] ? leftInfos : rightInfoList;

			if (!beatList.Contains(key)) return;

			var beatRect = key.GetComponent<RectTransform>();
			if (beatRect.anchoredPosition.x < keyPosLine.anchoredPosition.x)
				beatRect.anchoredPosition = new Vector2(keyPosLine.anchoredPosition.x, 0);

			beatList.Remove(key);
			key.time = main.GetTimeByPosition(beatRect.anchoredPosition.x);

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
			//top line
			var beatList = selectedLine == btnLines[0] ? leftInfos : rightInfoList;

			if (beatList.Count < 1)
			{
				beat.Init(selectedBeat.time);
				beatList.Add(beat);
				return;
			}

			var index = 0;
			var keyTime = beat.time;

			for (int i = 0; i < beatList.Count; i++)
			{
				if (keyTime > beatList[i].time)
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
			if (selectedLine == null) return;
			var beatList = selectedLine == btnLines[0] ? leftInfos : rightInfoList;

			var key = FindBeatByTime(time);
			//return when can't find key
			if (key == null)
				return;

			beatList.Remove(key);
			Destroy(key.gameObject);
		}

		public void AdjustKeysPos()
		{
			keyPosLine.anchoredPosition = new Vector2(main.GetPositionByTime(selectedBeat.time), 0);
			//adjust all object
			foreach (var k in leftInfos)
				k.GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(k.time), 0);
			foreach (var b in rightInfoList)
				b.GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(b.time), 0);
		}

		EditorBeat FindBeatByTime(float time)
		{
			if (selectedLine == null) return null;
			var beatList = selectedLine == btnLines[0] ? leftInfos : rightInfoList;

			foreach (var k in beatList)
			{
				if (Mathf.Abs(k.time - time) < .02) return k;
			}
			return null;
		}

		public EditorBeat FindClosestBeat(float targetTime, bool findNext)
		{
			if (selectedLine == null) return null;
			var beatList = selectedLine == btnLines[0] ? leftInfos : rightInfoList;

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

		public void SaveChanges()
		{
			if (selectedBeat == null) return;
			selectedBeat.SaveBeatInfos(leftInfos, rightInfoList);
		}
	}
}