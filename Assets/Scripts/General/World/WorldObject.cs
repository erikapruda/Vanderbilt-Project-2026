using System.Collections;
using UnityEngine;

public class WorldObject : MonoBehaviour
{
    [Tooltip("The distance from the world origin at which this object despawns")]
    public float despawnDistance = 20f;

    [SerializeField]
    private bool isBoundedByWorldBounds = true;

    [SerializeField]
    [Tooltip("Whether or not to move this object when the world is centered on another object")]
    private bool isMovedByWorld = true;

    [HideInInspector]
    public bool shouldDestroyOnDespawn;

    private WaitForSeconds despawnCheckFrequency = new(0.5f);

    private Rigidbody2D rb;

    private Coroutine despawnRoutine;

    void Awake()
    {
        TryGetComponent(out rb);
    }

    void OnEnable()
    {
        despawnRoutine = StartCoroutine(CheckDespawn());
    }

    void OnDisable()
    {
        StopCoroutine(despawnRoutine);
    }

    void Update()
    {
        if (isBoundedByWorldBounds)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, WorldBounds.Singleton.LeftX, WorldBounds.Singleton.RightX), transform.position.y, transform.position.z);
        }
    }

    void FixedUpdate()
    {
        if (isMovedByWorld && rb != null)
        {
            World.MoveObject(rb);
        }
    }

    IEnumerator CheckDespawn()
    {
        while (true)
        {
            yield return despawnCheckFrequency;

            if (gameObject.activeSelf && transform.position.magnitude >= despawnDistance)
            {
                gameObject.SetActive(false);
            }
        }
    }
}