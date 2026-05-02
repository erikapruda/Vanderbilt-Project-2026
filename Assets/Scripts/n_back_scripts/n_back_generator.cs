using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Color = UnityEngine.Color;

public class n_back_generator : MonoBehaviour
{
    public static bool SHOW_COLOR_RESPONSE;
    public const float TIME_INTERVAL = 3f;    
    public int AMOUNT_BACK = 1;

    public static bool ROUND_BASED = true;      //bool for if the test is round based or not
    public static int ROUND_NUM = 5;           //num of rounds if rounds based.

    public TextMeshProUGUI number_text;          
    public Image backgroundImage;               
    public nBackVerification nBackVerifier;   
    public static float N_BACK_START_TIME = 0f;

    public struct results
    {
        public float reaction_time;
        public bool correctness;
        public char current_letter;
        public char letter_n_back;
    }

    public static List<results> results_array = new List<results>();
    
    char[] all_letters = {
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
    };

    public List<char> letters_list = new List<char>();
    public static char correct_letter;

    void Start()
    {   
        results_array.Clear();
        backgroundImage.color = Color.white;
        StartCoroutine(Change_n_back());
    }

    IEnumerator Change_n_back()
    {
        if (ROUND_BASED == false)
        {
            while (true)
            {
                yield return StartCoroutine(n_back_test());
            }
        }
        else
        {
            for (int i = 0; i < ROUND_NUM; i++)
            {
                yield return StartCoroutine(n_back_test());
            }

            number_text.text = "";
        }
    }

    IEnumerator n_back_test()
    {
        backgroundImage.color = Color.white;

        char nextLetter;
        int letterIndex;

        if (letters_list.Count >= AMOUNT_BACK && Random.value < 0.3f)
        {
            nextLetter = letters_list[letters_list.Count - AMOUNT_BACK];
        }
        else
        {
            letterIndex = Random.Range(0, 26);
            nextLetter = all_letters[letterIndex];
        }
        
        N_BACK_START_TIME = Time.realtimeSinceStartup; 
        
        number_text.text = nextLetter.ToString();
        
        letters_list.Add(nextLetter);
        
        yield return new WaitForSeconds(TIME_INTERVAL);

        if(letters_list.Count > AMOUNT_BACK)
        {
            correct_letter = letters_list[(letters_list.Count - 1) - AMOUNT_BACK]; 
            bool correctness = nBackVerifier.CompareWords();
            
            if(SHOW_COLOR_RESPONSE == true)
            {
                if (correctness == true)
                {
                    backgroundImage.color = new Color(0.01f, 1f, 0.01f, 1f);
                }
                else
                {
                    backgroundImage.color = new Color(1f, 0.01f, 0.01f, 1f);
                }

                yield return new WaitForSeconds(1.0f);
                backgroundImage.color = Color.white;
            }

            results newResult = new results();
            newResult.reaction_time = nBackVerifier.reactionTime; 
            newResult.correctness = correctness;
            newResult.current_letter = nextLetter;
            newResult.letter_n_back = correct_letter;

            results_array.Add(newResult);
        }
        
        backgroundImage.color = Color.white;
    }
}