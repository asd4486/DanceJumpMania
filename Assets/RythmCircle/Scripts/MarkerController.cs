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
        var playerCenter = new Vector3(playerHead.transform.position.x, playerHead.transform.position.y * 0.8f);
        //var handPos = new Vector3(currentHand.transform.position.x, currentHand.transform.position.y, 0);
        float f_AngleBetween = AngleTo(playerCenter, currentHand.transform.position);

        var point = FindPoint(f_AngleBetween);
        myRect.anchoredPosition = new Vector2(point.x, point.y);
    }

    float AngleTo(Vector2 this_, Vector2 to)
    {
        Vector2 direction = to - this_;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    Vector3 FindPoint(float angle)
    {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * circleRadius;
    }
}
