using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("HP")]
    public int maxHealth = 100;
    public int currentHealth;
    public float invincibilityTime = 0.5f;

    [Header("UI")]
    public Slider healthBar;
    // 移除 public GameObject healTextPrefab;

    [Header("VFX")]
    public GameObject hitEffectPrefab;
    public GameObject healEffectPrefab;
    public int blinkTimes = 3;
    public Color blinkColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    private bool isBlinking = false;
    private float blinkDuration;
    private float blinkTimer = 0f;
    private int currentBlinkCount = 0;
    private bool isShowingOriginalColor = false;

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        UpdateHealthUI();
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;

            if (isBlinking)
            {
                blinkTimer -= Time.deltaTime;

                if (blinkTimer <= 0f)
                {
                    blinkTimer = blinkDuration;

                    isShowingOriginalColor = !isShowingOriginalColor;

                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = isShowingOriginalColor ? originalColor : blinkColor;
                    }

                    if (!isShowingOriginalColor)
                    {
                        currentBlinkCount++;

                        if (currentBlinkCount >= blinkTimes)
                        {
                            isBlinking = false;
                        }
                    }
                }
            }

            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                isBlinking = false;
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = originalColor;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;

        currentHealth -= damage;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("PlayerHit");

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        StartBlink();

        isInvincible = true;
        invincibilityTimer = invincibilityTime;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void StartBlink()
    {
        if (spriteRenderer != null)
        {
            isBlinking = true;
            currentBlinkCount = 0;
            blinkDuration = invincibilityTime / (blinkTimes * 2);
            blinkTimer = blinkDuration;
            isShowingOriginalColor = false;
            spriteRenderer.color = blinkColor;
        }
    }

    public void Heal(int amount)
    {
        int actualHeal = Mathf.Min(amount, maxHealth - currentHealth);
        currentHealth += actualHeal;

        UpdateHealthUI();

        if (AudioManager.Instance != null && actualHeal > 0)
            AudioManager.Instance.PlaySound("Heal");

        if (healEffectPrefab != null && actualHeal > 0)
        {
            Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void Die()
    {
        isBlinking = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("PlayerDeath");

        GameManager gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver(false);
        }
    }

    public void IncreaseMaxHealth(float multiplier)
    {
        int oldMaxHealth = maxHealth;
        maxHealth = Mathf.RoundToInt(maxHealth * (1 + multiplier));

        currentHealth += (maxHealth - oldMaxHealth);

        UpdateHealthUI();
    }
}