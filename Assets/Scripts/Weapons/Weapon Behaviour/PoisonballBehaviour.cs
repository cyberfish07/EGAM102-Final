using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonballBehaviour : ProjectileWeaponBehaviour
{
    [Header("Effect")]
    public float poisonDuration = 3f;
    public int poisonDamagePerTick = 3;
    public float poisonTickRate = 0.5f;
    public float knockbackForce = 2f;

    public GameObject poisonEffect;

    private List<EnemyStats> poisonedEnemies = new List<EnemyStats>();

    protected override void Start()
    {
        base.Start();
        poisonDuration = 7.0f;
        poisonDamagePerTick = 4;
        poisonTickRate = 0.4f;
        knockbackForce = 0.5f;
    }

    IEnumerator ApplyPoisonEffect(EnemyStats enemy, SpriteRenderer sprite)
    {
        float elapsedTime = 0f;
        float nextTickTime = 0f;
        float nextBlinkTime = 0f;
        bool isBlinking = false;
        Color originalColor = sprite ? sprite.color : Color.white;
        Color poisonColor = Color.green;

        while (elapsedTime < poisonDuration && enemy != null)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= nextTickTime)
            {
                nextTickTime = elapsedTime + poisonTickRate;
                if (enemy != null)
                {
                    enemy.TakeDamage(poisonDamagePerTick);
                }
            }

            if (elapsedTime >= nextBlinkTime && sprite != null)
            {
                nextBlinkTime = elapsedTime + 0.1f;
                isBlinking = !isBlinking;
                sprite.color = isBlinking ? poisonColor : originalColor;
            }

            yield return null;
        }

        if (sprite != null)
        {
            sprite.color = originalColor;
        }

        if (enemy != null)
        {
            poisonedEnemies.Remove(enemy);
        }
    }

    protected override void HandleEnemyCollision(Collider2D collision)
    {
        EnemyStats enemy = collision.GetComponent<EnemyStats>();
        EnemyMovement enemyMovement = collision.GetComponent<EnemyMovement>();
        SpriteRenderer enemySprite = collision.GetComponent<SpriteRenderer>();

        if (enemy != null)
        {
            if (enemyMovement != null)
            {
                enemyMovement.ApplyKnockback(direction, knockbackForce * 0.1f, 0.05f);
            }

            if (!poisonedEnemies.Contains(enemy))
            {
                poisonedEnemies.Add(enemy);

                enemy.Flash(Color.green);

                StartCoroutine(ApplyPoisonEffect(enemy, enemySprite));

                if (poisonEffect != null)
                {
                    GameObject effect = Instantiate(poisonEffect, collision.transform);
                    Destroy(effect, poisonDuration);
                }
            }

            Destroy(gameObject);
        }
    }
}