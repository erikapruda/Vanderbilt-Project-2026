using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WorldObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The distance from the world origin at which this object despawns")]
    private float despawnDistance = 20f;

    [Tooltip("Events to fire when obstacle collides with a car")]
    public UnityEvent OnHitCar;

    private WaitForSeconds despawnCheckFrequency = new(0.5f);

    public WorldSpawner WorldSpawner { get; set; }

    void Awake()
    {
        StartCoroutine(CheckDespawn());
    }

    IEnumerator CheckDespawn()
    {
        while (true)
        {
            yield return despawnCheckFrequency;

            if (transform.position.magnitude >= despawnDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDestroy()
    {
        WorldSpawner.NumObstaclesSpawned--;
    }
}