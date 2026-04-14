using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    [SerializeField]
    private float _leftX;

    [SerializeField]
    private float _rightX;

    public float LeftX { get { return _leftX; } }
    public float RightX { get { return _rightX; } }

    public static WorldBounds Singleton { get; private set; }
    
    void Awake()
    {
        Singleton = this;
        if (_leftX >= _rightX)
        {
            Debug.LogWarning($"Warning: {this} has incompatible values for LeftX and RightX!", this);
        }
    }
}