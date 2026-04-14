using UnityEngine;

public class TravelSpawner : MonoBehaviour
{
    [Tooltip("The prefab to use when spawning")]
    public GameObject Prefab;

    [Tooltip("Where to spawn the object")]
    public Transform SpawnerTransform;

    [Tooltip("How many meters to travel to spawn object")]
    public float SpawnFrequency;

    public bool UseObjectPool;

    // The amount of distance left to travel before spawning again
    private float distanceLeftToNextSpawn;

    // The spawner's previous frame position
    private Vector3 previousPosition;

    void Awake()
    {
        SpawnerTransform ??= transform;
        distanceLeftToNextSpawn = SpawnFrequency;
        previousPosition = SpawnerTransform.position;
    }

    void Update()
    {
        if (SpawnerTransform == null) return;

        distanceLeftToNextSpawn -= Vector3.Distance(previousPosition, SpawnerTransform.position);

        if (distanceLeftToNextSpawn <= 0)
        {
            distanceLeftToNextSpawn = SpawnFrequency;

            Instantiate(Prefab, SpawnerTransform.position, SpawnerTransform.rotation);
        }
    }
}