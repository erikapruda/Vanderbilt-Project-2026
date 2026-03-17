using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System;

public class arithmetic_generator : MonoBehaviour
{
    public const float TIME_INTERVAL = 2.0f;
    public const float VOICE_INTERVAL = 3.0f;
    public TextMeshProUGUI numberText;
    public arithmeticVerification arithmeticVerifier;

    public int[] numbers_array = { 0, 0, 0, 0 };
    public int[] added_num_array = { 0, 0, 0, 0 };
    public string correctNumber = null;
    private string [] validWords = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen"};
    private string [] correctAnswers = {"zero", "zero", "zero", "zero"};
    private bool [] answerResults = { false, false, false, false };

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(change_number());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator change_number()
    {
        int adding_num = 0;


        while (true)
        {
            adding_num = UnityEngine.Random.Range(1, 5);
            numberText.text = "Add " + adding_num + " to all numbers.";
            yield return new WaitForSeconds(3.0f);

            
            for(int i = 0; i < numbers_array.Length; i++)
            {
                
                numbers_array[i] = UnityEngine.Random.Range(0, 11);

                if(i > 0)
                {
                    while (numbers_array[i] == numbers_array[i - 1])
                    {
                        numbers_array[i] = UnityEngine.Random.Range(0, 11);
                    }
                }
                
            }

            for(int i = 0; i < added_num_array.Length; i++)
            {
                added_num_array[i] += (numbers_array[i] + adding_num);
            }

            for (int i = 0; i < numbers_array.Length; i++)
            {
                numberText.text = "" + numbers_array[i];
                yield return new WaitForSeconds(TIME_INTERVAL);
            }

            for (int i = 0; i < correctAnswers.Length; i++) // Converts correct answers to string for comparison at end.
            {
                correctAnswers[i] = validWords[added_num_array[i]];
            }


            answerResults = { false, false, false, false }; // Resets results array for each round.
            for (int i = 0; i < correctAnswers.Length; i++)
            {
                correctNumber = correctAnswers[i];
                yield return new WaitForSeconds(VOICE_INTERVAL); // Set a time interval for the user to respond with each answer.
                answerResults[i] = arithmeticVerifier.CompareWords(); // Stores whether each number was correct or not in array.
            }

            numberText.text = "Correct Numbers: " + added_num_array[0] + ", " + added_num_array[1] + ", " + added_num_array[2] + ", " + added_num_array[3];

            yield return new WaitForSeconds(TIME_INTERVAL);

            Array.Clear(added_num_array, 0, added_num_array.Length);
            Array.Clear(numbers_array, 0, added_num_array.Length);
            adding_num = 0;
        }

        

    }
}
