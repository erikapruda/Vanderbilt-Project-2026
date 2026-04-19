using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject DebtGUI;

    public static bool IsUsingDebt { get; set; }

    internal static uint _seed = 0;

    public uint Seed
    {
        get => _seed;
        set
        {
            // Seed of 0 is invalid, so keep trying to generate valid seed
            while (value == 0)
            {
                value = (uint)Random.Range(int.MinValue, int.MaxValue);
            }
            _seed = value;

            // Seed the random number generator
            SeededRandom = new Unity.Mathematics.Random(_seed);
            SeededRandom.InitState(_seed);
        }
    }

    public bool IsUsingSeed { get; set; } = true;

    public Unity.Mathematics.Random SeededRandom;

    public static GameManager Singleton { get; private set; }

    void Awake()
    {
        Singleton = this;

        Seed = (uint)PlayerPrefs.GetInt("RequestedSeed", 1);

        Debug.Log($"Game Seed: {Seed}");
        
        if (DebtGUI != null)
        {
            DebtGUI.SetActive(IsUsingDebt);
        }
    }
}