using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The default seed when the game starts\n\nNote 1: Seed can be changed later\n\nNote 2: Seed of 0 is random seed")]
    private uint defaultStartingSeed = 0;

    [SerializeField]
    [Tooltip("Default decision for whether the game should use a seed or be completely random\n\nNote 1: Value can be changed later\n\nNote 2: Using a seed includes the scenario of a randomly generated seed")]
    private bool defaultIsUsingSeed = true;

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

        IsUsingSeed = PlayerPrefs.GetInt("UseSeed", defaultIsUsingSeed ? 1 : 0) == 1;
        if (IsUsingSeed)
        {
            uint requestedSeed = (uint)PlayerPrefs.GetInt("RequestedSeed", (int)defaultStartingSeed);
            Seed = requestedSeed;
        }
        else
        {
            Seed = 0;
        }

        Debug.Log($"Game Seed: {Seed}");
        
        if (DebtGUI != null)
        {
            DebtGUI.SetActive(IsUsingDebt);
        }
    }
}