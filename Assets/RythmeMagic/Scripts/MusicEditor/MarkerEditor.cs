using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RythhmMagic.MusicEditor
{
	public class MarkerEditor : MonoBehaviour
	{
		[SerializeField] EditorMarker[] markers;
		EditorMarker dragMarker;
		bool canEdit;

		// Start is called before the first frame update
		void Start()
		{
			HideAllMarkers();
		}

		private void Update()
		{
			if (!canEdit) return;

			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.collider != null && markers.Contains(hit.collider.transform.GetComponentInParent<EditorMarker>()))
					{
						dragMarker = hit.collider.transform.GetComponentInParent<EditorMarker>();
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (dragMarker != null)
				{
					dragMarker.DragEnd();
					dragMarker = null;
				}
			}

			if (dragMarker != null)
			{
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				dragMarker.SetPosition(new Vector3(pos.x, pos.y, dragMarker.transform.position.z), false);
			}
		}

		public void SetCanEdit(bool b)
		{
			canEdit = b;
		}

		public void HideAllMarkers()
		{
			foreach (var m in markers)
				m.gameObject.SetActive(false);
		}

		public void ActiveMarker(int index, bool active)
		{
			markers[index].gameObject.SetActive(active);
		}

		public void SetMarkerBeat(int index, EditorBeat beat)
		{
			markers[index].SetCurrentBeat(beat);
		}
	}
}