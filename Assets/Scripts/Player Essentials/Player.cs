using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Sound")]

    [Tooltip("The soundfx mixer to plug into the car sounds")]
    [SerializeField]
    private UnityEngine.Audio.AudioMixerGroup soundFxMixer;

    [Tooltip("The player car's engine sound")]
    [SerializeField]
    private AudioClip engineSound;

    [Tooltip("The volume of the engine sound")]
    [SerializeField]
    private float engineSoundVolume;

    [Tooltip("The pitch of the engine sound when the car is not moving")]
    [SerializeField]
    private float engineSoundIdlePitch;

    [Tooltip("The pitch of the engine sound when the car is driving at full speed")]
    [SerializeField]
    private float engineSoundRunningPitch;

    [Space]

    [Header("Animation")]

    [SerializeField]
    private string drivingAnimName;

    [SerializeField]
    private string drivingLeftAnimName;

    [SerializeField]
    private string drivingRightAnimName;

    [SerializeField]
    private string invincibilityAnimName;

    [Space]

    [Header("Controls")]

    private float maxLinearVelocity;
    //[HideInInspector]
    public float autoLinearVelocitySpeed;

    [Tooltip("The maximum speed the car can go")]
    [SerializeField]
    private float maxLinearVelocityEasy = 10f;
    [SerializeField]
    private float maxLinearVelocityMedium = 15f;
    [SerializeField]
    private float maxLinearVelocityHard = 20f;

    [Tooltip("The car's target speed when the accelerator and decelerator are idle")]
    public float autoLinearVelocitySpeedEasy = 10f;
    public float autoLinearVelocitySpeedMedium = 15f;
    public float autoLinearVelocitySpeedHard = 20f;

    [SerializeField]
    [Tooltip("How fast the car accelerates towards the target speed")]
    private float accelerationPower = 100f;

    [SerializeField]
    [Tooltip("How fast the car decelerates towards the target speed")]
    private float decelerationPower = 100f;

    [SerializeField]
    [Tooltip("The maximum angle in degrees that this car can turn")]
    private float maxRotationAngle = 100f;

    [SerializeField]
    [Tooltip("The car's steering acceleration when steering input is in use")]
    private float steeringPower = 100f;

    [SerializeField]
    [Tooltip("The car's steering acceleration when steering input is released")]
    private float steeringCenterPower = 100f;

    [SerializeField]
    private List<ParticleSystem> driftParticleSystems = new();

    [SerializeField]
    [Tooltip("The car's minimum horizontal speed at which smoke starts appearing around the tires")]
    private float driftSpeed = 6f;

    [Space]

    [Header("Interactions")]

    [Tooltip("Events to fire when crashing against an obstacle")]
    public UnityEvent OnHitObstacle;

    [SerializeField]
    [Tooltip("The time it takes in seconds for the car to correct from a crash")]
    private float recoveryTime = 3f;

    [SerializeField]
    [Tooltip("How long it takes before the player can collide against the same obstacle")]
    private float hitCooldown = 0.1f;

    Animator animator;

    Rigidbody2D rb;

    AudioSource engineSoundContainerAudioSource;

    WaitForSeconds recoveryWait;

    Coroutine recoverCarRoutine;

    float prevPosX;

    public static Player Singleton { get; private set; }

    public bool IsInvincible { get; private set; }

    private void Awake()
    {
        // Set the player speed based on the difficulty
        switch (PlayerPrefs.GetInt("Difficulty", 2))
        {
            case 1: // Easy
                autoLinearVelocitySpeed = autoLinearVelocitySpeedEasy;
                maxLinearVelocity = maxLinearVelocityEasy;
                break;
            case 2: // Medium
                autoLinearVelocitySpeed = autoLinearVelocitySpeedMedium;
                maxLinearVelocity = maxLinearVelocityMedium;
                break;
            case 3: // Hard
                autoLinearVelocitySpeed = autoLinearVelocitySpeedHard;
                maxLinearVelocity = maxLinearVelocityHard;
                break;
            default:
                break;
        }

        prevPosX = transform.position.x;
        TryGetComponent(out animator);
        rb = GetComponent<Rigidbody2D>();
        Singleton = this;

        World.CenteredTransform = transform;
        World.Origin = transform.position;

        // Create an engine sound container and play the engine sound
        GameObject engineSoundContainer = new("Player_Engine_Audio_Container", typeof(AudioSource));
        engineSoundContainer.transform.parent = transform;
        engineSoundContainer.transform.localPosition = Vector3.zero;
        engineSoundContainerAudioSource = engineSoundContainer.GetComponent<AudioSource>();
        engineSoundContainerAudioSource.clip = engineSound;
        engineSoundContainerAudioSource.outputAudioMixerGroup = soundFxMixer;
        engineSoundContainerAudioSource.volume = engineSoundVolume;
        engineSoundContainerAudioSource.pitch = engineSoundIdlePitch;
        engineSoundContainerAudioSource.loop = true;
        engineSoundContainerAudioSource.Play();
    }

    void FixedUpdate()
    {
        UpdateController();        
        PlayRoadParticles();
    }

    void UpdateController()
    {
        if (InputManager.IsGameplayInputEnabled)
        {
            float deltaTime = Time.fixedDeltaTime;

            // Adjust angular velocity based on steering
            if (InputManager.SteeringInput == 0f)
            {
                rb.MoveRotation(Mathf.MoveTowards(rb.rotation, 0f, steeringCenterPower * deltaTime));
            }
            else
            {
                rb.MoveRotation(Mathf.Clamp(rb.rotation + (steeringPower * -InputManager.SteeringInput * deltaTime), -maxRotationAngle, maxRotationAngle));
            }

            // Get current velocity direction and speed
            Vector2 velNorm = transform.up;
            float velMag = rb.linearVelocity.magnitude;

            // Accelerate/decelerate based on input
            float targetSpeed = InputManager.AccelerateInputHeld ? maxLinearVelocity : autoLinearVelocitySpeed;
            float celeration = InputManager.DecelerateInputHeld ? decelerationPower * deltaTime : accelerationPower * deltaTime;
            velMag = Mathf.MoveTowards(velMag, targetSpeed, celeration);
            
            // Update velocity
            rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, velNorm.x * velMag, celeration);
            rb.linearVelocityY = Mathf.MoveTowards(rb.linearVelocityY, targetSpeed, celeration);
        }

        // Limit backwards velocity
        rb.linearVelocityY = Mathf.Max(0.1f, rb.linearVelocityY);

        // Update World
        World.CurrentOffset = rb.position;
        rb.position = new Vector2(Mathf.Clamp(rb.position.x, WorldBounds.Singleton.LeftX, WorldBounds.Singleton.RightX), World.Origin.y);

        // Play engine sound with a pitch linearly interpolated by the car speed
        engineSoundContainerAudioSource.pitch = InputManager.IsGameplayInputEnabled ?
            engineSoundIdlePitch + ((engineSoundRunningPitch - engineSoundIdlePitch) * (rb.linearVelocityY / autoLinearVelocitySpeed)) :
            engineSoundIdlePitch;
    }

    void LateUpdate()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, WorldBounds.Singleton.LeftX, WorldBounds.Singleton.RightX), World.Origin.y, World.Origin.z);

        AnimationClip currentClip = null;
        if (animator != null && animator.GetCurrentAnimatorClipInfo(0).Length > 0)
          currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;

        if (currentClip != null && currentClip.name != invincibilityAnimName)
        {
            // Remove invincibility
            if (IsInvincible)
            {
                OnInvincibilityFinished();
            }

            if (InputManager.SteeringInput < -0.1f && currentClip.name != drivingLeftAnimName)
            {
                animator.Play(drivingLeftAnimName);
            }
            else if (InputManager.SteeringInput > 0.1f && currentClip.name != drivingRightAnimName)
            {
                animator.Play(drivingRightAnimName);
            }
            else if (Mathf.Abs(InputManager.SteeringInput) <= 0.1f && currentClip.name != drivingAnimName)
            {
                animator.Play(drivingAnimName);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionLogic(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        CollisionLogic(collision);
    }

    void CollisionLogic(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WorldObstacle worldObstacle) && worldObstacle.CurrentPlayerHitCooldown == 0f)
        {
            worldObstacle.CurrentPlayerHitCooldown = hitCooldown;

            Vector2 averageContactPoint = Vector2.zero;
            Vector2 averageKnockback = Vector2.zero;

            // Add knockback forces away from each other
            for (byte i = 0; i < collision.contactCount; i++)
            {
                // Calculate knockback
                Vector2 hitNormal = collision.contacts[i].normal;
                float hitStrength = collision.contacts[i].normalImpulse;

                averageKnockback += hitStrength * hitNormal;
                averageContactPoint += collision.contacts[i].point;
            }

            averageContactPoint /= collision.contactCount;
            averageKnockback /= collision.contactCount;

            // Penalize player if available
            if (worldObstacle.NumTimesPenaltyHit < worldObstacle.MaxPenalizedHits)
            {
                worldObstacle.NumTimesPenaltyHit++;
                OnHitObstacle?.Invoke();
                worldObstacle.OnHitCar?.Invoke();
                worldObstacle.HasHitPlayer = true;
                InputManager.IsGameplayInputEnabled = false;
                DebtSystem.AddDebt(worldObstacle.HitCost, averageContactPoint, -averageKnockback.normalized);
            }

            // Recover car after recovery time
            recoveryWait = new WaitForSeconds(recoveryTime);
            
            recoverCarRoutine ??= StartCoroutine(RecoverCar());
        }

        if (collision.gameObject.TryGetComponent(out CarAI car))
        {
            car.lostControl = true;
            car.rb.AddTorque(rb.angularVelocity * 0.5f);
        }
    }

    void PlayRoadParticles()
    {
        if (driftParticleSystems == null || driftParticleSystems.Count == 0) return;

        foreach (var driftParticleSystem in driftParticleSystems)
        {
            if (InputManager.IsGameplayInputEnabled && Mathf.Abs((prevPosX - rb.position.x) / Time.fixedDeltaTime) > driftSpeed)
            {
                if (!driftParticleSystem.emission.enabled)
                {
                    ParticleSystem.EmissionModule emissionModule = driftParticleSystem.emission;
                    emissionModule.enabled = true;
                }
            }
            else if (driftParticleSystem.emission.enabled)
            {
                ParticleSystem.EmissionModule emissionModule = driftParticleSystem.emission;
                emissionModule.enabled = false;
            }
        }

        prevPosX = rb.position.x;
    }

    IEnumerator RecoverCar()
    {
        yield return recoveryWait;

        // Reenable inputs
        InputManager.IsGameplayInputEnabled = true;

        // Reset orientation
        rb.rotation = 0f;

        // Reset angular velocity
        rb.angularVelocity = 0f;

        // Play invincibility animation and disable colliders if animation exists
        if (animator != null)
        {
            animator.Play(invincibilityAnimName);

            // Disable colliders to make invulnerable
            Collider2D[] colliders = new Collider2D[rb.attachedColliderCount];
            if (rb.GetAttachedColliders(colliders) > 0)
            {
                foreach (var collider in colliders)
                {
                    collider.isTrigger = true;
                }
            }

            IsInvincible = true;
        }

        recoverCarRoutine = null;
    }

    void OnInvincibilityFinished()
    {
        // Restore colliders
        Collider2D[] colliders = new Collider2D[rb.attachedColliderCount];
        if (rb.GetAttachedColliders(colliders) > 0)
        {
            foreach (var collider in colliders)
            {
                collider.isTrigger = false;
            }
        }

        IsInvincible = false;
        animator.Play(drivingAnimName);
    }
}