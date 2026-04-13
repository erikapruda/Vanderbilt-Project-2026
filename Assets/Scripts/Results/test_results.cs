using UnityEngine;
using System.IO;
using System;
public class test_results : MonoBehaviour
{   
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void export_all_results()
    {
        export_stroop_results();
        export_emotion_results();
        export_arithmetic_results();
        export_n_back_results();
    }

    public void export_stroop_results()
    {
        if(generator.results_array.Count != 0)
        {
            string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH_mm");
            string filePath = Path.Combine(Application.persistentDataPath, $"StroopResults_{timeStamp}.csv");
            
            try
            {
                // Create the file or overwrite it if it exists
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the header
                    writer.WriteLine("Index,ReactionTime,Correctness,ColorWord,Color");
                    
                    for(int i = 0; i < generator.results_array.Count; i++)
                    {   
                        string index = i.ToString() ;
                        string reactionTime = generator.results_array[i].reaction_time.ToString();
                        string correctness = decide_correctness(generator.results_array[i].correctness);
                        string color_word = generator.results_array[i].color_word.ToLower();
                        string color = decide_color(generator.results_array[i].color_index);

                        writer.WriteLine($"{index},{reactionTime},{correctness},{color_word},{color}");
                    }
                }
                
                Debug.Log($"Stroop Results CSV file saved to: {filePath}");
            }
            
            catch (IOException e)
            {
                Debug.LogError($"FILE LOCKED: Could not write to {filePath}. Is it open in Excel? {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"General Error writing Stroop CSV: {e.Message}");
            }
        }

        else if(generator.results_array.Count == 0)
        {
            Debug.Log("Stroop Results are empty. Stroop Results CSV file not saved!");
        }
    }

    public void export_emotion_results()
    {
        if(generator_emotion.results_array.Count != 0)
        {
            string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH_mm");
            string filePath = Path.Combine(Application.persistentDataPath, $"EmotionResults_{timeStamp}.csv");
            
            try
            {
                // Create the file or overwrite it if it exists
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Write the header
                    writer.WriteLine("Index,ReactionTime,Correctness,Word,IntendedEmotion");
                    
                    for(int i = 0; i < generator_emotion.results_array.Count; i++)
                    {   
                        string index = i.ToString() ;
                        string reactionTime = generator_emotion.results_array[i].reaction_time.ToString();
                        string correctness = decide_correctness(generator_emotion.results_array[i].correctness);
                        string word = generator_emotion.results_array[i].word.ToLower();
                        string emotion = decide_emotion(generator_emotion.results_array[i].emotion);
                        

                        writer.WriteLine($"{index},{reactionTime},{correctness},{word},{emotion}");
                    }
                }
                
                Debug.Log($"Emotion Results CSV file saved to: {filePath}");
            }
            catch (IOException e)
            {
                Debug.LogError($"FILE LOCKED: Could not write to {filePath}. Is it open in Excel? {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"General Error writing Emotion CSV: {e.Message}");
            }
        }

        else if(generator_emotion.results_array.Count == 0)
        {
            Debug.Log("Emotion Results are empty. Emotion Results CSV file not saved!");
        }

    }


    public void export_arithmetic_results(){
        // Accessing the list from the arithmetic_generator class
        if (arithmetic_generator.results_array.Count != 0)
        {
            string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH_mm");
            string filePath = Path.Combine(Application.persistentDataPath, $"ArithmeticResults_{timeStamp}.csv");

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    
                    writer.WriteLine("RoundIndex,QuestionIndex,InitialNumber,Modifier,CorrectNumber,Correctness,ReactionTime");

                    for (int i = 0; i < arithmetic_generator.results_array.Count; i++)
                    {
                        
                        string round = arithmetic_generator.results_array[i].adding_number_index.ToString();
                        string question = arithmetic_generator.results_array[i].question_index.ToString();
                        string initial = arithmetic_generator.results_array[i].initial_number.ToString();
                        string modifier = arithmetic_generator.results_array[i].adding_number.ToString();
                        string CorrectNumber = arithmetic_generator.results_array[i].correct_answer.ToString();
                        
                        string correctness = decide_correctness(arithmetic_generator.results_array[i].correctness);
                        string reactionTime = arithmetic_generator.results_array[i].reaction_time.ToString("F2"); // Formatted to 2 decimal places

                        writer.WriteLine($"{round},{question},{initial},{modifier},{CorrectNumber},{correctness},{reactionTime}");
                    }
                }

                Debug.Log($"Arithmetic Results CSV file saved to: {filePath}");
            }
            catch (IOException e)
            {
                Debug.LogError($"FILE LOCKED: Could not write to {filePath}. Is it open in Excel? {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"General Error writing Arithmetic CSV: {e.Message}");
            }
        }
        else
        {
            Debug.Log("Arithmetic Results are empty. CSV file not saved!");
        }
    }

    public void export_n_back_results()
{
    
    if (n_back_generator.results_array.Count != 0)
    {
        string timeStamp = DateTime.Now.ToString("dd-MM-yyyy_HH_mm");
        string filePath = Path.Combine(Application.persistentDataPath, $"NBackResults_{timeStamp}.csv");

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                
                writer.WriteLine("Index,CurrentLetter,LetterNBack,Correctness,ReactionTime");

                for (int i = 0; i < n_back_generator.results_array.Count; i++)
                {
                    

                    string index = i.ToString();
                    string currentLetter = n_back_generator.results_array[i].current_letter.ToString();
                    string letterNBack = n_back_generator.results_array[i].letter_n_back.ToString();
                    string correctness = decide_correctness(n_back_generator.results_array[i].correctness);
                    string reactionTime = n_back_generator.results_array[i].reaction_time.ToString("F2");

                    writer.WriteLine($"{index},{currentLetter},{letterNBack},{correctness},{reactionTime}");
                }
            }

            Debug.Log($"N-Back Results CSV file saved to: {filePath}");
        }
        catch (IOException e)
        {
            Debug.LogError($"FILE LOCKED: Could not write to {filePath}. Is it open in Excel? {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"General Error writing N-Back CSV: {e.Message}");
        }
    }
    else
        {
            Debug.Log("N-Back Results are empty. CSV file not saved!");
        }
}
    string decide_color(int index)
    {   
        
        if(index == 0)
        {
            return "red";
        }

        else if(index == 1)
        {
            return "blue";
        }

        else if(index == 2)
        {
            return "green";
        }

        else if(index == 3)
        {
            return "yellow";
        }

        else if(index == 4)
        {
            return "purple";
        }

        else if(index == 5)
        {
            return "orange";
        }
        else
        {
            return "error";
        }
    }

    string decide_correctness(bool correctness)
    {
        if (correctness == true)
        {
            return "Correct";
        }

        else
        {
            return "Incorrect";
        }
    }

    string decide_emotion(int emotion_num)
    {
        if(emotion_num == 0)
        {
            return "bad";
        }

        else if(emotion_num == 1)
        {
            return "good";
        }

        else
        {
            return "Error!!!!";
        }
    }
    
}