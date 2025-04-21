using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPickupSpawner : MonoBehaviour
{
    [Header("Spawning Setting")]
    public float initialDelay = 5f;
    public float spawnInterval = 10f;
    public float spawnDistanceFromPlayer = 15f;
    public int maxPickupsInWorld = 10;

    [Header("Pickups")]
    public GameObject[] pickupPrefabs;
    public float[] spawnWeights;

    [Header("Player")]
    public Transform player;

    private List<GameObject> activePickups = new List<GameObject>();
    private float spawnTimer;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        spawnTimer = initialDelay;

        if (spawnWeights.Length != pickupPrefabs.Length && pickupPrefabs.Length > 0)
        {
            spawnWeights = new float[pickupPrefabs.Length];
            for (int i = 0; i < spawnWeights.Length; i++)
            {
                spawnWeights[i] = 1f;
            }
        }
    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return;
        }

        spawnTimer -= Time.deltaTime;

        activePickups.RemoveAll(p => p == null);

        if (spawnTimer <= 0f && activePickups.Count < maxPickupsInWorld)
        {
            SpawnPickup();
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnPickup()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + new Vector3(randomDirection.x, randomDirection.y, 0) * spawnDistanceFromPlayer;

        GameObject pickupToSpawn = GetRandomPickup();

        if (pickupToSpawn != null)
        {
            GameObject pickup = Instantiate(pickupToSpawn, spawnPosition, Quaternion.identity);
            activePickups.Add(pickup);

            Destroy(pickup, 30f);
        }
    }

    private GameObject GetRandomPickup()
    {
        if (pickupPrefabs.Length == 0)
            return null;

        float totalWeight = 0;
        for (int i = 0; i < spawnWeights.Length; i++)
        {
            totalWeight += spawnWeights[i];
        }

        float randomValue = Random.Range(0, totalWeight);

        float weightSum = 0;
        for (int i = 0; i < pickupPrefabs.Length; i++)
        {
            weightSum += spawnWeights[i];
            if (randomValue <= weightSum)
            {
                return pickupPrefabs[i];
            }
        }

        return pickupPrefabs[0];
    }
}