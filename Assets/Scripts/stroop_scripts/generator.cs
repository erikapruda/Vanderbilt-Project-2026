using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
public class generator : MonoBehaviour
{

    public const float TIME_INTERVAL = 3f;    //time interval
    public float STROOP_START_TIME = 0f;      //variable for when stroop test displays a word
    public TextMeshProUGUI stroopText;          //canvas text 
    public stroopVerification stroopVerifier;   //speech-to-text script
    public Image backgroundImage;               //canvas image

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
        backgroundImage.color = Color.white;

        StartCoroutine(Change_Stroop());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string currentTargetColor; // Added to determine correct color to expect in speech recognition.

    
    //algorithm:
        //loop
            //get a random color word
            //get a random color
            //make sure the color word and color do not match
            //make sure the color word was not the same as before

            //if we have word, compare it to audio
                //if audio same as color, flash green
                //if audio not as same as color, or absent, flash red
    
    IEnumerator Change_Stroop(){

        while (true)
        {

            backgroundImage.color = Color.white;

            int wordIndex = Random.Range(0, color_words.Length);
            int colorIndex = Random.Range(0, colors.Length);

            while (wordIndex == colorIndex || (currentTargetColor == color_words[colorIndex])) //loop to make sure text does not match color, and color always switches
            {
                wordIndex = Random.Range(0, color_words.Length);
                colorIndex = Random.Range(0, colors.Length);
            }


            stroopText.text = color_words[wordIndex];
            stroopText.color = colors[colorIndex];

            STROOP_START_TIME = Time.realtimeSinceStartup;      //storing time of when word appears.

            Debug.Log("" + STROOP_START_TIME);

            currentTargetColor = color_words[colorIndex]; // Set public color variable for use in speech recognition.

            yield return new WaitForSeconds(TIME_INTERVAL);

            if (currentTargetColor != null)
            { // Calls verification command when a target color exists before changing to next stroop test.
                bool correctness = stroopVerifier.CompareWords();

                if (correctness == true)
                {
                    backgroundImage.color = new Color(0.01f, 1f, 0.01f, 0.68f);
                }

                else
                {
                    backgroundImage.color = new Color(1f, 0.01f, 0.01f, 0.68f);
                }

                yield return new WaitForSeconds(1.0f);

            }
        }
    }
}
