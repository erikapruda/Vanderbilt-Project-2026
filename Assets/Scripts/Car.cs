using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Car : MonoBehaviour
{
    [SerializeField]
    private Transform worldTransform;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private string drivingAnimName;

    [SerializeField]
    private string drivingLeftAnimName;

    [SerializeField]
    private string drivingRightAnimName;

    [SerializeField]
    private string invincibilityAnimName;

    [SerializeField]
    [Tooltip("The maximum speed the car can go")]
    private float maxLinearVelocity = 100f; // ~224 mph

    [SerializeField]
    [Tooltip("The car's target speed when the accelerator and decelerator are idle")]
    private float autoLinearVelocitySpeed = 50f;

    [SerializeField]
    [Tooltip("How fast the car accelerates towards the target speed")]
    private float accelerationPower = 100f;

    [SerializeField]
    [Tooltip("How fast the car decelerates towards the target speed")]
    private float decelerationPower = 100f;

    [SerializeField]
    [Tooltip("The maximum rotational speed the car can turn")]
    private float maxAngularVelocity = 100f;

    [SerializeField]
    [Tooltip("The car's steering acceleration when steering input is in use")]
    private float steeringPower = 100f;

    [SerializeField]
    [Tooltip("The car's steering acceleration when steering input is released")]
    private float steeringCenterPower = 100f;

    [SerializeField]
    [Tooltip("The time it takes in seconds for the car to correct from a crash")]
    private float recoveryTime = 1f;

    [Tooltip("Events to fire when crashing against an obstacle")]
    public UnityEvent OnHitObstacle;

    Rigidbody2D rb;

    WaitForSeconds recoveryWait;

    public static Car Singleton { get; private set; }

    public bool IsInvincible { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Singleton = this;
    }

    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;

        // Adjust angular velocity based on steering
        if (InputManager.SteeringInput == 0f)
        {
            rb.angularVelocity = Mathf.MoveTowards(rb.angularVelocity, 0f, steeringCenterPower * deltaTime);
        }
        else
        {
            rb.angularVelocity = Mathf.Clamp(rb.angularVelocity + (steeringPower * -InputManager.SteeringInput * deltaTime), -maxAngularVelocity, maxAngularVelocity);
        }

        // Get current velocity direction and speed
        Vector2 velNorm = transform.up;
        float velMag = rb.linearVelocity.magnitude;

        // Accelerate/decelerate based on input
        float targetSpeed = InputManager.AccelerateInputHeld ? maxLinearVelocity : autoLinearVelocitySpeed;
        float celeration = InputManager.DecelerateInputHeld ? decelerationPower * deltaTime : accelerationPower * deltaTime;
        velMag = Mathf.MoveTowards(velMag, targetSpeed, celeration);
        rb.linearVelocity = velNorm * velMag;

        MoveWorld();
    }

    // Move the world instead of the car for proper floating point world origin
    void MoveWorld()
    {
        if (worldTransform != null)
        {
            for (int i = 0; i < worldTransform.childCount; i++)
            {
                Transform childTrans = worldTransform.GetChild(i);

                // Check if object is a world object and move it if so
                if (childTrans.TryGetComponent<WorldObject>(out _) && childTrans.TryGetComponent(out Rigidbody2D childRb))
                {
                    childRb.MovePosition(childRb.position - rb.position);
                }
            }
            rb.position = Vector2.zero;
        }
    }

    void LateUpdate()
    {
        transform.position = Vector3.zero;

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
        if (collision.gameObject.TryGetComponent(out WorldObstacle worldObstacle))
        {
            OnHitObstacle?.Invoke();

            // Recover car after recovery time
            recoveryWait = new WaitForSeconds(recoveryTime);
            StartCoroutine(RecoverCar());
        }
    }

    IEnumerator RecoverCar()
    {
        yield return recoveryWait;

        // Reset orientation
        rb.rotation = 0f;

        // Reset velocities
        rb.linearVelocity = Vector2.zero;
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