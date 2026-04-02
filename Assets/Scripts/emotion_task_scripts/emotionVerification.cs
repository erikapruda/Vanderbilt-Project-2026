using UnityEngine;
using UnityEngine.Windows.Speech;

public class emotionVerification : MonoBehaviour
{
    KeywordRecognizer speechRecognizer;
    private string[] validWords = {"good", "bad"};
    private string lastSpokenWord = null;
    private System.DateTime refDateTime;
    private float refUnityTime;
    public float reactionTime = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        refDateTime = System.DateTime.Now; // Reference point for calculating reaction time.
        refUnityTime = Time.realtimeSinceStartup; // Reference point for calculating reaction time.

        speechRecognizer = new KeywordRecognizer(validWords, ConfidenceLevel.Low); // Recognition class that only listens for words specified in validWords list.
        speechRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        speechRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        lastSpokenWord = args.text.ToLower(); // Each time a word is spoken, lastSpokenWord stores that value.
        reactionTime = (refUnityTime + (float)(args.phraseStartTime - refDateTime).TotalSeconds) - generator_emotion.WORD_START_TIME; // Convert to float to use with Unity's time system.
    }

    public bool CompareWords(){
        if (lastSpokenWord == null){
            Debug.Log("No word detected");
            return false; 
        }

        if (lastSpokenWord == generator_emotion.wordType.ToLower()){ // Returns true/false based on comparsion to target. Also consumes last word to prevent
            lastSpokenWord = null;                                   // two tests from accidentally reusing the result if nothing is spoken
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
