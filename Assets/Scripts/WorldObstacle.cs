using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WorldObject))]
public class WorldObstacle : MonoBehaviour
{
    [SerializeField]
    private uint _hitCost;

    [Tooltip("Events to fire when obstacle collides with a car")]
    public UnityEvent OnHitCar;

    public WorldSpawner WorldSpawner { get; set; }

    public uint HitCost { get; private set; }

    public bool HasHitPlayer { get; set; }

    void Awake()
    {
        HitCost = _hitCost;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If you caused a wreck with this obstacle and this obstacle hits another obstacle you are liable
        if (HasHitPlayer && collision.gameObject.TryGetComponent(out WorldObstacle obstacle))
        {
            Player.Singleton.AddDebt(obstacle.HitCost);
            obstacle.HasHitPlayer = true;
        }
    }

    void OnDestroy()
    {
        WorldSpawner.NumObstaclesSpawned--;
    }
}