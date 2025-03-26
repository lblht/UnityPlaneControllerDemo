using System.Collections.Generic;
using UnityEngine;

public class WaterEffects : MonoBehaviour
{
    [SerializeField] GameObject waterSplashPrfab;
    [SerializeField] Vector2 emissionRateRange;
    [SerializeField] float soundEffectVolume;
    Dictionary<GameObject, GameObject> waterSplashObjects = new Dictionary<GameObject, GameObject>();

    void FixedUpdate()
    {
        foreach (KeyValuePair<GameObject, GameObject> waterSplashObject in waterSplashObjects)
        {
            if (waterSplashObject.Value.GetComponent<ParticleSystem>().isPlaying)
            {
                Vector3 position = new Vector3(waterSplashObject.Key.transform.position.x, 0f, waterSplashObject.Key.transform.position.z);
                waterSplashObject.Value.transform.position = position;
                float triggerHeight = gameObject.GetComponent<BoxCollider>().size.y - gameObject.GetComponent<BoxCollider>().center.y;
                float t = 1 - (Mathf.Clamp(waterSplashObject.Key.transform.position.y, 0f, triggerHeight) / triggerHeight);
                var particleEmission = waterSplashObject.Value.GetComponent<ParticleSystem>().emission;
                particleEmission.rateOverDistance = Mathf.Lerp(emissionRateRange.x, emissionRateRange.y, t);
                waterSplashObject.Value.GetComponent<AudioSource>().volume = soundEffectVolume * t;

                RaycastHit hit;
                if (Physics.Raycast(new Vector3(waterSplashObject.Key.transform.position.x, 0f, waterSplashObject.Key.transform.position.z), Vector3.up, out hit, 20f))
                {
                    if (!hit.collider.CompareTag("Player"))
                    {
                        particleEmission.rateOverDistance = 0f;
                        waterSplashObject.Value.GetComponent<AudioSource>().volume = 0f;
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject waterSplashEffect;
            if (!waterSplashObjects.ContainsKey(other.gameObject))
            {
                waterSplashEffect = Instantiate(waterSplashPrfab, Vector2.zero, Quaternion.identity);
                waterSplashEffect.transform.parent = transform;
                waterSplashEffect.transform.localScale = Vector3.one;
                waterSplashObjects.Add(other.gameObject, waterSplashEffect);
            }
            waterSplashObjects.TryGetValue(other.gameObject, out waterSplashEffect);
            waterSplashEffect.GetComponent<ParticleSystem>().Play();
            waterSplashEffect.GetComponent<AudioSource>().Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject waterSplashEffect;
            waterSplashObjects.TryGetValue(other.gameObject, out waterSplashEffect);
            waterSplashEffect.GetComponent<ParticleSystem>().Stop();
            waterSplashEffect.GetComponent<AudioSource>().Stop();
        }
    }
}
