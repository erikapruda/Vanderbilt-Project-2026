using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    [SerializeField]
    [Tooltip("Prefab to Instantiate")]
    GameObject Prefab;

    [SerializeField]
    [Tooltip("Max Active")]
    byte Max;

    GameObject[] objects;

    int nextIndex;

    public void Setup()
    {
        if (Prefab == null || Max == 0)
        {
            return;
        }

        objects = new GameObject[Max];
        for (int i = 0; i < Max; i++)
        {
            objects[i] = Object.Instantiate(Prefab, new Vector3(-200, -200, 0f), Quaternion.identity);
            objects[i].SetActive(false);
        }
    }

    public GameObject CreateObject(Vector3 position, Quaternion rotation)
    {
        if (Prefab == null || Max == 0)
        {
            return null;
        }

        GameObject obj = objects[nextIndex];
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }
        obj.SetActive(true);
        if (++nextIndex >= Max)
        {
            nextIndex = 0;
        }
        return obj;
    }
}