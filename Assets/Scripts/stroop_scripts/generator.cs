using UnityEngine;
using TMPro;

public class generator : MonoBehaviour
{

    public const float TIME_INTERVAL = 5.0f;
    public TextMeshProUGUI stroopText;
    public stroopVerification stroopVerifier;

    private string[] color_words = {"RED", "BLUE", "GREEN", "YELLOW", "PURPLE", "ORANGE"};

    private Color[] colors = {Color.red, 
    Color.blue, 
    Color.green,
    Color.yellow, 
    new Color(0.5f, 0f, 0.5f), //purple
    new Color(1f, 0.5f, 0f)    //Orange
    };

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("Change_Stroop", 0f, TIME_INTERVAL);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string currentTargetColor; // Added to determine correct color to expect in speech recognition.

    void Change_Stroop()
    {
        if (currentTargetColor != null){ // Calls verification command when a target color exists before changing to next stroop test.
            stroopVerifier.CompareWords();
        }

        int wordIndex = Random.Range(0, color_words.Length);
        int colorIndex = Random.Range(0, colors.Length);

        while(wordIndex == colorIndex)
        {
            wordIndex = Random.Range(0, color_words.Length);
            colorIndex = Random.Range(0, colors.Length);        
        }
        stroopText.text = color_words[wordIndex];
        stroopText.color = colors[colorIndex];
        currentTargetColor = color_words[colorIndex]; // Set public color variable for use in speech recognition.
        return;
    }
}
