using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Car : MonoBehaviour
{
    [SerializeField]
    private Transform WorldTransform;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (InputManager.SteeringInput == 0f)
        {
            rb.angularVelocity = Mathf.MoveTowards(rb.angularVelocity, 0f, steeringCenterPower * Time.deltaTime);
        }
        else
        {
            rb.angularVelocity = Mathf.Clamp(rb.angularVelocity + (steeringPower * InputManager.SteeringInput * Time.deltaTime), -maxAngularVelocity, maxAngularVelocity);
        }

        if (InputManager.AccelerateInputHeld)
        {

        }
        else
        {
            Vector2 velNorm = rb.linearVelocity.normalized;
            float velMag = rb.linearVelocity.magnitude;
            //Vector2 
        }

        //if (InputManager.AccelerateInputHeld)
        //{
        //    rb.AddForce
        //}
    }
}
