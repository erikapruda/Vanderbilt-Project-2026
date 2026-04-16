using UnityEngine;

public class Money : MonoBehaviour
{
    [Tooltip("How much to remove from debt when collected")]
    public uint worth;

    private Rigidbody2D rb;

    void Awake()
    {
        TryGetComponent(out rb);
    }

    void OnEnable()
    {
        if (!GameManager.IsUsingDebt)
        {
            gameObject.SetActive(false);
        }
        
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-10f, 10f));
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.TryGetComponent(out Player _))
        {
            Vector2 textVelocity = Vector2.up;
            if (rb != null)
            {
                textVelocity = -rb.linearVelocity.normalized;
            }
            
            DebtSystem.RemoveDebt(worth, transform.position, textVelocity);
            gameObject.SetActive(false);
        }
    }
}
