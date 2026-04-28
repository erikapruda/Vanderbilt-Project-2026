using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public ListRandomizer<GameObject> roads;

    public ListRandomizer<ObjectPool> carPoolsEasy;
    public ListRandomizer<ObjectPool> carPoolsMedium;
    public ListRandomizer<ObjectPool> carPoolsHard;

    [Tooltip("Keep this option checked unless you are testing")]
    public bool forceCarSpawn = true;

    private List<GameObject> roadList = new();
    private uint carSpawnIndex = 1u;

    private ListRandomizer<ObjectPool> carPools;

    void Awake()
    {
        switch (PlayerPrefs.GetInt("Difficulty", 2))
        {
            case 1: // Easy
                carPools = carPoolsEasy;
                break;
            case 2: // Medium
                carPools = carPoolsMedium;
                break;
            case 3: // Hard
                carPools = carPoolsHard;
                break;
            default:
                break;
        }

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
        int numCars = Random.Range(road.numCars.x, road.numCars.y);

        List<float> ySpawnPositions = new();

        var carArray = FindObjectsByType<CarAI>(FindObjectsSortMode.None);
        var carList = carArray.ToList();
        
        for (int i = 0; i < numCars; i++)
        {
            int laneIndex = Random.Range(0, road.lanePositions.Count);
            Vector3 lanePosition = road.lanePositions[laneIndex].position;

            float randX = Random.Range(-0.25f, 0.25f);
            float randY = Random.Range(-8f, -2f);
            
            ySpawnPositions.Add(randY);
            
            foreach (var yPos in ySpawnPositions)
            {
                if (randY < yPos + 3f && randY > yPos - 3f)
                {
                    randY += 6f;
                }
            }

            Vector3 randPosition = new(randX, randY, 0f);
            Vector3 spawnPos = lanePosition + randPosition;

            bool skipSpawn = false;

            foreach (var car in carList)
            {
                if (Vector2.Distance(car.transform.position, spawnPos) < 4f)
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
                
                carList.Add(carAI);
            }
        }
    }
}