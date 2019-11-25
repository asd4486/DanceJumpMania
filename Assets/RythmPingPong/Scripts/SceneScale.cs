using Valve.VR;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace RythmePingPong
{
    public class SceneScale : MonoBehaviour
    {
		Player player;
		[SerializeField] float zOffset;
        // Start is called before the first frame update
        void Start()
        {
			player = FindObjectOfType<Player>();
            //var pRect = new HmdQuad_t();
            //var chaperone = OpenVR.Chaperone;
            //bool checkPlayArea = (chaperone != null) && chaperone.GetPlayAreaRect(ref pRect);

            //Debug.Log("x = " + pRect.vCorners3.v0);
            //Debug.Log("z = " + pRect.vCorners3.v2);
        }

		private void LateUpdate()
		{
			transform.position = new Vector3(0, 0, player.transform.position.z + zOffset);
		}

	}
}
