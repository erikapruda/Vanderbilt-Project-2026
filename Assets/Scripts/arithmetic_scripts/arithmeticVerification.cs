using UnityEngine;
using UnityEngine.Windows.Speech;

public class arithmeticVerification : MonoBehaviour
{
    KeywordRecognizer speechRecognizer;
    private string[] validWords = {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen"};
    private string lastSpokenWord = null;

    void Start()
    {
        speechRecognizer = new KeywordRecognizer(validWords); // Recognition class that only listens for words specified in validWords list.
        speechRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        speechRecognizer.Start();
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

        if (lastSpokenWord == arithmetic_generator.correctNumber.ToLower()){ // Returns true/false based on comparsion to target. Also consumes last word to prevent two
            lastSpokenWord = null;                                   // tests from accidentally reusing the result if nothing is spoken
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