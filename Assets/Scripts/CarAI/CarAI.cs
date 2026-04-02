using System.Collections.Generic;
using System.Linq;
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

    [Header("How aggressive a car swerves away from another car when changing lanes")]
    public float turnSpeed = 1.0f;

    [Range(0, 6)]
    [Header("Delay in seconds for the vehicle to legally change lanes\n The timer resets when the opening closes")]
    public float turnDelay = 0.8f;

    [Header("A weight for factoring in random speed variability")]
    public float speedLimitLeniency = 1.0f;

    [Header("The number of people in the car. The number adds to\nthe death toll score")]
    public int passengers = 2;

    public float detectionDistance = 2.0f;

    public float semiDetectionDistance = 4.0f;
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
        targetSpeed = player.autoLinearVelocitySpeed - (player.autoLinearVelocitySpeed * Random.Range(0f, speedLimitLeniency));
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

            // Reduce turn speed when not swerving
            currentTurnSpeed = turnSpeed / 2;
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

            if (Vector2.Distance(transform.position, car.transform.position) <= semiDetectionDistance + 0.5f)
                farCars.Remove(car);
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

        List<Transform> lanePositions = ClosestRoad().lanePositions.ToList();

        List<Transform> rightLanes = ClosestRoad().lanePositions.Where(lane => lane.position.x > transform.position.x).ToList();
        List<Transform> leftLanes = ClosestRoad().lanePositions.Where(lane => lane.position.x < transform.position.x).ToList();

        if (isChangingLanes)
        {
            foreach (var car in cars)
            {
                if (car != null)
                {
                    if (car.GetComponent<Rigidbody2D>().linearVelocity.y > rb.linearVelocity.y)
                    {
                        lanePositions.AddRange(rightLanes);
                    }
                    else
                    {
                        lanePositions.AddRange(leftLanes);
                    }
                }
            }

            // Remove the starting lane from the lane positions to choose from
            lanePositions.RemoveAll(lane => lane.position == startingLane);

            var nextLaneIndex = Random.Range(0, lanePositions.Count);
            targetLane = lanePositions[nextLaneIndex].position;
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
            if (Vector2.Dot(transform.up, directionToCar) > 0.8f)
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime / reactionTime);
            else
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime / reactionTime);

            if (isChangingLanes)
            {
                // If the car is moving to an occupied lane, return to the starting lane
                if ((transform.position.x < targetLane.x && Vector2.Dot(transform.right, directionToCar) > 0.8f) ||
                (transform.position.x > targetLane.x && Vector2.Dot(transform.right, directionToCar) < -0.8f))
                {
                    targetLane = startingLane;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw top left line
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position, Vector3.forward, detectionDistance);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, Vector3.forward, semiDetectionDistance);

        if (cars.Count > 0)
        {
            Gizmos.color = Color.blue;

            foreach (var car in cars)
            {
                if (car != null)
                {
                    Vector2 directionToCar = (car.transform.position - transform.position).normalized;



                    if (Vector2.Dot(transform.up, directionToCar) > 0.8f)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(transform.position, car.transform.position);
                    }
                    else if ((transform.position.x < targetLane.x && Vector2.Dot(transform.right, directionToCar) > 0.8f) ||
                            (transform.position.x > targetLane.x && Vector2.Dot(transform.right, directionToCar) < -0.8f))
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(transform.position, car.transform.position);
                    }
                    else
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(transform.position, car.transform.position);
                    }
                }
            }

        }
    }
}