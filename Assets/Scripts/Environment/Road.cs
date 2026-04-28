using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public List<Transform> lanePositions;

    public Vector2Int numCarsEasy = new(0, 3);
    public Vector2Int numCarsMedium = new(0, 6);
    public Vector2Int numCarsHard = new(1, 8);

    [HideInInspector]
    public Vector2Int numCars;

    private void Awake()
    {
        switch (PlayerPrefs.GetInt("Difficulty", 2))
        {
            case 1: // Easy
                numCars = numCarsEasy;
                break;
            case 2: // Medium
                numCars = numCarsMedium;
                break;
            case 3: // Hard
                numCars = numCarsHard;
                break;
            default:
                break;
        }
    }
}