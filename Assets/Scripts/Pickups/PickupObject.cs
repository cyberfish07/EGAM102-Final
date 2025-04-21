using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Experience,
    Health,
}

public class PickupObject : MonoBehaviour
{
    [Header("Pickup Setting")]
    public PickupType type;
    public int value = 1;
    public float moveSpeed = 5f;
    public float attractDistance = 3f;
    public float destroyDelay = 30f;

    [Header("VFX")]
    public GameObject collectEffectPrefab;

    private Transform player;
    private bool isMovingToPlayer = false;
    private float destroyTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        destroyTimer = destroyDelay;
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            return;
        }

        destroyTimer -= Time.deltaTime;
        if (destroyTimer <= 0)
        {
            PlayCollectEffect();
            Destroy(gameObject);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attractDistance || isMovingToPlayer)
        {
            isMovingToPlayer = true;
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect(collision.gameObject);
        }
    }
    void Collect(GameObject playerObj)
    {
        PlayCollectEffect();

        switch (type)
        {
            case PickupType.Experience:
                ExperienceManager expManager = playerObj.GetComponent<ExperienceManager>();
                if (expManager != null)
                {
                    expManager.AddExperience(value);
                }
                break;

            case PickupType.Health:
                PlayerStats playerStats = playerObj.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.Heal(value);
                }
                break;
        }

        if (AudioManager.Instance != null)
        {
            switch (type)
            {
                case PickupType.Experience:
                    AudioManager.Instance.PlaySound("ExpPickup");
                    break;
                case PickupType.Health:
                    AudioManager.Instance.PlaySound("HealthPickup");
                    break;
            }
        }

        Destroy(gameObject);
    }

    void PlayCollectEffect()
    {
        if (collectEffectPrefab != null)
        {
            Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
        }
    }
}