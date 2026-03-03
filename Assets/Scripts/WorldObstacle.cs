using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WorldObject))]
public class WorldObstacle : MonoBehaviour
{
    [Tooltip("Events to fire when obstacle collides with a car")]
    public UnityEvent OnHitCar;

    public WorldSpawner WorldSpawner { get; set; }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Car playerCar))
        {
            OnHitCar?.Invoke();
        }
    }

    void OnDestroy()
    {
        WorldSpawner.NumObstaclesSpawned--;
    }
}