using UnityEngine;

namespace RythhmMagic
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance = null;
        private static readonly object padlock = new object();

        public static GameManager Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new GameManager();
                    return instance;
                }
            }
        }

        public float markerDistance = 40;
        [SerializeField] float basicSpeed = 1;
        //for adjust marker speed
        [SerializeField] float AdjustmentSpeed;
        public float MarkerSpeed { get { return basicSpeed + AdjustmentSpeed; } }

    }
}