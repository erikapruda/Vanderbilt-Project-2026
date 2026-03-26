using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public ListRandomizer<GameObject> roads;

    void Start()
    {
        Instantiate(roads.GetRandom(), new Vector3(-3.5f, -4f, 0), Quaternion.identity, transform);
    }

    void Update()
    {
        GameObject[] roadGameObjects = GameObject.FindGameObjectsWithTag("Road");
        
        if (roadGameObjects[^1].transform.position.y < 2)
        {
            GameObject road = roads.GetRandom();

            Vector3 spawnPos = roadGameObjects[^1].transform.position + new Vector3(0f, 11.5f, 0f);

            Instantiate(road, spawnPos, Quaternion.identity, transform);
        }
    }
}