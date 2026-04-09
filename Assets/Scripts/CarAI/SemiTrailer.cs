using UnityEngine;

public class SemiTrailer : MonoBehaviour
{
    public CarAI parentCarAI;
    
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (parentCarAI.lostControl)
            return;

        // Make the semi-trailer lerp towards the same rotation as the car, but with a delay to simulate the trailer's movement
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, parentCarAI.rb.rotation, 0.1f));
    }
}