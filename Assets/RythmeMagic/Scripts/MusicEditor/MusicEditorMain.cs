using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public enum MoveModes
	{
		Free,
		Magnet
	}

	public class MusicEditorMain : MonoBehaviour
	{
		MarkerEditor markerEditor;
		float bpm;

		[SerializeField] MusicSheetObject musicSheet;

		[SerializeField] AudioSource myAudio;
		float clipLenght;

		[SerializeField] AudioSource metronomeAudio;

		[SerializeField] ScrollRect mapScrollRect;
		[SerializeField] RectTransform musicMapContent;

		float defaultMapWidth;
		public float mapWidth { get; private set; }
		[SerializeField] float mapWidthOffset;
		int zoomStep = 1;

		[SerializeField] RawImage musicMapImage;
		[SerializeField] RawImage musicMapMiniImage;

		[SerializeField] Transform timeLineParent;
		[SerializeField] GameObject timeLineTextPrefab;
		List<RectTransform> timeLineTexts = new List<RectTransform>();

		[SerializeField] BtnMusicProgress progressBtn;
		[SerializeField] Text textProgress;

		//for dragging object in music map
		public RectTransform RectRefPoint;

		[SerializeField] BeatInfoEditor beatInfoEditor;

		public MoveModes moveMode { get; private set; }
		[SerializeField] Button btnMagnet;

		void Start()
		{
			UnityEngine.XR.XRSettings.enabled = false;
			progressBtn.onDragAction += OnClickPauseMusic;

			markerEditor = FindObjectOfType<MarkerEditor>();

			if (musicSheet == null || musicSheet.music == null) return;

			myAudio.clip = musicSheet.music;
			clipLenght = myAudio.clip.length;

			bpm = UniBpmAnalyzer.AnalyzeBpm(musicSheet.music);

			//draw music map
			Texture2D map = musicSheet.music.PaintWaveformSpectrum(9600, 100, Color.green);
			musicMapImage.texture = musicMapMiniImage.texture = map;
			defaultMapWidth = mapWidth = musicMapContent.sizeDelta.x - mapWidthOffset * 2;

			//create time line text
			var roundTime = clipLenght.RoundUp(5);
			int count = Mathf.RoundToInt(roundTime / 5);
			for (int i = 0; i < count; i++)
			{
				var o = Instantiate(timeLineTextPrefab);
				o.transform.SetParent(timeLineParent, false);
				var timeSpan = System.TimeSpan.FromMinutes(5 * i);
				if (5 * i % 10 == 0)
					o.GetComponent<Text>().text = timeSpan.Hours.ToString("00") + ":" + timeSpan.Minutes.ToString("00");
				else
					o.GetComponent<Text>().enabled = false;

				timeLineTexts.Add(o.GetComponent<RectTransform>());
			}
			SetupTimeLineTexts();

			beatInfoEditor.Init(musicSheet.beatList);

			btnMagnet.onClick.AddListener(OnClickActiveMagnet);
		}


		private void Update()
		{
			SetProgressText();

			if (myAudio.clip == null || !myAudio.isPlaying)
				return;

			var xPos = mapWidth * (myAudio.time / clipLenght);
			progressBtn.SetXPos(xPos);

			//auto follow progress when play
			if (mapScrollRect.horizontalScrollbar.value < (xPos - defaultMapWidth) / mapWidth)
				mapScrollRect.horizontalScrollbar.value = xPos / mapWidth;

			PlayMetronome();
		}

		//for metronome 
		float nowBeatTime = -1;
		void PlayMetronome()
		{
			//play metronome
			var currentBeat = beatInfoEditor.FindBeatByTimeInAllPiste(myAudio.time);
			if (currentBeat != null && currentBeat.time != nowBeatTime)
			{
				metronomeAudio.Play();
				nowBeatTime = currentBeat.time;
			}
		}

		private void SetProgressText()
		{
			var timeSpan = System.TimeSpan.FromMinutes(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
			string hh = timeSpan.Hours.ToString("00");
			string mm = timeSpan.Minutes.ToString("00");
			string ss = timeSpan.Seconds.ToString("00");
			textProgress.text = hh + ":" + mm + ":" + ss;
		}

		public void OnClickSaveData()
		{
			musicSheet.beatList.Clear();

			foreach (var group in beatInfoEditor.beatGroupsL)
			{
				var beatObj = new MusicSheetObject.Beat();

				beatObj.startTime = group.beatList[0].time;

				var newInfo = new MusicSheetObject.BeatInfo();
				newInfo.type = group.beatList.Count < 2 ? BeatTypes.Default : BeatTypes.Holding;
				foreach (var beat in group.beatList)
					newInfo.posList.Add(new MusicSheetObject.PosInfo() { time = beat.time, pos = beat.pos });

				beatObj.infos.Add(newInfo);
				musicSheet.beatList.Add(beatObj);
			}

			foreach (var group in beatInfoEditor.beatGroupsR)
			{
				var beatObj = musicSheet.beatList.Where(b => Mathf.Abs(b.startTime - group.beatList[0].time) < 0.02f).FirstOrDefault();
				if (beatObj != null)
				{
					var newInfo = new MusicSheetObject.BeatInfo();
					newInfo.type = group.beatList.Count < 2 ? BeatTypes.Default : BeatTypes.Holding;
					foreach (var beat in group.beatList)
						newInfo.posList.Add(new MusicSheetObject.PosInfo() { time = beat.time, pos = beat.pos });

					beatObj.infos.Add(newInfo);
				}
				else
				{
					beatObj = new MusicSheetObject.Beat();

					beatObj.startTime = group.beatList[0].time;

					var newInfo = new MusicSheetObject.BeatInfo();
					newInfo.type = group.beatList.Count < 2 ? BeatTypes.Default : BeatTypes.Holding;
					foreach (var beat in group.beatList)
						newInfo.posList.Add(new MusicSheetObject.PosInfo() { time = beat.time, pos = beat.pos });

					beatObj.infos.Add(newInfo);
					musicSheet.beatList.Add(beatObj);
				}
			}

			musicSheet.SaveData(JsonUtility.ToJson(musicSheet));
			EditorGUIUtility.PingObject(musicSheet);
			Debug.Log("Data saved");
		}

		public void OnClickStartMusic()
		{
			if (myAudio.isPlaying) OnClickPauseMusic();
			else
			{
				myAudio.time = GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);
				myAudio.Play();
			}
		}

		public void OnClickPauseMusic()
		{
			nowBeatTime = -1;
			myAudio.Pause();
		}

		public void OnClickStopMusic()
		{
			progressBtn.SetXPos(0);
			mapScrollRect.horizontalScrollbar.value = 0;
			myAudio.Stop();
		}

		public void OnClickAddKey()
		{
			beatInfoEditor.OnClickAddBeat(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
		}

		public void OnClickRemoveKey()
		{
			beatInfoEditor.OnClickRemoveKey(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
		}

		public void OnClickFindKey(bool findNext)
		{
			var piste = beatInfoEditor.GetSelectedPiste();
			if (piste == null) return;

			var beat = beatInfoEditor.FindClosestBeat(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x), piste.Value, findNext);
			if (beat == null) return;

			OnClickPauseMusic();
			progressBtn.SetXPos(beat.rectTransfom.anchoredPosition.x);
		}

		public void OnClickZoom(bool zoomIn)
		{
			zoomStep += zoomIn ? 1 : -1;
			zoomStep = Mathf.Clamp(zoomStep, 1, 50);

			var progressTime = GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);

			mapWidth = defaultMapWidth * zoomStep;
			musicMapContent.sizeDelta = new Vector2(mapWidth + mapWidthOffset * 2, musicMapContent.sizeDelta.y);

			//adjust progress button
			progressBtn.SetXPos(GetPositionByTime(progressTime));

			SetupTimeLineTexts();

			//adjust all object
			beatInfoEditor.AdjustBeatPos();
		}

		public float GetPositionByTime(float time)
		{
			return time / clipLenght * mapWidth;
		}

		public float GetTimeByPosition(float xPos)
		{
			return xPos / mapWidth * clipLenght;
		}

		void SetupTimeLineTexts()
		{
			var roundTime = clipLenght.RoundUp(5);
			var roundWidth = roundTime / clipLenght * mapWidth;
			var dist = roundWidth / timeLineTexts.Count;
			for (int i = 0; i < timeLineTexts.Count; i++)
				timeLineTexts[i].anchoredPosition = new Vector2(dist * i, 0);
		}

		void OnClickActiveMagnet()
		{
			moveMode = moveMode == MoveModes.Free ? MoveModes.Magnet : MoveModes.Free;
			btnMagnet.GetComponent<Image>().color = moveMode == MoveModes.Free ? Color.white : Color.green;
		}

		//magnet mode
		public EditorBeat AttractToBeat(float xPos)
		{
			var closestBeat = new EditorBeat();
			var piste = beatInfoEditor.GetSelectedPiste();
			var time = GetTimeByPosition(xPos);
			if (piste == null)
			{
				closestBeat = beatInfoEditor.FindClosestBeat(time, BeatPiste.Left);
				if (closestBeat == null)
					closestBeat = beatInfoEditor.FindClosestBeat(time, BeatPiste.Right);
			}
			else
				closestBeat = beatInfoEditor.FindClosestBeat(time, piste.Value);

			if (closestBeat == null) return null;
			if (Mathf.Abs(xPos - closestBeat.rectTransfom.anchoredPosition.x) < 10)
				return closestBeat;

			return null;
		}
	}
}