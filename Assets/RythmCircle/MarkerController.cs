using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MarkerController : MonoBehaviour
{
    RectTransform myRect;
    [SerializeField] float circleRadius;

    [SerializeField] Hand currentHand;
    [SerializeField] Transform playerHead;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //var playerCenter = new Vector3(playerHead.transform.position.x, playerHead.transform.position.y * 0.8f, 0);
        //var handPos = new Vector3(currentHand.transform.position.x, currentHand.transform.position.y, 0);
        float f_AngleBetween = Vector3.SignedAngle(playerHead.transform.position, currentHand.transform.position, Vector3.up); // Returns an angle between 0 and 180
        Debug.Log(playerHead.transform.position + "  " + currentHand.transform.position);

        var point = FindPoint(f_AngleBetween);
        myRect.anchoredPosition = new Vector2(point.x, point.y);
    }

    Vector3 FindPoint(float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * (Vector3.right * circleRadius);
    }
}
