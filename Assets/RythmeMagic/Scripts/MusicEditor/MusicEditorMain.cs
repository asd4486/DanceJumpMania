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

		[SerializeField] Transform timeLineParent;
		[SerializeField] GameObject timeLineTextPrefab;
		List<RectTransform> timeLineTexts = new List<RectTransform>();

		[SerializeField] BtnMusicProgress progressBtn;
		[SerializeField] Text textProgress;

		//for dragging object in music map
		public RectTransform RectRefPoint;

		[SerializeField] Button btnEditKey;

		[SerializeField] MusicEditor musicEditor;
		[SerializeField] BeatInfoEditor beatInfoEditor;
		public IMusicEditor currentEditor { get; private set; }

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
			musicMapImage.texture = map;
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

			musicEditor.Init(musicSheet.beatList);

			btnEditKey.onClick.AddListener(OnClickChangeEditMode);
			currentEditor = musicEditor;
			beatInfoEditor.gameObject.SetActive(false);

			btnMagnet.onClick.AddListener(OnClickActiveMagnet);
		}


		private void Update()
		{
			SetProgressText();

			if (myAudio.clip == null || !myAudio.isPlaying)
				return;

			var xPos = mapWidth * (myAudio.time / clipLenght);
			progressBtn.rectTransfom.anchoredPosition = new Vector2(xPos, 0);

			//auto follow progress when play
			if (mapScrollRect.horizontalScrollbar.value < (xPos - defaultMapWidth) / mapWidth)
				mapScrollRect.horizontalScrollbar.value = xPos / mapWidth;

			PlayMetronome();
		}

		//for metronome 
		EditorBeat nowBeat = new EditorBeat();
		void PlayMetronome()
		{
			//play metronome
			var currentBeat = currentEditor.FindBeatByTime(myAudio.time);
			if (currentBeat != null && currentBeat.time != nowBeat.time)
			{
				metronomeAudio.Play();
				nowBeat = currentBeat;
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
			foreach (var beat in musicEditor.beatList)
			{
				var beatObj = new MusicSheetObject.Beat();

				if (beat.leftBeatInfos.Count > 0)
				{
					beatObj.startTime = beat.leftBeatInfos[0].time;

					var newInfo = new MusicSheetObject.BeatInfo();
					newInfo.type = beat.leftBeatInfos.Count < 2 ? BeatTypes.Default : BeatTypes.Holding;
					foreach (var detail in beat.leftBeatInfos)
						newInfo.posList.Add(new MusicSheetObject.PosInfo() { time = detail.time, pos = detail.pos });

					beatObj.infos.Add(newInfo);
				}
				if(beat.rightBeatInfos.Count > 0)
				{
					if (beatObj.startTime > beat.rightBeatInfos[0].time)
						beatObj.startTime = beat.rightBeatInfos[0].time;

					var newInfo = new MusicSheetObject.BeatInfo();
					newInfo.type = beat.rightBeatInfos.Count < 2 ? BeatTypes.Default : BeatTypes.Holding;
					foreach (var detail in beat.rightBeatInfos)
						newInfo.posList.Add(new MusicSheetObject.PosInfo() { time = detail.time, pos = detail.pos });

					beatObj.infos.Add(newInfo);
				}

				musicSheet.beatList.Add(beatObj);
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
			nowBeat = new EditorBeat();
			myAudio.Pause();
		}

		public void OnClickStopMusic()
		{
			progressBtn.rectTransfom.anchoredPosition = Vector2.zero;
			mapScrollRect.horizontalScrollbar.value = 0;
			myAudio.Stop();
		}

		public void OnClickAddKey()
		{
			currentEditor.OnClickAddBeat(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
			//auto switch to beat info edit mode if create a new beat
			if (currentEditor is MusicEditor) OnClickChangeEditMode();
		}

		//call when move a key by hand
		public void AdjustBeatInBeatList(EditorBeat beat)
		{
			currentEditor.AdjustBeatInBeatList(beat);
		}

		public void OnClickRemoveKey()
		{
			currentEditor.OnClickRemoveKey(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
		}

		public void OnClickFindKey(bool findNext)
		{
			var beat = currentEditor.FindClosestBeat(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x), findNext);
			if (beat == null)
				return;

			OnClickPauseMusic();
			progressBtn.rectTransfom.anchoredPosition = new Vector2(beat.GetComponent<RectTransform>().anchoredPosition.x, 0);
			//refresh markers info
			if (currentEditor is BeatInfoEditor) beatInfoEditor.SetBeatPosToMarker();
		}

		public void OnClickZoom(bool zoomIn)
		{
			zoomStep += zoomIn ? 1 : -1;
			zoomStep = Mathf.Clamp(zoomStep, 1, 50);

			var progressTime = GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);

			mapWidth = defaultMapWidth * zoomStep;
			musicMapContent.sizeDelta = new Vector2(mapWidth + mapWidthOffset * 2, musicMapContent.sizeDelta.y);

			//adjust progress button
			progressBtn.rectTransfom.anchoredPosition = new Vector2(GetPositionByTime(progressTime), 0);

			SetupTimeLineTexts();

			//adjust all object
			musicEditor.AdjustKeysPos();
			if (currentEditor is BeatInfoEditor) beatInfoEditor.AdjustKeysPos();
		}

		public float GetPositionByTime(float time)
		{
			return time / clipLenght * mapWidth;
		}

		public float GetTimeByPosition(float xPos)
		{
			return xPos / mapWidth * clipLenght;
		}

		void OnClickChangeEditMode()
		{
			markerEditor.SetCanEdit(currentEditor is MusicEditor);

			if (currentEditor is BeatInfoEditor)
			{
				btnEditKey.GetComponent<Image>().color = Color.white;
				currentEditor = musicEditor;
				//save changes of one beat
				beatInfoEditor.QuitAndSave();
				beatInfoEditor.gameObject.SetActive(false);
				return;
			}

			var beat = musicEditor.FindBeatByTime(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
			//auto create new beat when pass to beat info mode
			if (beat == null)
			{
				OnClickAddKey();
				beat = musicEditor.FindBeatByTime(GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x));
			}

			beatInfoEditor.Init(beat);
			currentEditor = beatInfoEditor;
			beatInfoEditor.gameObject.SetActive(true);
			btnEditKey.GetComponent<Image>().color = Color.green;
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
	}
}