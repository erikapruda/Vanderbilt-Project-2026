using UnityEngine;
using UnityEngine.Windows.Speech;

public class arithmeticVerification : MonoBehaviour
{
    KeywordRecognizer speechRecognizer;
    private string[] validWords = {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen"};
    private string lastSpokenWord = null;
    public float reactionTime = 0f;
    private System.DateTime refDateTime;
    private float refUnityTime;

    void Start()
    {
        refDateTime = System.DateTime.Now; // Reference point for calculating reaction time.
        refUnityTime = Time.realtimeSinceStartup; // Reference point for calculating reaction time.

        speechRecognizer = new KeywordRecognizer(validWords, ConfidenceLevel.Low); // Recognition class that only listens for words specified in validWords list.
        speechRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        speechRecognizer.Start();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        lastSpokenWord = args.text.ToLower(); // Each time a word is spoken, lastSpokenWord stores that value.
        reactionTime = (refUnityTime + (float)(args.phraseStartTime - refDateTime).TotalSeconds) - arithmetic_generator.ARITHMETIC_START_TIME; // Convert to float to use with Unity's time system.
    }

     public bool CompareWords(){
        if (lastSpokenWord == null){
            Debug.Log("No word detected");
            return false; 
        }

        if (lastSpokenWord == arithmetic_generator.correctNumber.ToLower()){ // Returns true/false based on comparsion to target. Also consumes last word to prevent two
            lastSpokenWord = null;                                   // tests from accidentally reusing the result if nothing is spoken
            Debug.Log("Correct");
            Debug.Log("Reaction Time: " + reactionTime);
            return true;
        }
        else{
            lastSpokenWord = null;
            Debug.Log("Incorrect");
            Debug.Log("Reaction Time: " + reactionTime);
            return false;
        }
     }
}