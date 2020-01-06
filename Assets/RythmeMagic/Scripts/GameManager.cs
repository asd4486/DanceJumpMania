using UnityEngine;

namespace RythhmMagic
{
    public class GameManager : MonoBehaviour
    {
        public float markerDistance = 10;
        public float markerTime = 2;
        public float MarkerSpeed { get { return markerDistance / markerTime; } }
    }
}