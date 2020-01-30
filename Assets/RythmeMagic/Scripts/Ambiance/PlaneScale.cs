using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlaneScale : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var pRect = new HmdQuad_t();
        var chaperone = OpenVR.Chaperone;
        bool checkPlayArea = (chaperone != null) && chaperone.GetPlayAreaRect(ref pRect);
        transform.localScale = new Vector3(pRect.vCorners3.v0, transform.localScale.y, pRect.vCorners3.v2);

        //Debug.Log("x = " + pRect.vCorners3.v0);
        //Debug.Log("z = " + pRect.vCorners3.v2);
    }

}
