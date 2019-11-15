using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RythmePingPong
{
    public class PongSpawner : MonoBehaviour
    {
        [SerializeField] GameObject pingPongPrefab;
        [SerializeField] GameObject grenadePrefab;

        //[SerializeField] float BPM;

        Coroutine spawnCoroutine;
        float SpawnMaxAngle;

        [SerializeField] float defaultSpawnDelay = 1.2f;
        float spawnDelay;

        // Start is called before the first frame update
        void Start()
        {
            SpawnMaxAngle = Mathf.Abs(Mathf.Atan2(5.5f - 1.4f, 0 - 0.9f) * (180 / Mathf.PI));
            if (SpawnMaxAngle > 90) SpawnMaxAngle -= 90;
            spawnDelay = defaultSpawnDelay;

        }

        public void ResetSpawnPong()
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
            spawnDelay = defaultSpawnDelay;
        }

        public void StartSpawnPong()
        {
            if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
            spawnCoroutine = StartCoroutine(SpawnCorou());
        }

        IEnumerator SpawnCorou()
        {
            while (true)
            {
                //Time.timeScale = 0.2f;
                yield return new WaitForSeconds(spawnDelay);

                transform.eulerAngles = new Vector3(0, Random.Range(-SpawnMaxAngle, SpawnMaxAngle));

                GameObject prefab = pingPongPrefab;
                if (spawnDelay <= 0.95f)
                {
                    var rand = Random.Range(0, 6);
                    prefab = rand > 0 ? pingPongPrefab : grenadePrefab;
                }

                var o = Instantiate(prefab, transform.position, transform.rotation);
                o.GetComponent<AIPingPong>().SetUpZSpeed(-7f);
            }
        }

        public void SetSpawnDelay(float delay)
        {
            spawnDelay = delay;
        }

        public void LevelUp()
        {
            if (spawnDelay <= 0.25f)
                return;

            float value = 0;
            if (spawnDelay > 0.9f) value = 0.1f;
            else if (spawnDelay <= 0.9f && spawnDelay > 0.6f) value = 0.05f;
            else if (spawnDelay <= 0.6f) value = 0.025f;

            spawnDelay -= value;
            if (spawnDelay < 0.25f) spawnDelay = 0.25f;
        }
    }
}