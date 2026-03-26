using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    public float detectionDistance = 2.0f;

    private List<float> directionsToCar = new();

    private Animator animator;

    private Rigidbody2D rb;

    private float targetSpeed = 0;

    private Player player;
    private List<GameObject> cars = new();

    [HideInInspector]
    public Transform targetLane;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = FindObjectsByType<Player>(FindObjectsSortMode.None)[0];
        animator.Play("Left Turn Signal");

        targetSpeed = 15 + (15 * Random.Range(-speedLimitLeniency, speedLimitLeniency));
    }

    // Update is called once per frame
    void Update()
    {
        DetectCar();
        ChooseLane();
        AvoidCar();

        Vector2 velNorm = transform.up;
        float velMag = rb.linearVelocity.magnitude;
        velMag = Mathf.MoveTowards(velMag, targetSpeed, 15);
        rb.linearVelocity = velNorm * velMag;
    }

    void DetectCar()
    {
        cars.Clear();
        
        var carList = FindObjectsByType<CarAI>(FindObjectsSortMode.None);

        cars.Add(player.gameObject);

        foreach (CarAI car in carList)
            cars.Add(car.gameObject);

        directionsToCar.Clear();

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
        bool changeLane = laneChangeProbability <= Random.Range(0, 1);

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
    }

    void AvoidCar()
    {
        
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