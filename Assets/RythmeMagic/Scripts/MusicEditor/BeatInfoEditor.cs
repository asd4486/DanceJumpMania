using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public class BeatInfoEditor : MonoBehaviour, IMusicEditor
	{
		[SerializeField] BtnMusicProgress progressBtn;

		MusicEditorMain main;
		MarkerEditor markerEditor;

		List<List<EditorBeat>> beatInfoList = new List<List<EditorBeat>>();
		EditorBeat[] editBeatInfos = new EditorBeat[2];

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
			progressBtn.onDragAction += SetBeatInfoPosToMarker;
			markerEditor.onDragMarkerAction += SetMarkerPosToBeatInfo;

			OnClickSelectLine(btnLines[0]);
		}

		public void SetBeatInfoPosToMarker()
		{
			var time = main.GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);
			for (int i = 0; i < beatInfoList.Count; i++)
			{
				var beat = FindClosestBeat(time, i);
				if (beat != null && editBeatInfos[i] != beat)
				{
					editBeatInfos[i] = beat;
					markerEditor.SetMarkerPos(i, beat.pos);
				}
			}
		}

		private void SetMarkerPosToBeatInfo()
		{
			for (int i = 0; i < editBeatInfos.Length; i++)
			{
				var info = editBeatInfos[i];
				if (info != null)
					info.pos = markerEditor.GetMarkerPos(i);
			}
		}

		public void Init(EditorBeat _beat)
		{
			selectedBeat = _beat;
			keyPosLine.anchoredPosition = new Vector2(selectedBeat.GetComponent<RectTransform>().anchoredPosition.x, 0);

			//clear old beat list
			foreach (var infos in beatInfoList)
				foreach (var b in infos)
					Destroy(b.gameObject);

			beatInfoList.Clear();
			beatInfoList.Add(new List<EditorBeat>());
			beatInfoList.Add(new List<EditorBeat>());

			// first set
			if (selectedBeat.beatInfoList == null || selectedBeat.beatInfoList.Count < 1)
			{
				var o = Instantiate(beatPrefab.gameObject);
				o.transform.SetParent(btnLines[0].transform, false);

				var newBeat = o.GetComponent<EditorBeat>();
				//set selected beat time if is start point
				AddBeatToList(newBeat);

				return;
			}

			//load beat infos
			for (int i = 0; i < selectedBeat.beatInfoList.Count; i++)
			{
				Vector2 startPos = Vector2.zero;
				var infoList = selectedBeat.beatInfoList[i];

				for (int j = 0; j < infoList.Count; j++)
				{
					var info = infoList[j];
					if (j == 0) startPos = info.pos;

					var o = Instantiate(beatPrefab.gameObject);
					o.transform.SetParent(btnLines[i].transform, false);

					var newBeat = o.GetComponent<EditorBeat>();
					//set selected beat time if is start point
					newBeat.Init(info.time);
					newBeat.pos = info.pos;
					beatInfoList[i].Add(newBeat);
					SetBeatInfoPosToMarker();
				}

				//start position for markers
				markerEditor.ActiveMarker(i, true);
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
			var beatList = selectedLine == btnLines[0] ? beatInfoList[0] : beatInfoList[1];

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
			var lineIndex = selectedLine == btnLines[0] ? 0 : 1;

			//top line or bottom line
			var beatList = beatInfoList[lineIndex];

			if (beatList.Count < 1)
			{
				//active marker if is first element
				markerEditor.ActiveMarker(lineIndex, true);

				beat.Init(selectedBeat.time);
				beatList.Add(beat);
				//refresh markers info
				SetBeatInfoPosToMarker();
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
			SetBeatInfoPosToMarker();
		}

		public void OnClickRemoveKey(float time)
		{
			if (selectedLine == null) return;
			var beatIndex = selectedLine == btnLines[0] ? 0 : 1;
			//top line or bottom line
			var beatList = beatInfoList[beatIndex];

			var key = FindBeatByTime(time);
			//return when can't find key
			if (key == null)
				return;

			beatList.Remove(key);
			Destroy(key.gameObject);

			//disable marker when beat list is empty
			if (beatList.Count < 1)
				markerEditor.ActiveMarker(beatIndex, false);
			//refresh markers info
			SetBeatInfoPosToMarker();
		}

		public void AdjustKeysPos()
		{
			keyPosLine.anchoredPosition = new Vector2(main.GetPositionByTime(selectedBeat.time), 0);
			//adjust all object
			foreach (var infos in beatInfoList)
				foreach (var b in infos)
					b.GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(b.time), 0);
		}

		EditorBeat FindBeatByTime(float time)
		{
			if (selectedLine == null) return null;
			//top line or bottom line
			var beatList = selectedLine == btnLines[0] ? beatInfoList[0] : beatInfoList[1];

			foreach (var k in beatList)
				if (Mathf.Abs(k.time - time) < .02) return k;

			return null;
		}

		public EditorBeat FindClosestBeat(float targetTime, bool findNext)
		{
			if (selectedLine == null) return null;
			//top line or bottom line
			var beatList = selectedLine == btnLines[0] ? beatInfoList[0] : beatInfoList[1];

			EditorBeat closestKey = null;
			if (beatList.Count < 1) return closestKey;

			if (findNext) beatList = beatList.Where(k => k.time > targetTime).ToList();
			else beatList = beatList.Where(k => k.time < targetTime).ToList();

			var closestTime = float.MaxValue;

			foreach (var k in beatList)
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

		//find current line closest beat
		//for showing marker editor infos
		EditorBeat FindClosestBeat(float targetTime, int index)
		{
			var beatList = beatInfoList[index].Where(k => k.time <= targetTime).ToList();

			EditorBeat closestKey = null;
			if (beatList.Count < 1) return closestKey;

			var closestTime = float.MaxValue;
			foreach (var k in beatList)
			{
				var time = Mathf.Abs(k.time - targetTime);
				if (time < closestTime)
				{
					closestTime = time;
					closestKey = k;
				}
			}
			return closestKey;
		}

		public void QuitAndSave()
		{
			markerEditor.HideAllMarkers();

			var list = new List<List<EditorBeat>>();
			foreach (var info in beatInfoList)
				if (info.Count > 0) list.Add(info);

			if (selectedBeat == null) return;
			selectedBeat.SaveBeatInfos(list);
		}
	}
}