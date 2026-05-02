using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System;
using Color = UnityEngine.Color;

public class arithmetic_generator : MonoBehaviour
{
    public static bool SHOW_COLOR_RESPONSE;
    public const float TIME_INTERVAL = 2.0f;
    public const float VOICE_INTERVAL = 3.0f;
    public static float ARITHMETIC_START_TIME;

    public static bool ROUND_BASED = true;      //bool for if the test is round based or not
    public static int ROUND_NUM = 2;           //num of rounds if rounds based.

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
        public int adding_number_index; //Index(position) for the adding number
        public int question_index; //Index(position) for the current question
        public int initial_number; //value user is adding TO
        public int adding_number; //what number is being added
        public bool correctness; //correctness of answer
        public int correct_answer;
        public float reaction_time; //reaction time
    };

    public static List<results> results_array = new List<results>();

    void Start()
    {
        results_array.Clear();
        StartCoroutine(change_number());
    }

    IEnumerator change_number()
    {
        if (ROUND_BASED == false)
        {
            while (true)
            {
                yield return StartCoroutine(arithmetic_test());
            }
        }
        else
        {
            for (int i = 0; i < ROUND_NUM; i++)
            {
                yield return StartCoroutine(arithmetic_test());
            }

            numberText.text = "";
        }
    }

    private int num_index = 0; // Moved outside to persist across coroutine calls

    IEnumerator arithmetic_test()
    {
        int adding_num = UnityEngine.Random.Range(1, 5);
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
            added_num_array[i] = (numbers_array[i] + adding_num);
        }

        for (int i = 0; i < numbers_array.Length; i++)
        {
            numberText.text = "" + numbers_array[i];
            yield return new WaitForSeconds(TIME_INTERVAL);
        }

        for (int i = 0; i < correctAnswers.Length; i++) 
        {
            correctAnswers[i] = validWords[added_num_array[i]];
        }

        Array.Fill(answerResults, false); 
        for (int i = 0; i < correctAnswers.Length; i++)
        {
            correctNumber = correctAnswers[i];
            numberText.text = "What is the answer to number " + (i + 1) + "?";
            ARITHMETIC_START_TIME = Time.realtimeSinceStartup; 
            yield return new WaitForSeconds(VOICE_INTERVAL); 
            answerResults[i] = arithmeticVerifier.CompareWords();

            if(SHOW_COLOR_RESPONSE == true)
            {
                if(answerResults[i] == true)
                {
                    arithmetic_background.color = new Color(0.01f, 1f, 0.01f, 1f);
                }
                else
                {
                    arithmetic_background.color = new Color(1f, 0.01f, 0.01f, 1f);
                }

                yield return new WaitForSeconds(1.0f);
            }
            arithmetic_background.color = Color.white;

            results newResult = new results();
            newResult.adding_number_index = num_index;
            newResult.question_index = i;
            newResult.initial_number = numbers_array[i];
            newResult.adding_number = adding_num;
            newResult.correct_answer = added_num_array[i];
            newResult.correctness = answerResults[i];
            newResult.reaction_time = arithmeticVerifier.reactionTime;

            results_array.Add(newResult);
        }

        numberText.text = "Correct Numbers: " + added_num_array[0] + ", " + added_num_array[1] + ", " + added_num_array[2] + ", " + added_num_array[3];

        yield return new WaitForSeconds(TIME_INTERVAL);

        Array.Clear(added_num_array, 0, added_num_array.Length);
        Array.Clear(numbers_array, 0, added_num_array.Length);
        adding_num = 0;
        num_index++;
    }
}