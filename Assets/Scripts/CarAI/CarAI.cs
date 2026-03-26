using UnityEngine;

enum States
{
    Passive,
    TurnRight,
    TurnLeft
}

enum Detection
{
    Left,
    Right,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class CarAI : MonoBehaviour
{
    [Range(0, 1)]
    [Header("probability% per sec for a lane change")]
    public float laneChangeProbability = 0.01f;

    [Range(0, 1)]
    [Header("A weight added to the lane change probability\nwhen the car is situated behind another")]
    public float hostility = 0.0f;

    [Range(0, 3)]
    [Header("Seconds it takes to react to traffic conditions")]
    public float reactionTime = 1.5f;

    [Header("How aggressive a lane change or turn is")]
    public float turnSpeed = 1.0f;

    [Range(0, 6)]
    [Header("Delay in seconds for the vehicle to legally change lanes\n The timer resets when the opening closes")]
    public float turnDelay = 0.8f;

    [Header("A weight for factoring in random speed variability")]
    public float speedLimitLeniency = 1.0f;

    [Header("The value of a car is USD. The amount is the maximum\npoints a player can lose")]
    public int carHitPenalty = 20000;

    [Header("The number of people in the car. The number adds to\nthe death toll score")]
    public int passengers = 2;

    public Transform TopLeft;
    public Transform TopRight;
    public Transform BottomLeft;
    public Transform BottomRight;
    public Transform Left;
    public Transform Right;
    public Transform TopLeftEnd;
    public Transform TopRightEnd;
    public Transform BottomLeftEnd;
    public Transform BottomRightEnd;
    public Transform LeftEnd;
    public Transform RightEnd;

    private Detection detection;

    private Animator animator;

    private Rigidbody2D rb;

    private float targetSpeed = 0;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.Play("Left Turn Signal");

        targetSpeed = 15 + (15 * Random.Range(-speedLimitLeniency, speedLimitLeniency));
    }

    // Update is called once per frame
    void Update()
    {
        DetectCar();
        bool changeLane = laneChangeProbability <= Random.Range(0, 1);

        Vector2 velNorm = transform.up;
        float velMag = rb.linearVelocity.magnitude;
        velMag = Mathf.MoveTowards(velMag, targetSpeed, 15);
        rb.linearVelocity = velNorm * velMag;
    }

    void DetectCar()
    {
        RaycastHit2D raycastTopLeft = Physics2D.Raycast(TopLeft.position, TopLeftEnd.position - TopLeft.position,
            Vector2.Distance(TopLeft.position, TopLeftEnd.position));
        RaycastHit2D raycastTopRight = Physics2D.Raycast(TopRight.position, TopRightEnd.position - TopRight.position,
            Vector2.Distance(TopRight.position, TopRightEnd.position));
        RaycastHit2D raycastBottomLeft = Physics2D.Raycast(BottomLeft.position, BottomLeftEnd.position - BottomLeft.position,
            Vector2.Distance(BottomLeft.position, BottomLeftEnd.position));
        RaycastHit2D raycastBottomRight = Physics2D.Raycast(BottomRight.position, BottomRightEnd.position - BottomRight.position,
            Vector2.Distance(BottomRight.position, BottomRightEnd.position));
        RaycastHit2D raycastLeft = Physics2D.Raycast(Left.position, LeftEnd.position - Left.position,
            Vector2.Distance(Left.position, LeftEnd.position));
        RaycastHit2D raycastRight = Physics2D.Raycast(Right.position, RightEnd.position - Right.position,
            Vector2.Distance(Right.position, RightEnd.position));

        if (raycastTopLeft.collider != null && raycastTopLeft.collider.tag == "CarAI")
            detection |= Detection.TopLeft;
        else
            detection &= ~Detection.TopLeft;
        if (raycastTopRight.collider != null && raycastTopRight.collider.tag == "CarAI")
            detection |= Detection.TopRight;
        else
            detection &= ~Detection.TopRight;
        if (raycastBottomLeft.collider != null && raycastBottomLeft.collider.tag == "CarAI")
        {
            detection |= Detection.BottomLeft;
        }
        else
        {
            detection &= ~Detection.BottomLeft;
        }
        if (raycastBottomRight.collider != null && raycastBottomRight.collider.tag == "CarAI")
        {
            detection |= Detection.BottomRight;
        }
        else
        {
            detection &= ~Detection.BottomRight;
        }
        if (raycastLeft.collider != null && raycastLeft.collider.tag == "CarAI")
        {
            detection |= Detection.Left;
        }
        else
        {
            detection &= ~Detection.Left;
        }
        if (raycastRight.collider != null && raycastRight.collider.tag == "CarAI")
        {
            detection |= Detection.Right;
        }
        else
        {
            detection &= ~Detection.Right;
        }
    }

    void OnDrawGizmos()
    {
        // Draw top left line
        Gizmos.DrawLine(TopLeft.position, TopLeftEnd.position);
        // Draw top right line
        Gizmos.DrawLine(TopRight.position, TopRightEnd.position);
        // Draw bottom left line
        Gizmos.DrawLine(BottomLeft.position, BottomLeftEnd.position);
        // Draw bottom right
        Gizmos.DrawLine(BottomRight.position, BottomRightEnd.position);
        // Draw left line
        Gizmos.DrawLine(Left.position, LeftEnd.position);
        // Draw right line
        Gizmos.DrawLine(Right.position, RightEnd.position);
    }
}