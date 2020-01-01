using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic.MusicEditor
{
	public class MusicEditorController : MonoBehaviour
	{
		MusicEditorMain main;
		MarkerEditor markerEditor;

		[SerializeField] Camera uiCamera;
		Vector3 worldPoint;
		Vector3 worldPoint2D;
		//for dragging object in music map
		[SerializeField] RectTransform rectRefPoint;
		[SerializeField] BtnMusicProgress progressBtn;

		EditorMarker dragMarker;
		EditorKeyBase dragKey;

		private void Start()
		{
			main = GetComponent<MusicEditorMain>();
			markerEditor = FindObjectOfType<MarkerEditor>();
		}

		// Update is called once per frame
		void Update()
		{
			worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			worldPoint2D = uiCamera.ScreenToWorldPoint(Input.mousePosition);
			if (Input.GetMouseButtonDown(0))
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					SetProgressBtnPos();
					main.OnClickAddBeatInGroup();
				}
				else
				{
					TouchDragCollider();
				}
			}
			else if (Input.GetMouseButton(0))
			{
				Dragging();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				DragEnd();
			}

			if (Input.GetKeyDown(KeyCode.Delete))
				main.OnClickRemoveKey();
		}

		void TouchDragCollider()
		{
			if (dragMarker != null || dragKey != null)
				return;

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
				if (hit.collider != null && hit.collider.GetComponentInParent<EditorMarker>() != null)
				{
					var marker = hit.collider.GetComponentInParent<EditorMarker>();
					markerEditor.SetDragMarker(marker, worldPoint);
					dragMarker = marker;
				}

			RaycastHit2D hit2D = Physics2D.Raycast(worldPoint2D, Vector2.zero);
			if (hit2D.collider != null && hit2D.collider.GetComponent<EditorKeyBase>() != null)
			{
				var keybase = hit2D.collider.GetComponent<EditorKeyBase>();
				dragKey = keybase;
			}
		}

		void Dragging()
		{
			if (dragMarker != null)
			{
				markerEditor.DraggingMarker(worldPoint);
			}
			if (dragKey != null)
			{
				dragKey.OnDragSetPos(GetMouseXPosInUI());
			}
		}

		void DragEnd()
		{
			if (dragMarker != null)
			{
				markerEditor.DragEnd();
				dragMarker = null;
			}
			if (dragKey != null)
			{
				dragKey.OnDragEnd();
				dragKey = null;
			}
		}	

		float GetMouseXPosInUI()
		{
			Vector2 mousePos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectRefPoint, Input.mousePosition, uiCamera, out mousePos);
			var xPos = Mathf.Clamp(mousePos.x, 0, main.mapWidth);
			return xPos;
		}

		public void SetProgressBtnPos()
		{
			main.PauseMusic();
			progressBtn.SetXPos(GetMouseXPosInUI());
		}
	}
}