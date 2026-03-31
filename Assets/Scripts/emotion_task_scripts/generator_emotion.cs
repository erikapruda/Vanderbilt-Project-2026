using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class generator_emotion : MonoBehaviour
{

    public Image emotion_background;    //emotion canvas image
    private const float TIME_INTERVAL = 3f;
    public TextMeshProUGUI textbox;
    public TextMeshProUGUI good_textbox;
    public TextMeshProUGUI bad_textbox;
    public emotionVerification emotionVerifier;
    private int wordIndex = 0;
    private int newWordIndex = 0;
    public static float WORD_START_TIME = 0f;

    private string[] words = { 
   
    "Horrendous", "Happy", "Joy", "Malicious", "Dismay", "Punishment", "Excitement", "Disaster", "Hope",
    
    "Paradise", "Exultant", "Victorious", "Wonderful", "Blessed", "Glorious", "Delight", "Honest", "Nurture", "Loyal", "Luminous", "Vibrant",
    
    "Nightmare", "Abhorrent", "Catastrophe", "Despair", "Damaged", "Hideous", "Failure", "Deceit", "Betrayal", "Cruel", "Obscure", "Rotting"
};

    private Dictionary<string, int> wordsDict = new Dictionary<string, int> { // Added dictionary for words association, 0 for bad, 1 for good.
        {"Horrendous", 0}, {"Happy", 1}, {"Joy", 1}, {"Malicious", 0}, {"Dismay", 0}, {"Punishment", 0}, {"Excitement", 1}, {"Disaster", 0}, {"Hope", 1},
    
    {"Paradise", 1}, {"Exultant", 1}, {"Victorious", 1}, {"Wonderful", 1}, {"Blessed", 1}, {"Glorious", 1}, {"Delight", 1}, {"Honest", 1}, {"Nurture", 1}, {"Loyal", 1}, {"Luminous", 1}, {"Vibrant", 1},
    
    {"Nightmare", 0}, {"Abhorrent", 0}, {"Catastrophe", 0}, {"Despair", 0}, {"Damaged", 0}, {"Hideous", 0}, {"Failure", 0}, {"Deceit", 0}, {"Betrayal", 0}, {"Cruel", 0}, {"Obscure", 0}, {"Rotting", 0}
    };

    public struct results
    {
        public float reaction_time;
        public bool correctness;
    };
    List<results> results_array = new List<results>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        emotion_background.color = Color.white;
        StartCoroutine(Change_Text());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static string wordType = null;

    IEnumerator Change_Text()
    {

        // good_textbox.text = "";
        // bad_textbox.text = "";


        // textbox.text = "Welcome to Stroop Task";
        // //wait 5 seconds
        // yield return new WaitForSeconds(2.0f);
            

        // textbox.text = "Please respond with either good/bad";
        // yield return new WaitForSeconds(2.0f);

        good_textbox.text = "Good";
        bad_textbox.text = "Bad";    

        //algorithm:
                //loop
                    //get new word
                    
                    //if we have a word, compare it to audio.
                        //if same word, flash green
                        //if diff word, flash red
                    

        while (true)
        {   
            emotion_background.color = Color.white;

            newWordIndex = Random.Range(0, words.Length);


            while (newWordIndex == wordIndex)
            {
                newWordIndex = Random.Range(0, words.Length);
            }

            wordIndex = newWordIndex;

            if (wordsDict[words[newWordIndex]] == 0){
                wordType = "bad";
            }
            else{
                wordType = "good";
            }
            

            textbox.text = words[wordIndex];
            WORD_START_TIME = Time.realtimeSinceStartup;      //storing time of when word appears.

            yield return new WaitForSeconds(2.5f);
            emotion_background.color = Color.white;
            
            
            if (wordType != null){ // Calls verification command when a target word association exists before changing to next stroop test.
                results temp_results = new results();
                bool emotion_correctness = emotionVerifier.CompareWords();

                if(emotion_correctness == true)
                {
                    //change background
                    emotion_background.color = new Color(0.01f, 1f, 0.01f, 0.68f);

                }

                else if(emotion_correctness == false)
                {
                    //change background
                    emotion_background.color = new Color(1f, 0.01f, 0.01f, 0.68f);
                }

                temp_results.reaction_time = emotionVerifier.reactionTime;
                temp_results.correctness = emotion_correctness;

                results_array.Add(temp_results);

                yield return new WaitForSeconds(1.0f);
            }

            // emotion_background.color = Color.white;

            // newWordIndex = Random.Range(0, words.Length);


            // while (newWordIndex == wordIndex)
            // {
            //     newWordIndex = Random.Range(0, words.Length);
            // }

            // wordIndex = newWordIndex;

            // if (wordsDict[words[newWordIndex]] == 0){
            //     wordType = "bad";
            // }
            // else{
            //     wordType = "good";
            // }
            

            // textbox.text = words[wordIndex];
            // yield return new WaitForSeconds(2.5f);
            
        }
    }
}
