using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    [Header("Basic Values")]
    public int maxHealth = 30;
    public int currentHealth;
    public int damage = 10;
    public int scoreValue = 100;

    [Header("Drop Setting")]
    public PickupSpawner pickupSpawner;
    public float dropValueMultiplier = 1f;

    [Header("Particles")]
    public GameObject deathEffectPrefab;

    [Header("Visual Effects")]
    public int blinkTimes = 3;
    public Color blinkColor = Color.red;

    public bool isUnderBurningEffect = false;
    public bool isUnderFreezingEffect = false;
    public bool isUnderPoisonEffect = false;

    private bool isBlinking = false;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private Coroutine blinkCoroutine;
    private GameManager gameManager;

    private void Start()
    {
        currentHealth = maxHealth;
        gameManager = FindAnyObjectByType<GameManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (pickupSpawner == null)
        {
            pickupSpawner = GetComponent<PickupSpawner>();

            if (pickupSpawner == null)
            {
                pickupSpawner = gameObject.AddComponent<PickupSpawner>();
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("EnemyHit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Flash(Color flashColor, float duration = 0.5f)
    {
        if (spriteRenderer != null)
        {
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
            }

            blinkCoroutine = StartCoroutine(BlinkEffect(flashColor, duration));
        }
    }

    private IEnumerator BlinkEffect(Color blinkColor, float duration)
    {
        isBlinking = true;

        float blinkInterval = duration / (blinkTimes * 2);

        for (int i = 0; i < blinkTimes; i++)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkInterval);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        isBlinking = false;
    }

    void Die()
    {
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue);
        }

        if (pickupSpawner != null)
        {
            pickupSpawner.DropLoot(transform.position, dropValueMultiplier);
        }

        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("EnemyDeath");

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
}