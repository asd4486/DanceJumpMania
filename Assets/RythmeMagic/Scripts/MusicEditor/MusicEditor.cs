//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace RythhmMagic.MusicEditor
//{
//	public class MusicEditor : MonoBehaviour, IMusicEditor
//	{
//		protected MusicEditorMain main;
//		MarkerEditor markerEditor;
//		public List<EditorBeat> beatList { get; private set; }

//		[SerializeField] EditorBeat beatPrefab;

//		[SerializeField] EditorMarker markerPrefab;
//		List<EditorMarker> markerList = new List<EditorMarker>();
//		BtnMusicProgress progressBtn;

//		// Start is called before the first frame update
//		void Awake()
//		{
//			main = FindObjectOfType<MusicEditorMain>();
//			markerEditor = FindObjectOfType<MarkerEditor>();
//			beatList = new List<EditorBeat>();
//			progressBtn = FindObjectOfType<BtnMusicProgress>();
//		}

//		private void Start()
//		{
//			progressBtn.onDragAction += ShowBeatsPosMarker;
//		}

//		//load all beats
//		public void Init(List<MusicSheetObject.Beat> list)
//		{
//			foreach (var beatInfos in list)
//			{
//				var o = Instantiate(beatPrefab.gameObject);
//				o.transform.SetParent(transform, false);

//				var newBeat = o.GetComponent<EditorBeat>();
//				newBeat.Init(beatInfos.startTime);

//				for (int i = 0; i < beatInfos.infos.Count; i++)
//				{
//					var posInfos = new List<BeatPosInfo>();
//					foreach (var b in beatInfos.infos[i].posList)
//						posInfos.Add(new BeatPosInfo() { time = b.time, pos = b.pos });

//					if (i == 0) newBeat.leftBeatInfos = posInfos;
//					else if (i == 1) newBeat.rightBeatInfos = posInfos;
//				}

//				AddBeatToList(newBeat);
//			}
//		}

//		//for visual
//		void ShowBeatsPosMarker()
//		{
//			var time = main.GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);
//			var pisteBeatsL = new List<BeatPosInfo>();
//			var pisteBeatsR = new List<BeatPosInfo>();

//			//get beat by piste
//			foreach (var b in beatList)
//			{
//				foreach (var leftB in b.leftBeatInfos)
//				{
//					if(leftB.time <= time)
//						pisteBeatsL.Add(leftB);
//				}					
//				foreach (var rightB in b.rightBeatInfos)
//				{
//					if(rightB.time <= time)
//						pisteBeatsR.Add(rightB);
//				}		
//			}

//			//var beatL = GetClosestBeat(time, pisteBeatsL);
//			//if (beatL != null)
//			//{
//			//	//show beat if pointer is in a holding beat
//			//	//if pointer is in the beat
//			//	if ((pisteBeatsL.IndexOf(beatL) < pisteBeatsL.Count - 1 || FindBeatByTime(time) == beatL))
//			//	{
//			//		if (editBeatL != beatL)
//			//		{
//			//			editBeatL = beatL;
//			//			markerEditor.SetMarkerBeat(0, beatL);
//			//			markerEditor.ActiveMarker(0, true);
//			//		}
//			//	}
//			//	else
//			//		markerEditor.ActiveMarker(0, false);
//			//}
//			//else
//			//	markerEditor.ActiveMarker(0, false);

//			//var beatR = GetClosestBeat(time, pisteBeatsR);
//			//if (beatR != null)
//			//{
//			//	//show beat if pointer is in a holding beat
//			//	//if pointer is in the beat
//			//	if ((rightBeatInfos.IndexOf(beatR) < rightBeatInfos.Count - 1 || FindBeatByTime(time) == beatR))
//			//	{
//			//		if (editBeatR != beatR)
//			//		{
//			//			editBeatR = beatR;
//			//			markerEditor.SetMarkerBeat(1, beatR);
//			//			markerEditor.ActiveMarker(1, true);
//			//		}
//			//	}
//			//	else
//			//		markerEditor.ActiveMarker(1, false);
//			//}
//			//else
//			//	markerEditor.ActiveMarker(1, false);
//		}

//		public virtual void OnClickAddBeat(float time)
//		{
//			//don't create key when key existed
//			if (FindBeatByTime(time) != null)
//				return;

//			var o = Instantiate(beatPrefab.gameObject);
//			o.transform.SetParent(transform, false);

//			var beat = o.GetComponent<EditorBeat>();
//			beat.Init(time);

//			AddBeatToList(beat);
//		}

//		public void AdjustBeatInBeatList(EditorBeat _beat)
//		{
//			if (!beatList.Contains(_beat)) return;

//			beatList.Remove(_beat);

//			//overried old key if existe
//			var oldKey = FindBeatByTime(_beat.time);
//			if (oldKey != null)
//			{
//				beatList.Remove(oldKey);
//				Destroy(oldKey.gameObject);
//			}

//			//readd key in key list
//			AddBeatToList(_beat);
//		}

//		void AddBeatToList(EditorBeat beat)
//		{
//			if (beatList.Count < 1)
//			{
//				beatList.Add(beat);
//				return;
//			}

//			var index = 0;
//			var beatTime = beat.time;

//			for (int i = 0; i < beatList.Count; i++)
//			{
//				if (beatTime > beatList[i].time)
//					index += 1;
//				else
//					break;
//			}

//			if (index >= beatList.Count)
//				beatList.Add(beat);
//			else
//				beatList.Insert(index, beat);
//		}

//		public void OnClickRemoveKey(float time)
//		{
//			var key = FindBeatByTime(time);
//			//return when can't find key
//			if (key == null)
//				return;

//			beatList.Remove(key);
//			Destroy(key.gameObject);
//		}

//		public void AdjustKeysPos()
//		{
//			//adjust all object
//			foreach (var k in beatList)
//			{
//				k.GetComponent<RectTransform>().anchoredPosition = new Vector2(main.GetPositionByTime(k.time), 0);
//			}
//		}

//		public EditorBeat FindBeatByTime(float time)
//		{
//			foreach (var k in beatList)
//			{
//				if (Mathf.Abs(k.time - time) < .02) return k;
//			}
//			return null;
//		}

//		public EditorBeat FindClosestBeat(float targetTime)
//		{
//			return GetClosestBeat(targetTime, beatList);
//		}

//		public EditorBeat FindClosestBeat(float targetTime, bool findNext)
//		{
//			List<EditorBeat> list = new List<EditorBeat>();
//			if (findNext) list = beatList.Where(k => k.time > targetTime).ToList();
//			else list = beatList.Where(k => k.time < targetTime).ToList();

//			return GetClosestBeat(targetTime, list);
//		}

//		EditorBeat GetClosestBeat(float targetTime, List<EditorBeat> list)
//		{
//			EditorBeat closestBeat = null;
//			if (beatList.Count < 1) return null;
//			var closestTime = float.MaxValue;

//			foreach (var k in list)
//			{
//				var time = Mathf.Abs(k.time - targetTime);
//				if (time < closestTime && time >= .02)
//				{
//					closestTime = time;
//					closestBeat = k;
//				}
//			}
//			return closestBeat;
//		}

//		BeatPosInfo GetClosestBeat(float targetTime, List<BeatPosInfo> list)
//		{
//			BeatPosInfo closestBeat = null;
//			if (beatList.Count < 1) return null;
//			var closestTime = float.MaxValue;

//			foreach (var k in list)
//			{
//				var time = Mathf.Abs(k.time - targetTime);
//				if (time < closestTime && time >= .02)
//				{
//					closestTime = time;
//					closestBeat = k;
//				}
//			}
//			return closestBeat;
//		}
//	}
//}
