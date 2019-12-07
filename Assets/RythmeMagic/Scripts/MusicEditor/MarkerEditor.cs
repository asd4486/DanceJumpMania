using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarkerEditor : MonoBehaviour
{
	[SerializeField] Transform[] markers;
	Transform dragMarker;
	List<TextMeshPro> markerTexts = new List<TextMeshPro>();

	public event System.Action onDragMarkerAction;

	Vector3 screenPoint;

	// Start is called before the first frame update
	void Start()
	{
		HideAllMarkers();
		foreach (var m in markers)
			markerTexts.Add(m.GetComponentInChildren<TextMeshPro>());
	}

	private void Update()
	{
		SetMarkerPosText();

		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider != null && markers.Contains(hit.collider.transform.parent))
				{
					dragMarker = hit.collider.transform.parent;
					screenPoint = Camera.main.WorldToScreenPoint(dragMarker.transform.position);
				}
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			dragMarker = null;
		}

		if (dragMarker != null)
		{
			if (onDragMarkerAction != null) onDragMarkerAction();

			Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
			Vector3 pos = Camera.main.ScreenToWorldPoint(cursorPoint);
			dragMarker.position = new Vector3(pos.x, pos.y, dragMarker.transform.position.z);
		}
	}

	void SetMarkerPosText()
	{
		for(int i = 0; i < markers.Length; i++)
		{
			var marker = markers[i];
			if (marker.gameObject.activeSelf)
			{
				var pos = marker.localPosition;
				markerTexts[i].text = "X:" + pos.x.ToString("F2") + "  Y:" + pos.y.ToString("F2");
			}			
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

	public void SetMarkerPos(int index, Vector2 pos)
	{
		markers[index].transform.localPosition = pos;
	}

	public Vector2 GetMarkerPos(int index)
	{
		return markers[index].transform.localPosition;
	}
}
