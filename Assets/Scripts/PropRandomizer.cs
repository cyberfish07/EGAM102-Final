using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints;
    public List<GameObject> propPrefabs;
    private List<GameObject> usedSpawnPoints = new List<GameObject>();

    void Start()
    {
        SpawnProps();
    }

    void Update()
    {

    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnPoints)
        {
            if (usedSpawnPoints.Contains(sp))
            {
                continue;
            }
            int rand = Random.Range(0, propPrefabs.Count);
            GameObject prop = Instantiate(propPrefabs[rand], sp.transform.position, Quaternion.identity);
            prop.transform.parent = sp.transform;

            usedSpawnPoints.Add(sp);
        }
    }

    public void ResetSpawnPoints()
    {
        usedSpawnPoints.Clear();
    }
}