using UnityEngine;
using UnityEngine.Windows.Speech;

public class emotionVerification : MonoBehaviour
{
    KeywordRecognizer speechRecognizer;
    private string[] validWords = {"good", "bad"};
    private string lastSpokenWord = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speechRecognizer = new KeywordRecognizer(validWords); // Recognition class that only listens for words specified in validWords list.
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
    }

    public bool CompareWords(){
        if (lastSpokenWord == null){
            Debug.Log("No word detected");
            return false; 
        }

        if (lastSpokenWord == generator_emotion.wordType.ToLower()){ // Returns true/false based on comparsion to target. Also consumes last word to prevent
            lastSpokenWord = null;                                   // two tests from accidentally reusing the result if nothing is spoken
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
