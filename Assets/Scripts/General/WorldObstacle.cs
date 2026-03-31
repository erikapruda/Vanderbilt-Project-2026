using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WorldObject))]
[RequireComponent(typeof(Rigidbody2D))]
public class WorldObstacle : MonoBehaviour
{
    [SerializeField]
    private uint _hitCost;

    [SerializeField]
    private ObjectPool SparkParticleSystemPrefabPool;

    [SerializeField]
    [Tooltip("Whether the player hitting this obstacle will make subsequent obstacle crashes blamed on the player")]
    private bool ShouldPropagateDebt = true;

    public uint MaxPenalizedHits = 1;

    [Tooltip("Events to fire when obstacle collides with a car")]
    public UnityEvent OnHitCar;

    public WorldSpawner WorldSpawner { get; set; }

    public uint HitCost { get; private set; }

    public uint NumTimesPenaltyHit { get; set; } = 0u;

    public float CurrentPlayerHitCooldown { get; set; } = 0f;

    public bool HasHitPlayer { get; set; }

    private Rigidbody2D rb;

    private Camera cam;

    void Awake()
    {
        cam = Camera.main;
        SparkParticleSystemPrefabPool.Setup();
        HitCost = _hitCost;
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        CurrentPlayerHitCooldown = 0f;
        NumTimesPenaltyHit = 0;
        HasHitPlayer = false;
    }

    void Update()
    {
        CurrentPlayerHitCooldown = Mathf.Max(0f, CurrentPlayerHitCooldown - Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If you caused a wreck with this obstacle and this obstacle hits another obstacle you are liable
        if (ShouldPropagateDebt && HasHitPlayer && collision.gameObject.TryGetComponent(out WorldObstacle obstacle) && !obstacle.HasHitPlayer && obstacle.IsOnScreen())
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
        if (SparkParticleSystemPrefabPool != null)
        {
            for (byte i = 0; i < collision.contactCount; i++)
            {
                GameObject particleSystemInstance = SparkParticleSystemPrefabPool.CreateObject(new Vector3(collision.contacts[i].point.x, collision.contacts[i].point.y, -2f), Quaternion.identity);
                if (particleSystemInstance.TryGetComponent(out Rigidbody2D rbInstance))
                {
                    rbInstance.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity + collision.contacts[i].normal, 1.0f);
                }
                if (particleSystemInstance.TryGetComponent(out ParticleSystem particleSystem))
                {
                    particleSystem.Play();
                }
            }
        }
    }

    void OnDisable()
    {
        if (WorldSpawner != null)
        {
            WorldSpawner.NumObstaclesSpawned--;
        }
    }

    public bool IsOnScreen()
    {
        // Check if the transform is within the viewport bounds
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        return viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1;
    }
}