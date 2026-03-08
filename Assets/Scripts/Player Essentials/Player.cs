using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Debt")]

    [SerializeField]
    private TextMeshProUGUI debtText;

    [SerializeField]
    private uint debtLabelMult = 1;

    [SerializeField]
    private byte debtLabelDecimalPlaces = 2;

    [Space]

    [Header("Animation")]

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

    [Space]

    [Header("Controls")]

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

    [Space]

    [Header("Interactions")]

    [SerializeField]
    private Transform worldTransform;

    [SerializeField]
    private GameObject debtTextPrefab;

    [Tooltip("Events to fire when crashing against an obstacle")]
    public UnityEvent OnHitObstacle;

    [SerializeField]
    [Tooltip("The time it takes in seconds for the car to correct from a crash")]
    private float recoveryTime = 3f;

    [SerializeField]
    [Tooltip("Extra knockback force to apply when hitting an obstacle")]
    [Range(0f, 1f)]
    private float crashKnockbackForce = 0.25f;

    Rigidbody2D rb;

    WaitForSeconds recoveryWait;

    public static Player Singleton { get; private set; }

    public bool IsInvincible { get; private set; }

    public System.Numerics.BigInteger Debt { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Singleton = this;
    }

    void FixedUpdate()
    {
        if (InputManager.IsGameplayInputEnabled)
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
        }

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

                // Check if object is a world object and move it on the y if so
                if (childTrans.TryGetComponent<WorldObject>(out _) && childTrans.TryGetComponent(out Rigidbody2D childRb))
                {
                    childRb.MovePosition(childRb.position + (childRb.linearVelocity * Time.fixedDeltaTime) - new Vector2(0f, rb.position.y));
                }
            }
            rb.position = new Vector2(rb.position.x, 0f);
        }
    }

    void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

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
        if (collision.gameObject.TryGetComponent(out WorldObstacle worldObstacle) && !worldObstacle.HasHitPlayer)
        {
            OnHitObstacle?.Invoke();
            worldObstacle.OnHitCar?.Invoke();
            worldObstacle.HasHitPlayer = true;

            InputManager.IsGameplayInputEnabled = false;
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
            AddDebt(worldObstacle.HitCost, averageContactPoint, -averageKnockback.normalized);

            // Apply knockback
            averageKnockback *= crashKnockbackForce;
            collision.rigidbody.AddForce(-averageKnockback, ForceMode2D.Impulse);
            rb.AddForce(averageKnockback, ForceMode2D.Impulse);
                
            // Recover car after recovery time
            recoveryWait = new WaitForSeconds(recoveryTime);
            StartCoroutine(RecoverCar());
        }
    }

    public void AddDebt(uint value, Vector2 debtTextPosition = default, Vector2 debtTextVelocity = default)
    {
        Debt += value;
        debtText.text = $"Debt ${GetDebtText(Debt)}"; ;

        // Create debt text at collision point
        if (debtTextPrefab != null)
        {
            GameObject instance = Instantiate(debtTextPrefab, debtTextPosition, Quaternion.identity);

            if (instance.TryGetComponent(out TextMeshPro text))
            {
                text.text = $"-${GetDebtText(value)}";
            }

            if (instance.TryGetComponent(out Rigidbody2D textRb))
            {
                textRb.linearVelocity = debtTextVelocity;
                textRb.angularVelocity = Vector2.SignedAngle(Vector2.up, debtTextVelocity);
            }
        }

        // Play debt add animation
        if (debtText.gameObject.TryGetComponent(out Animation animation))
        {
            animation.Play();
        }
    }

    string GetDebtText(System.Numerics.BigInteger value)
    {
        uint THRESHOLD_K = 1000 * debtLabelMult;
        uint THRESHOLD_M = 1000000 * debtLabelMult;
        ulong THRESHOLD_B = 1000000000ul * debtLabelMult;

        uint chosenLabelUnitPlace;
        if (value < THRESHOLD_K)
        {
            chosenLabelUnitPlace = 1;
        }
        else if (value < THRESHOLD_M)
        {
            chosenLabelUnitPlace = 1000;
        }
        else if (value < THRESHOLD_B)
        {
            chosenLabelUnitPlace = 1000000;
        }
        else
        {
            chosenLabelUnitPlace = 1000000000;
        }

        System.Numerics.BigInteger scaledDebt = value / chosenLabelUnitPlace;
        System.Numerics.BigInteger leftOverDebt = value % chosenLabelUnitPlace;
        byte numZerosBefore = (byte)(Mathf.Floor(Mathf.Log10(chosenLabelUnitPlace)) - Mathf.Floor(Mathf.Log10((float)leftOverDebt)) - 1f);

        char debtLabel = chosenLabelUnitPlace switch
        {
            1 => ' ',
            1000 => 'K',
            1000000 => 'M',
            1000000000 => 'B',
            _ => ' '
        };

        // Construct decimal place
        string leftOverDebtString = "";
        for (byte i = 0; i < numZerosBefore; i++)
        {
            leftOverDebtString += "0";
        }
        leftOverDebtString += leftOverDebt.ToString();
        leftOverDebtString = leftOverDebtString[0..Mathf.Min(leftOverDebtString.Length, debtLabelDecimalPlaces)];
        return leftOverDebtString.Length == 0 || leftOverDebt == 0 || numZerosBefore >= debtLabelDecimalPlaces ? $"{scaledDebt}{debtLabel}" : $"{scaledDebt}.{leftOverDebtString}{debtLabel}";
    }

    IEnumerator RecoverCar()
    {
        yield return recoveryWait;

        // Reenable inputs
        InputManager.IsGameplayInputEnabled = true;

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