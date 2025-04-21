using UnityEngine;

public class FireballController : WeaponController
{
    [Header("Special Effect")]
    public float burnDuration = 3f;
    public int burnDamagePerTick = 5;
    public float burnTickRate = 0.5f;
    public float knockbackForce = 3f;
    private int currentDirectionIndex = 0;

    private Vector3[] directions = new Vector3[8];

    protected override void Start()
    {
        base.Start();

        float angle = 0;
        for (int i = 0; i < 8; i++)
        {
            float rad = angle * Mathf.Deg2Rad;
            directions[i] = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;
            angle += 45;
        }
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedFireball = Instantiate(weaponData.prefab);
        spawnedFireball.transform.position = transform.position;

        FireballBehaviour fireballBehaviour = spawnedFireball.GetComponent<FireballBehaviour>();
        if (fireballBehaviour != null)
        {
            Vector3 direction;
            Transform nearestEnemy = FindNearestEnemy();
            if (nearestEnemy != null)
            {
                direction = (nearestEnemy.position - transform.position).normalized;
            }
            else if (pm != null)
            {
                direction = pm.lastMovedVector.normalized;
            }
            else
            {
                direction = Vector3.right;
            }

            fireballBehaviour.DirectionChecker(direction);
            fireballBehaviour.burnDuration = burnDuration;
            fireballBehaviour.burnDamagePerTick = burnDamagePerTick;
            fireballBehaviour.burnTickRate = burnTickRate;
            fireballBehaviour.knockbackForce = knockbackForce;
        }
    }
}