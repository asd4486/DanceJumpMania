using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic.MusicEditor
{
	public class EditorMarkerFx : MonoBehaviour
	{
		EditorBeatGroup targetBeatGroup;
		public void Init(EditorBeatGroup group)
		{
			targetBeatGroup = group;
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
