using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventAfterTime : MonoBehaviour
{
    public float fireRate;

    public UnityEvent OnAfterTime;

    private WaitForSeconds wait;

    void Awake()
    {
        wait = new(fireRate);
        StartCoroutine(FireAfterTime());
    }

    IEnumerator FireAfterTime()
    {
        while (true)
        {
            yield return wait;

            OnAfterTime?.Invoke();
        }
    }
    
    public void Destroy()
    {
        Object.Destroy(gameObject);
    }
}