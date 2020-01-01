using UnityEngine;

namespace RythhmMagic.MusicEditor
{
	public class MarkerEditor : MonoBehaviour
	{
		[SerializeField] EditorMarker[] markers;
		EditorMarker dragMarker;
		Vector3 dragOffset;

		// Start is called before the first frame update
		void Start()
		{
			HideAllMarkers();
		}

		public void SetDragMarker(EditorMarker marker, Vector3 pos)
		{
			if (marker.currentBeat == null)
				return;

			dragMarker = marker;
			dragOffset = dragMarker.transform.position - pos;
		}

		public void DraggingMarker(Vector3 pos)
		{
			if (dragMarker == null) return;
			dragMarker.DragSetPosition(transform.InverseTransformPoint(pos + dragOffset));
		}

		public void DragEnd()
		{
			if (dragMarker == null)
				return;

			dragMarker.DragEnd();
			dragMarker = null;
		}

		public void HideAllMarkers()
		{
			foreach (var m in markers)
				m.gameObject.SetActive(false);
		}

		public void ActiveMarker(int index, bool active)
		{
			markers[index].SetActive(active);
		}

		public void SetMarkerBeat(int index, EditorBeat beat)
		{
			markers[index].SetCurrentBeat(beat);
		}

		public void SetMarkerPos(int index, Vector3 pos)
		{
			markers[index].DragSetPosition(pos);
		}
	}
}