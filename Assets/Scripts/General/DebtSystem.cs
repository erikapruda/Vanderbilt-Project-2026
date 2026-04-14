using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DebtSystem : MonoBehaviour
{
    [SerializeField]
    private uint debtLabelMult = 1;

    [SerializeField]
    private byte debtLabelDecimalPlaces = 2;

    [SerializeField]
    private GameObject debtTextPrefab;

    private static DebtSystem Singleton;

    private TextMeshProUGUI debtText;

    public static System.Numerics.BigInteger Debt { get; private set; }

    void Awake()
    {
        Debt = 0;
        Singleton = this;
        debtText = GetComponent<TextMeshProUGUI>();
    }

    public static void AddDebt(uint value, Vector2 debtTextPosition = default, Vector2 debtTextVelocity = default)
    {
        if (!GameManager.IsUsingDebt) return;

        Debt += value;
        Singleton.debtText.text = $"Debt ${GetDebtText(Debt)}"; ;

        // Create debt text at collision point
        if (Singleton.debtTextPrefab != null)
        {
            GameObject instance = Instantiate(Singleton.debtTextPrefab, debtTextPosition, Quaternion.identity);

            if (instance.TryGetComponent(out TextMeshPro text))
            {
                text.text = $"-${GetDebtText(value)}";
            }

            if (instance.TryGetComponent(out Rigidbody2D textRb))
            {
                textRb.linearVelocity = debtTextVelocity;
                textRb.angularVelocity = Vector2.SignedAngle(Vector2.up, debtTextVelocity);
            }
        }

        // Play debt add animation
        if (Singleton.debtText.gameObject.TryGetComponent(out Animation animation))
        {
            animation.Stop();
            animation.Play();
        }
    }

    static string GetDebtText(System.Numerics.BigInteger value)
    {
        uint THRESHOLD_K = 1000 * Singleton.debtLabelMult;
        uint THRESHOLD_M = 1000000 * Singleton.debtLabelMult;
        ulong THRESHOLD_B = 1000000000ul * Singleton.debtLabelMult;

        uint chosenLabelUnitPlace;
        if (value < THRESHOLD_K)
        {
            chosenLabelUnitPlace = 1;
        }
        else if (value < THRESHOLD_M)
        {
            chosenLabelUnitPlace = 1000;
        }
        else if (value < THRESHOLD_B)
        {
            chosenLabelUnitPlace = 1000000;
        }
        else
        {
            chosenLabelUnitPlace = 1000000000;
        }

        System.Numerics.BigInteger scaledDebt = value / chosenLabelUnitPlace;
        System.Numerics.BigInteger leftOverDebt = value % chosenLabelUnitPlace;
        byte numZerosBefore = (byte)(Mathf.Floor(Mathf.Log10(chosenLabelUnitPlace)) - Mathf.Floor(Mathf.Log10((float)leftOverDebt)) - 1f);

        char debtLabel = chosenLabelUnitPlace switch
        {
            1 => ' ',
            1000 => 'K',
            1000000 => 'M',
            1000000000 => 'B',
            _ => ' '
        };

        // Construct decimal place
        string leftOverDebtString = "";
        for (byte i = 0; i < numZerosBefore; i++)
        {
            leftOverDebtString += "0";
        }
        leftOverDebtString += leftOverDebt.ToString();
        leftOverDebtString = leftOverDebtString[0..Mathf.Min(leftOverDebtString.Length, Singleton.debtLabelDecimalPlaces)];
        return leftOverDebtString.Length == 0 || leftOverDebt == 0 || numZerosBefore >= Singleton.debtLabelDecimalPlaces ? $"{scaledDebt}{debtLabel}" : $"{scaledDebt}.{leftOverDebtString}{debtLabel}";
    }
}