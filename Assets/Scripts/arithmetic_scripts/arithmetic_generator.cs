using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;

public class arithmetic_generator : MonoBehaviour
{
    public const float TIME_INTERVAL = 2.0f;
    public const float VOICE_INTERVAL = 3.0f;
    public static float ARITHMETIC_START_TIME = 0f;
    public TextMeshProUGUI numberText;

    public Image arithmetic_background;
    public arithmeticVerification arithmeticVerifier;
    
    public int[] numbers_array = { 0, 0, 0, 0 };
    public int[] added_num_array = { 0, 0, 0, 0 };
    public static string correctNumber = null;
    private string [] validWords = {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen"};
    private string [] correctAnswers = {"zero", "zero", "zero", "zero"};
    private bool [] answerResults = { false, false, false, false };

    public struct results
    {
        public float reaction_time;
        public bool correctness;
        public int question_number;
    };

    List<results> results_array = new List<results>();

    
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


             Array.Fill(answerResults, false); // Resets results array for each round.
            for (int i = 0; i < correctAnswers.Length; i++)
            {
                correctNumber = correctAnswers[i];
                numberText.text = "What is the answer to number " + (i + 1) + "?";
                ARITHMETIC_START_TIME = Time.realtimeSinceStartup; // Set start time for each question to calculate reaction time.
                yield return new WaitForSeconds(VOICE_INTERVAL); // Set a time interval for the user to respond with each question.
                answerResults[i] = arithmeticVerifier.CompareWords();
                
                if(answerResults[i] == true)
                {
                    //change background
                    arithmetic_background.color = new Color(0.01f, 1f, 0.01f, 0.68f);

                }

                else if(answerResults[i] == false)
                {
                    //change background
                    arithmetic_background.color = new Color(1f, 0.01f, 0.01f, 0.68f);
                }

                yield return new WaitForSeconds(1.0f);
                arithmetic_background.color = Color.white;

                results newResult = new results();
                newResult.question_number = i + 1;
                newResult.correctness = answerResults[i];
                newResult.reaction_time = arithmeticVerifier.reactionTime;
                results_array.Add(newResult);
            }


            numberText.text = "Correct Numbers: " + added_num_array[0] + ", " + added_num_array[1] + ", " + added_num_array[2] + ", " + added_num_array[3];

            yield return new WaitForSeconds(TIME_INTERVAL);

            Array.Clear(added_num_array, 0, added_num_array.Length);
            Array.Clear(numbers_array, 0, added_num_array.Length);
            adding_num = 0;
        }

        

    }
}
