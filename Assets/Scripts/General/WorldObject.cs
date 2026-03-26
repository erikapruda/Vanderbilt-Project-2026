using System.Collections;
//using Unity.VisualScripting;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    [Tooltip("The distance from the world origin at which this object despawns")]
    public float despawnDistance = 20f;

    [SerializeField]
    private bool isBoundedByWorldBounds = true;

    private WaitForSeconds despawnCheckFrequency = new(0.5f);

    private Camera mainCamera;
    void Awake()
    {
        mainCamera = Camera.main;
        StartCoroutine(CheckDespawn());
    }

    void Update()
    {
        if (isBoundedByWorldBounds)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, WorldBounds.Singleton.LeftX, WorldBounds.Singleton.RightX), transform.position.y, transform.position.z);            
        }
    }

    IEnumerator CheckDespawn()
    {
        while (true)
        {
            yield return despawnCheckFrequency;

            if (transform.position.magnitude >= despawnDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}