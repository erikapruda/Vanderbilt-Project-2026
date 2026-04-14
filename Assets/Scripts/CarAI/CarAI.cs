using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

    [Header("A weight for factoring in random speed variability")]
    [Range(0.2f, 0.9f)]
    public float speedLimitLeniency = 0.4f;

    public float detectionDistance = 2.0f;

    public float semiDetectionDistance = 4.0f;

    public Vector3 detectionOffset = new(0f, 0f, 0f);

    [HideInInspector]
    public Rigidbody2D rb;

    private float targetSpeed = 0;

    private float currentSpeed;

    private float turnTimer = 0f;

    private bool isChangingLanes = false;

    [HideInInspector]
    public bool lostControl = false;

    private float currentTurnSpeed;

    private Player player;
    readonly private List<GameObject> cars = new();

    [HideInInspector]
    public Vector2 targetLane;

    private Vector2 startingLane;

    void Awake()
    {
        player = FindObjectsByType<Player>(FindObjectsSortMode.None)[0];
    }

    void OnEnable()
    {
        targetSpeed = player.autoLinearVelocitySpeed - (player.autoLinearVelocitySpeed * Random.Range(0.2f, speedLimitLeniency));
        currentSpeed = targetSpeed;
        currentTurnSpeed = turnSpeed;
        startingLane = targetLane;
        rb.linearVelocityX = 0f;
        rb.linearVelocityY = currentSpeed;
        rb.angularVelocity = 0f;
        rb.rotation = 0f;

        isChangingLanes = false;
        lostControl = false;
        turnTimer = 0f;
    }

    void Update()
    {
        // If the car is no longer straight and at a steep angle, start crashing state
        if (!lostControl && Mathf.Abs(Vector2.Angle(transform.up, Vector3.up)) > 60f)
        {
            lostControl = true;
        }

        DetectCar();

        if (lostControl)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.deltaTime * 5f);
            return;
        }
        else
        {
            ChooseLane();
            AvoidCar();
        }
    }

    void FixedUpdate()
    {
        if (!lostControl)
        {
            RotateCar();
        }
        rb.linearVelocityX = Mathf.MoveTowards(rb.linearVelocityX, rb.transform.up.x * rb.linearVelocity.magnitude, Time.fixedDeltaTime * 2f);
        rb.linearVelocityY = Mathf.MoveTowards(rb.linearVelocityY, currentSpeed, Time.fixedDeltaTime * 2f);
    }

    void RotateCar()
    {
        float targetDirectionX;

        if (isChangingLanes)
        {
            targetDirectionX = targetLane.x - rb.position.x;
            // Reduce turn speed when changing lanes
            currentTurnSpeed = turnSpeed / 2;
        }
        else
        {
            targetDirectionX = startingLane.x - rb.position.x;
            currentTurnSpeed = turnSpeed;
        }

        if (Mathf.Abs(targetDirectionX) < 0.75f)
        {
            if (isChangingLanes)
            {
                isChangingLanes = false;
                startingLane = targetLane;
            }

            targetDirectionX = 0f;
        }

        float angle = targetDirectionX * -turnSpeed;

        Debug.DrawRay(rb.transform.position, rb.transform.right * targetDirectionX, Color.brown, Time.fixedDeltaTime);
        
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, angle, Time.fixedDeltaTime * 50f));
    }

    void DetectCar()
    {
        cars.Clear();

        var carList = FindObjectsByType<CarAI>(FindObjectsSortMode.None);

        if (Vector2.Distance(transform.position + detectionOffset, player.gameObject.transform.position) <= detectionDistance)
            cars.Add(player.gameObject);

        foreach (CarAI car in carList)
            cars.Add(car.gameObject);

        List<GameObject> farCars = new();

        foreach (var car in cars)
        {
            if (Vector2.Distance(transform.position + detectionOffset, car.transform.position) > detectionDistance)
                farCars.Add(car);

            if (Vector2.Distance(transform.position + detectionOffset, car.transform.position) <= semiDetectionDistance)
                farCars.Remove(car);
        }

        cars.RemoveAll(car => farCars.Contains(car));
    }

    void ChooseLane()
    {
        if (isChangingLanes)
            return;

        // Probability per second to change lanes
        turnTimer += Time.deltaTime;

        if (turnTimer < 1f)
            return;

        turnTimer = 0;

        float changeLaneProbability = laneChangeProbability + (hostility * cars.Count);
        var random = Random.Range(0f, 1f);
        isChangingLanes = random <= changeLaneProbability;

        if (!isChangingLanes)
            return;

        List<Transform> lanePositions = ClosestRoad().lanePositions.ToList();

        List<Transform> rightLanes = ClosestRoad().lanePositions.Where(lane => lane.position.x > transform.position.x).ToList();
        List<Transform> leftLanes = ClosestRoad().lanePositions.Where(lane => lane.position.x < transform.position.x).ToList();
        
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
        lanePositions.RemoveAll(lane => new Vector2(lane.position.x, lane.position.y) == startingLane);

        var nextLaneIndex = Random.Range(0, lanePositions.Count);
        targetLane = lanePositions[nextLaneIndex].position;
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
            if (Vector2.Dot(transform.up, directionToCar) > 0.9f)
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CarAI car))
        {
            // Only lose control if the collision is strong enough
            if (collision.relativeVelocity.magnitude < 2f)
                return;
            
            lostControl = true;
            car.lostControl = true;
            car.rb.AddTorque(rb.angularVelocity * 0.5f);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw top left line
        Handles.color = Color.blue;
        Handles.DrawWireDisc(transform.position + detectionOffset, Vector3.forward, detectionDistance);
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position + detectionOffset, Vector3.forward, semiDetectionDistance);

        if (cars.Count > 0)
        {
            Gizmos.color = Color.blue;

            foreach (var car in cars)
            {
                if (car != null)
                {
                    Vector2 directionToCar = (car.transform.position - transform.position).normalized;



                    if (Vector2.Dot(transform.up, directionToCar) > 0.9f)
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
#endif
}