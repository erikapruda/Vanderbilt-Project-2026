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

    void Awake()
    {
        HitCost = _hitCost;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player playerCar))
        {
            OnHitCar?.Invoke();
        }
    }

    void OnDestroy()
    {
        WorldSpawner.NumObstaclesSpawned--;
    }
}