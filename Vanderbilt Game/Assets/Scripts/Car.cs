using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Car : MonoBehaviour
{
    [SerializeField]
    private Transform worldTransform;

    [SerializeField]
    private float maxLinearVelocity = 100f; // ~224 mph

    [SerializeField]
    private float autoLinearVelocitySpeed = 50f;

    [SerializeField]
    private float accelerationPower = 100f;

    [SerializeField]
    private float decelerationPower = 100f;

    [SerializeField]
    private float maxAngularVelocity = 100f;

    [SerializeField]
    private float steeringPower = 100f;

    [SerializeField]
    private float steeringCenterPower = 100f;

    Rigidbody2D rb;

    public static Car Singleton { get; private set; }

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
            rb.angularVelocity = Mathf.Clamp(rb.angularVelocity + (steeringPower * InputManager.SteeringInput * deltaTime), -maxAngularVelocity, maxAngularVelocity);
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

    void MoveWorld()
    {
        // Move world instead of car
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
    }
}