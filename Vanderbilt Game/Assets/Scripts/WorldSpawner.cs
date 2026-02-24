using System.Collections;
using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [SerializeField]
    private ListRandomizer<GameObject> spawnableObstaclePrefabs;

    [SerializeField]
    private ListRandomizer<Transform> spawnerTransforms;

    [SerializeField]
    private float spawnRate = Mathf.Infinity;

    [SerializeField]
    private ushort maxObstacles = 1000;

    private WaitForSeconds waitSpawnRate;

    Coroutine spawnRoutine;

    public bool ShouldSpawnObstacles { get; set; } = true;

    public ushort NumObstaclesSpawned { get; set; } = 0;

    void Awake()
    {
        waitSpawnRate = new(spawnRate);
        spawnRoutine = StartCoroutine(SpawnObstacle());
    }

    void Update()
    {
        if (spawnableObstaclePrefabs.Count == 0)
        {
            ShouldSpawnObstacles = false;            
        }

        if (ShouldSpawnObstacles && spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnObstacle());
        }
        
        if (!ShouldSpawnObstacles && spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    IEnumerator SpawnObstacle()
    {
        while (true)
        {
            yield return waitSpawnRate;

            if (spawnableObstaclePrefabs.Count > 0 && NumObstaclesSpawned < maxObstacles)
            {
                // Choose the prefab to use at random
                GameObject chosenPrefab = spawnableObstaclePrefabs.GetRandom();

                // Create instance of the prefab
                GameObject newObstacle = Instantiate(chosenPrefab, transform);

                // Choose a random position to place the obstacle
                Vector3 worldSpawnPosition = spawnerTransforms.GetRandom().position;

                // Place the obstacle at the chosen world position
                newObstacle.transform.position = worldSpawnPosition;

                // Update obstacle counter
                NumObstaclesSpawned++;
                if (newObstacle.TryGetComponent(out WorldObject worldObject))
                {
                    worldObject.WorldSpawner = this;
                }
            }
        }
    }
}