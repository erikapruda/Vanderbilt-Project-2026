using UnityEngine;

public class TravelSpawner : MonoBehaviour
{
    [Tooltip("The prefab pool to use when spawning")]
    public ObjectPool PrefabPool;

    [Tooltip("Where to spawn the object")]
    public Transform SpawnerTransform;

    [Tooltip("How many meters to travel to spawn object")]
    public float SpawnFrequency;

    // The amount of distance left to travel before spawning again
    private float distanceLeftToNextSpawn;

    // The spawner's previous frame position
    private Vector3 previousPosition;

    void Awake()
    {
        if (SpawnerTransform == null)
        {
            SpawnerTransform = transform;
        }
        distanceLeftToNextSpawn = SpawnFrequency;
        previousPosition = SpawnerTransform.position;
        PrefabPool.Setup();
    }

    void Update()
    {
        if (SpawnerTransform == null) return;

        distanceLeftToNextSpawn -= Vector3.Distance(previousPosition, SpawnerTransform.position);
        previousPosition = SpawnerTransform.position;

        if (distanceLeftToNextSpawn <= 0)
        {
            distanceLeftToNextSpawn = SpawnFrequency;
            PrefabPool.CreateObject(SpawnerTransform.position, SpawnerTransform.rotation);
        }
    }
}