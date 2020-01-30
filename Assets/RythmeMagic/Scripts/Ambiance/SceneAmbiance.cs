using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythhmMagic
{
    public class SceneAmbiance : MonoBehaviour
    {
        [SerializeField] ParticleSystem fxAmbiance;
        [SerializeField] ParticleSystem fxAmbianceInGame;
       
        public void PlayAmbianceFx(bool inGame)
        {
            if (!inGame)
            {
                fxAmbiance.Play();
                fxAmbianceInGame.Stop();
            }
            else
            {
                fxAmbianceInGame.Play();
                fxAmbiance.Stop();
            }
        }
    }
}