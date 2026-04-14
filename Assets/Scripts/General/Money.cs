using UnityEngine;

public class Money : MonoBehaviour
{
    [Tooltip("How much to remove from debt when collected")]
    public uint worth;

    void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-10f, 10f));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
