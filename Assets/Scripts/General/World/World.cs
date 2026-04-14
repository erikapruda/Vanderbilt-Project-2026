using UnityEngine;

public static class World
{
    public static Transform CenteredTransform { get; set; }

    public static Vector3 Origin { get; set; }

    public static Vector3 CurrentOffset { get; set; }

    public static void MoveObject(Rigidbody2D rb)
    {
        rb.MovePosition(rb.position + (rb.linearVelocity * Time.fixedDeltaTime) - new Vector2(0f, CurrentOffset.y - Origin.y));
    }
}