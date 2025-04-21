using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceballBehaviour : ProjectileWeaponBehaviour
{
    [Header("Effect")]
    public float slowDuration = 3f;
    public float slowFactor = 0.5f;
    public float knockbackForce = 2f;

    public GameObject iceEffect;

    private List<EnemyMovement> slowedEnemies = new List<EnemyMovement>();

    protected override void Start()
    {
        base.Start();
        slowDuration = 5.0f;
        slowFactor = 0.3f;
        knockbackForce = 1.5f;
    }

    IEnumerator ApplyIceEffect(EnemyMovement enemy, SpriteRenderer sprite)
    {
        float originalSpeed = enemy.moveSpeed;
        Color originalColor = sprite.color;

        enemy.moveSpeed *= slowFactor;
        sprite.color = Color.blue;

        yield return new WaitForSeconds(slowDuration);

        if (enemy != null)
        {
            enemy.moveSpeed = originalSpeed;
            slowedEnemies.Remove(enemy);
        }

        if (sprite != null)
        {
            sprite.color = originalColor;
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
                enemyMovement.ApplyKnockback(direction, knockbackForce * 0.5f, 0.1f);

                if (!slowedEnemies.Contains(enemyMovement))
                {
                    slowedEnemies.Add(enemyMovement);

                    if (enemySprite != null)
                    {
                        StartCoroutine(ApplyIceEffect(enemyMovement, enemySprite));
                    }

                    if (iceEffect != null)
                    {
                        GameObject effect = Instantiate(iceEffect, collision.transform);
                        Destroy(effect, slowDuration);
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}