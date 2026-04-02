using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class nBackVerification : MonoBehaviour
{

    


    //public generator stroopTest;
    KeywordRecognizer speechRecognizer;
    private string[] validWords = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"};
    private string lastSpokenWord = null;
    private char lastSpokenChar;
    public float reactionTime = 0f;
    private System.DateTime refDateTime;
    private float refUnityTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Created time references to use Unity's time and convert from the sytem time provided by the speech recognition event args.
        refDateTime = System.DateTime.Now; // Reference point for calculating reaction time.
        refUnityTime = Time.realtimeSinceStartup; // Reference point for calculating reaction time.
        
        speechRecognizer = new KeywordRecognizer(validWords, ConfidenceLevel.Low); // Recognition class that only listens for words specified in validColors list.
        speechRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        speechRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        lastSpokenWord = args.text.ToUpper(); // Each time a letter is spoken, lastSpokenWord stores that value in uppercase.
        lastSpokenChar = lastSpokenWord[0]; // Convert the recognized word to a char for comparison.
        reactionTime = (refUnityTime + (float)(args.phraseStartTime - refDateTime).TotalSeconds) - n_back_generator.N_BACK_START_TIME; // Convert to float to use with Unity's time system.
        Debug.Log("Reaction Time: " + reactionTime);
    }

    public bool CompareWords(){
        if (lastSpokenWord == null){
            Debug.Log("No word detected");
            
            return false; 
        }

        if (lastSpokenChar == n_back_generator.correct_letter){ // Returns true/false based on comparsion to target. Also consumes last word to prevent two
            lastSpokenWord = null;  
            lastSpokenChar = '\0'; // Clear the character
            Debug.Log("Correct");
            Debug.Log("Reaction Time: " + reactionTime);
            return true;
        }
        else{
            lastSpokenWord = null;
            lastSpokenChar = '\0'; // Clear the character
            Debug.Log("Incorrect");
            Debug.Log("Reaction Time: " + reactionTime);
            return false;
        }
    }
}
