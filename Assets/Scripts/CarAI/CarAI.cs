using UnityEngine;

enum States
{
    Passive,
    TurnRight,
    TurnLeft
}

[RequireComponent(typeof(Animator))]
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

    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Left Turn Signal");
    }

    // Update is called once per frame
    void Update()
    {
        bool changeLane = laneChangeProbability <= Random.Range(0, 1);
    }
}
