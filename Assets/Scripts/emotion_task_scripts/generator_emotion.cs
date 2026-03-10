using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;
public class generator_emotion : MonoBehaviour
{
    private const float TIME_INTERVAL = 3f;
    public TextMeshProUGUI textbox;
    public TextMeshProUGUI good_textbox;
    public TextMeshProUGUI bad_textbox;
    public emotionVerification emotionVerifier;
    private int wordIndex = 0;
    private int newWordIndex = 0;

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    async void Start()
    {
        good_textbox.text = "";
        bad_textbox.text = "";


        textbox.text = "Welcome to Stroop Task!";
        //wait 5 seconds
        await Task.Delay(2500);

        textbox.text = "Say if a word is good/bad..";
        await Task.Delay(2000);


        InvokeRepeating("Change_Text", 0f, TIME_INTERVAL);
        good_textbox.text = "Good";
        bad_textbox.text = "Bad";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static string wordType = null;

    void Change_Text()
    {
        if (wordType != null){ // Calls verification command when a target word association exists before changing to next stroop test.
            emotionVerifier.CompareWords();
        }

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
        
        return;
    }
}
