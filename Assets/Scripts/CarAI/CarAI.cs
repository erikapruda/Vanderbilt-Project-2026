using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
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
    public float reactionTime = 1f;

    [Header("How aggressive a lane change or turn is")]
    public float turnSpeed = 1.0f;

    [Range(0, 6)]
    [Header("Delay in seconds for the vehicle to legally change lanes\n The timer resets when the opening closes")]
    public float turnDelay = 0.8f;

    [Header("A weight for factoring in random speed variability")]
    public float speedLimitLeniency = 1.0f;

    [Header("The number of people in the car. The number adds to\nthe death toll score")]
    public int passengers = 2;

    public float detectionDistance = 2.0f;

    private Animator animator;

    private Rigidbody2D rb;

    private float targetSpeed = 0;
    
    private float currentSpeed;

    private float turnTimer = 0f;

    private bool isChangingLanes = false;

    private bool lostControl = false;

    private float currentTurnSpeed;

    private Player player;
    readonly private List<GameObject> cars = new();

    [HideInInspector]
    public Vector3 targetLane;

    private Vector3 startingLane;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = FindObjectsByType<Player>(FindObjectsSortMode.None)[0];
    }

    void OnEnable()
    {
        animator.Play("Left Turn Signal");
        targetSpeed = 12 + (15 * Random.Range(-speedLimitLeniency, speedLimitLeniency));
        currentSpeed = targetSpeed;
        startingLane = targetLane;
        rb.angularVelocity = 0f;
        rb.rotation = 0f;
        
        isChangingLanes = false;
        lostControl = false;
        turnTimer = 0f;
        currentSpeed = 0f;
        currentSpeed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // If the car is no longer straight and at a steep angle, start crashing state
        if (Mathf.Abs(Vector2.Angle(transform.up, Vector3.up)) > 60f)
            lostControl = true;

        DetectCar();

        if (!lostControl)
        {
            ChooseLane();
            AvoidCar();
            RotateCar();
        }
        else
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 2);
        
        rb.linearVelocity = currentSpeed * transform.up;
    }

    void RotateCar()
    {
        Vector3 targetDirection;

        if (isChangingLanes && turnTimer > turnDelay + 1f)
        {
            targetDirection = (targetLane - transform.position).normalized;
        }
        else
        {
            targetDirection = (startingLane - transform.position).normalized;
        }

        if (transform.position.x < targetLane.x + 0.05f && transform.position.x > targetLane.x - 0.05f)
        {
            if (isChangingLanes)
            {
                isChangingLanes = false;
                startingLane = targetLane;
                turnTimer = 0f;
            }
            else
            {
                // if we are close enough to the target lane, align with it and stop turning
                transform.rotation = Quaternion.FromToRotation(transform.up, Vector3.up) * transform.rotation;
                targetDirection = transform.up;
            }

            currentTurnSpeed = turnSpeed;
        }

        float angle = Vector2.SignedAngle(transform.up, targetDirection.x * Vector2.right);
        transform.Rotate(0, 0, angle * currentTurnSpeed * Time.deltaTime);
    }

    void DetectCar()
    {
        cars.Clear();
        
        var carList = FindObjectsByType<CarAI>(FindObjectsSortMode.None);

        if (Vector2.Distance(transform.position, player.gameObject.transform.position) <= detectionDistance + 0.5f)
            cars.Add(player.gameObject);

        foreach (CarAI car in carList)
            cars.Add(car.gameObject);

        List<GameObject> farCars = new();

        foreach (var car in cars)
        {
            if (Vector2.Distance(transform.position, car.transform.position) > detectionDistance + 0.5f)
                farCars.Add(car);
        }

        cars.RemoveAll(car => farCars.Contains(car));
    }

    void ChooseLane()
    {
        // Probability per second to change lanes
        turnTimer += Time.deltaTime;
        
        if (turnTimer < 1f && !isChangingLanes)
            return;

        float changeLaneProbability = laneChangeProbability + (hostility * cars.Count);
        isChangingLanes = changeLaneProbability <= Random.Range(0f, 1f);

        if (isChangingLanes)
        {
            while (targetLane == startingLane)
            {
                var nextLaneIndex = Random.Range(0, ClosestRoad().lanePositions.Count);
                targetLane = ClosestRoad().lanePositions[nextLaneIndex].position;
            }
        }
    }

    Road ClosestRoad()
    {
        var roads = FindObjectsByType<Road>(FindObjectsSortMode.None);

        Road closestRoad = null;
        float closestDistance = Mathf.Infinity;

        foreach (var road in roads)
        {
            float distance = Vector2.Distance(transform.position, road.gameObject.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestRoad = road;
            }
        }

        return closestRoad;
    }

    void AvoidCar()
    {
        foreach (var car in cars)
        {
            Vector2 directionToCar = (car.transform.position - transform.position).normalized;

            // if the car is in front of us, slow down, otherwise speed up to target speed
            if (Vector2.Dot(transform.up, directionToCar) > 0.5f)
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime / reactionTime);
            else
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / reactionTime);

            if (isChangingLanes)
            {
                // If the car is moving to an occupied lane, return to the starting lane
                if ((transform.position.x < targetLane.x && Vector2.Dot(transform.right, directionToCar) > 0.5f) ||
                (transform.position.x > targetLane.x && Vector2.Dot(transform.right, directionToCar) < -0.5f))
                {
                    targetLane = startingLane;
                    currentTurnSpeed = turnSpeed * 2;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw top left line
        Handles.DrawWireDisc(transform.position, Vector3.forward, detectionDistance);

        if (cars.Count > 0)
        {
            Gizmos.color = Color.blue;
            
            foreach (var car in cars)
            {
                if (car != null)
                    Gizmos.DrawLine(transform.position, car.transform.position);
            }
        }
    }
}