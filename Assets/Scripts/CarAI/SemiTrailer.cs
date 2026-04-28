using UnityEngine;

public class SemiTrailer : MonoBehaviour
{
    public CarAI parentCarAI;
    
    private Transform anchor;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anchor = transform;
    }
    
    void Update()
    {
        // Update semitruck sound when driving by player
        SemitruckSoundPlayer.SetVolume(parentCarAI.transform.position);

        if (parentCarAI.lostControl)
            return;

        // Make the trailer's angular velocity face upwards the car's rotation, but with a delay to simulate the trailer's movement
        rb.SetRotation(Mathf.LerpAngle(rb.rotation, parentCarAI.rb.rotation, Time.deltaTime * 2f));
        // transform.position = anchor.position;
    }
}