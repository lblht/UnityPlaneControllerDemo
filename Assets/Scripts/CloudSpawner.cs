using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] GameObject cloudPrefab;
    [SerializeField] Vector3 boundaries;
    [SerializeField] Vector2Int numberOfCloudsRange;
    [SerializeField] Vector2Int maxParticlesRange;
    [SerializeField] Material[] cloudMaterials;
    int numberOfClouds;

    void Awake()
    {
        numberOfClouds = Random.Range(numberOfCloudsRange.x, numberOfCloudsRange.y);
        SpawnClouds();
    }

    void SpawnClouds()
    {
        for (int i = 0; i < numberOfClouds; i++)
        {
            float randomX = Random.Range(-boundaries.x / 2f, boundaries.x / 2f);
            float randomY = Random.Range(-boundaries.y / 2f, boundaries.y / 2f);
            float randomZ = Random.Range(-boundaries.z / 2f, boundaries.z / 2f);
            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);
            Vector3 randomSize = new Vector3(Random.Range(1f, 5f), Random.Range(1f, 3f), Random.Range(1f, 5f));
            GameObject cloud = Instantiate(cloudPrefab, transform.position + randomPosition, Quaternion.identity);
            cloud.transform.parent = transform;
            cloud.transform.localScale = randomSize;
            ParticleSystem particleSystem = cloud.GetComponent<ParticleSystem>();
            ParticleSystem.MainModule particleSystemMain = particleSystem.main;
            particleSystemMain.maxParticles = Random.Range(maxParticlesRange.x, maxParticlesRange.y);
            ParticleSystemRenderer particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            particleSystemRenderer.material = cloudMaterials[Random.Range(0, cloudMaterials.Length - 1)];
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, boundaries);
    }
}
