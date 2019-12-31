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

	public class BeatInfoEditor : MonoBehaviour
	{
		[SerializeField] BtnMusicProgress progressBtn;

		[SerializeField] EditorBeat beatPrefab;
		[SerializeField] EditorBeatGroup beatGroupPrefab;

		MusicEditorMain main;
		MarkerEditor markerEditor;

		[HideInInspector] public List<EditorBeatGroup> beatGroupsL = new List<EditorBeatGroup>();
		[HideInInspector] public List<EditorBeatGroup> beatGroupsR = new List<EditorBeatGroup>();

		[SerializeField] Button[] btnBeatPiste;
		Button selectedBtnPiste;

		private void Awake()
		{
			main = FindObjectOfType<MusicEditorMain>();
			markerEditor = FindObjectOfType<MarkerEditor>();
		}

		private void Start()
		{
			foreach (var btn in btnBeatPiste)
				btn.onClick.AddListener(() => OnClickSelectPiste(btn));
			progressBtn.onSetPosAction += ShowMarkerBeatPos;

			OnClickSelectPiste(btnBeatPiste[0]);
		}

		public void Init(List<MusicSheetObject.Beat> list)
		{
			foreach (var beatInfos in list)
			{
				for (int i = 0; i < beatInfos.infos.Count; i++)
				{
					var newGroup = CreateBeatGroup();
					newGroup.transform.SetParent(btnBeatPiste[i].transform, false);

					var groupBeats = new List<EditorBeat>();
					foreach (var info in beatInfos.infos[i].posList)
					{
						var beat = CreateBeat();
						beat.onDragEndAction += AdjustBeatInList;
						beat.Init(info.time, info.pos, newGroup);
						beat.transform.SetParent(btnBeatPiste[i].transform, false);
						groupBeats.Add(beat);
					}

					newGroup.Init(groupBeats);
					newGroup.onDestroyAction += RemoveBeatGroup;
					if (i == 0)
						AddBeatGroup(newGroup, BeatPiste.Left);
					else if (i == 1)
						AddBeatGroup(newGroup, BeatPiste.Right);
				}
			}
		}

		EditorBeat CreateBeat()
		{
			return Instantiate(beatPrefab.gameObject).GetComponent<EditorBeat>();
		}

		EditorBeatGroup CreateBeatGroup()
		{
			return Instantiate(beatGroupPrefab.gameObject).GetComponent<EditorBeatGroup>();
		}

		float showBeatTime;
		void ShowMarkerBeatPos()
		{
			var time = main.GetTimeByPosition(progressBtn.rectTransfom.anchoredPosition.x);

			if (Mathf.Abs(time - showBeatTime) <= 0.02f) return;
			showBeatTime = time;

			EditorBeat beatL = null;
			var groupL = FindBeatGroupByTime(time, BeatPiste.Left);

			if (groupL != null)
			{
				var pos = groupL.GetTimeCurrentPos(time, out beatL);
				markerEditor.SetMarkerBeat(0, beatL);
				markerEditor.SetMarkerPos(0, pos);
				markerEditor.ActiveMarker(0, true);
			}
			else
				markerEditor.ActiveMarker(0, false);

			EditorBeat beatR = null;
			var groupR = FindBeatGroupByTime(time, BeatPiste.Right);
			if (groupR != null)
			{
				var pos = groupL.GetTimeCurrentPos(time, out beatR);
				markerEditor.SetMarkerBeat(1, beatR);
				markerEditor.SetMarkerPos(0, pos);
				markerEditor.ActiveMarker(1, true);
			}
			else
				markerEditor.ActiveMarker(1, false);
		}

		void OnClickSelectPiste(Button piste)
		{
			selectedBtnPiste = piste;
			foreach (var p in btnBeatPiste)
				p.GetComponent<Image>().color = p == selectedBtnPiste ? new Color(1, 1, 0, 0.7f) : new Color(0, 0, 0, 0.8f);
		}

		public void OnClickAddBeat(float time)
		{
			//don't create key when key existed
			var piste = GetSelectedPiste();
			if (piste == null || FindBeatByTime(time, piste.Value) != null) return;

			var beat = CreateBeat();
			beat.transform.SetParent(selectedBtnPiste.transform, false);

			var currentGroup = FindBeatGroupByTime(time, piste.Value);
			if (currentGroup == null)
			{
				currentGroup = CreateBeatGroup();
				currentGroup.transform.SetParent(selectedBtnPiste.transform, false);
				AddBeatGroup(currentGroup, piste.Value);
			}

			beat.Init(time, Vector2.zero, currentGroup);
			currentGroup.AddBeat(beat);
		}

		public void AdjustBeatInList(EditorBeat _beat)
		{
			var piste = GetSelectedPiste();
			if (piste == null) return;

			var group = _beat.currentGroup;
			if (group == null) return;

			group.AdjustBeatInList(_beat);

			var groups = piste.Value == BeatPiste.Left ? beatGroupsL : beatGroupsR;
			groups.Remove(group);

			//readd group in list
			AddBeatGroup(group, piste.Value);
		}

		void AddBeatGroup(EditorBeatGroup group, BeatPiste piste)
		{
			//top line or bottom line
			var beatGroups = piste == BeatPiste.Left ? beatGroupsL : beatGroupsR;

			if (beatGroups.Count < 1)
			{
				beatGroups.Add(group);
				//refresh markers info
				ShowMarkerBeatPos();
				return;
			}

			var index = 0;
			var groupStartTime = group.beatList[0].time;

			for (int i = 0; i < beatGroups.Count; i++)
			{
				if (groupStartTime > beatGroups[i].beatList[0].time)
					index += 1;
				else
					break;
			}

			if (index >= beatGroups.Count)
				beatGroups.Add(group);
			else
				beatGroups.Insert(index, group);

			//refresh markers info
			ShowMarkerBeatPos();
		}

		public void OnClickRemoveKey(float time)
		{
			var piste = GetSelectedPiste();
			if (piste == null) return;

			var beat = FindBeatByTime(time, piste.Value);
			//return when can't find key
			if (beat == null)
				return;

			var group = beat.currentGroup;
			group.RemoveBeat(beat);

			//refresh markers info
			ShowMarkerBeatPos();
		}

		void RemoveBeatGroup(EditorBeatGroup group)
		{
			//left piste or right piste
			var groupList = selectedBtnPiste == btnBeatPiste[0] ? beatGroupsL : beatGroupsR;

			if (!groupList.Contains(group))
				return;

			groupList.Remove(group);
		}

		public void AdjustBeatPos()
		{
			//adjust all object
			foreach (var group in beatGroupsL)
			{
				foreach (var b in group.beatList)
					b.rectTransfom.anchoredPosition = new Vector2(main.GetPositionByTime(b.time), 0);

				//refresh group lenght
				group.SetGroupLenght();
			}

			foreach (var group in beatGroupsR)
			{
				foreach (var b in group.beatList)
					b.rectTransfom.anchoredPosition = new Vector2(main.GetPositionByTime(b.time), 0);

				//refresh group lenght
				group.SetGroupLenght();
			}
		}

		EditorBeatGroup FindBeatGroupByTime(float time, BeatPiste piste)
		{
			var list = piste == BeatPiste.Left ? beatGroupsL : beatGroupsR;

			foreach (var group in list)
			{
				//find group which contains this time
				if (group.CheckTimeInRange(time))
					return group;
			}
			return null;
		}

		EditorBeat FindBeatByTime(float time, BeatPiste piste)
		{
			var groups = piste == BeatPiste.Left ? beatGroupsL : beatGroupsR;

			foreach (var group in groups)
			{
				foreach (var b in group.beatList)
					if (Mathf.Abs(b.time - time) < .02) return b;
			}
			return null;
		}

		public EditorBeat FindBeatByTimeInAllPiste(float time)
		{
			foreach (var group in beatGroupsL)
			{
				foreach (var b in group.beatList)
					if (Mathf.Abs(b.time - time) < .02) return b;
			}
			foreach (var group in beatGroupsR)
			{
				foreach (var b in group.beatList)
					if (Mathf.Abs(b.time - time) < .02) return b;
			}
			return null;
		}

		public EditorBeat FindClosestBeat(float targetTime, BeatPiste piste)
		{
			var groups = piste == BeatPiste.Left ? beatGroupsL : beatGroupsR;
			var beatList = new List<EditorBeat>();

			foreach (var group in groups)
				foreach (var b in group.beatList)
					beatList.Add(b);

			return GetClosestBeat(targetTime, beatList);
		}

		public EditorBeat FindClosestBeat(float targetTime, BeatPiste piste, bool findNext)
		{
			//top line or bottom line
			var groups = piste == BeatPiste.Left ? beatGroupsL : beatGroupsR;
			var beatList = new List<EditorBeat>();

			foreach (var group in groups)
				foreach (var b in group.beatList)
					beatList.Add(b);

			if (findNext)
				beatList = beatList.Where(b => b.time > targetTime).ToList();
			else
				beatList = beatList.Where(b => b.time < targetTime).ToList();

			return GetClosestBeat(targetTime, beatList);
		}

		EditorBeat GetClosestBeat(float targetTime, List<EditorBeat> beatList)
		{
			EditorBeat closestBeat = null;
			if (beatList.Count < 1) return closestBeat;

			var closestTime = float.MaxValue;
			foreach (var b in beatList)
			{
				var time = Mathf.Abs(b.time - targetTime);
				if (time < closestTime)
				{
					closestTime = time;
					closestBeat = b;
				}
			}
			return closestBeat;
		}

		public BeatPiste? GetSelectedPiste()
		{
			if (selectedBtnPiste == null) return null;
			return selectedBtnPiste == btnBeatPiste[0] ? BeatPiste.Left : BeatPiste.Right;
		}
	}
}
