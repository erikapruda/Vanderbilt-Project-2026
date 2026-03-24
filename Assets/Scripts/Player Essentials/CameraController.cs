using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float shakeSpeed = 50f;

    [SerializeField] private float maxShakeMagnitude = 5f;

    private Vector3 startPosition;

    private Vector3 targetShakePosition;

    private float currentShakeMagnitude;

    private float currentShakeDivisor = 1f;

    void Awake()
    {
        startPosition = transform.position;
        targetShakePosition = transform.position;
    }

    void Update()
    {
        float distFromTarget = Vector2.Distance(transform.position, targetShakePosition);

        if (currentShakeMagnitude > 0.01f)
        {
            // Set new target position if the target is reached
            if (distFromTarget < 0.05f)
            {
                targetShakePosition = startPosition + Random.insideUnitSphere * currentShakeMagnitude;
                currentShakeMagnitude /= currentShakeDivisor;
            }
        }
        else
        {
            targetShakePosition = startPosition;
            currentShakeMagnitude = 0f;
            currentShakeDivisor = 1f;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetShakePosition, Mathf.Max(distFromTarget, 1f) * shakeSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, startPosition.z);
    }

    public void SetShakeMagDivisor(float div)
    {
        currentShakeDivisor = Mathf.Max(1f, div);
    }

    public void SetShakeMagnitude(float mag)
    {
        currentShakeMagnitude = Mathf.Min(currentShakeMagnitude + mag, maxShakeMagnitude);
    }
}