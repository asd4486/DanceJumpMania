using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

namespace RythhmMagic
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] SteamVR_LaserPointer currrentLaser;
        RythmMagicMain main;
        // Start is called before the first frame update
        void Start()
        {
            main = FindObjectOfType<RythmMagicMain>();
        }

        public void OnClickLaunchGame()
        {
            main.StartGame();
        }

        public void ActiveUI(bool active)
        {
            gameObject.SetActive(active);
            currrentLaser.holder.SetActive(active);
        }
    }
}