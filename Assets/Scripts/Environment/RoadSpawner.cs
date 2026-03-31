using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public ListRandomizer<GameObject> roads;

    public ListRandomizer<ObjectPool> carPools;

    private List<GameObject> roadList = new();

    void Awake()
    {
        foreach (var carPool in carPools.Items)
        {
            carPool.Setup();
        }
    }

    void Start()
    {
        roadList.Add(Instantiate(roads.GetRandom(), new Vector3(-3.5f, -4f, 0), Quaternion.identity, transform));
    }

    void Update()
    {
        roadList.RemoveAll(road => road == null);

        if (roadList[^1].transform.position.y < -2f)
        {
            GameObject road = roads.GetRandom();
            Vector3 spawnPos = roadList[^1].transform.position + new Vector3(0f, 11.5f, 0f);

            road = Instantiate(road, spawnPos, Quaternion.identity, transform);
            roadList.Add(road);

            SpawnCars(road.GetComponent<Road>());
        }
    }

    void SpawnCars(Road road)
    {
        int numCars = UnityEngine.Random.Range(road.numCars.x, road.numCars.y);

        List<float> ySpawnPositions = new();

        for (int i = 0; i < numCars; i++)
        {
            int laneIndex = UnityEngine.Random.Range(0, road.lanePositions.Count);

            Vector3 lanePosition = road.lanePositions[laneIndex].position;

            float randX = UnityEngine.Random.Range(-0.5f, 0.5f);
            float randY = UnityEngine.Random.Range(-8f, -2f);
            
            ySpawnPositions.Add(randY);
            
            foreach (var yPos in ySpawnPositions)
            {
                if (randY < yPos + 2f && randY > yPos - 2f)
                {
                    randY += 6f;
                }
            }

            Vector3 randPosition = new(randX, randY, 0f);
            Vector3 spawnPos = lanePosition + randPosition;

            ObjectPool carPool = carPools.GetRandom();
            GameObject car = carPool.CreateObject(spawnPos, Quaternion.identity);
            car.transform.parent = transform;
            car.GetComponent<CarAI>().targetLane = road.lanePositions[laneIndex].position;

            if (car.name.Contains("Semi"))
                road.lanePositions.RemoveAt(laneIndex);
        }
    }
}