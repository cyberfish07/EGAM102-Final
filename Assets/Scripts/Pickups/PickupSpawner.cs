using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PickupDrop
    {
        public GameObject pickupPrefab;
        public float dropChance = 0.5f;
        [Range(1, 5)]
        public int maxDropAmount = 1;
    }

    [Header("Spawning Setting")]
    public PickupDrop[] possibleDrops;
    public float experienceDropChance = 1.0f;
    public GameObject experienceGemPrefab;

    public void DropLoot(Vector3 position, float valueMultiplier = 1f)
    {
        if (Random.value <= experienceDropChance && experienceGemPrefab != null)
        {
            int gemCount = Mathf.FloorToInt(Random.Range(1, 4) * valueMultiplier);

            for (int i = 0; i < gemCount; i++)
            {
                GameObject expGem = Instantiate(experienceGemPrefab, position, Quaternion.identity);

                Vector2 offset = Random.insideUnitCircle * 0.5f;
                expGem.transform.position += new Vector3(offset.x, offset.y, 0);

                PickupObject pickup = expGem.GetComponent<PickupObject>();
                if (pickup != null)
                {
                    pickup.value = (int)(Random.Range(1, 3) * valueMultiplier);
                }
            }
        }

        foreach (PickupDrop drop in possibleDrops)
        {
            if (Random.value <= drop.dropChance)
            {
                int count = Random.Range(1, drop.maxDropAmount + 1);

                for (int i = 0; i < count; i++)
                {
                    GameObject pickup = Instantiate(
                        drop.pickupPrefab,
                        position,
                        Quaternion.identity
                    );

                    Vector2 offset = Random.insideUnitCircle * 0.5f;
                    pickup.transform.position += new Vector3(offset.x, offset.y, 0);
                }
            }
        }
    }
}