using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public float waveDuration = 25f;
    public GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 1.5f;
    public int enemiesPerWave = 10;
    public float spawnDistanceFromPlayer = 15f;
    public Transform playerTransform;

    private float waveTimer = 0f;
    private bool isSpawning = false;
    private Coroutine currentWaveCoroutine;

    private void Start()
    {
        if (playerTransform == null)
        {
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("NO PlayerMovement!");
                return;
            }
        }

        StartSpawning();
    }

    private void Update()
    {
        if (isSpawning)
        {
            waveTimer += Time.deltaTime;

            if (waveTimer >= waveDuration)
            {
                waveTimer = 0f;
                SpawnNewWave();
            }
        }
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            waveTimer = 0f;
            SpawnNewWave();
        }
    }

    public void StopSpawning()
    {
        if (isSpawning)
        {
            isSpawning = false;
            if (currentWaveCoroutine != null)
            {
                StopCoroutine(currentWaveCoroutine);
                currentWaveCoroutine = null;
            }
        }
    }

    private void SpawnNewWave()
    {
        if (currentWaveCoroutine != null)
        {
            StopCoroutine(currentWaveCoroutine);
        }

        currentWaveCoroutine = StartCoroutine(SpawnWaveCoroutine());
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnSingleEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }

        currentWaveCoroutine = null;
    }

    private void SpawnSingleEnemy()
    {
        if (enemyPrefabs.Length == 0 || playerTransform == null)
            return;

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Vector2 spawnDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = playerTransform.position + new Vector3(spawnDirection.x, spawnDirection.y, 0) * spawnDistanceFromPlayer;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }

    public float GetWaveTimeRemaining()
    {
        return waveDuration - waveTimer;
    }
}