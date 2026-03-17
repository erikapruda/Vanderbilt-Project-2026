using UnityEngine;

enum States
{
    Passive,
    TurnRight,
    TurnLeft
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


    private Animator animator;
    private BoxCollider2D collider;
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        animator.Play("Left Turn Signal");
    }

    // Update is called once per frame
    void Update()
    {
        bool changeLane = laneChangeProbability <= Random.Range(0, 1);
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