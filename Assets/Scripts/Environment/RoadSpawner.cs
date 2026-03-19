using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public ListRandomizer<GameObject> roads;

    void Update()
    {
        GameObject[] roadGameObjects = GameObject.FindGameObjectsWithTag("Road");

        if (roadGameObjects.Length < 3)
        {
            GameObject road = roads.GetRandom();
            
            SpriteRenderer roadSprite = road.GetComponent<SpriteRenderer>();

            Vector3 spawnPos; 
            if (roadGameObjects.Length > 1)
                spawnPos = roadGameObjects[roadGameObjects.Length - 1].transform.position + new Vector3(0f, roadSprite.size.y, 0f);
            else
                spawnPos = new Vector3(0f, 0f, 0f);

            Instantiate(road, spawnPos, Quaternion.AngleAxis(90, Vector3.forward), transform);
        }
    }
}