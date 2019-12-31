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
		Vector3 dragOffset;

		// Start is called before the first frame update
		void Start()
		{
			HideAllMarkers();
		}

		private void Update()
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit))
				{
					if (hit.collider != null && hit.collider.transform.GetComponentInParent<EditorMarker>() != null)
					{
						var marker = hit.collider.transform.GetComponentInParent<EditorMarker>();
						if (marker.currentBeat != null)
						{
							dragMarker = marker;
							dragOffset = dragMarker.transform.position - mousePos;
						}
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
				var pos = transform.InverseTransformPoint(mousePos + dragOffset);
				dragMarker.SetPosition(pos);
			}
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

		public void SetMarkerPos(int index, Vector3 pos)
		{
			markers[index].SetPosition(pos);
		}
	}
}