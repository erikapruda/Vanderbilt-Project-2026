using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public ListRandomizer<GameObject> roads;

    public ListRandomizer<ObjectPool> carPools;

    [Tooltip("Keep this option checked unless you are testing")]
    public bool forceCarSpawn = true;

    private List<GameObject> roadList = new();
    private uint carSpawnIndex = 1u;

    void Awake()
    {
        foreach (var carPool in carPools.Items)
        {
            carPool.Setup();
        }
    }

    void Start()
    {
        roadList.Add(Instantiate(roads.GetRandom(), new Vector3(-3.5f, -4f, 0), Quaternion.identity));
    }

    void Update()
    {
        roadList.RemoveAll(road => road == null);

        if (roadList[^1].transform.position.y < -2f)
        {
            GameObject road = roads.GetRandom();
            Vector3 spawnPos = roadList[^1].transform.position + new Vector3(0f, 11.5f, 0f);

            road = Instantiate(road, spawnPos, Quaternion.identity);
            roadList.Add(road);

            SpawnCars(road.GetComponent<Road>());
        }
    }

    void SpawnCars(Road road)
    {
        int numCars = UnityEngine.Random.Range(road.numCars.x, road.numCars.y);

        List<float> ySpawnPositions = new();
        var carList = FindObjectsByType<CarAI>(FindObjectsSortMode.None)
            .OrderBy(car => car.transform.position.sqrMagnitude)
            .ThenBy(car => car.transform.position.x)
            .ThenBy(car => car.transform.position.y)
            .ThenBy(car => car.transform.position.z)
            .ToArray();

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

            bool skipSpawn = false;

            foreach (var car in carList)
            {
                if (Vector3.Distance(car.transform.position, spawnPos) < 6f)
                {
                    skipSpawn = true;
                    break;
                }
            }

            if (skipSpawn)
                continue;
            
            ObjectPool carPool = carPools.GetRandom();
            GameObject spawnedCar = carPool.CreateObject(spawnPos, Quaternion.identity, false, forceCarSpawn);
            
            if (spawnedCar != null)
            {
                CarAI carAI = spawnedCar.GetComponent<CarAI>();
                carAI.SetSpawnSeed(GameManager.Singleton.Seed ^ carSpawnIndex);
                carAI.targetLane = lanePosition;
                spawnedCar.SetActive(true);

                carSpawnIndex++;

                if (spawnedCar.name.Contains("Semi"))
                    road.lanePositions.RemoveAt(laneIndex);
            }
        }
    }
}