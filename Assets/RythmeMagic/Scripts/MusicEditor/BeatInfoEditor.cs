using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public enum BeatPiste
	{
		Left,
		Right
	}

	public class BeatInfoEditor : MonoBehaviour, IMusicEditor
	{
		[SerializeField] BtnMusicProgress progressBtn;

		MusicEditorMain main;
		MarkerEditor markerEditor;

		List<EditorBeat> leftBeatInfos = new List<EditorBeat>();
		List<EditorBeat> rightBeatInfos = new List<EditorBeat>();
		EditorBeat editBeatL = new EditorBeat();
		EditorBeat editBeatR = new EditorBeat();

		[SerializeField] EditorBeat beatPrefab;

		EditorBeat selectedBeat;
		[SerializeField] RectTransform keyPosLine;

		[SerializeField] Button[] btnLines;
		Button selectedLine;

		private void Awake()
		{
			main = FindObjectOfType<MusicEditorMain>();
			markerEditor = FindObjectOfType<MarkerEditor>();
		}

		private void Start()
		{
			foreach (var btn in btnLines)
				btn.onClick.AddListener(() => OnClickSelectLine(btn));
			progressBtn.onDragAction += SetBeatPosToMarker;

			OnClickSelectLine(btnLines[0]);
		}

		public void SetBeatPosToMarker()
		{
			var time = main.GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);

			var beatL = FindClosestBeat(time, BeatPiste.Left);
			if (beatL != null)
			{
				//show beat if pointer is in a holding beat
				//if pointer is in the beat
				if ((leftBeatInfos.IndexOf(beatL) < leftBeatInfos.Count - 1 || FindBeatByTime(time) == beatL))
				{
					if (editBeatL != beatL)
					{
						editBeatL = beatL;
						markerEditor.SetMarkerBeat(0, beatL);
						markerEditor.ActiveMarker(0, true);
					}
				}
				else
					markerEditor.ActiveMarker(0, false);
			}
			else
				markerEditor.ActiveMarker(0, false);

			var beatR = FindClosestBeat(time, BeatPiste.Right);
			if (beatR != null)
			{
				//show beat if pointer is in a holding beat
				//if pointer is in the beat
				if ((rightBeatInfos.IndexOf(beatR) < rightBeatInfos.Count - 1 || FindBeatByTime(time) == beatR))
				{
					if (editBeatR != beatR)
					{
						editBeatR = beatR;
						markerEditor.SetMarkerBeat(1, beatR);
						markerEditor.ActiveMarker(1, true);
					}
				}
				else
					markerEditor.ActiveMarker(1, false);
			}
			else
				markerEditor.ActiveMarker(1, false);
		}

		public void Init(EditorBeat _beat)
		{
			selectedBeat = _beat;
			keyPosLine.anchoredPosition = new Vector2(selectedBeat.GetComponent<RectTransform>().anchoredPosition.x, 0);

			//clear old beat list
			foreach (var b in leftBeatInfos)
				Destroy(b.gameObject);
			foreach (var b in rightBeatInfos)
				Destroy(b.gameObject);

			leftBeatInfos.Clear();
			rightBeatInfos.Clear();

			// first set
			if (selectedBeat.leftBeatInfos.Count < 1 && selectedBeat.rightBeatInfos.Count < 1)
			{
				var o = Instantiate(beatPrefab.gameObject);
				o.transform.SetParent(btnLines[0].transform, false);

				var newBeat = o.GetComponent<EditorBeat>();
				//set selected beat time if is start point
				AddBeatToList(newBeat);
				return;
			}

			//load beat infos
			if (selectedBeat.leftBeatInfos.Count > 0)
			{
				for (int i = 0; i < selectedBeat.leftBeatInfos.Count; i++)
				{
					var info = selectedBeat.leftBeatInfos[i];

					var o = Instantiate(beatPrefab.gameObject);
					o.transform.SetParent(btnLines[0].transform, false);

					var newBeat = o.GetComponent<EditorBeat>();
					//set selected beat time if is start point
					newBeat.Init(info.time);
					newBeat.pos = info.pos;
					leftBeatInfos.Add(newBeat);
					SetBeatPosToMarker();
				}
			}

			if (selectedBeat.rightBeatInfos.Count > 0)
			{
				for (int i = 0; i < selectedBeat.rightBeatInfos.Count; i++)
				{
					var info = selectedBeat.rightBeatInfos[i];

					var o = Instantiate(beatPrefab.gameObject);
					o.transform.SetParent(btnLines[1].transform, false);

					var newBeat = o.GetComponent<EditorBeat>();
					//set selected beat time if is start point
					newBeat.Init(info.time);
					newBeat.pos = info.pos;
					rightBeatInfos.Add(newBeat);
					SetBeatPosToMarker();
				}
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
			//don't create key when key existed
			if (selectedLine == null || FindBeatByTime(time) != null) return;

			var o = Instantiate(beatPrefab.gameObject);
			o.transform.SetParent(selectedLine.transform, false);

			var beat = o.GetComponent<EditorBeat>();
			beat.Init(time);

			AddBeatToList(beat);
		}

		public void AdjustBeatInBeatList(EditorBeat key)
		{
			//top line or bottom line
			var beatList = selectedLine == btnLines[0] ? leftBeatInfos : rightBeatInfos;

			if (!beatList.Contains(key)) return;

			var beatRect = key.GetComponent<RectTransform>();
			if (beatRect.anchoredPosition.x < keyPosLine.anchoredPosition.x)
				beatRect.anchoredPosition = new Vector2(keyPosLine.anchoredPosition.x, 0);

			beatList.Remove(key);

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
			var piste = selectedLine == btnLines[0] ? BeatPiste.Left : BeatPiste.Right;
			//top line or bottom line
			var beatList = piste == BeatPiste.Left ? leftBeatInfos : rightBeatInfos;

			if (beatList.Count < 1)
			{
				beat.Init(selectedBeat.time);
				beatList.Add(beat);
				//refresh markers info
				SetBeatPosToMarker();
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

			//refresh markers info
			SetBeatPosToMarker();
		}

		public void OnClickRemoveKey(float time)
		{
			if (selectedLine == null) return;
			var piste = selectedLine == btnLines[0] ? BeatPiste.Left : BeatPiste.Right;
			//top line or bottom line
			var beatList = piste == BeatPiste.Left ? leftBeatInfos : rightBeatInfos;

			var key = FindBeatByTime(time);
			//return when can't find key
			if (key == null)
				return;

			beatList.Remove(key);
			Destroy(key.gameObject);

			//refresh markers info
			SetBeatPosToMarker();
		}

		public void AdjustKeysPos()
		{
			keyPosLine.anchoredPosition = new Vector2(main.GetPositionByTime(selectedBeat.time), 0);
			//adjust all object
			foreach (var b in leftBeatInfos)
				b.GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(b.time), 0);
			foreach (var b in rightBeatInfos)
				b.GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(b.time), 0);
		}

		public EditorBeat FindBeatByTime(float time)
		{
			if (selectedLine == null) return null;
			//top line or bottom line
			var beatList = selectedLine == btnLines[0] ? leftBeatInfos : rightBeatInfos;

			foreach (var k in beatList)
				if (Mathf.Abs(k.time - time) < .02) return k;

			return null;
		}

		public EditorBeat FindClosestBeat(float targetTime)
		{
			if (selectedLine == null) return null;
			//top line or bottom line
			var beatList = selectedLine == btnLines[0] ? leftBeatInfos : rightBeatInfos;
			return GetClosestBeat(targetTime, beatList);
		}

		public EditorBeat FindClosestBeat(float targetTime, bool findNext)
		{
			if (selectedLine == null) return null;
			//top line or bottom line
			var beatList = selectedLine == btnLines[0] ? leftBeatInfos : rightBeatInfos;

			if (findNext) beatList = beatList.Where(k => k.time > targetTime).ToList();
			else beatList = beatList.Where(k => k.time < targetTime).ToList();

			return GetClosestBeat(targetTime, beatList);
		}

		//find current line closest beat
		//for showing marker editor infos
		EditorBeat FindClosestBeat(float targetTime, BeatPiste piste)
		{
			var beatList = piste == BeatPiste.Left ? leftBeatInfos : rightBeatInfos;
			beatList = beatList.Where(k => k.time <= targetTime).ToList();
			return GetClosestBeat(targetTime, beatList);
		}

		EditorBeat GetClosestBeat(float targetTime, List<EditorBeat> beatList)
		{
			EditorBeat closestBeat = null;
			if (beatList.Count < 1) return closestBeat;

			var closestTime = float.MaxValue;
			foreach (var k in beatList)
			{
				var time = Mathf.Abs(k.time - targetTime);
				if (time < closestTime)
				{
					closestTime = time;
					closestBeat = k;
				}
			}
			return closestBeat;
		}

		public void QuitAndSave()
		{
			markerEditor.HideAllMarkers();

			if (selectedBeat == null) return;
			selectedBeat.SaveBeatInfos(leftBeatInfos, rightBeatInfos);
		}
	}
}