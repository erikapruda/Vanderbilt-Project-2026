using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WorldObject))]
[RequireComponent(typeof(Rigidbody2D))]
public class WorldObstacle : MonoBehaviour
{
    [SerializeField]
    private uint _hitCost;

    [SerializeField]
    private GameObject SparkParticleSystemPrefab;

    public uint MaxPenalizedHits = 1;

    [Tooltip("Events to fire when obstacle collides with a car")]
    public UnityEvent OnHitCar;

    public WorldSpawner WorldSpawner { get; set; }

    public uint HitCost { get; private set; }

    public uint NumTimesPenaltyHit { get; set; } = 0u;

    public float CurrentPlayerHitCooldown { get; set; } = 0f;

    public bool HasHitPlayer { get; set; }

    private Rigidbody2D rb;

    void Awake()
    {
        HitCost = _hitCost;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CurrentPlayerHitCooldown = Mathf.Max(0f, CurrentPlayerHitCooldown - Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If you caused a wreck with this obstacle and this obstacle hits another obstacle you are liable
        if (HasHitPlayer && collision.gameObject.TryGetComponent(out WorldObstacle obstacle) && !obstacle.HasHitPlayer)
        {
            // Find average contact point
            Vector2 averageContactPoint = Vector2.zero;
            Vector2 averageKnockback = Vector2.zero;
            for (byte i = 0; i < collision.contactCount; i++)
            {
                averageContactPoint += collision.contacts[i].point;
                averageKnockback += collision.contacts[i].normal * collision.contacts[i].normalImpulse;
            }
            averageContactPoint /= collision.contactCount;
            averageKnockback /= collision.contactCount;

            Player.Singleton.AddDebt(obstacle.HitCost, averageContactPoint, -averageKnockback.normalized);
            obstacle.HasHitPlayer = true;
        }
        
        // Spawn particles
        if (SparkParticleSystemPrefab != null)
        {
            for (byte i = 0; i < collision.contactCount; i++)
            {
                GameObject particleSystemInstance = Instantiate(SparkParticleSystemPrefab, new Vector3(collision.contacts[i].point.x, collision.contacts[i].point.y, 0f), Quaternion.identity);
                if (particleSystemInstance.TryGetComponent(out Rigidbody2D rbInstance))
                {
                    rbInstance.linearVelocity = rb.linearVelocity + collision.contacts[i].normal;
                }
            }
        }
    }

    void OnDestroy()
    {
        if (WorldSpawner != null)
        {
            WorldSpawner.NumObstaclesSpawned--;            
        }
    }
}