using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class n_back_generator : MonoBehaviour
{

    public const float TIME_INTERVAL = 3f;    //time interval

    public int AMOUNT_BACK = 2;
    public TextMeshProUGUI number_text;          //canvas text 
    public Image backgroundImage;               //canvas image
    public nBackVerification nBackVerifier;   //speech-to-text script
    public static float N_BACK_START_TIME = 0f;

    //char array length 26 to store all letters in alphabet.
    char[] all_letters = {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

    public List<char> letters_list = new List<char>();
    public static char correct_letter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backgroundImage.color = Color.white;

        StartCoroutine(Change_n_back());

    }

    // Update is called once per frame
    void Update()
    {

    }

   


    IEnumerator Change_n_back()
    {

        while (true)
        {

            backgroundImage.color = Color.white;

           

            char nextLetter;
            int letterIndex;


            if (letters_list.Count >= 2 && Random.value < 0.3f)
            {
                
                nextLetter = letters_list[letters_list.Count - AMOUNT_BACK];
                //Debug.Log("Forced a match!");

            }


            else
            {
                letterIndex = Random.Range(0, 26);
                nextLetter = all_letters[letterIndex];
            }

            N_BACK_START_TIME = Time.realtimeSinceStartup; // Start reaction timer when new letter is displayed.
            number_text.text = nextLetter.ToString();
            letters_list.Add(nextLetter);

            /*
            
            for(int i = 0; i < letters_list.Count; i++)
            {
                Debug.Log(letters_list[i] + ", ");
            }

            */

            if(letters_list.Count >= 3)
            {
                correct_letter = letters_list[(letters_list.Count - 1) - AMOUNT_BACK]; //second to last from the most recent character
                //Debug.Log("" + correct_letter);
                bool correctness = nBackVerifier.CompareWords();
                if(correct_letter == letters_list[letters_list.Count - 1])
                {
                    Debug.Log("Same");
                }

                else
                {
                    //Debug.Log("Different");
                }

            }
            yield return new WaitForSeconds(TIME_INTERVAL);


 

        }


    }
}
