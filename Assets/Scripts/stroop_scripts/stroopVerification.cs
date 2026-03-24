using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class stroopVerification : MonoBehaviour
{

    


    //public generator stroopTest;
    KeywordRecognizer speechRecognizer;
    private string[] validColors = {"red", "blue", "yellow", "orange", "green", "purple"};
    private string lastSpokenWord = null;
    public float reactionTime = 0f;
    private System.DateTime refDateTime;
    private float refUnityTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Created time references to use Unity's time and convert from the sytem time provided by the speech recognition event args.
        refDateTime = System.DateTime.Now; // Reference point for calculating reaction time.
        refUnityTime = Time.realtimeSinceStartup; // Reference point for calculating reaction time.
        
        speechRecognizer = new KeywordRecognizer(validColors); // Recognition class that only listens for words specified in validColors list.
        speechRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        speechRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        lastSpokenWord = args.text.ToLower(); // Each time a color is spoken, lastSpokenWord stores that value.
        reactionTime = (refUnityTime + (float)(args.phraseStartTime - refDateTime).TotalSeconds) - generator.STROOP_START_TIME; // Convert to float to use with Unity's time system.
        Debug.Log("Reaction Time: " + reactionTime);
    }

    public bool CompareWords(){
        if (lastSpokenWord == null){
            Debug.Log("No word detected");
            
            return false; 
        }

        if (lastSpokenWord == generator.currentTargetColor.ToLower()){ // Returns true/false based on comparsion to target. Also consumes last word to prevent two
            lastSpokenWord = null;                                      // tests from accidentally reusing the result is nothing is spoken
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
