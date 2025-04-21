using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Data")]
    public WeaponScriptableObject weaponData;

    [Header("Multiplier")]
    public float damageMultiplier = 1f;
    public float cooldownMultiplier = 1f;
    public float areaMultiplier = 1f;

    protected float currentCooldown;
    protected PlayerMovement pm;

    public float CurrentDamage
    {
        get { return weaponData.damage * damageMultiplier; }
    }

    public float CurrentCooldown
    {
        get { return weaponData.cooldownDuration * cooldownMultiplier; }
    }

    protected virtual void Start()
    {
        pm = FindAnyObjectByType<PlayerMovement>();
        currentCooldown = CurrentCooldown;
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        currentCooldown = CurrentCooldown;
    }

    protected Transform FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
            return null;

        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = enemy.transform;
                }
            }
        }

        return nearestEnemy;
    }
}