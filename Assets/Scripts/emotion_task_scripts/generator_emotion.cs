using UnityEngine;
using TMPro;
using System.Threading.Tasks;
public class generator_emotion : MonoBehaviour
{
    private const float TIME_INTERVAL = 3f;
    public TextMeshProUGUI textbox;
    public TextMeshProUGUI good_textbox;
    public TextMeshProUGUI bad_textbox;
    private int wordIndex = 0;
    private int newWordIndex = 0;

    private string[] words = { 
   
    "Horrendous", "Happy", "Joy", "Malicious", "Dismay", "Punishment", "Excitement", "Disaster", "Hope",
    
    "Paradise", "Exultant", "Victorious", "Wonderful", "Blessed", "Glorious", "Delight", "Honest", "Nurture", "Loyal", "Luminous", "Vibrant",
    
    "Nightmare", "Abhorrent", "Catastrophe", "Despair", "Damaged", "Hideous", "Failure", "Deceit", "Betrayal", "Cruel", "Obscure", "Rotting"
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

    void Change_Text()
    {
        newWordIndex = Random.Range(0, words.Length);


        while (newWordIndex == wordIndex)
        {
            newWordIndex = Random.Range(0, words.Length);
        }

        wordIndex = newWordIndex;


        textbox.text = words[wordIndex];
        
        return;
    }
}
