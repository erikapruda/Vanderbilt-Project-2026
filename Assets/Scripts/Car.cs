using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Car : MonoBehaviour
{
    [SerializeField]
    private Transform worldTransform;

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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out WorldObject worldObject))
        {
            OnHitObstacle?.Invoke();
            worldObject.OnHitCar?.Invoke();
        }
    }
}