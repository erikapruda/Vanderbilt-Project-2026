using UnityEngine;
using UnityEngine.Windows.Speech;

public class stroopVerification : MonoBehaviour
{
    public generator stroopTest;
    KeywordRecognizer speechRecognizer;
    private string[] validColors = {"red", "blue", "yellow", "orange", "green", "purple"};
    private string lastSpokenWord = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speechRecognizer = new KeywordRecognizer(validColors); // Recognition class that only listens for words specified in validColors list.
        speechRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        speechRecognizer.Start();

        InvokeRepeating("CompareWords", generator.TIME_INTERVAL, generator.TIME_INTERVAL); // Compares words on same interval as stroop changes.

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        lastSpokenWord = args.text.ToLower(); // Each time a color is spoken, lastSpokenWord stores that value.
    }

    public bool CompareWords(){
        if (lastSpokenWord == null){
            Debug.Log("No word detected");
            return false; 
        }

        if (lastSpokenWord == generator.currentTargetColor.ToLower()){ // Returns true/false based on comparsion to target. Also consumes last word to prevent two
            lastSpokenWord = null;                                      // tests from accidentally reusing the result is nothing is spoken
            Debug.Log("Correct");
            return true;
        }
        else{
            lastSpokenWord = null;
            Debug.Log("Incorrect");
            return false;
        }
    }
}
