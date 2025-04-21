using UnityEngine;

public class PoisonballController : WeaponController
{
    [Header("Special Effect")]
    public float poisonDuration = 3f;
    public int poisonDamagePerTick = 3;
    public float poisonTickRate = 0.5f;
    public float knockbackForce = 2f;

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
        GameObject spawnedPoisonball = Instantiate(weaponData.prefab);
        spawnedPoisonball.transform.position = transform.position;

        PoisonballBehaviour poisonballBehaviour = spawnedPoisonball.GetComponent<PoisonballBehaviour>();
        if (poisonballBehaviour != null)
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

            poisonballBehaviour.DirectionChecker(direction);
            poisonballBehaviour.poisonDuration = poisonDuration;
            poisonballBehaviour.poisonDamagePerTick = poisonDamagePerTick;
            poisonballBehaviour.poisonTickRate = poisonTickRate;
            poisonballBehaviour.knockbackForce = knockbackForce;
        }
    }
}