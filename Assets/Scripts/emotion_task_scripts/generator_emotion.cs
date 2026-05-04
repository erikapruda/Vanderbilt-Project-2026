using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using Color = UnityEngine.Color;

public class generator_emotion : MonoBehaviour
{
    private const float TIME_INTERVAL = 3f;
    public static bool SHOW_COLOR_RESPONSE;
    private int wordIndex = 0;
    private int newWordIndex = 0;
    public static float WORD_START_TIME;

    public static bool ROUND_BASED = true;      //bool for if the test is round based or not
    public static int ROUND_NUM = 5;           //num of rounds if rounds based.

    public Image emotion_background;    //emotion canvas image
    public TextMeshProUGUI textbox;
    public TextMeshProUGUI good_textbox;
    public TextMeshProUGUI bad_textbox;
    public emotionVerification emotionVerifier;
    public GameTimer gameTimer;
    
    private string[] words = { 
       "Horrendous", "Happy", "Joy", "Malicious", "Dismay", "Punishment", "Excitement", "Disaster", "Hope",
       "Paradise", "Exultant", "Victorious", "Wonderful", "Blessed", "Glorious", "Delight", "Honest", "Nurture", "Loyal", "Luminous", "Vibrant",
       "Nightmare", "Abhorrent", "Catastrophe", "Despair", "Damaged", "Hideous", "Failure", "Deceit", "Betrayal", "Cruel", "Obscure", "Rotting"
    };

    private Dictionary<string, int> wordsDict = new Dictionary<string, int> { 
        {"Horrendous", 0}, {"Happy", 1}, {"Joy", 1}, {"Malicious", 0}, {"Dismay", 0}, {"Punishment", 0}, {"Excitement", 1}, {"Disaster", 0}, {"Hope", 1},
        {"Paradise", 1}, {"Exultant", 1}, {"Victorious", 1}, {"Wonderful", 1}, {"Blessed", 1}, {"Glorious", 1}, {"Delight", 1}, {"Honest", 1}, {"Nurture", 1}, {"Loyal", 1}, {"Luminous", 1}, {"Vibrant", 1},
        {"Nightmare", 0}, {"Abhorrent", 0}, {"Catastrophe", 0}, {"Despair", 0}, {"Damaged", 0}, {"Hideous", 0}, {"Failure", 0}, {"Deceit", 0}, {"Betrayal", 0}, {"Cruel", 0}, {"Obscure", 0}, {"Rotting", 0}
    };

    public struct results
    {
        public float reaction_time;
        public bool correctness;
        public string word;
        public int emotion;  //0 for bad, 1 for good.
    };

    public static List<results> results_array = new List<results>();

    void Start()
    {
        ROUND_BASED = PlayerPrefs.GetInt("UsingPromptCount", 0) == 1;
        ROUND_NUM = PlayerPrefs.GetInt("PromptCount", 5);

        results_array.Clear();
        emotion_background.color = Color.white;
        StartCoroutine(Change_Text());
    }

    public static string wordType = null;

    IEnumerator Change_Text()
    {
        good_textbox.text = "Good";
        bad_textbox.text = "Bad";    

        if (ROUND_BASED == false)
        {
            while (true)
            {
                yield return StartCoroutine(emotion_test());
            }
        }
        else
        {
            for (int i = 0; i < ROUND_NUM; i++)
            {
                yield return StartCoroutine(emotion_test());
            }

            textbox.text = "";
            good_textbox.text = "";
            bad_textbox.text = "";
            
            if (gameTimer != null)
            {
                gameTimer.EndGame();
            }
        }
    }

    IEnumerator emotion_test()
    {
        emotion_background.color = Color.white;

        if (emotionVerifier != null)
        {
            emotionVerifier.ResetAnswer();
        }

        newWordIndex = Random.Range(0, words.Length);

        while (newWordIndex == wordIndex)
        {
            newWordIndex = Random.Range(0, words.Length);
        }

        wordIndex = newWordIndex;

        if (wordsDict[words[newWordIndex]] == 0)
        {
            wordType = "bad";
        }
        else
        {
            wordType = "good";
        }

        textbox.text = words[wordIndex];
        WORD_START_TIME = Time.realtimeSinceStartup;

        // Prompt-based mode waits for a voice answer.
        // Minute-based mode keeps the original timed behavior.
        if (ROUND_BASED)
        {
            yield return new WaitUntil(() => emotionVerifier != null && emotionVerifier.HasAnswered);
        }
        else
        {
            yield return new WaitForSeconds(2.5f);
        }

        emotion_background.color = Color.white;

        if (wordType != null)
        {
            results temp_results = new results();
            bool emotion_correctness = emotionVerifier.CompareWords();

            if (SHOW_COLOR_RESPONSE == true)
            {
                if (emotion_correctness == true)
                {
                    emotion_background.color = new Color(0.01f, 1f, 0.01f, 1f);
                }
                else
                {
                    emotion_background.color = new Color(1f, 0.01f, 0.01f, 1f);
                }

                yield return new WaitForSeconds(1.0f);
            }

            temp_results.reaction_time = emotionVerifier.reactionTime;
            temp_results.correctness = emotion_correctness;
            temp_results.word = words[wordIndex];
            temp_results.emotion = wordsDict[words[newWordIndex]];

            results_array.Add(temp_results);
        }

        emotion_background.color = Color.white;
    }
}